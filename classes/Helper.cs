using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SpotiTube
{
    static class Helper
    {
        public static async void executeInUiThread(Action function)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                function();
            });
        }

        public static async void ErrorDialog(string title, string content) {
            Helper.executeInUiThread(async () =>
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = title,
                    Content = content,
                    CloseButtonText = "Ok"
                };
                try
                {
                    await dialog.ShowAsync();
                }
                catch (Exception) { };
            });
        }

        public static async Task<string> InputTextDialogAsync(string title, string buttonText, string boxText = "")
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = true;
            inputTextBox.Height = 32;
            inputTextBox.Text = boxText;

            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = buttonText;
            dialog.SecondaryButtonText = "Cancel";

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return "";
        }

        public static async Task<Boolean> ButtonDialog(string content) {
            ContentDialog dialog = new ContentDialog();
            dialog.Content = content;
            dialog.IsSecondaryButtonEnabled = false;
            dialog.PrimaryButtonText = "OK";
            await dialog.ShowAsync();
            return true;
        }

        public static async Task<string> folderPicker()
        {
                var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                folderPicker.FileTypeFilter.Add("*");
                Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    Windows.Storage.AccessCache.StorageApplicationPermissions.
                    FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                    return folder.Path;
                }
                else
                    return null;    
        }

        public static async Task<StorageFile> filePicker() {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");
            return await openPicker.PickSingleFileAsync();
        }

        public static BitmapImage base64toBmp(String data) {
            byte[] bData = Convert.FromBase64String(data);
            var ims = new InMemoryRandomAccessStream();
            var dataWriter = new DataWriter(ims);

            dataWriter.WriteBytes(bData);
            dataWriter.StoreAsync();
            ims.Seek(0);

            var img = new BitmapImage();
            img.SetSource(ims);
            return img;
        }

        public async static Task<IRandomAccessStream> base64toStream(String data)
        {
            byte[] bData = Convert.FromBase64String(data);
            var ims = new InMemoryRandomAccessStream();
            var dataWriter = new DataWriter(ims);
            dataWriter.WriteBytes(bData);
            await dataWriter.StoreAsync();
            ims.Seek(0);
            return ims;
        }

        public static String ImageURLToBase64(String url) {
            using (WebClient client = new WebClient())
            {
                byte[] data = client.DownloadData(url);
                return Convert.ToBase64String(data);
            }
        }

        public static Boolean checkIfOnline() {
            try { 
                Ping ping = new Ping();
                PingReply pr = ping.Send("8.8.8.8", 500);
                return (pr.Status == IPStatus.Success);
            }catch(Exception) {
                return false;
            }
        }
    }
}
