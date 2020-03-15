using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SpotiTube
{
    public static class Downloader
    {
        //public static async void downloadPlaylist(Playlist plist) {
        //    foreach (Song s in plist.Songlist) {
        //        await downloadSong(s);
        //        ((Song)plist.Songlist.Where(song => song.SongURL == s.SongURL)).isDownloaded = true;
        //    }
        //}

        public static async Task<Boolean> downloadSong(Song song, Progress<double> progHandler) {
            try
            {           
                var settings = await DataIO.readSettings();
                var path = settings.downloadPath;

                var client = new YoutubeClient();
                var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(YoutubeClient.ParseVideoId(song.SongURL));

                MediaStreamInfoSet info = await client.GetVideoMediaStreamInfosAsync(YoutubeClient.ParseVideoId(song.SongURL));             
                var audioStreamInfo = info.Audio.OrderBy(s => s.Bitrate).Last();

                using (var memoryStream = new MemoryStream())
                {
                    await client.DownloadMediaStreamAsync(audioStreamInfo, memoryStream, progress:progHandler);
                    byte[] bArray = memoryStream.ToArray();

                    StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(path);
                    var storageFile = await sf.CreateFileAsync(song.DownloadTitle + ".mp3");

                    await FileIO.WriteBytesAsync(storageFile, bArray);              }           
            }
            catch (Exception e){
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }
    }


}
