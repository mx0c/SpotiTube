using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;

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
            this.downloadPath = null;
            this._downloadProgress = 0;
            this.Thumbnail = thumbnailBase64;
        }

        public String downloadPath { get; set; }
        public String Title { get; set; }
        public String DownloadTitle { get; set; }
        public String SongURL { get; set; }
        private String _Thumbnail { get; set; }
        public String Thumbnail {
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
                RaisePropertyChanged("downloadProgress");
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
        public bool IsDownloaded { get; set; }
        public double DownloadProgress { get; set; }
        public ObservableCollection<Song> Songlist { get; set; }

        public Playlist(string name) {
            Title = name;
            IsDownloaded = false;
            DownloadProgress = 0.0;
        }
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
