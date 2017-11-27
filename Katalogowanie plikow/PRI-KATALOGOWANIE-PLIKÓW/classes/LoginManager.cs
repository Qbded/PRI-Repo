using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes
{
    class LoginManager
    {
        // Staram się przeprowadzać operacje na tablicach bajtów, 
        // nie na stringach, by uniknąć problemów z kodowaniem

        private readonly int keySizeInBits = 256;
        private readonly int saltSizeInBits = 128;

        private readonly Encoding textEncoding = Encoding.UTF8;


        /// <summary>
        /// Gets password hash from config file, or null if hash is not set
        /// </summary>
        /// <returns>byte array, or null is hash is not set</returns>
        private byte[] GetPasswordHash()
        {
            String passwordHashString = 
                ConfigManager.ReadValue(ConfigManager.PASSWORD_HASH_KEY);
            if(passwordHashString == null || passwordHashString.Length <= 0)
            {
                return null;
            }
            return StringToBytes(passwordHashString);
        }

        /// <summary>
        /// Gets salt from config file, or null if salt is not set
        /// </summary>
        /// <returns>byte array, null if salt is not set</returns>
        private byte[] GetPasswordSalt()
        {
            String passwordSaltString =
                ConfigManager.ReadValue(ConfigManager.PASSWORD_SALT_KEY);
            if(passwordSaltString == null || passwordSaltString.Length <= 0)
            {
                return null;
            }
            return StringToBytes(passwordSaltString);
        }


        private byte[] StringToBytes(String s)
        {
            if(s == null || s.Length <= 0)
            {
                return null;
            }
            //return System.Text.Encoding.UTF8.GetBytes(s);
            return Convert.FromBase64String(s);
        }

        private String BytesToString(byte[] b)
        {
            if(b == null)
            {
                return "";
            }
            //return System.Text.Encoding.UTF8.GetString(b);
            return Convert.ToBase64String(b);
        }


        public bool VerifyPassword(String newPassword)
        {
            byte[] passwordHash = GetPasswordHash();
            if(passwordHash == null)
            {
                Console.WriteLine("Password hash is null during password verification attempt");
                return false;
            }

            byte[] passwordSalt = GetPasswordSalt();
            if(passwordSalt == null)
            {
                Console.WriteLine("Salt is null during password verification attempt");
                return false;
            }

            //byte[] newPasswordBytes = StringToBytes(newPassword);
            byte[] newPasswordHash = HashPassword(newPassword);

            if (passwordHash.SequenceEqual(newPasswordHash))
            {
                Console.WriteLine("Password is valid");
                return true;
            }
            else
            {
                Console.WriteLine("Password is invalid");
                Console.WriteLine("Old password hash: " + 
                    BytesToString(passwordHash));
                Console.WriteLine("New password hash: " +
                    BytesToString(newPasswordHash));
                return false;
            }
        }


        private byte[] HashPassword(String password)
        {
            byte[] salt = null;
            if (!SaltIsSet())
            {
                salt = GenerateSalt();
            }
            else
            {
                salt = StringToBytes(ConfigManager.ReadValue(
                    ConfigManager.PASSWORD_SALT_KEY));
            }

            HashAlgorithm hashAlgorithm = SHA256.Create();
            return hashAlgorithm.ComputeHash(
                textEncoding.GetBytes(password + BytesToString(salt)));
        }


        private byte[] GenerateSalt()
        {
            RNGCryptoServiceProvider rng = 
                new RNGCryptoServiceProvider();
            byte[] salt = new byte[saltSizeInBits / 8];
            rng.GetBytes(salt);
            ConfigManager.WriteValue(
                ConfigManager.PASSWORD_SALT_KEY,
                BytesToString(salt)
            );

            // If password is set, remove password hash, 
            // because password can no longer match the hash
            // with new salt.
            if (PasswordIsSet())
            {
                ConfigManager.RemoveValue(
                    ConfigManager.PASSWORD_HASH_KEY);
            }

            return salt;
        }


        public bool PasswordIsSet()
        {
            if (GetPasswordHash() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private bool SaltIsSet()
        {
            if(GetPasswordSalt() == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// Sets new password for app instance
        /// </summary>
        /// <param name="newPassword"></param>
        public void SetPassword(String newPassword)
        {
            byte[] salt = GenerateSalt();
            byte[] hash = HashPassword(newPassword);
            ConfigManager.WriteValue(
                ConfigManager.PASSWORD_HASH_KEY,
                BytesToString(hash));
        }
    }
}
