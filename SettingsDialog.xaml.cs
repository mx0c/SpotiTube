using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace SpotiTube
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public SettingsDialog()
        {
            this.InitializeComponent();
        }

        //Cancel 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        //Backup
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        //Location
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var path = await Helper.folderPicker();
            if (path != null)
            {
                var settings = await DataIO.readSettings();
                settings.downloadPath = path;
                await DataIO.saveSettings(settings);
            }
            this.Hide();
        }
    }
}
