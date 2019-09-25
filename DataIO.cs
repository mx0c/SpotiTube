using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Windows.Storage;

namespace SpotiTube
{
    static class DataIO
    {
        public async static Task<Setting> readSettings() {
            StorageFile file;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync("settings.json");
            }
            catch (FileNotFoundException)
            {
                return await createSettings();
            }
            var content = await FileIO.ReadTextAsync(file);
            Setting setting = JsonConvert.DeserializeObject<Setting>(content);
            return setting;
        }

        public async static Task saveSettings(Setting setting) {
            String json = JsonConvert.SerializeObject(setting);
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync("settings.json");
            await FileIO.WriteTextAsync(file, json);
        }

        private static async Task<Setting> createSettings() {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("settings.json");
            var setting = new Setting();
            setting.downloadPath = ApplicationData.Current.LocalFolder.Path;
            String json = JsonConvert.SerializeObject(setting);
            await FileIO.WriteTextAsync(file, json);
            return setting;
        }

        public async static Task<List<Playlist>> ReadPlaylists()
        {
            StorageFile file;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync("playlists.json");
            }
            catch (FileNotFoundException) {
                return (await createPlaylist());
            }
            var content = await FileIO.ReadTextAsync(file);
            var jArr = (Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(content);
            List<Playlist> playlists = jArr.ToObject<List<Playlist>>();
            return playlists;
        }

        private static async Task<List<Playlist>> createPlaylist()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("playlists.json");
            var newPlaylist = new Playlist();
            newPlaylist.Title = "new Playlist";
            List<Playlist> ret = new List<Playlist>();
            ret.Add(newPlaylist);
            String json = JsonConvert.SerializeObject(ret);
            await FileIO.WriteTextAsync(file, json);
            return ret;
        }

        public async static Task RemovePlaylist(string name)
        {
            var allPlayLists = await ReadPlaylists();
            var i = allPlayLists.FindIndex(x => x.Title == name);
            allPlayLists.RemoveAt(i);
            String json = JsonConvert.SerializeObject(allPlayLists);
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync("playlists.json");
            await FileIO.WriteTextAsync(file, json);
        }

        public async static Task SavePlaylist(Playlist pList, String overridePListTitle = null)
        {
            var allPlayLists = await ReadPlaylists();
            int i;

            if (overridePListTitle == null)
                i = allPlayLists.FindIndex(x => x.Title == pList.Title);
            else
                i = allPlayLists.FindIndex(x => x.Title == overridePListTitle);

            if (i == -1)
                //neue Playlist anlegen
                allPlayLists.Add(pList);
            else
                allPlayLists[i] = pList;

            String json = JsonConvert.SerializeObject(allPlayLists);
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync("playlists.json");
            await FileIO.WriteTextAsync(file, json);
        }        
    }
}
