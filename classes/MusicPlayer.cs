﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using System.Timers;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.Storage;
using Windows.Media;
using Windows.Storage.Streams;
using System.IO;

namespace SpotiTube
{
    class MusicPlayer
    {
        public int currentSongDuration { get; set; }
        public int currentSongTime { get; set; }
        public Playlist currentPlaylist { get; set; }
        public Song currentSong { get; set; }
        public bool random { get; set; }
        public bool loop { get; set; }
        public MediaPlayer mPlayer = new MediaPlayer();
        private Timer clock = new Timer();
        
        private TextBlock currentDuration;
        private TextBlock currentTime;
        private Slider timeSlider;
        private ListView MainListView;
        private Button playButton;
        private TextBlock currPlayingLabel;
        private Rectangle currPlayingRect;
        private SystemMediaTransportControls smtc;
        private StackPanel StartLogoStackPanel;

        public MusicPlayer(TextBlock CurrentDuration, TextBlock CurrentTime, Slider TimeSlider, ListView mainList, Button playButton, Rectangle currRect, TextBlock currLabel, StackPanel startlogostackpanel) {
            this.currentDuration = CurrentDuration;
            this.currentTime = CurrentTime;
            this.timeSlider = TimeSlider;
            this.MainListView = mainList;
            this.playButton = playButton;
            this.currPlayingRect = currRect;
            this.currPlayingLabel = currLabel;
            this.StartLogoStackPanel = startlogostackpanel;

            this.mPlayer.MediaEnded += (sender, eventArgs) => {
                if (!loop)
                {
                    skipSong(true);
                }
                else
                {
                    this.Play();
                }
            };

            this.mPlayer.CurrentStateChanged += (player, obj) => {
                Helper.executeInUiThread(() =>
                {
                    if (player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused || player.PlaybackSession.PlaybackState == MediaPlaybackState.Opening ||
                    player.PlaybackSession.PlaybackState == MediaPlaybackState.None)
                    {
                        this.playButton.Content = "\uE768";
                    }
                    else
                    {
                        this.playButton.Content = "\uE769";
                    }
                });

                switch (this.mPlayer.PlaybackSession.PlaybackState)
                {
                    case MediaPlaybackState.Playing:
                        smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
                        break;
                    case MediaPlaybackState.Paused:
                        smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
                        break;
                    default:
                        break;
                }
            };

            this.smtc = SystemMediaTransportControls.GetForCurrentView();
            this.smtc.IsNextEnabled = false;
            this.smtc.IsPlayEnabled = false;
            this.smtc.IsPauseEnabled = false;
            this.smtc.IsPreviousEnabled = false;
            this.smtc.ButtonPressed += Smtc_ButtonPressed;

            this.clock.Elapsed += new ElapsedEventHandler(onTimerEvent);
            this.clock.Interval = 500;
            this.clock.Enabled = true;
        }

