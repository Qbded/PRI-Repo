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
        /// Gets password hash from config file, or null if hash is not set
        /// </summary>
        /// <returns>byte array, or null is hash is not set</returns>
        private byte[] GetPasswordHash()
        {
            String passwordHashString = 
                ConfigManager.ReadString(ConfigManager.PASSWORD_HASH_KEY);
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
                ConfigManager.ReadString(ConfigManager.PASSWORD_SALT_KEY);
            if(passwordSaltString == null || passwordSaltString.Length <= 0)
            {
                return null;
            }
            return StringToBytes(passwordSaltString);
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


        private byte[] HashPassword(String password)
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
            /*
            HashAlgorithm hashAlgorithm = SHA256.Create();
            return hashAlgorithm.ComputeHash(
                textEncoding.GetBytes(password + BytesToString(salt)));
            */
            
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

        #endregion


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
                AppCryptoDataStorage.Password = newPassword;
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
            AppCryptoDataStorage.Password = newPassword;
        }



        /// <summary>
        /// Launch app instance login form if password is set,
        /// or new password registration form if password is not set.
        /// Exits the app if user closes login or registration forms.
        /// </summary>
        public void DisplayLoginRegisterForm()
        {
            if (PasswordIsSet())
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
