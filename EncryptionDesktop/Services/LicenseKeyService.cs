using EncryptionDesktop.Models;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EncryptionDesktop.Services
{
    public class LicenseKeyService
    {
        private readonly EncryptionService _encryptionService;
        private const string EncryptionSalt = "DataBaseGenerator-License";


        public LicenseKeyService(EncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }


        public void GenerateKeys(string privateKeyPath, string publicKeyPath, string encryptionSalt, string encryptionKey)
        {
            if (File.Exists(privateKeyPath) || File.Exists(publicKeyPath))
            {
                throw new IOException("Файлы ключей уже существуют. Перезапишите их вручную, если это необходимо.");
            }

            using var rsa = RSA.Create(3072);

            string privateKey = rsa.ExportPkcs8PrivateKeyPem();
            string publicKey = rsa.ExportSubjectPublicKeyInfoPem();

            string encryptedPrivateKey = _encryptionService.Encrypt(privateKey, encryptionSalt, encryptionKey);

            File.WriteAllText(privateKeyPath, encryptedPrivateKey);
            File.WriteAllText(publicKeyPath, publicKey);
        }

        public void GenerateLicense(string privateKeyPath, string licensePath, string encryptionKey, LicensePayload payload)
        {
            if (!File.Exists(privateKeyPath))
                throw new FileNotFoundException("Private key file not found.", privateKeyPath);

            if (File.Exists(licensePath))
                throw new IOException("License file already exists.");

            string encryptedPrivateKey = File.ReadAllText(privateKeyPath);

            string privateKey = _encryptionService.Decrypt(encryptedPrivateKey, EncryptionSalt, encryptionKey);

            byte[] payloadBytes = JsonSerializer.SerializeToUtf8Bytes(payload);

            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);

            byte[] signature = rsa.SignData(payloadBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

            var document = new LicenseDocument
            {
                Payload = Convert.ToBase64String(payloadBytes),
                Signature = Convert.ToBase64String(signature)
            };

            string json = JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(licensePath, json, Encoding.UTF8);
        }

        public bool VerifyLicense(string publicKeyPath, string licensePath)
        {
            if (!File.Exists(publicKeyPath))
                throw new FileNotFoundException("Public key file not found.", publicKeyPath);

            if (!File.Exists(licensePath))
                throw new FileNotFoundException("License file not found.", licensePath);

            string publicKey = File.ReadAllText(publicKeyPath);
            string json = File.ReadAllText(licensePath);

            var document = JsonSerializer.Deserialize<LicenseDocument>(json)
                ?? throw new InvalidDataException("Invalid license document.");

            byte[] payloadBytes = Convert.FromBase64String(document.Payload);
            byte[] signature = Convert.FromBase64String(document.Signature);

            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);

            return rsa.VerifyData(
                payloadBytes,
                signature,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pss);
        }
    }
}
