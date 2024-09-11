using CommunityToolkit.Mvvm.ComponentModel;
using EncryptionDesktop.Services;
using EncryptionDesktop.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ApplicationSettings;

namespace EncryptionDesktop.ViewModels
{
    public partial class ShellViewModel : ObservableRecipient
    {

        private readonly Dictionary<string, Type> _pagetages = new Dictionary<string, Type>
        {
           { "EncryptPage", typeof(EncryptPage) },
           { "DecryptPage", typeof(DecryptPage) },
           { "Settings", typeof(EncryptionSettingsPage) }
        };


        public void NavigationView(string pageTag, Frame frame)
        {
            if (_pagetages.TryGetValue(pageTag, out Type pageType))
            {
                frame.Navigate(pageType);
            }
        }
    }
}
