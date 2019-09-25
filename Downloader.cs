using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using MediaToolkit;
using VideoLibrary;
using MediaToolkit.Model;

namespace SpotiTube
{
    public static class Downloader
    {
        public static async void downloadPlaylist(Playlist plist) {
            foreach (Song s in plist.Songlist) {
                await downloadSong(s);
                ((Song)plist.Songlist.Where(song => song.SongURL == s.SongURL)).isDownloaded = true;
            }
        }

        public static async Task<Boolean> downloadSong(Song song) {
            try
            {           
                var settings = await DataIO.readSettings();
                var path = settings.downloadPath;

                var client = new YoutubeClient();
                var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(YoutubeClient.ParseVideoId(song.SongURL));

                var info = await client.GetVideoMediaStreamInfosAsync(YoutubeClient.ParseVideoId(song.SongURL));
                var audioStream = info.Audio.OrderBy(s => s.Bitrate).Last();
                using (var input = await client.GetMediaStreamAsync(audioStream))
                using (var output = File.Create(path + "/" + song.Title + ".mp3"))
                await input.CopyToAsync(output);
            }
            catch (Exception e){
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }
    }
}
