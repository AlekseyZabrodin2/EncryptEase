using EncryptionDesktop.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EncryptionDesktop.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LicenseGeneratorPage : Page
    {
        public LicenseGeneratorPage()
        {
            InitializeComponent();

            DataContext = App.GetService<LicenseGeneratorViewModel>();
        }

        private void EncryptionPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LicenseGeneratorViewModel viewModel &&
                sender is PasswordBox passwordBox)
            {
                viewModel.EncryptionKey = passwordBox.Password;
            }
        }
    }
}
