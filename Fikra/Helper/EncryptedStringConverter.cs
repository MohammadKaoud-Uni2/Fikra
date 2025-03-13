using EntityFrameworkCore.EncryptColumn.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SparkLink.Helper
{
 
        public class EncryptedStringConverter : ValueConverter<string, string>
        {
            private readonly IEncryptionProvider _encryptionProvider;

            public EncryptedStringConverter(IEncryptionProvider encryptionProvider)
                : base(v => encryptionProvider.Encrypt(v), v => encryptionProvider.Decrypt(v))
            {
                _encryptionProvider = encryptionProvider;
            }
        }
    
}
