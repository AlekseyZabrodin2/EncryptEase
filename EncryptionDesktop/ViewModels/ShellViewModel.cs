using CommunityToolkit.Mvvm.ComponentModel;
using EncryptionDesktop.Views;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace EncryptionDesktop.ViewModels
{
    public partial class ShellViewModel : ObservableRecipient
    {

        private readonly Dictionary<string, Type> _pagetages = new Dictionary<string, Type>
        {
           { "EncryptPage", typeof(EncryptPage) },
           { "DecryptPage", typeof(DecryptPage) },
           { "LicenseGeneratorPage", typeof(LicenseGeneratorPage) },
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
