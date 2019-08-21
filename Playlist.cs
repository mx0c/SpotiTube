using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiTube
{

    public class Song
    {
        public String Title { get; set; }
        public String SongURL { get; set; }
        public String ThumbnailURL { get; set; }
        public String addedAt { get; set; }
        public String duration { get; set; }

        public Song(String Title, String songUrl, String addedAt, String ThumbnailURL, String duration)
        {
            this.Title = Title;
            this.SongURL = songUrl;
            this.addedAt = addedAt;
            this.ThumbnailURL = ThumbnailURL;
            this.duration = duration;
        }
    }

    class Playlist
    {
        public String Title { get; set; }
        public List<Song> Songlist { get; set; }
    }
}
