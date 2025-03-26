using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Diagnostics.Eventing.Reader;

namespace Fikra.Helper
{
    public class RSAKeyGenerator
    {
        private readonly IConfiguration _configuration;
        private readonly string _appSettingsFile = "appsettings.json";
        private string publicKey;
        private string privateKey;
        public RSAKeyGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void GenerateAndSaveKeys()
        {


            if (File.Exists(_appSettingsFile))
            {
                var config = _configuration.GetSection("RSAKeys");
                publicKey = config["PublicKey"];
                privateKey = config["PrivateKey"];

                if (string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey))
                {
                    GenerateAndStoreNewKeys();
                }
                else
                {
                    return;
                }

            }

        }

        private void GenerateAndStoreNewKeys()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
                privateKey = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey());


                var appSettings = JsonSerializer.Deserialize<JsonObject>(File.ReadAllText(_appSettingsFile));


                appSettings["RSAKeys"] = new JsonObject
         {
             { "PublicKey", publicKey },
             { "PrivateKey", privateKey }
         };


                File.WriteAllText(_appSettingsFile, appSettings.ToString());

                
            }
        }
    }
}
