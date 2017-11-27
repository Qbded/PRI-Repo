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


        private static readonly String configFileLocation =
            AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        private static readonly String[] configKeyArray =
        {
            "DBLocation",

            PASSWORD_HASH_KEY,
            PASSWORD_SALT_KEY
        };

        private static readonly String[] configDefaultValueArray =
        {
            AppDomain.CurrentDomain.BaseDirectory + "/db/DB.db",

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
        /// Returns value from config file, or and empty string
        /// if value is not set
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String ReadValue(String key)
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
    }
}
