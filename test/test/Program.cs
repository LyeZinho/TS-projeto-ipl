using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    internal class Program
    {
        public static (string PublicKey, string PrivateKey) GenerateKeys()
        {
            using (var rsa = RSA.Create())
            {
                var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
                var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
                return (publicKey, privateKey);
            }
        }

        public static string EncryptMessage(string message, string publicKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                var dataToEncrypt = Encoding.UTF8.GetBytes(message);
                var encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
                return Convert.ToBase64String(encryptedData);
            }
        }

        public static string DecryptMessage(string encryptedMessage, string privateKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                var dataToDecrypt = Convert.FromBase64String(encryptedMessage);
                var decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        public static void Main(string[] args)
        {
            var (pubkey, privkey) = GenerateKeys();
            string message = "Test";
            string encript = EncryptMessage(message, pubkey);
            string decript = DecryptMessage(encript, privkey);
            Console.WriteLine(message);
            Console.WriteLine(encript);
            Console.WriteLine(decript);

        }
    }
}
