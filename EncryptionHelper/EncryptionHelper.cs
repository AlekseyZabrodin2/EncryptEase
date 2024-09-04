using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionHelper
{
    public class EncryptionHelper
    {
        private static readonly string _encryptionKey = "12345";
        private static readonly string _saltText = "1234567890";

        public string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            // Генерация случайного IV
            byte[] iv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }

            // Преобразование текстовой соли в массив байтов
            byte[] saltBytes = Encoding.UTF8.GetBytes(_saltText);

            using (Aes aes = Aes.Create())
            {
                byte[] keyBytes = Rfc2898DeriveBytes.Pbkdf2(_encryptionKey, saltBytes, 10000, HashAlgorithmName.SHA256, 32);
                aes.Key = keyBytes;
                aes.IV = iv;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(clearBytes, 0, clearBytes.Length);

                    // Объединение IV и зашифрованных данных
                    byte[] result = new byte[iv.Length + encryptedBytes.Length];
                    Array.Copy(iv, 0, result, 0, iv.Length);
                    Array.Copy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            // Извлечение IV и зашифрованных данных
            byte[] iv = new byte[16];
            byte[] encryptedBytes = new byte[cipherBytes.Length - iv.Length];

            Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
            Array.Copy(cipherBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

            // Преобразование текстовой соли в массив байтов
            byte[] saltBytes = Encoding.UTF8.GetBytes(_saltText);

            using (Aes aes = Aes.Create())
            {
                byte[] keyBytes = Rfc2898DeriveBytes.Pbkdf2(_encryptionKey, saltBytes, 10000, HashAlgorithmName.SHA256, 32);
                aes.Key = keyBytes;
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.Unicode.GetString(decryptedBytes);
                }
            }
        }
    }
}
