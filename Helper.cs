using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Core;
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

        public static async Task<string> InputTextDialogAsync(string title, string buttonText, string boxText = "")
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
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
    }
}