        private void Smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    this.resume();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    this.PauseSong();
                    break;
                case SystemMediaTransportControlsButton.Next:
                    this.skipSong(true);
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    this.skipSong(false);
                    break;
                default:
                    break;
            }
        }

        public void onTimerEvent(object source, ElapsedEventArgs e) {
            if (this.mPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                Helper.executeInUiThread(() =>
                { 
                    this.currentTime.Text = $"{this.mPlayer.PlaybackSession.Position.Minutes}:{this.mPlayer.PlaybackSession.Position.Seconds.ToString("D2")}";
                    this.currentDuration.Text = $"{this.mPlayer.PlaybackSession.NaturalDuration.Minutes}:{this.mPlayer.PlaybackSession.NaturalDuration.Seconds.ToString("D2")}";
                    try
                    {
                        this.timeSlider.Value = this.mPlayer.PlaybackSession.Position.TotalSeconds / (this.mPlayer.PlaybackSession.NaturalDuration.TotalSeconds / 100.0);
                    }
                    catch (Exception) { }
                });
            }
        }

        public async Task<String> GetAudioStream(String youtubeURL)
        {
            var client = new YoutubeClient();

            // Get stream manifest
            var manifest = await client.Videos.Streams.GetManifestAsync(youtubeURL);

            // Get Audio stream with highest Bitrate
            var streamInfo = manifest.GetAudioOnly().OrderByDescending(x => x.Bitrate).FirstOrDefault();

            return streamInfo.Url;
        }

        public void skipSong(bool direction = true)
        {
            if (currentPlaylist == null) return;
            if (random) {
                var randInt = new Random().Next(0, currentPlaylist.Songlist.Count);
                this.currentSong = currentPlaylist.Songlist[randInt];
                this.Play();
                this.MainListView.SelectedItem = currentPlaylist.Songlist[randInt];
                return;
            }

            var temp = currentSong;
            var i = currentPlaylist.Songlist.IndexOf(currentPlaylist.Songlist.Where(x => x == temp).FirstOrDefault());

            //forward
            if (direction)
            {
                if (i == currentPlaylist.Songlist.Count-1)
                    i = -1;
                this.currentSong = currentPlaylist.Songlist[++i];
                this.Play();
            }
            //backward
            else {
                if (i == 0)
                    i = 1;
                this.currentSong = currentPlaylist.Songlist[--i];
                this.Play();
            }
            currentSong = currentPlaylist.Songlist[i];
            temp = currentSong;

            Helper.executeInUiThread(() =>
            {
                this.MainListView.SelectedItem = temp;
            });
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

        public async void Play()
        {
            if (currentSong == null && currentPlaylist == null)
                return;

            if (currentPlaylist != null && currentSong == null)
                this.currentSong = currentPlaylist.Songlist.FirstOrDefault();

            if (currentSong.isDownloaded)
            {
                try
                {
                    StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(currentSong.downloadPath);
                    var storageFile = await sf.GetFileAsync(currentSong.DownloadTitle + ".mp3");
                    this.mPlayer.Source = MediaSource.CreateFromStorageFile(storageFile);
                    this.mPlayer.Play();
                }
                catch (FileNotFoundException) {
                    Helper.ErrorDialog("Song not found!", "File was probably deleted. Please download it again.");
                    var i = currentPlaylist.Songlist.IndexOf(currentSong);
                    currentSong.isDownloaded = false;
                    currentPlaylist.Songlist.RemoveAt(i);
                    currentPlaylist.Songlist.Insert(i, currentSong);
                    await DataIO.SavePlaylist(currentPlaylist);
                    skipSong();
                    //TODO: update UI
                    return;
                }
            }
            else
            {
                //if (!Helper.checkIfOnline())
                //    return;
                try
                {
                    var stream = await this.GetAudioStream(currentSong.SongURL);
                    this.mPlayer.Source = MediaSource.CreateFromUri(new Uri(stream));
                    this.mPlayer.Play();
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    return;
                }
                catch (NullReferenceException){
                    //happens if "video" is not playable for some reason
                    Helper.ErrorDialog("Cant Play this Song","For some reason this song can't be played.");
                    skipSong();
                    return;
                }
            }
            
            Helper.executeInUiThread(() =>
            {
                this.StartLogoStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.currPlayingRect.Fill = new ImageBrush
                {
                    ImageSource = Helper.base64toBmp(currentSong.Thumbnail)
                };
                this.currPlayingLabel.Text = currentSong.Title;
            });

            this.smtc.IsNextEnabled = true;
            this.smtc.IsPlayEnabled = true;
            this.smtc.IsPauseEnabled = true;
            this.smtc.IsPreviousEnabled = true;
            this.smtc.DisplayUpdater.Thumbnail = RandomAccessStreamReference.CreateFromStream(await Helper.base64toStream(currentSong.Thumbnail));
            this.smtc.DisplayUpdater.Update();
        }

        public void seek(double percentage)
        {
            this.mPlayer.PlaybackSession.Position = this.mPlayer.PlaybackSession.NaturalDuration.Multiply(percentage / 100);
        }
    }
}