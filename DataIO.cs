using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace SpotiTube
{
    static class DataIO
    {
        public static List<Playlist> ReadPlaylists()
        {
            List<Playlist> playlists = new List<Playlist>();
            var playlistFileNames = Directory.GetFiles($"{System.Environment.CurrentDirectory}\\playlists");
            foreach (var filename in playlistFileNames)
            {
                var jsonString = File.ReadAllText($"{System.Environment.CurrentDirectory}\\playlists\\{filename}");
                Playlist playlist = (Playlist)JsonConvert.DeserializeObject(jsonString);
                playlists.Append(playlist);
            }
            return playlists;
        }

        public static void SavePlaylist(Playlist[] lists)
        {
            foreach(Playlist list in lists)
            {
                String json = JsonConvert.SerializeObject(lists);
                File.WriteAllText($"{System.Environment.CurrentDirectory}\\playlists\\{list.Title}", json);
            }
        }        
    }
}
