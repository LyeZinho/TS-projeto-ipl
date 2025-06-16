using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace chatlib
{
    public class Encript
    {
        // Metodo para gerar um par de chaves RSA para a chave privada e pública

        /*
         Uso:
            var (publicKey, privateKey) = Encript.GenerateKeys();
            var encryptedMessage = Encript.EncryptMessage("Mensagem secreta", publicKey);
         */
        public static (string PublicKey, string PrivateKey) GenerateKeys()
        {
            using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(2048)) // 2048 bits é um tamanho comum para chaves RSA
            {
                var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey()); // Exporta a chave pública
                var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey()); // Exporta a chave privada
                return (publicKey, privateKey);
            }
        }

        // Metodo para criptografar uma mensagem usando a chave pública
        /*
          Uso:
            var encryptedMessage = Encript.EncryptMessage("Mensagem secreta", publicKey);
            var decryptedMessage = Encript.DecryptMessage(encryptedMessage, privateKey);
         */
        public static string EncryptMessage(string message, string publicKey)
        {
            using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider())
            {
                rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
                var dataToEncrypt = Encoding.UTF8.GetBytes(message);
                var encryptedData = rsa.Encrypt(dataToEncrypt, System.Security.Cryptography.RSAEncryptionPadding.OaepSHA256);
                return Convert.ToBase64String(encryptedData);
            }
        }

        // Metodo para descriptografar uma mensagem usando a chave privada
        /*
          Uso:
            var decryptedMessage = Encript.DecryptMessage(encryptedMessage, privateKey);
         */
        public static string DecryptMessage(string encryptedMessage, string privateKey)
        {
            using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider())
            {
                rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
                var dataToDecrypt = Convert.FromBase64String(encryptedMessage);
                var decryptedData = rsa.Decrypt(dataToDecrypt, System.Security.Cryptography.RSAEncryptionPadding.OaepSHA256);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        // Metodo para gerar um salt aleatório
        // O salt é usado para adicionar aleatoriedade ao processo de hashing de senhas
        public static string GenerateSalt(int size = 16)
        {
            var saltBytes = new byte[size];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Metodo para gerar um hash de senha usando SHA256
        // O hash é gerado a partir da senha e do salt, garantindo que senhas iguais tenham hashes diferentes
        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var saltedPassword = password + salt;
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashBytes);
            }
        }

        // Metodo para verificar se a senha informada corresponde ao hash armazenado
        public static bool VerifyPassword(string password, string salt, string hash)
        {
            var computedHash = HashPassword(password, salt);
            return computedHash == hash;
        }

        // Encripta o salt usando um método de criptografia simétrica (AES)
        public static string EncryptSalt(string salt, string key)
        {
            using (var aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes("dSFUOhFQY4Q1.uc2azn0Q8h3L2pCoG");

                using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, saltBytes, 10000))
                {
                    aes.Key = keyDerivationFunction.GetBytes(32); // 256 bits
                }

                aes.GenerateIV();
                var iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                using (var ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length); // Escreve o IV no início

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        var dataBytes = Encoding.UTF8.GetBytes(salt);
                        cs.Write(dataBytes, 0, dataBytes.Length);
                        cs.FlushFinalBlock(); // Corrigido: garante que todos os dados sejam escritos
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }


        // Desencripta o salt usando o mesmo método de criptografia simétrica (AES)
        public static string DecryptSalt(string encryptedSalt, string key)
        {
            var fullCipher = Convert.FromBase64String(encryptedSalt);

            using (var aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes("dSFUOhFQY4Q1.uc2azn0Q8h3L2pCoG");

                using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, saltBytes, 10000))
                {
                    aes.Key = keyDerivationFunction.GetBytes(32);
                }

                var iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        // Metodo para encriptar e desencriptar dados usando AES
        public static string EncryptData(string data, string key)
        {
            using (var aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes("dSFUOhFQY4Q1.uc2azn0Q8h3L2pCoG");

                using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, saltBytes, 10000))
                {
                    aes.Key = keyDerivationFunction.GetBytes(32);
                }

                aes.GenerateIV();
                var iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                using (var ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length); // Escreve IV no início

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        var dataBytes = Encoding.UTF8.GetBytes(data);
                        cs.Write(dataBytes, 0, dataBytes.Length);
                        cs.FlushFinalBlock(); // Corrigido
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }


        // Metodo para desencriptar dados usando AES
        public static string DecryptData(string encryptedData, string key)
        {
            var fullCipher = Convert.FromBase64String(encryptedData);

            using (var aes = Aes.Create())
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes("dSFUOhFQY4Q1.uc2azn0Q8h3L2pCoG");

                using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, saltBytes, 10000))
                {
                    aes.Key = keyDerivationFunction.GetBytes(32);
                }

                var iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static string EncryptDataFixedIV(string data, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] saltBytes = Encoding.UTF8.GetBytes("dSFUOhFQY4Q1.uc2azn0Q8h3L2pCoG");
                using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, saltBytes, 10000))
                {
                    aes.Key = keyDerivationFunction.GetBytes(32);
                }

                aes.IV = Encoding.UTF8.GetBytes("1234567890ABCDEF"); // IV fixo de 16 bytes

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                    cs.Write(dataBytes, 0, dataBytes.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string DecryptDataFixedIV(string encryptedData, string key)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedData);

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] saltBytes = Encoding.UTF8.GetBytes("dSFUOhFQY4Q1.uc2azn0Q8h3L2pCoG");
                using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, saltBytes, 10000))
                {
                    aes.Key = keyDerivationFunction.GetBytes(32);
                }

                aes.IV = Encoding.UTF8.GetBytes("1234567890ABCDEF"); // mesmo IV fixo

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        // Metodo para gerar um UUID (Universally Unique Identifier)
        public static string GenerateUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
