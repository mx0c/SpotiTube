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
        private bool maximized = false;

        public MainPage()
        {
            this.InitializeComponent();
            this.musicPlayer = new MusicPlayer(this.CurrentDuration, this.CurrentTime, this.TimeSlider);
            this.TimeSlider.AddHandler(PointerReleasedEvent, new PointerEventHandler(TimeSlider_OnPointerRelease),true);
            Task.Run(() => this.loadPlaylists()).Wait();
        }

        private async void loadPlaylists()
        {
            try
            {
                List<Playlist> lists = await DataIO.ReadPlaylists();
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (Playlist list in lists)
                    {
                        this.PlaylistList.Items.Add(list);
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
            var i = this.currentlyPlayingPlaylist.Songlist.FindIndex(x => x == this.currentlyPlayingSong);
            if (i == 0)
                i = 1;
            this.musicPlayer.PlaySong(this.currentlyPlayingPlaylist.Songlist[--i].SongURL);
            this.currentlyPlayingSong = this.currentlyPlayingPlaylist.Songlist[i];
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {    
            var i = this.currentlyPlayingPlaylist.Songlist.FindIndex(x => x == this.currentlyPlayingSong);
            if (i == this.currentlyPlayingPlaylist.Songlist.Count)
                i = -1;
            this.musicPlayer.PlaySong(this.currentlyPlayingPlaylist.Songlist[++i].SongURL);
            this.currentlyPlayingSong = this.currentlyPlayingPlaylist.Songlist[i];
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.musicPlayer.mPlayer.CurrentState == MediaPlayerState.Stopped || this.musicPlayer.mPlayer.CurrentState == MediaPlayerState.Closed)
            {
                this.PlayButton.Content = "\uE769";
                this.musicPlayer.PlaySong(this.latestSelectedSong.SongURL);
            }
            else if (this.musicPlayer.mPlayer.CurrentState == MediaPlayerState.Paused)
            {
                this.PlayButton.Content = "\uE769";
                this.musicPlayer.resume();
            }
            else
            {
                this.PlayButton.Content = "\uE768";
                this.musicPlayer.PauseSong();
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
                    this.MainListView.Items.Add(new Song(item.Title,item.Url,"",item.Thumbnail,item.Duration));                  
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

        private void StackPanel_Drop(object sender, DragEventArgs e)
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
            DataIO.SavePlaylist(pl);
        }

        private void StackPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "Zu Playlist hinzufügen";
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout((StackPanel)sender);
            flyoutBase.ShowAt((StackPanel)sender);
            this.selectedPlaylist = ((StackPanel)sender).DataContext as Playlist;
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
            this.PlayButton.Content = "\uE769";
            var song = ((sender as Grid).DataContext as Song);
            this.musicPlayer.PlaySong(song.SongURL);
            this.currentlyPlayingPlaylist = this.selectedPlaylist;
            this.currentlyPlayingSong = song; ;
        }

        private async Task<string> InputTextDialogAsync(string title,string buttonText)
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
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

        private void MaximizeMinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.maximized)
            {
                Grid.SetColumn(this.mainGrid, 1);
                Grid.SetColumnSpan(this.mainGrid, 2);
                Grid.SetColumnSpan(this.navbar, 1);
                this.navbar.Visibility = Visibility.Collapsed;
                this.maximizeButton.Visibility = Visibility.Visible;
                this.maximized = true;
            }
            else
            {
                Grid.SetColumn(this.mainGrid, 2);
                Grid.SetColumnSpan(this.navbar, 2);
                Grid.SetColumnSpan(this.mainGrid, 1);
                this.navbar.Visibility = Visibility.Visible;
                this.maximizeButton.Visibility = Visibility.Collapsed;
                this.maximized = false;
            }
        }
    }
}
