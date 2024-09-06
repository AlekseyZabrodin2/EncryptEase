using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EncryptionDesktop.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace EncryptionDesktop.ViewModels
{
    public partial class EncryptViewModel : ObservableObject
    {

        [ObservableProperty]
        private string? _inputEncryptingText;

        [ObservableProperty]
        private string? _encryptionKey;

        [ObservableProperty]
        private string? _saltText;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EncryptCommand))]
        public bool _isEnabledButtons;

        [ObservableProperty]
        private string? _resultEncryptingText;

        [ObservableProperty]
        private int _sizeKey;

        [ObservableProperty]
        private int _numberOfIterations;

        [ObservableProperty]
        private List<HashAlgorithmName> _algorithmName;

        [ObservableProperty]
        private HashAlgorithmName _selectedAlgorithmName;

        [ObservableProperty]
        private EncryptionService _serviceEncryption;

        [ObservableProperty]
        private string? _resultStatus;

        [ObservableProperty]
        private bool _infoBarIsOpen;

        [ObservableProperty]
        private InfoBarSeverity _infoBarSeverity;

        private bool _bit16IsChecked;
        public bool Bit16IsChecked
        {
            get => _bit16IsChecked;
            set
            {
                SetProperty(ref _bit16IsChecked, value);
                if (_bit16IsChecked == true)
                {
                    Bit32IsChecked = !_bit16IsChecked;
                    Bit24IsChecked = !_bit16IsChecked;
                    SizeKey = 16;
                }
            }
        }

        private bool _bit24IsChecked;
        public bool Bit24IsChecked
        {
            get => _bit24IsChecked;
            set
            {
                SetProperty(ref _bit24IsChecked, value);
                if (_bit24IsChecked == true)
                {
                    Bit16IsChecked = !_bit24IsChecked;
                    Bit32IsChecked = !_bit24IsChecked;
                    SizeKey = 24;
                }
            }
        }

        private bool _bit32IsChecked;        
        public bool Bit32IsChecked
        {
            get => _bit32IsChecked;
            set
            {
                SetProperty(ref _bit32IsChecked, value);
                if (_bit32IsChecked == true)
                {
                    Bit16IsChecked = !_bit32IsChecked;
                    Bit24IsChecked = !_bit32IsChecked;
                    SizeKey = 32;
                }
            }
        }

        private bool _iterations10000IsChecked;
        public bool Iterations10000IsChecked
        {
            get => _iterations10000IsChecked;
            set
            {
                SetProperty(ref _iterations10000IsChecked, value);
                if (_iterations10000IsChecked == true)
                {
                    Iterations25000IsChecked = !_iterations10000IsChecked;
                    NumberOfIterations = 10000;
                }
            }
        }

        private bool _iterations25000IsChecked;
        public bool Iterations25000IsChecked
        {
            get => _iterations25000IsChecked;
            set
            {
                SetProperty(ref _iterations25000IsChecked, value);
                if (_iterations25000IsChecked == true)
                {
                    Iterations10000IsChecked = !_iterations25000IsChecked;
                    NumberOfIterations = 25000;
                }
            }
        }



        public EncryptViewModel()
        {
            AlgorithmName = new List<HashAlgorithmName> { HashAlgorithmName.MD5, HashAlgorithmName.SHA256, HashAlgorithmName.SHA3_512, HashAlgorithmName.SHA3_384 };
            PropertyChanged += EncryptProperties_PropertyChanged;
        }

        private void EncryptProperties_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InputEncryptingText) || 
                e.PropertyName == nameof(SaltText) ||
                e.PropertyName == nameof(EncryptionKey))
            {
                if (string.IsNullOrEmpty(InputEncryptingText) ||
                    string.IsNullOrEmpty(SaltText) ||
                    string.IsNullOrEmpty(EncryptionKey))
                {
                    IsEnabledButtons = false;
                    InfoBarIsOpen = true;
                    InfoBarSeverity = InfoBarSeverity.Error;
                    ResultStatus = $"One of the fields is empty";
                }
                else
                {
                    IsEnabledButtons = true;
                    InfoBarIsOpen = false;
                    ResultStatus = string.Empty;
                }
            }
        }



        [RelayCommand(CanExecute = nameof(IsEnabledButtons))]
        private void Encrypt()
        {
            ServiceEncryption = new();
            
            try
            {
                ResultEncryptingText = ServiceEncryption.Encrypt(InputEncryptingText!, SaltText!, EncryptionKey!);

                InfoBarIsOpen = true;
                InfoBarSeverity = InfoBarSeverity.Success;
                ResultStatus = "Encryption completed successfully";
            }
            catch (Exception ex)
            {
                InfoBarIsOpen = true;
                InfoBarSeverity = InfoBarSeverity.Error;
                ResultStatus = ex.Message.ToString();
            }
        }

        [RelayCommand]
        private void CopyResultInClipboard()
        {
            if (!string.IsNullOrEmpty(ResultEncryptingText))
            {
                var package = new DataPackage();
                package.SetText(ResultEncryptingText);
                Clipboard.SetContent(package);
            }
        }

        [RelayCommand]
        private void CopySaltInClipboard()
        {
            if (!string.IsNullOrEmpty(SaltText))
            {
                var package = new DataPackage();
                package.SetText(SaltText);
                Clipboard.SetContent(package);
            }
        }

        [RelayCommand]
        private void CopyKeyInClipboard()
        {
            if (!string.IsNullOrEmpty(EncryptionKey))
            {
                var package = new DataPackage();
                package.SetText(EncryptionKey);
                Clipboard.SetContent(package);
            }
        }
    }
}
