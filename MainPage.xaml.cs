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

namespace App1
{
    public sealed partial class MainPage : Page
    {
        private MusicPlayer musicPlayer;
        public MainPage()
        {
            this.InitializeComponent();
            this.musicPlayer = new MusicPlayer(this.CurrentDuration, this.CurrentTime, this.TimeSlider);
            this.TimeSlider.AddHandler(PointerReleasedEvent, new PointerEventHandler(TimeSlider_OnPointerRelease),true);
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
            
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.musicPlayer.mPlayer.CurrentState == MediaPlayerState.Stopped || this.musicPlayer.mPlayer.CurrentState == MediaPlayerState.Closed)
            {
                this.PlayButton.Content = "\uE769";
                this.musicPlayer.PlaySong(this.SearchBar.Text);
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
                var items = new VideoSearch();
                foreach (var item in items.SearchQuery(this.SearchBar.Text, 1))
                {
                    this.MainListView.Items.Add(new Song(item.Title,item.Url,new DateTime(),item.Thumbnail,item.Duration));
                    
                }
            }
        }

        private void onItemClicked(object sender, ItemClickEventArgs e)
        {
            Song selectedSong = (Song)e.ClickedItem;
            this.PlayButton.Content = "\uE769";
            this.musicPlayer.PlaySong(selectedSong.SongURL);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
