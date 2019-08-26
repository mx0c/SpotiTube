using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Models.MediaStreams;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using System.Timers;
using Windows.UI.Core;

namespace SpotiTube
{
    class MusicPlayer
    {
        public int currentSongDuration { get; set; }
        public int currentSongTime { get; set; }
        public MediaPlayer mPlayer = new MediaPlayer();
        private Timer clock = new Timer();

        private TextBlock currentDuration;
        private TextBlock currentTime;
        private Slider timeSlider;
        private ListView MainListView;

        public MusicPlayer(TextBlock CurrentDuration, TextBlock CurrentTime, Slider TimeSlider, ListView mainList) {
            this.currentDuration = CurrentDuration;
            this.currentTime = CurrentTime;
            this.timeSlider = TimeSlider;
            this.MainListView = mainList;

            this.clock.Elapsed += new ElapsedEventHandler(onTimerEvent);
            this.clock.Interval = 500;
            this.clock.Enabled = true;
        }

        public async void onTimerEvent(object source, ElapsedEventArgs e) {
            if (this.mPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                { 
                    this.currentTime.Text = $"{this.mPlayer.PlaybackSession.Position.Minutes}:{this.mPlayer.PlaybackSession.Position.Seconds.ToString("D2")}";
                    this.currentDuration.Text = $"{this.mPlayer.PlaybackSession.NaturalDuration.Minutes}:{this.mPlayer.PlaybackSession.NaturalDuration.Seconds.ToString("D2")}";
                    this.timeSlider.Value = this.mPlayer.PlaybackSession.Position.TotalSeconds / (this.mPlayer.PlaybackSession.NaturalDuration.TotalSeconds / 100.0);
                });
            }
        }

        public async Task<String> GetAudioStream(String youtubeURL)
        {
            var client = new YoutubeClient();
            var converter = new YoutubeConverter(client);
            var mediaStreamInfoSet = await client.GetVideoMediaStreamInfosAsync(youtubeURL.Split('=')[1]);
            var audioStreamInfo = mediaStreamInfoSet.Audio.WithHighestBitrate();
            return audioStreamInfo.Url;
        }

        public void skipSong(bool direction, ref Song currentSong, Playlist currentPlaylist,bool random = false)
        {
            if (currentPlaylist == null) return;
            if (random) {
                var randInt = new Random().Next(0, currentPlaylist.Songlist.Count);
                this.PlaySong(currentPlaylist.Songlist[randInt].SongURL);
                this.MainListView.SelectedItem = currentPlaylist.Songlist[randInt];
                return;
            }

            var temp = currentSong;
            var i = currentPlaylist.Songlist.FindIndex(x => x == temp);

            //right
            if (direction)
            {
                if (i == currentPlaylist.Songlist.Count-1)
                    i = -1;
                this.PlaySong(currentPlaylist.Songlist[++i].SongURL);
            }
            else {
                if (i == 0)
                    i = 1;
                this.PlaySong(currentPlaylist.Songlist[--i].SongURL);
            }
            currentSong = currentPlaylist.Songlist[i];
            this.MainListView.SelectedItem = currentSong;
        }

        public void PauseSong()
        {
            this.mPlayer.Pause();
        }

        public void resume() {
            this.mPlayer.Play();
        }

        public void adjustVolume(double percentage)
        {
            this.mPlayer.Volume = percentage / 100.0;
        }

        public async void PlaySong(String youtubeURL)
        {
            var stream = await this.GetAudioStream(youtubeURL); 
            this.mPlayer.Source = MediaSource.CreateFromUri(new Uri(stream));      
            this.mPlayer.Play();
        }

        public void seek(double percentage)
        {
            this.mPlayer.PlaybackSession.Position = this.mPlayer.PlaybackSession.NaturalDuration.Multiply(percentage / 100);
        }

    }
}