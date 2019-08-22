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

        public MusicPlayer(TextBlock CurrentDuration, TextBlock CurrentTime, Slider TimeSlider) {
            this.currentDuration = CurrentDuration;
            this.currentTime = CurrentTime;
            this.timeSlider = TimeSlider;
            this.clock.Elapsed += new ElapsedEventHandler(onTimerEvent);
            this.clock.Interval = 500;
            this.clock.Enabled = true;
        }

        public async void onTimerEvent(object source, ElapsedEventArgs e) {
            if (this.mPlayer.CurrentState == MediaPlayerState.Playing)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                { 
                    this.currentTime.Text = $"{this.mPlayer.Position.Minutes}:{this.mPlayer.Position.Seconds.ToString("D2")}";
                    this.currentDuration.Text = $"{this.mPlayer.NaturalDuration.Minutes}:{this.mPlayer.NaturalDuration.Seconds.ToString("D2")}";
                    this.timeSlider.Value = this.mPlayer.Position.TotalSeconds / (this.mPlayer.NaturalDuration.TotalSeconds / 100.0);
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
            if (random) {
                this.PlaySong(currentPlaylist.Songlist[new Random().Next(0, currentPlaylist.Songlist.Count)].SongURL);
                return;
            }

            var temp = currentSong;
            var i = currentPlaylist.Songlist.FindIndex(x => x == temp);

            //right
            if (direction)
            {
                if (i == currentPlaylist.Songlist.Count)
                    i = -1;
                this.PlaySong(currentPlaylist.Songlist[++i].SongURL);
            }
            else {
                if (i == currentPlaylist.Songlist.Count)
                    i = 1;
                this.PlaySong(currentPlaylist.Songlist[--i].SongURL);
            }
            currentSong = currentPlaylist.Songlist[i];
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
            this.mPlayer.Position = this.mPlayer.NaturalDuration.Multiply(percentage / 100);
        }

    }
}