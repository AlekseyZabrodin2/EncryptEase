using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EncryptionDesktop
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private UISettings _settings;

        public MainWindow()
        {
            this.InitializeComponent();
            Content = null;
            _settings = new UISettings();
        }

        //private void NavigationToView(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        //{
        //    if (args.SelectedItemContainer is NavigationViewItem selectedItem)
        //    {
        //        string selectedTag = selectedItem.Tag.ToString();
        //        //_mainViewModel.NavigationView(selectedTag, contentFrame);
        //    }
        //}
    }
}
