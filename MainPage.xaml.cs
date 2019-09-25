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

namespace App1
{
    public sealed partial class MainPage : Page
    {
        private MusicPlayer musicPlayer;
        private Song latestSelectedSong;
        private List<Song> selectedSongs = new List<Song>();
        private Song draggedSong;
        private Playlist selectedPlaylist;
        private Playlist currentlyPlayingPlaylist;
        private Song currentlyPlayingSong;
        private bool loop = false;
        private bool random = false;
        private Timer scrollTimer;

        public MainPage()
        {
            this.InitializeComponent();
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
                List<Playlist> lists = await DataIO.ReadPlaylists();
                Helper.executeThreadSafe(() =>
                {
                    foreach (Playlist list in lists)
                    {
                        this.PlaylistList.Items.Add(list);
                    }
                    //Add first loaded Playlist in MainListView
                    foreach (Song s in lists[0].Songlist)
                    {
                        this.MainListView.Items.Add(s);
                    }
                    this.selectedPlaylist = lists[0];
                    this.PlaylistList.SelectedIndex = 0;
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
                this.MainListView.Items.Clear();
                if (this.selectedSongs !=  null)
                    this.selectedSongs.Clear();

                var items = new VideoSearch();
                foreach (var item in items.SearchQuery(this.SearchBar.Text, 1))
                {
                    this.MainListView.Items.Add(new Song(item.Title,item.Url,"none",item.Thumbnail,item.Duration));                  
                }
            }
        }

        private void onItemClicked(object sender, ItemClickEventArgs e)
        {
            this.latestSelectedSong = (Song)e.ClickedItem;
            this.selectedSongs.Add((Song)e.ClickedItem);
        }

        private async void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            String plName = await this.InputTextDialogAsync("Playlist name", "Add");
            if (plName == "")
                return;
            var npl = new Playlist { Title = plName };
            await DataIO.SavePlaylist(npl);
            this.PlaylistList.Items.Clear();
            this.loadPlaylists();
        }

        private void PlaylistList_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.MainListView.Items.Clear();
            var playlist = (Playlist)e.ClickedItem;
            this.selectedPlaylist = playlist;
            if (playlist.Songlist == null)
                //wenn keine songs enthalten
                return;
            foreach (var song in playlist.Songlist)
            {
                this.MainListView.Items.Add(song);
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
                pl.Songlist = new List<Song>();
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
            String nTitle = await this.InputTextDialogAsync("Rename Playlist", "Rename");
            String oldTitle = this.selectedPlaylist.Title;
            this.selectedPlaylist.Title = nTitle;
            await DataIO.SavePlaylist(this.selectedPlaylist, oldTitle);
            this.PlaylistList.Items.Clear();
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

        private async Task<string> InputTextDialogAsync(string title, string buttonText, string boxText = "")
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            inputTextBox.Text = boxText;
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = buttonText;
            dialog.SecondaryButtonText = "Cancel";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return "";
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
            this.MainListView.Items.Remove(toDelete);
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

        private void Ellipse_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var ellipse = (sender as Ellipse);
            var tmp = new ImageBrush();
            tmp.ImageSource = new BitmapImage(new Uri((ellipse.DataContext as Song).ThumbnailURL));
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
            if (await Downloader.downloadSong(song)) {
                List<Playlist> plist = await DataIO.ReadPlaylists();
                this.selectedPlaylist.Songlist.Find(x => x.SongURL == song.SongURL).isDownloaded = true;
                await DataIO.SavePlaylist(this.selectedPlaylist, this.selectedPlaylist.Title);
                this.PlaylistList.Items.Clear();
                this.MainListView.Items.Clear();
                this.loadPlaylists();
            }
        }
    }
}
