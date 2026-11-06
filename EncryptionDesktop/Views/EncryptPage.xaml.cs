using EncryptionDesktop.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EncryptionDesktop.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EncryptPage : Page
    {
        public EncryptViewModel ViewModel { get; }

        public EncryptPage()
        {
            ViewModel = App.GetService<EncryptViewModel>();
            this.InitializeComponent();
            DataContext = new EncryptViewModel();
        }
    }
}
