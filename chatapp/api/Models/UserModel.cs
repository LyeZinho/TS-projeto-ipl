using chatlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Entity;


namespace api.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string PublicKeyEncrypted { get; set; } = string.Empty;
        public string PrivateKeyEncrypted { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string UniqueId { get; set; } = Guid.NewGuid().ToString();

        // Construtor padrão necessário para Entity Framework
        public UserModel(string username, string password, string publicKey, string privateKey, string salt, string uniqueId)
        {
            Username = username;
            PasswordHash = password;
            PublicKey = publicKey;
            PrivateKey = privateKey;
            Salt = salt;
            UniqueId = uniqueId;
        }

        // Construtor vazio necessário para Entity Framework
        public UserModel() { }


        // Getters e Setters para os campos privados
        public string GetPasswordHash()
        {
            return PasswordHash;
        }

        public UserModel SetPasswordHash(string hash)
        {
            PasswordHash = hash;
            return this;
        }

        public string GetPublicKey()
        {
            return PublicKey;
        }

        public UserModel SetPublicKey(string publicKey)
        {
            PublicKey = publicKey;
            return this;
        }

        public string GetPrivateKey()
        {
            return PrivateKey;
        }

        public UserModel SetPrivateKey(string privateKey)
        {
            PrivateKey = privateKey;
            return this;
        }

        public string GetPublicKeyEncrypted()
        {
            string saltkey = System.Configuration.ConfigurationManager.AppSettings["SaltKey"];
            return Encript.DecryptDataFixedIV(PublicKeyEncrypted, saltkey);
        }

        public UserModel SetPublicKeyEncrypted(string publicKeyEncrypted)
        {
            string saltkey = System.Configuration.ConfigurationManager.AppSettings["SaltKey"];
            PublicKeyEncrypted = Encript.EncryptDataFixedIV(publicKeyEncrypted, saltkey);
            return this;
        }

        public string GetPrivateKeyEncrypted()
        {
            string saltkey = System.Configuration.ConfigurationManager.AppSettings["SaltKey"];
            return Encript.DecryptDataFixedIV(PrivateKeyEncrypted, saltkey);
        }

        public void SetPrivateKeyEncrypted(string privateKeyEncrypted)
        {
            string saltkey = System.Configuration.ConfigurationManager.AppSettings["SaltKey"];
            PrivateKeyEncrypted = Encript.EncryptDataFixedIV(privateKeyEncrypted, saltkey);
        }

        public string GetSalt()
        {
            return Salt;
        }

        public void SetSalt(string salt)
        {
            Salt = salt;
        }

        // Crud com Entity Framework

        public static UserModel CreateUser(UserModel user)
        {
            using (var context = new AplicationDBContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
                return user;
            }
        }

        public static UserModel GetUserByUsername(string username)
        {
            using (var context = new AplicationDBContext())
            {
                return context.Users.FirstOrDefault(u => u.Username == username);
            }
        }

        public static UserModel GetUserById(string uniqueId)
        {
            using (var context = new AplicationDBContext())
            {
                return context.Users.FirstOrDefault(u => u.UniqueId == uniqueId);
            }
        }

        public static List<UserModel> GetAllUsers()
        {
            using (var context = new AplicationDBContext())
            {
                return context.Users.ToList();
            }
        }

        public static void UpdateUser(UserModel user)
        {
            using (var context = new AplicationDBContext())
            {
                var existingUser = context.Users.FirstOrDefault(u => u.UniqueId == user.UniqueId);
                if (existingUser != null)
                {
                    existingUser.Username = user.Username;
                    existingUser.SetPasswordHash(user.GetPasswordHash());
                    existingUser.SetPublicKey(user.GetPublicKey());
                    existingUser.SetPrivateKey(user.GetPrivateKey());
                    existingUser.SetPublicKeyEncrypted(user.GetPublicKeyEncrypted());
                    existingUser.SetPrivateKeyEncrypted(user.GetPrivateKeyEncrypted());
                    existingUser.SetSalt(user.GetSalt());
                    context.SaveChanges();
                }
            }
        }

        public static bool ValidateUser(string username, string password)
        {
            using (var context = new AplicationDBContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    string saltkey = System.Configuration.ConfigurationManager.AppSettings["SaltKey"];
                    string decryptedSalt = Encript.DecryptDataFixedIV(user.GetSalt(), saltkey);
                    string hashedPassword = Encript.HashPassword(password, decryptedSalt);
                    return hashedPassword == user.GetPasswordHash();
                }
                return false;
            }
        }

        public static bool CheckPassword(string username, string password)
        {
            using (var context = new AplicationDBContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    string saltkey = System.Configuration.ConfigurationManager.AppSettings["SaltKey"];
                    string decryptedSalt = Encript.DecryptDataFixedIV(user.GetSalt(), saltkey);
                    string hashedPassword = Encript.HashPassword(password, decryptedSalt);
                    return hashedPassword == user.GetPasswordHash();
                }
                return false;
            }
        }
    }
}
