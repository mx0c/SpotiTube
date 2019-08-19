using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotiTube
{

    public class Song
    {
        public String Title{ get; set; }
        public String SongURL { get; set; }
        public String ThumbnailURL { get; set; }
        public DateTime addedAt { get; set; }

        public Song(String Title, String songUrl, DateTime addedAt, String ThumbnailURL)
        {
            this.Title = Title;
            this.SongURL = songUrl;
            this.addedAt = addedAt;
            this.ThumbnailURL = ThumbnailURL;
        }
    }

    class Playlist
    {
        private String Title;
        private List<Song> songlist;
    }
}
