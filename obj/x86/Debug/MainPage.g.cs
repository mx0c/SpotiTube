﻿#pragma checksum "C:\Users\mariu\Desktop\projs\Spotitube\Spotitube\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6D45A0C07EC48B1FB8FB5C568E3B8F5E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace App1
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // MainPage.xaml line 16
                {
                    global::Windows.UI.Xaml.Controls.Grid element2 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    ((global::Windows.UI.Xaml.Controls.Grid)element2).SizeChanged += this.Grid_SizeChanged;
                }
                break;
            case 3: // MainPage.xaml line 27
                {
                    this.navbar = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 4: // MainPage.xaml line 77
                {
                    this.SearchBar = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.SearchBar).KeyUp += this.SearchBar_KeyUp;
                }
                break;
            case 5: // MainPage.xaml line 80
                {
                    this.Main = (global::Windows.UI.Xaml.Controls.Border)(target);
                }
                break;
            case 6: // MainPage.xaml line 147
                {
                    this.randomButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.randomButton).Click += this.RandomButton_Click;
                }
                break;
            case 7: // MainPage.xaml line 148
                {
                    this.loopButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.loopButton).Click += this.LoopButton_Click;
                }
                break;
            case 8: // MainPage.xaml line 149
                {
                    global::Windows.UI.Xaml.Controls.Button element8 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element8).Click += this.LeftButton_Click;
                }
                break;
            case 9: // MainPage.xaml line 150
                {
                    this.PlayButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.PlayButton).Click += this.PlayButton_Click;
                }
                break;
            case 10: // MainPage.xaml line 151
                {
                    global::Windows.UI.Xaml.Controls.Button element10 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element10).Click += this.RightButton_Click;
                }
                break;
            case 11: // MainPage.xaml line 153
                {
                    this.VolumeSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                    ((global::Windows.UI.Xaml.Controls.Slider)this.VolumeSlider).PointerCaptureLost += this.VolumeSlider_PointerCaptureLost;
                    ((global::Windows.UI.Xaml.Controls.Slider)this.VolumeSlider).ValueChanged += this.VolumeSlider_Changed;
                }
                break;
            case 12: // MainPage.xaml line 154
                {
                    this.TimeSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                }
                break;
            case 13: // MainPage.xaml line 155
                {
                    this.CurrentTime = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 14: // MainPage.xaml line 156
                {
                    this.CurrentDuration = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 15: // MainPage.xaml line 82
                {
                    this.MainListView = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    ((global::Windows.UI.Xaml.Controls.ListView)this.MainListView).RightTapped += this.MainListView_RightTapped;
                    ((global::Windows.UI.Xaml.Controls.ListView)this.MainListView).ItemClick += this.onItemClicked;
                }
                break;
            case 16: // MainPage.xaml line 85
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element16 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element16).Click += this.DeleteSongButton_Click;
                }
                break;
            case 17: // MainPage.xaml line 86
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element17 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element17).Click += this.DownloadSong_Click;
                }
                break;
            case 19: // MainPage.xaml line 107
                {
                    global::Windows.UI.Xaml.Controls.Grid element19 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    ((global::Windows.UI.Xaml.Controls.Grid)element19).DoubleTapped += this.Grid_DoubleTapped;
                    ((global::Windows.UI.Xaml.Controls.Grid)element19).DragStarting += this.Grid_DragStarting;
                }
                break;
            case 20: // MainPage.xaml line 115
                {
                    global::Windows.UI.Xaml.Controls.Image element20 = (global::Windows.UI.Xaml.Controls.Image)(target);
                    ((global::Windows.UI.Xaml.Controls.Image)element20).PointerEntered += this.Ellipse_PointerEntered;
                    ((global::Windows.UI.Xaml.Controls.Image)element20).PointerExited += this.Ellipse_PointerExited;
                    ((global::Windows.UI.Xaml.Controls.Image)element20).Tapped += this.Ellipse_Tapped;
                }
                break;
            case 21: // MainPage.xaml line 39
                {
                    global::Windows.UI.Xaml.Controls.Button element21 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element21).Click += this.AddPlaylistButton_Click;
                }
                break;
            case 22: // MainPage.xaml line 67
                {
                    this.currPlayingSongRect = (global::Windows.UI.Xaml.Shapes.Rectangle)(target);
                }
                break;
            case 23: // MainPage.xaml line 68
                {
                    this.scrollviewer = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                    ((global::Windows.UI.Xaml.Controls.ScrollViewer)this.scrollviewer).Loaded += this.Scrollviewer_Loaded;
                    ((global::Windows.UI.Xaml.Controls.ScrollViewer)this.scrollviewer).Unloaded += this.Scrollviewer_Unloaded;
                }
                break;
            case 24: // MainPage.xaml line 71
                {
                    this.currPlayingSongLabel = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 25: // MainPage.xaml line 42
                {
                    this.PlaylistList = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    ((global::Windows.UI.Xaml.Controls.ListView)this.PlaylistList).RightTapped += this.StackPanel_RightTapped;
                    ((global::Windows.UI.Xaml.Controls.ListView)this.PlaylistList).ItemClick += this.PlaylistList_ItemClick;
                }
                break;
            case 26: // MainPage.xaml line 45
                {
                    this.EditButton = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)this.EditButton).Click += this.EditButton_Click;
                }
                break;
            case 27: // MainPage.xaml line 46
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element27 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element27).Click += this.DeleteButton_Click;
                }
                break;
            case 28: // MainPage.xaml line 47
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element28 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element28).Click += this.DownloadPlaylist_Click;
                }
                break;
            case 29: // MainPage.xaml line 52
                {
                    global::Windows.UI.Xaml.Controls.StackPanel element29 = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                    ((global::Windows.UI.Xaml.Controls.StackPanel)element29).Drop += this.StackPanel_Drop;
                    ((global::Windows.UI.Xaml.Controls.StackPanel)element29).DragEnter += this.StackPanel_DragEnter;
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

