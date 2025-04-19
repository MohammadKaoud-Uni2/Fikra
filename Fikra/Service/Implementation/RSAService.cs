using Fikra.Service.Interface;
using MimeKit.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Fikra.Service.Implementation
{
    public class RSAService : IRSAService
    {
        private readonly RSA _rsa;

        public RSAService(IConfiguration configuration)
        {
            var publicKey = configuration["RSAKeys:PublicKey"];
            var privateKey = configuration["RSAKeys:PrivateKey"];

            if (string.IsNullOrWhiteSpace(publicKey) || string.IsNullOrWhiteSpace(privateKey))
            {
                throw new Exception("RSA Keys are not available in configuration.");
            }

            _rsa = RSA.Create();
            _rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
            _rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);
        }


        public string SignData(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signedBytes = _rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signedBytes);
        }

        public bool VerifySignature(string data, string signature)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            return _rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        public string EncryptData(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = _rsa.Encrypt(dataBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encryptedBytes);
        }
        public string DecryptData(string encryptedData)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
