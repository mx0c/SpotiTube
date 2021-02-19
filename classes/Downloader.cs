using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;

namespace SpotiTube
{
    public static class Downloader
    {
        public static async void downloadPlaylist(Playlist plist) {
            var songCount = plist.Songlist.Count;
            int songsFinished = 0;
            foreach (Song s in plist.Songlist) {
                if (!s.isDownloaded)
                {
                    var i = plist.Songlist.IndexOf(s);
                    await downloadSong(s, null);
                    var songTmp = ((Song)plist.Songlist.Where(song => song.SongURL == s.SongURL));
                    songTmp.isDownloaded = true;
                    songTmp.isDownloading = false;
                    songTmp.downloadPath = (await DataIO.readSettings()).downloadPath;
                    plist.Songlist.RemoveAt(i);
                    plist.Songlist.Insert(i, songTmp);
                    await DataIO.SavePlaylist(plist);
                }
                songsFinished++;
                plist.DownloadProgress = songsFinished / songCount;
            }
        }

        public static async Task<Boolean> downloadSong(Song song, Progress<double> progHandler) {
            try
            {           
                var settings = await DataIO.readSettings();
                var path = settings.downloadPath;
                var client = new YoutubeClient();

                // Get stream manifest
                var manifest = await client.Videos.Streams.GetManifestAsync(song.SongURL);

                // Get Audio stream with highest Bitrate
                var streamInfo = manifest.GetAudioOnly().OrderByDescending(x => x.Bitrate).FirstOrDefault();

                // Download stream to mp3 file
                using (var memoryStream = new MemoryStream())
                {
                    if(progHandler != null)
                        await client.Videos.Streams.DownloadAsync(streamInfo, $"video.{streamInfo.Container}", progress: progHandler);               
                    else
                        await client.Videos.Streams.DownloadAsync(streamInfo, $"video.{streamInfo.Container}");
                    byte[] bArray = memoryStream.ToArray();

                    StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(path);
                    var storageFile = await sf.CreateFileAsync(song.DownloadTitle + ".mp3");

                    await FileIO.WriteBytesAsync(storageFile, bArray);              }           
            }
            catch (Exception){
                return false;
            }
            return true;
        }
    }


}
