using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes
{
    class DatabaseEncryptor
    {
        private String decryptedFileDirectory;
        private String encryptedFileDirectory;

        private readonly int keySizeBytes = 32;
        private readonly int saltSizeBytes = 16;
        private readonly int IVSizeBytes = 16;


        public DatabaseEncryptor()
        {
            if(ConfigManager.ConfigFileExists())
            {
                decryptedFileDirectory = ConfigManager.ReadString(ConfigManager.LOCAL_DATABASE_LOCATION);
                encryptedFileDirectory = ConfigManager.ReadString(ConfigManager.PROGRAM_LOCATION) + @"\db\encrypted_catalog";
            }

            /*
            DirectoryInfo directory_grabber = new DirectoryInfo(Application.StartupPath);
            String target_directory;

            if (directory_grabber.Name == "Release" || directory_grabber.Name == "Debug")
            {
                //MessageBox.Show("Program w wersji roboczej!");
                target_directory = directory_grabber.Parent.Parent.FullName.ToString(); // przejście do folderu o dwa poziomy w górę
            }
            else
            {
                //MessageBox.Show("Program w wersji ostatecznej!");
                target_directory = directory_grabber.Parent.FullName.ToString(); // przejście do folderu o poziom w górę
            }

            //TODO Pobieranie lokacji bazy danych z pliku config
            decryptedFileDirectory = target_directory + @"\db\catalog.fdb"; // Lokacja katalogu tworzonego przez program
            encryptedFileDirectory = target_directory + @"\db\encrypted_catalog";
            */
        }


        public void EncryptDatabaseFile()
        {
            if (!File.Exists(decryptedFileDirectory))
            {
                //Database is not dectypted
                return;
            }

            String password = AppCryptoDataStorage.Password;

            byte[] salt = ConfigManager.ReadByte(
                ConfigManager.DB_ENCR_SALT_KEY);
            if(salt == null)
            {
                salt = GenerateSalt();
                SetNewSalt(salt);
            }

            byte[] IV = ConfigManager.ReadByte(
                ConfigManager.DB_ENCR_IV_KEY);
            if(IV == null)
            {
                IV = GenerateIV();
                SetNewIV(IV);
            }


            Rijndael rijndael = Rijndael.Create();

            Rfc2898DeriveBytes rfc2898DeriveBytes =
                new Rfc2898DeriveBytes(password, salt, 512);

            byte[] key = rfc2898DeriveBytes.GetBytes(keySizeBytes);

            RijndaelManaged rijndaelManaged = new RijndaelManaged();

            using (FileStream inputFileStream = new FileStream(
                decryptedFileDirectory,
                FileMode.Open))
            using (FileStream outputFileStream = new FileStream(
                encryptedFileDirectory,
                FileMode.OpenOrCreate))
            using (CryptoStream cryptoStream = new CryptoStream(
                outputFileStream,
                rijndaelManaged.CreateEncryptor(key, IV),
                CryptoStreamMode.Write))
            {
                int data;
                while ((data = inputFileStream.ReadByte()) != -1)
                {
                    cryptoStream.WriteByte((byte)data);
                }
            }

            File.Delete(decryptedFileDirectory);
        }


        public void DecryptDatabaseFile()
        {
            if (!File.Exists(encryptedFileDirectory))
            {
                // Database is not encrypted or does not exist
                return;
            }

            String password = AppCryptoDataStorage.Password;

            byte[] salt = ConfigManager.ReadByte(
                ConfigManager.DB_ENCR_SALT_KEY);
            if(salt == null)
            {
                MessageBox.Show("Nie można odszyfrować bazy danych: " +
                    "brakująca wartość salt używana przy dekrypcji!\n",
                    "Błąd deszyfracji bazy danych programu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            byte[] IV = ConfigManager.ReadByte(
                ConfigManager.DB_ENCR_IV_KEY);
            if(IV == null)
            {
                MessageBox.Show("Nie można odszyfrować bazy danych: " +
                    "brakująca wartość IV używana przy dekrypcji!\n",
                    "Błąd deszyfracji bazy danych programu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }


            Rijndael rijndael = Rijndael.Create();
            Rfc2898DeriveBytes rfc2898DeriveBytes =
                new Rfc2898DeriveBytes(password, salt, 512);
            byte[] key = rfc2898DeriveBytes.GetBytes(keySizeBytes);

            RijndaelManaged rijndaelManaged = new RijndaelManaged();

            using (FileStream inputFileStream = new FileStream(
                encryptedFileDirectory,
                FileMode.Open))
            using (FileStream outputFileStream = new FileStream(
                decryptedFileDirectory,
                FileMode.OpenOrCreate))
            using (CryptoStream cryptoStream = new CryptoStream(
                inputFileStream,
                rijndaelManaged.CreateDecryptor(key, IV),
                CryptoStreamMode.Read))
            {
                int data;
                while ((data = cryptoStream.ReadByte()) != -1)
                {
                    outputFileStream.WriteByte((byte)data);
                }
            }

            File.Delete(encryptedFileDirectory);
        }


        private void SetNewSalt(byte[] salt)
        {
            ConfigManager.WriteValue(
                ConfigManager.DB_ENCR_SALT_KEY,
                salt);
        }

        private byte[] GenerateSalt()
        {
            byte[] newSalt = new byte[saltSizeBytes];
            new RNGCryptoServiceProvider().GetBytes(newSalt);
            return newSalt;
        }


        private void SetNewIV(byte[] IV)
        {
            ConfigManager.WriteValue(
                ConfigManager.DB_ENCR_IV_KEY,
                IV);
        }

        private byte[] GenerateIV()
        {
            byte[] newIV = new byte[IVSizeBytes];
            new RNGCryptoServiceProvider().GetBytes(newIV);
            return newIV;
        }
    }
}
