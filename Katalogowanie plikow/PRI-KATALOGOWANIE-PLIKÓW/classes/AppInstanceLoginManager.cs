using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes
{
    class AppInstanceLoginManager
    {
        // Staram się przeprowadzać operacje na tablicach bajtów, 
        // nie na stringach, by uniknąć problemów z kodowaniem

        // private readonly int keySizeInBits = 256;
        private readonly int saltSizeInBits = 128;

        private readonly Encoding textEncoding = Encoding.UTF8;


        #region Password hashing logic

        /// <summary>
        /// Gets salt from config file, or null if salt is not set
        /// </summary>
        /// <returns>byte array, null if salt is not set</returns>
        private byte[] GetPasswordSalt()
        {
            String passwordSaltString =
                ConfigManager.ReadString(ConfigManager.PASSWORD_SALT_KEY);
            if(passwordSaltString == null || passwordSaltString.Length <= 0)
            {
                return null;
            }
            return StringToBytes(passwordSaltString);
        }

        private byte[] GetDigest()
        {
            String DigestString =
                ConfigManager.ReadString(ConfigManager.DIGEST_HASH_KEY);
            if (DigestString == null || DigestString.Length <= 0)
            {
                return null;
            }
            return StringToBytes(DigestString);
        }


        /// <summary>
        /// Converts base64 string to byte array
        /// </summary>
        /// <param name="s">Base64 string</param>
        /// <returns>Returns byte[], or null if s is empty</returns>
        private byte[] StringToBytes(String s)
        {
            if(s == null || s.Length <= 0)
            {
                return null;
            }
            //return System.Text.Encoding.UTF8.GetBytes(s);
            return Convert.FromBase64String(s);
        }

        /// <summary>
        /// Converts byte array to base64 string
        /// </summary>
        /// <param name="b"></param>
        /// <returns>Returns base64 string, or an empty string if b is null</returns>
        private String BytesToString(byte[] b)
        {
            if(b == null)
            {
                return "";
            }
            //return System.Text.Encoding.UTF8.GetString(b);
            return Convert.ToBase64String(b);
        }


        private byte[] ComputeDigest(String password)
        {
            byte[] salt = null;
            if (!SaltIsSet())
            {
                salt = GenerateSalt();
            }
            else
            {
                salt = StringToBytes(ConfigManager.ReadString(
                    ConfigManager.PASSWORD_SALT_KEY));
            }
            
            // Zamiast konkatenować ze sobą hasło i salt i dawać wynik takowej do SHA256 - mieszam za pomocą PBKDF2.
            Rfc2898DeriveBytes hashComputer = new Rfc2898DeriveBytes(password, salt, 512);
            return hashComputer.GetBytes(32);
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

            // If password is set, remove digest hash, 
            // because password can no longer match the hash
            // with new salt.
            if (DigestIsSet())
            {
                ConfigManager.RemoveValue(
                    ConfigManager.DIGEST_HASH_KEY);
            }

            return salt;
        }

        #endregion


        public bool VerifyPassword(String newPassword)
        {
            byte[] digest = GetDigest();
            if(digest == null)
            {
                Console.WriteLine("Digest is null during password verification attempt");
                return false;
            }

            if (digest.SequenceEqual(ComputeDigest(newPassword)))
            {
                Console.WriteLine("Password is valid");
                // We generate and store all the private keys to use from our password:

                // Local database encryption key generation.
                byte[] database_encryption_salt = new byte[saltSizeInBits / 8];
                if (ConfigManager.ReadString(ConfigManager.DB_ENCR_SALT_KEY) == null || ConfigManager.ReadString(ConfigManager.DB_ENCR_SALT_KEY).Length == 0)
                {
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    rng.GetBytes(database_encryption_salt);
                    ConfigManager.WriteValue(ConfigManager.DB_ENCR_SALT_KEY,database_encryption_salt);
                    database_encryption_salt = ConfigManager.ReadByte(ConfigManager.DB_ENCR_SALT_KEY);
                }
                else
                {
                    database_encryption_salt = ConfigManager.ReadByte(ConfigManager.DB_ENCR_SALT_KEY);
                }

                Rfc2898DeriveBytes DB_encryption_key_computer = new Rfc2898DeriveBytes(newPassword, database_encryption_salt, 512);

                byte[] database_encryption_key = DB_encryption_key_computer.GetBytes(32);

                AppCryptoDataStorage.DB_local_key = database_encryption_key;

                // Private key generation for peer list creation

                // Private key generation for external catalogue creation

                // Public key is derived from network connection password

                return true;
            }
            else
            {
                Console.WriteLine("Password is invalid");
                Console.WriteLine("Old digest: " + 
                    BytesToString(digest));
                Console.WriteLine("New digest: " +
                    BytesToString(ComputeDigest(newPassword)));
                return false;
            }
        }


        public bool DigestIsSet()
        {
            if (GetDigest() == null)
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
        /// Computes and writes the new digest value to config for app instance
        /// </summary>
        /// <param name="newPassword"></param>
        public void SetDigest(String newPassword)
        {
            byte[] salt = GenerateSalt();
            byte[] digest = ComputeDigest(newPassword);

            ConfigManager.WriteValue(
                ConfigManager.DIGEST_HASH_KEY,
                BytesToString(digest));
        }



        /// <summary>
        /// Launch app instance login form if password is set,
        /// or new password registration form if password is not set.
        /// Exits the app if user closes login or registration forms.
        /// </summary>
        public void DisplayLoginRegisterForm()
        {
            if (DigestIsSet())
            {
                LoginOrDie();
            }
            else
            {
                RegisterPasswordOrDie();
            }
        }


        /// <summary>
        /// Displays login form, closes app if user closes form without logging in
        /// </summary>
        public void LoginOrDie()
        {
            LoginForm loginForm = new LoginForm();
            DialogResult authorized = loginForm.ShowDialog();
            if (!(authorized == DialogResult.OK))
            {
                Application.Exit();
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// Displays new password form, closes app if user closes without choosing password
        /// </summary>
        public void RegisterPasswordOrDie()
        {
            NewPasswordForm newPasswordForm = new NewPasswordForm();
            DialogResult registered = newPasswordForm.ShowDialog();
            if (!(registered == DialogResult.OK))
            {
                Application.Exit();
                Environment.Exit(0);
            }
        }
    }
}
