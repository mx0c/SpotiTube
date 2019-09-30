using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SpotiTube;
using Windows.Media.Playback;
using YoutubeSearch;
using Windows.ApplicationModel.DataTransfer;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;
using System.Timers;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        private MusicPlayer musicPlayer;
        private Song latestSelectedSong;
        private Song draggedSong;
        private Playlist selectedPlaylist;
        private ObservableCollection<Song> mainListViewItemSource;
        private Playlist currentlyPlayingPlaylist;
        private Song currentlyPlayingSong;
        private bool loop = false;
        private bool random = false;
        private Timer scrollTimer;

        public MainPage()
        {
            this.InitializeComponent();
            this.mainListViewItemSource = new ObservableCollection<Song>();
            this.musicPlayer = new MusicPlayer(this.CurrentDuration, this.CurrentTime, this.TimeSlider, this.MainListView, this.PlayButton, this.currPlayingSongRect, this.currPlayingSongLabel);
            this.TimeSlider.AddHandler(PointerReleasedEvent, new PointerEventHandler(TimeSlider_OnPointerRelease),true);
            Task.Run(() => this.loadPlaylists()).Wait();
            this.musicPlayer.mPlayer.MediaEnded += (sender, eventArgs) => {
                if (!this.loop)
                {
                    this.musicPlayer.skipSong(true, ref this.currentlyPlayingSong, this.currentlyPlayingPlaylist, random);
                }
                else
                {
                    this.musicPlayer.PlaySong(this.currentlyPlayingSong);
                }
            };
            Task.Run(() => this.loadSettings()).Wait();   
        }

        private async void loadSettings()
        {
            var settings = await DataIO.readSettings();
            Helper.executeThreadSafe(() =>
            {
                this.VolumeSlider.Value = settings.volumePerc;
            });            
        }

        private async void loadPlaylists()
        {
            try
            {  
                ObservableCollection<Playlist> lists = await DataIO.ReadPlaylists();
                Helper.executeThreadSafe(() =>
                {
                    
                    this.selectedPlaylist = lists[0];
                    this.MainListView.ItemsSource =  this.mainListViewItemSource;
                    mainListViewItemSource.Clear();
                    PlaylistList.ItemsSource = lists;
                    this.PlaylistList.SelectedIndex = 0;

                    if (lists[0].Songlist == null)
                        return;

                    foreach (Song s in this.selectedPlaylist.Songlist) {
                        this.mainListViewItemSource.Add(s);
                    }   
                });
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

        public void changeCurrentDurationText(String currDur)
        {
            this.CurrentDuration.Text = currDur;
        }

        public void changeCurrentTimeText(String currTime)
        {
            this.CurrentTime.Text = currTime;
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            this.musicPlayer.skipSong(false, ref this.currentlyPlayingSong, this.currentlyPlayingPlaylist, this.random);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            this.musicPlayer.skipSong(true, ref this.currentlyPlayingSong, this.currentlyPlayingPlaylist, this.random);         
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.musicPlayer.mPlayer.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.None:
                    if (this.latestSelectedSong != null) this.musicPlayer.PlaySong(this.latestSelectedSong);            
                    break;
                case MediaPlaybackState.Paused:
                    this.musicPlayer.resume();
                    break;
                default:
                    this.musicPlayer.PauseSong();
                    break;
            }
        }

        private void VolumeSlider_Changed(object sender, RoutedEventArgs e) {
            try
            {
                this.musicPlayer.adjustVolume(this.VolumeSlider.Value);             
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

        private void TimeSlider_OnPointerRelease(object sender, PointerRoutedEventArgs e)
        {
            this.musicPlayer.seek(this.TimeSlider.Value);
        }

        private void SearchBar_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {            
                var items = new VideoSearch();
                this.mainListViewItemSource.Clear();               
                foreach (var item in items.SearchQuery(this.SearchBar.Text, 1))
                {
                    var s = new Song(item.Title, item.Url, "none", Helper.ImageURLToBase64(item.Thumbnail), item.Duration);
                    this.mainListViewItemSource.Add(s);
                }
            }
        }

        private void onItemClicked(object sender, ItemClickEventArgs e)
        {
            this.latestSelectedSong = (Song)e.ClickedItem;
        }

        private async void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            String plName = await Helper.InputTextDialogAsync("Playlist name", "Add");
            if (plName == "")
                return;
            var npl = new Playlist { Title = plName };
            await DataIO.SavePlaylist(npl);
            this.PlaylistList.Items.Clear();
            this.loadPlaylists();
        }

        private void PlaylistList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var playlist = (Playlist)e.ClickedItem;
            this.selectedPlaylist = playlist;
            this.mainListViewItemSource.Clear();
            
            if (this.selectedPlaylist.Songlist == null)
                return;

            foreach (Song s in this.selectedPlaylist.Songlist)
            {
                this.mainListViewItemSource.Add(s);
            }
        }

        private void Grid_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            args.AllowedOperations = DataPackageOperation.Copy;
            var song = (sender as Grid).DataContext as Song;
            this.draggedSong = song;
            DateTime today = DateTime.Today;
            draggedSong.addedAt = today.ToString("dd/MM/yyyy");
        }

        private async void StackPanel_Drop(object sender, DragEventArgs e)
        {
            Playlist pl = ((StackPanel)sender).DataContext as Playlist;
            if (pl.Songlist != null)
            {
                pl.Songlist.Add(this.draggedSong);
            }
            else
            {
                pl.Songlist = new ObservableCollection<Song>();
                pl.Songlist.Add(this.draggedSong);
            }
            await DataIO.SavePlaylist(pl);
        }

        private void StackPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Zu Playlist hinzufügen";
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var lv = (ListView)sender;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(lv);
            var type = e.OriginalSource.GetType();
            dynamic tmp = Convert.ChangeType(e.OriginalSource, type);
            this.selectedPlaylist = tmp.DataContext as Playlist;
            flyoutBase.ShowAt(tmp);
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            String nTitle = await Helper.InputTextDialogAsync("Rename Playlist", "Rename");
            String oldTitle = this.selectedPlaylist.Title;
            this.selectedPlaylist.Title = nTitle;
            await DataIO.SavePlaylist(this.selectedPlaylist, oldTitle);
            this.loadPlaylists();
        }

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var song = ((sender as Grid).DataContext as Song);
            this.musicPlayer.PlaySong(song);
            this.MainListView.SelectedItem = song;
            this.currentlyPlayingPlaylist = this.selectedPlaylist;
            this.currentlyPlayingSong = song;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            await DataIO.RemovePlaylist(this.selectedPlaylist.Title);
            this.PlaylistList.Items.Clear();
            this.loadPlaylists();
        }
    
        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.loop)
            {
                this.loop = true;
                this.loopButton.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            }
            else
            {
                this.loop = false;
                this.loopButton.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            }
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.random)
            {
                this.random = true;
                this.randomButton.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            }
            else
            {
                this.random = false;
                this.randomButton.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            }
        }

        private async void DeleteSongButton_Click(object sender, RoutedEventArgs e)
        {
            var toDelete = (sender as MenuFlyoutItem).DataContext as Song;
            this.selectedPlaylist.Songlist.Remove(toDelete);
            await DataIO.SavePlaylist(this.selectedPlaylist);
            this.mainListViewItemSource.Remove(toDelete);
        }

        private void MainListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var lv = (ListView)sender;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(lv);
            var type = e.OriginalSource.GetType();
            dynamic tmp = Convert.ChangeType(e.OriginalSource, type);
            flyoutBase.ShowAt(tmp);
        }

        private void Ellipse_PointerEntered(Object sender, PointerRoutedEventArgs e)
        {
            var tmp = new ImageBrush();
            tmp.ImageSource = new BitmapImage(new Uri(base.BaseUri, @"/Assets/play.png"));
            (sender as Ellipse).Fill = tmp;                
        }

        private async void Ellipse_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var ellipse = (sender as Ellipse);
            var tmp = new ImageBrush();
            tmp.ImageSource = await Helper.base64toBmp((ellipse.DataContext as Song).ThumbnailBase64);
            ellipse.Fill = tmp;
        }

        private void Ellipse_Tapped(object sender, TappedRoutedEventArgs e)
        {        
            var song = ((sender as Ellipse).DataContext as Song);
            this.musicPlayer.PlaySong(song);
            this.MainListView.SelectedItem = song;
            this.currentlyPlayingPlaylist = this.selectedPlaylist;
            this.currentlyPlayingSong = song;
        }

        private void DownloadPlaylist_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Scrollviewer_Loaded(object sender, RoutedEventArgs e)
        {
            scrollTimer = new Timer();
            scrollTimer.Interval = 50;
            var forward = true;
            scrollTimer.Elapsed += (source, args) => {
                Helper.executeThreadSafe(() =>
                {
                    if(forward)
                        scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + 1);
                    else
                        scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - 1);

                    if (scrollviewer.HorizontalOffset == scrollviewer.ScrollableWidth)
                        forward = false;
                    else if (scrollviewer.HorizontalOffset == 0)
                        forward = true;
                });
            };
            scrollTimer.Start();
        }

        private void Scrollviewer_Unloaded(object sender, RoutedEventArgs e)
        {
            scrollTimer.Stop();
        }

        private async void VolumeSlider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            var setting = await DataIO.readSettings();
            setting.volumePerc = (int)this.VolumeSlider.Value;
            await DataIO.saveSettings(setting);
        }

        private async void DownloadSong_Click(object sender, RoutedEventArgs e)
        {
            var song = ((sender as MenuFlyoutItem).DataContext as Song);
            var songItem = this.mainListViewItemSource.Where(s => s.SongURL == song.SongURL).FirstOrDefault();

            if (songItem.isDownloaded || songItem.isDownloading)
                return;

            var i = this.mainListViewItemSource.IndexOf(songItem);

            songItem.isDownloading = true;
            mainListViewItemSource.RemoveAt(i);
            mainListViewItemSource.Insert(i, songItem);

            var progHandler = new Progress<double>(p => songItem.downloadProgress = p * 100.0);

            if (await Downloader.downloadSong(song, progHandler)) {
                songItem.isDownloaded = true;
                songItem.isDownloading = false;
                this.selectedPlaylist.Songlist.RemoveAt(i);
                this.selectedPlaylist.Songlist.Insert(i, songItem);
                await DataIO.SavePlaylist(this.selectedPlaylist);
                mainListViewItemSource.RemoveAt(i);
                mainListViewItemSource.Insert(i, songItem);
            }
        }

        private void Ellipse_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
