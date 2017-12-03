using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PRI_KATALOGOWANIE_PLIKÓW.classes
{
    static class ConfigManager
    {
        public static readonly String PASSWORD_HASH_KEY = "passwordHash";
        public static readonly String PASSWORD_SALT_KEY = "passwordSalt";
        public static readonly String DB_ENCR_SALT_KEY = "dbEnc_salt";
        public static readonly String DB_ENCR_IV_KEY = "dbEnc_IV";


        private static readonly String configFileLocation =
            AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        private static readonly String[] configKeyArray =
        {
            "DBLocation",

            PASSWORD_HASH_KEY,
            PASSWORD_SALT_KEY,

            DB_ENCR_IV_KEY,
            DB_ENCR_SALT_KEY
        };

        private static readonly String[] configDefaultValueArray =
        {
            AppDomain.CurrentDomain.BaseDirectory + "/db/DB.db",

            "",
            "",

            "",
            ""
        };


        //public ConfigManager()
        //{
        //    Console.WriteLine("New configManager created");
        //    // TODO
        //    // More in-depth config file verification
        //    if (!ValidateConfigFile()) { CreateNewConfigFile(); }
        //}


        public static bool ConfigFileExists()
        {
            if (System.IO.File.Exists(configFileLocation))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void WriteValue(String key, String value)
        {
            Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None
                );
            KeyValueConfigurationCollection settings =
                configuration.AppSettings.Settings;
            if (!ValueExists(key, settings))
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configuration.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection(
                configuration.AppSettings.SectionInformation.Name);
            settings = null;
            configuration = null;
        }

        public static void WriteValue(String key, byte[] value)
        {
            WriteValue(key, BytesToString(value));
        }


        public static void RemoveValue(String key)
        {
            Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None
                );
            KeyValueConfigurationCollection settings =
                configuration.AppSettings.Settings;
            if(ValueExists(key, settings))
            {
                settings.Remove(key);
                configuration.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection(
                    configuration.AppSettings.SectionInformation.Name);
            }
            settings = null;
            configuration = null;
        }


        /// <summary>
        /// Returns string value from config file, or empty string
        /// if value is not set
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String ReadString(String key)
        {
            Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(
                    ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings =
                configuration.AppSettings.Settings;

            if(!ValueExists(key, settings))
            {
                return "";
            }
            else
            {
                return settings[key].Value.ToString();
            }
        }

        /// <summary>
        /// Returns byte array representation of value from config file,
        /// or null string if value is not set
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] ReadByte(String key)
        {
            String value = ReadString(key);
            return StringToBytes(value);
        }


        private static bool ValueExists(String key, 
            KeyValueConfigurationCollection settings = null)
        {
            if (settings == null)
            {
                Configuration configuration =
                    ConfigurationManager.OpenExeConfiguration(
                        ConfigurationUserLevel.None);
                settings =
                    configuration.AppSettings.Settings;
            }

            if(settings[key] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public static bool ValidateConfigFile()
        {
            Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None
                );
            KeyValueConfigurationCollection settings =
                configuration.AppSettings.Settings;

            foreach (String key in configKeyArray)
            {
                if (ValueExists(key, settings))
                {
                    Console.WriteLine("config file is invalid, creating new one");
                    settings = null;
                    configuration = null;
                    return false;
                }
            }

            settings = null;
            configuration = null;
            return true;
        }


        public static void CreateNewConfigFile()
        {
            if (ConfigFileExists())
            {
                System.IO.File.Delete(configFileLocation);
            }
            for (int i = 0; i < configKeyArray.Length; i++)
            {
                WriteValue(
                    configKeyArray[i], 
                    configDefaultValueArray[i]);
            }
            Console.WriteLine("New configFile created at " + 
                configFileLocation);
        }


        /// <summary>
        /// Converts base64 string to byte array
        /// </summary>
        /// <param name="s">Base64 string</param>
        /// <returns>Returns byte[], or null if s is empty</returns>
        private static byte[] StringToBytes(String s)
        {
            if (s == null || s.Length <= 0)
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
        private static String BytesToString(byte[] b)
        {
            if (b == null)
            {
                return "";
            }
            //return System.Text.Encoding.UTF8.GetString(b);
            return Convert.ToBase64String(b);
        }
    }
}
