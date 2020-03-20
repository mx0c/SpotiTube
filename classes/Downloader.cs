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
                var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(YoutubeClient.ParseVideoId(song.SongURL));

                MediaStreamInfoSet info = await client.GetVideoMediaStreamInfosAsync(YoutubeClient.ParseVideoId(song.SongURL));             
                var audioStreamInfo = info.Audio.OrderBy(s => s.Bitrate).Last();

                using (var memoryStream = new MemoryStream())
                {
                    if(progHandler != null)
                        await client.DownloadMediaStreamAsync(audioStreamInfo, memoryStream, progress:progHandler);
                    else
                        await client.DownloadMediaStreamAsync(audioStreamInfo, memoryStream);
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
