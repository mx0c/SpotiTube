using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace SpotiTube
{
    public class Song : INotifyPropertyChanged 
    {
        public Song(String Title, String songUrl, String addedAt, String thumbnailBase64, String duration)
        {
            StringWriter myWriter = new StringWriter();
            HttpUtility.HtmlDecode(Title, myWriter);
            var decodedTitle = myWriter.ToString();
            this.Title = decodedTitle;

            var invalidChars = new string[] { "<",">",":","/","\\","|","*","?","\""};
            foreach (var c in invalidChars)
            {
                Title = Title.Replace(c, string.Empty);
            }
            this.DownloadTitle = Title;

            this.SongURL = songUrl;
            this.addedAt = addedAt;
            this.duration = duration;
            this.isDownloaded = false;
            this.isDownloading = false;
            this._downloadProgress = 0;
            this.ThumbnailBase64 = thumbnailBase64;

            Helper.executeThreadSafe(()=>this.init());
        }

        public async void init() {
            this.Thumbnail = await Helper.base64toBmp(this.ThumbnailBase64);           
        }
      
        public String Title { get; set; }
        public String DownloadTitle { get; set; }
        public String SongURL { get; set; }
        public String ThumbnailBase64 { get; set; }
        [JsonIgnore]
        private BitmapImage _Thumbnail { get; set; }
        [JsonIgnore]
        public BitmapImage Thumbnail {
            get { return _Thumbnail; }
            set {
                _Thumbnail = value;
                RaisePropertyChanged("Thumbnail");    
            }
        }
        public String addedAt { get; set; }
        public String duration { get; set; }
        public Boolean isDownloaded { get; set; }
        public Boolean isDownloading { get; set; }
        private double _downloadProgress;
        public double downloadProgress {
            get { return _downloadProgress; }
            set
            {
                _downloadProgress = value;
                RaisePropertyChanged();
            }
        }     

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class Playlist
    {
        public String Title { get; set; }
        public ObservableCollection<Song> Songlist { get; set; }
    }

    class Setting
    {
        public Setting()
        {
            this.downloadPath = "";
            this.volumePerc = 0;
        }

        public String downloadPath { get; set; }
        public int volumePerc { get; set; }
    }
}
