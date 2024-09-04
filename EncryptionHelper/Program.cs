namespace EncryptionHelper
{
    internal class Program
    {
        private static EncryptionHelper _encryptionHelper = new();

        static void Main(string[] args)
        {

            var newPassword = "123";

            string encryptPassword = _encryptionHelper.Encrypt(newPassword);

            string decryptedPassword = _encryptionHelper.Decrypt(encryptPassword);


            Console.WriteLine($"newPassword - {newPassword}");

            Console.WriteLine($"encryptPassword - {encryptPassword}");

            Console.WriteLine($"decryptedPassword - {decryptedPassword}");
        }
    }
}
