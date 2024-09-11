using CommunityToolkit.Mvvm.ComponentModel;
using EncryptionDesktop.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDesktop.ViewModels
{
    public partial class SettingPageViewModel : ObservableObject
    {
        public enum ThemeModeEnum
        {
            Light,
            Dark,
            Use_System_Setting
        }

        [ObservableProperty]
        private List<ThemeModeEnum> _themeModes;

        [ObservableProperty]
        public ElementTheme _theme = ElementTheme.Default;

        [ObservableProperty]
        private ThemeModeEnum _selectedThemeMode;


        public SettingPageViewModel()
        {
            ThemeModes = Enum.GetValues(typeof(ThemeModeEnum)).Cast<ThemeModeEnum>().ToList();
            SelectedThemeMode = ThemeModeEnum.Use_System_Setting;
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ChangeTheme(SelectedThemeMode);
        }

        public void ChangeTheme(ThemeModeEnum selectedTheme)
        {
            
            if (selectedTheme == ThemeModeEnum.Light)
            {
                if (App.MainWindow.Content is FrameworkElement rootElement)
                {

                    Theme = ElementTheme.Light;
                    rootElement.RequestedTheme = Theme;
                    TitleBarHelper.UpdateTitleBar(Theme);
                }
            }
            else if (selectedTheme == ThemeModeEnum.Dark)
            {
                if (App.MainWindow.Content is FrameworkElement rootElement)
                {
                    Theme = ElementTheme.Dark;
                    rootElement.RequestedTheme = Theme;
                    TitleBarHelper.UpdateTitleBar(Theme);
                }
            }
            else
            {
                if (App.MainWindow.Content is FrameworkElement rootElement)
                {
                    Theme = ElementTheme.Default;
                    rootElement.RequestedTheme = Theme;
                    TitleBarHelper.UpdateTitleBar(Theme);
                }
            }
        }
    }
}
