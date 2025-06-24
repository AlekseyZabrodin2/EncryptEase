using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EncryptionHelper
{
    public class Program
    {
        private static readonly IHost _host;
        private static EncryptionHelper _encryptionHelper;

        static Program()
        {
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;

                    config.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "AppSettings/encryptionSettings.json"));

                    // add UserSecrets when Development
                    if (env.IsDevelopment())
                    {
                        config.AddUserSecrets<Program>();
                    }

                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    services.AddTransient<EncryptionHelper>();

                    services.Configure<EncryptionSettings>(configuration.GetSection(nameof(EncryptionSettings)));
                });

            _host = hostBuilder.Build();

            _encryptionHelper = _host.Services.GetRequiredService<EncryptionHelper>();
        }


        static void Main(string[] args)
        {
            var newPassword = "123";

            string encryptPassword = _encryptionHelper.Encrypt(newPassword);
            string decryptedPassword = _encryptionHelper.Decrypt(encryptPassword);

            Console.WriteLine($"Password - {newPassword}");
            Console.WriteLine($"Encrypt Password - {encryptPassword}");
            Console.WriteLine($"Decrypted Password - {decryptedPassword}");
        }
    }
}
