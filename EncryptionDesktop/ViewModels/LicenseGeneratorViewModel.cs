using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EncryptionDesktop.Models;
using EncryptionDesktop.Services;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace EncryptionDesktop.ViewModels;

public partial class LicenseGeneratorViewModel : ObservableObject
{
    private readonly LicenseKeyService _licenseKeyService;

    [ObservableProperty]
    private string _customer = string.Empty;

    [ObservableProperty]
    private string _product = "DataBaseGenerator";

    [ObservableProperty]
    private string _licenseId = GenerateLicenseId();

    [ObservableProperty]
    private int _selectedLicenseType;

    [ObservableProperty]
    private DateTimeOffset? _expiryDate = DateTimeOffset.Now.AddYears(1);

    [ObservableProperty]
    private string _encryptionKey = string.Empty;

    [ObservableProperty]
    private string _selectedFolder = string.Empty;

    [ObservableProperty]
    private bool _infoBarIsOpen;

    [ObservableProperty]
    private string _resultStatus = string.Empty;

    [ObservableProperty]
    private LicensePayload _payLoad;

    [ObservableProperty]
    private InfoBarSeverity _infoBarSeverity = InfoBarSeverity.Informational;

    [ObservableProperty]
    private Symbol _copyPathIcon = Symbol.Copy;

    [ObservableProperty]
    private string _licenseContent = string.Empty;

    public bool IsExpiryDateEnabled => SelectedLicenseType == 1;

    [ObservableProperty]
    private string _selectedLicensePath = string.Empty;



    public LicenseGeneratorViewModel(LicenseKeyService licenseKeyService)
    {
        _licenseKeyService = licenseKeyService;
    }



    public string DisplayFolder => string.IsNullOrEmpty(SelectedFolder)
        ? string.Empty
        : SelectedFolder.Length > 45
            ? "..." + SelectedFolder[^32..]
            : SelectedFolder;

    partial void OnSelectedFolderChanged(string value)
    {
        OnPropertyChanged(nameof(DisplayFolder));
    }

    public string DisplayLicensePath => string.IsNullOrEmpty(SelectedLicensePath)
        ? string.Empty
        : SelectedLicensePath.Length > 45
            ? "..." + SelectedLicensePath[^32..]
            : SelectedLicensePath;

    partial void OnSelectedLicensePathChanged(string value)
    {
        OnPropertyChanged(nameof(DisplayLicensePath));
    }

    partial void OnSelectedLicenseTypeChanged(int value)
    {
        OnPropertyChanged(nameof(IsExpiryDateEnabled));

        if (value == 0)
            ExpiryDate = null;
        else
            ExpiryDate ??= DateTimeOffset.Now.AddYears(1);
    }

