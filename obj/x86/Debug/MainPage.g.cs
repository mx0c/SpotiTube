﻿#pragma checksum "C:\Users\mariu\Desktop\projs\Spotitube\Spotitube\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FC0D3BB58DC6D26E5DB6BA90EE0B0F89"
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
            case 2: // MainPage.xaml line 82
                {
                    this.Main = (global::Windows.UI.Xaml.Controls.Border)(target);
                }
                break;
            case 3: // MainPage.xaml line 126
                {
                    this.SearchBar = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                    ((global::Windows.UI.Xaml.Controls.TextBox)this.SearchBar).KeyUp += this.SearchBar_KeyUp;
                }
                break;
            case 4: // MainPage.xaml line 130
                {
                    global::Windows.UI.Xaml.Controls.Button element4 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element4).Click += this.LeftButton_Click;
                }
                break;
            case 5: // MainPage.xaml line 131
                {
                    this.PlayButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.PlayButton).Click += this.PlayButton_Click;
                }
                break;
            case 6: // MainPage.xaml line 132
                {
                    global::Windows.UI.Xaml.Controls.Button element6 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element6).Click += this.RightButton_Click;
                }
                break;
            case 7: // MainPage.xaml line 134
                {
                    this.VolumeSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                    ((global::Windows.UI.Xaml.Controls.Slider)this.VolumeSlider).ValueChanged += this.VolumeSlider_Changed;
                }
                break;
            case 8: // MainPage.xaml line 135
                {
                    this.TimeSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                }
                break;
            case 9: // MainPage.xaml line 136
                {
                    this.CurrentTime = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 10: // MainPage.xaml line 137
                {
                    this.CurrentDuration = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 11: // MainPage.xaml line 83
                {
                    this.MainViewer = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                }
                break;
            case 12: // MainPage.xaml line 84
                {
                    this.MainListView = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    ((global::Windows.UI.Xaml.Controls.ListView)this.MainListView).ItemClick += this.onItemClicked;
                }
                break;
            case 14: // MainPage.xaml line 101
                {
                    global::Windows.UI.Xaml.Controls.Grid element14 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    ((global::Windows.UI.Xaml.Controls.Grid)element14).DoubleTapped += this.Grid_DoubleTapped;
                    ((global::Windows.UI.Xaml.Controls.Grid)element14).DragStarting += this.Grid_DragStarting;
                }
                break;
            case 15: // MainPage.xaml line 38
                {
                    global::Windows.UI.Xaml.Controls.Button element15 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)element15).Click += this.AddPlaylistButton_Click;
                }
                break;
            case 16: // MainPage.xaml line 41
                {
                    this.PlaylistList = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    ((global::Windows.UI.Xaml.Controls.ListView)this.PlaylistList).ItemClick += this.PlaylistList_ItemClick;
                }
                break;
            case 17: // MainPage.xaml line 44
                {
                    global::Windows.UI.Xaml.Controls.StackPanel element17 = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                    ((global::Windows.UI.Xaml.Controls.StackPanel)element17).RightTapped += this.StackPanel_RightTapped;
                    ((global::Windows.UI.Xaml.Controls.StackPanel)element17).Drop += this.StackPanel_Drop;
                    ((global::Windows.UI.Xaml.Controls.StackPanel)element17).DragEnter += this.StackPanel_DragEnter;
                }
                break;
            case 18: // MainPage.xaml line 47
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element18 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element18).Click += this.EditButton_Click;
                }
                break;
            case 19: // MainPage.xaml line 48
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element19 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element19).Click += this.DeleteButton_Click;
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

