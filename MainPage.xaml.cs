using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using SpotiTube;
using Windows.Media.Playback;
using YouTubeSearch;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.Timers;
using System.Collections.ObjectModel;
using Helper = SpotiTube.Helper;
using Windows.UI.Popups;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        private MusicPlayer musicPlayer;
        private Song latestSelectedSong;
        private Song draggedSong;
        private Playlist selectedPlaylist;      
        private Timer scrollTimer;
        private SongListViewModel songListViewModel = new SongListViewModel();
        private Boolean searching = false;

        public MainPage()
        {
            this.InitializeComponent();
            this.musicPlayer = new MusicPlayer(this.CurrentDuration, this.CurrentTime, this.TimeSlider, this.MainListView, this.PlayButton, this.currPlayingSongRect, this.currPlayingSongLabel, this.StartLogoStackPanel);
            this.TimeSlider.AddHandler(PointerReleasedEvent, new PointerEventHandler(TimeSlider_OnPointerRelease),true);
            Task.Run(() => this.loadPlaylists()).Wait();
            Task.Run(() => this.loadSettings()).Wait();
        }

        private async void loadSettings()
        {
            var settings = await DataIO.readSettings();
            Helper.executeInUiThread(() =>
            {
                this.VolumeSlider.Value = settings.volumePerc;
            });            
        }

        private async void loadPlaylists()
        {
            try
            {  
                ObservableCollection<Playlist> lists = await DataIO.ReadPlaylists();
                Helper.executeInUiThread(() =>
                {                  
                    this.selectedPlaylist = lists[0];
                    this.MainListView.ItemsSource =  songListViewModel.songListObservable;
                    songListViewModel.songListObservable.Clear();

                    PlaylistList.Items.Clear();
                    foreach (Playlist pl in lists) {
                        PlaylistList.Items.Add(pl);
                    }
                    this.PlaylistList.SelectedIndex = 0;

                    if (lists[0].Songlist == null)
                        return;

                    foreach (Song s in this.selectedPlaylist.Songlist) {
                        this.songListViewModel.songListObservable.Add(s);
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
            this.musicPlayer.skipSong(false);
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            this.musicPlayer.skipSong(true);         
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.musicPlayer.mPlayer.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.None:
                    if (this.latestSelectedSong != null)
                        this.musicPlayer.currentSong = this.latestSelectedSong;
                        this.musicPlayer.Play();            
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

        private async void SearchBar_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                this.searching = true;
                var items = new VideoSearch();
                this.songListViewModel.songListObservable.Clear();

                LoadingDialog dialog = new LoadingDialog();
                var t = dialog.ShowAsync();

                var videos = await items.GetVideos(this.SearchBar.Text, 1);
                foreach (var item in videos)
                {
                    var s = new Song(item.getTitle(), item.getUrl(), "none", Helper.ImageURLToBase64(item.getThumbnail()), item.getDuration());
                    this.songListViewModel.songListObservable.Add(s);
                }
                t.Cancel();
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
            searching = false;
            var playlist = (Playlist)e.ClickedItem;
            this.selectedPlaylist = playlist;
            this.songListViewModel.songListObservable.Clear();
            
            if (this.selectedPlaylist.Songlist == null)
                return;

            foreach (Song s in this.selectedPlaylist.Songlist)
            {
                this.songListViewModel.songListObservable.Add(s);
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
            this.musicPlayer.currentSong = song;
            this.musicPlayer.Play();
            this.MainListView.SelectedItem = song;
            this.musicPlayer.currentPlaylist = this.selectedPlaylist;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            await DataIO.RemovePlaylist(this.selectedPlaylist.Title);
            Helper.executeInUiThread(()=>this.PlaylistList.Items.Clear());
            this.loadPlaylists();
        }
    
        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.musicPlayer.loop)
            {
                this.musicPlayer.loop = true;
                this.loopButton.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            }
            else
            {
                this.musicPlayer.loop = false;
                this.loopButton.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            }
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.musicPlayer.random)
            {
                this.musicPlayer.random = true;
                this.randomButton.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            }
            else
            {
                this.musicPlayer.random = false;
                this.randomButton.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            }
        }

        private async void DeleteSongButton_Click(object sender, RoutedEventArgs e)
        {
            var toDelete = (sender as MenuFlyoutItem).DataContext as Song;
            this.selectedPlaylist.Songlist.Remove(toDelete);
            await DataIO.SavePlaylist(this.selectedPlaylist);
            this.songListViewModel.songListObservable.Remove(toDelete);
        }

        private void MainListView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (searching)
                return;

            var lv = (ListView)sender;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(lv);
            var type = e.OriginalSource.GetType();
            dynamic tmp = Convert.ChangeType(e.OriginalSource, type);
            flyoutBase.ShowAt(tmp);
        }

        private void Ellipse_PointerEntered(Object sender, PointerRoutedEventArgs e)
        {
            var tmp  = new BitmapImage(new Uri(base.BaseUri, @"/Assets/play.png"));
            var img = sender as Image;
            img.Stretch = Stretch.Uniform;
            img.Source = tmp;                
        }

        private void Ellipse_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var img = (sender as Image);
            var tmp = Helper.base64toBmp((img.DataContext as Song).Thumbnail);
            img.Stretch = Stretch.Fill;
            img.Source = tmp;
        }

        private void Ellipse_Tapped(object sender, TappedRoutedEventArgs e)
        {        
            var song = ((sender as Image).DataContext as Song);
            this.musicPlayer.currentSong = song;
            this.musicPlayer.Play();
            this.MainListView.SelectedItem = song;
            this.musicPlayer.currentPlaylist = this.selectedPlaylist;
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
                Helper.executeInUiThread(() =>
                {
                    if(forward)
                        scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + 0.5);
                    else
                        scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - 0.5);

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
            var songItem = this.songListViewModel.songListObservable.Where(s => s.SongURL == song.SongURL).FirstOrDefault();

            if (songItem.isDownloaded || songItem.isDownloading)
                return;

            var i = this.songListViewModel.songListObservable.IndexOf(songItem);

            songItem.isDownloading = true;
            songListViewModel.songListObservable.RemoveAt(i);
            songListViewModel.songListObservable.Insert(i, songItem);

            var progHandler = new Progress<double>(p => songItem.downloadProgress = p * 100.0);

            if (await Downloader.downloadSong(song, progHandler)) {
                songItem.isDownloaded = true;
                songItem.isDownloading = false;
                songItem.downloadPath = (await DataIO.readSettings()).downloadPath;
                this.selectedPlaylist.Songlist.RemoveAt(i);
                this.selectedPlaylist.Songlist.Insert(i, songItem);
                await DataIO.SavePlaylist(this.selectedPlaylist);
                songListViewModel.songListObservable.RemoveAt(i);
                songListViewModel.songListObservable.Insert(i, songItem);
            }
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Image_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var img = (sender as Image);
            if (img.DataContext as Song != null)
            {
                var tmp = Helper.base64toBmp((img.DataContext as Song).Thumbnail);
                img.Stretch = Stretch.Fill;
                img.Source = tmp;
            }
        }

        private void TextBlock_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var txtBlock = sender as TextBlock;
            txtBlock.FontFamily = new FontFamily("Arial");
            txtBlock.Text = "►";
        }

        private void TextBlock_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            var txtBlock = sender as TextBlock;
            txtBlock.FontFamily = new FontFamily("Segoe MDL2 Assets");
            txtBlock.Text = "\uE90B";
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var txtBlock = sender as TextBlock;
            var playlist = txtBlock.DataContext as Playlist;

            searching = false;          
            this.selectedPlaylist = playlist;
            this.musicPlayer.currentPlaylist = playlist;
            this.musicPlayer.currentSong = playlist.Songlist.FirstOrDefault();
            this.musicPlayer.Play();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }
    }
}
