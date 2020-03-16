using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace SpotiTube
{
    public sealed partial class LoadingDialog : ContentDialog
    {
        private BitmapImage ImageSource { get; set; }
        public LoadingDialog()
        {
            this.InitializeComponent();
            ImageSource = new BitmapImage(new Uri("ms-appx:///Spotitube/Assets/loading.gif"));
        }
    }
}