    [RelayCommand]
    private void GenerateLicense()
    {
        if (string.IsNullOrWhiteSpace(Customer))
        {
            ShowMessage("Enter the customer name.", InfoBarSeverity.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedFolder))
        {
            ShowMessage("Select a folder.", InfoBarSeverity.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(EncryptionKey))
        {
            ShowMessage("Enter the encryption password.", InfoBarSeverity.Error);
            return;
        }

        if (SelectedLicenseType == 1 && !ExpiryDate.HasValue)
        {
            ShowMessage("Select the license expiration date.", InfoBarSeverity.Error);
            return;
        }

        try
        {
            var payload = new LicensePayload
            {
                Customer = Customer.Trim(),
                Product = Product.Trim(),
                LicenseId = LicenseId,
                IssuedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = SelectedLicenseType == 1
                    ? ExpiryDate!.Value.UtcDateTime
                    : null
            };

            _licenseKeyService.GenerateLicense(
                Path.Combine(SelectedFolder, "private_key.enc"),
                Path.Combine(SelectedFolder, "activation.lic"),
                EncryptionKey,
                payload);
            
            ShowMessage("License generated successfully.", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowMessage($"License generation error: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private void VerifyLicense()
    {
        if (string.IsNullOrWhiteSpace(SelectedFolder))
        {
            ShowMessage("Select a folder with public key.", InfoBarSeverity.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedLicensePath))
        {
            ShowMessage("Select a license file.", InfoBarSeverity.Error);
            return;
        }

        try
        {
            bool isValid = _licenseKeyService.VerifyLicense(Path.Combine(SelectedFolder, "public_key.pem"), SelectedLicensePath);

            ShowMessage(
                isValid ? "License signature is valid." : "Invalid license signature.",
                isValid ? InfoBarSeverity.Success : InfoBarSeverity.Error);
        }
        catch (Exception ex)
        {
            ShowMessage($"License verification error: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private async Task SelectLicenseFileAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };

        picker.FileTypeFilter.Add(".lic");

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();

        if (file is not null)
            SelectedLicensePath = file.Path;
    }

    [RelayCommand]
    private void DecodeLicense()
    {
        if (string.IsNullOrWhiteSpace(SelectedFolder))
        {
            ShowMessage("Select a folder.", InfoBarSeverity.Error);
            return;
        }

        try
        {
            string licensePath = Path.Combine(SelectedFolder, "activation.lic");

            string json = File.ReadAllText(licensePath);

            var document = JsonSerializer.Deserialize<LicenseDocument>(json)
                ?? throw new InvalidDataException("Invalid license document.");

            byte[] payloadBytes = Convert.FromBase64String(document.Payload);

            LicenseContent = Encoding.UTF8.GetString(payloadBytes);

            ShowMessage("License decoded successfully.", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowMessage($"License decoding error: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private void ModifyLicense()
    {
        if (string.IsNullOrWhiteSpace(LicenseContent))
        {
            ShowMessage("Decode a license first.", InfoBarSeverity.Error);
            return;
        }

        try
        {
            var originalDocument = JsonSerializer.Deserialize<LicenseDocument>(
                File.ReadAllText(Path.Combine(SelectedFolder, "activation.lic")))
                ?? throw new InvalidDataException("Invalid license document.");

            // Проверяем, что введённый текст является корректным JSON.
            using var jsonDocument = JsonDocument.Parse(LicenseContent);

            var modifiedDocument = new LicenseDocument
            {
                Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(LicenseContent)),
                Signature = originalDocument.Signature
            };

            string outputPath = Path.Combine(SelectedFolder, "activation_modified.lic");

            File.WriteAllText(
                outputPath,
                JsonSerializer.Serialize(modifiedDocument, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

            ShowMessage("Modified license saved.", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowMessage($"License modification error: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private void GenerateNewLicenseId()
    {
        LicenseId = GenerateLicenseId();
    }

    [RelayCommand]
    private void GenerateKeys()
    {
        if (string.IsNullOrWhiteSpace(EncryptionKey))
        {
            ShowMessage("Enter the encryption password.", InfoBarSeverity.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedFolder))
        {
            ShowMessage("Select a folder for saving keys.", InfoBarSeverity.Error);
            return;
        }

        try
        {
            _licenseKeyService.GenerateKeys(
                Path.Combine(SelectedFolder, "private_key.enc"),
                Path.Combine(SelectedFolder, "public_key.pem"),
                "DataBaseGenerator-License",
                EncryptionKey);

            ShowMessage("RSA keys generated successfully.", InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowMessage($"Key generation error: {ex.Message}", InfoBarSeverity.Error);
        }
    }

    [RelayCommand]
    private async Task SelectFolderAsync()
    {
        var picker = new FolderPicker();
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeFilter.Add("*");

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        var folder = await picker.PickSingleFolderAsync();

        if (folder is not null)
            SelectedFolder = folder.Path;
    }

    [RelayCommand]
    private async Task CopyFolderPath()
    {
        if (string.IsNullOrWhiteSpace(SelectedFolder))
            return;

        var dataPackage = new DataPackage();
        dataPackage.SetText(SelectedFolder);
        Clipboard.SetContent(dataPackage);

        CopyPathIcon = Symbol.Accept;
        ShowMessage("Folder path copied.", InfoBarSeverity.Success);

        await Task.Delay(500);
        CopyPathIcon = Symbol.Copy;
    }

    private static string GenerateLicenseId()
    {
        return $"DG-{Guid.NewGuid():N}".ToUpperInvariant();
    }

    private void ShowMessage(string message, InfoBarSeverity severity)
    {
        ResultStatus = message;
        InfoBarSeverity = severity;
        InfoBarIsOpen = true;
    }
}