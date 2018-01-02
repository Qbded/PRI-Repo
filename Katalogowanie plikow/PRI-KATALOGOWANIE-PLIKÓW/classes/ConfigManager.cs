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
        public static readonly String DIGEST_HASH_KEY = "digestHash";
        public static readonly String PASSWORD_SALT_KEY = "passwordSalt";
        public static readonly String DB_ENCR_SALT_KEY = "dbEnc_salt";
        public static readonly String DB_ENCR_IV_KEY = "dbEnc_IV";

        public static readonly String DATABASE_ENGINE_LOCATION = "DBEngineLocation";
        public static readonly String LOCAL_DATABASE_LOCATION = "CatalogLocation";
        public static readonly String EXTERNAL_DATABASES_LOCATION = "ExternalCatalogsLocation";

        public static readonly String PROGRAM_LOCATION = "ProgramLocation";
        public static readonly String OUTPUT_LOCATION = "OutputLocation";

        public static readonly String TCP_SECONDS_TO_TIMEOUT = "TCPSecondsToTimeout"

        private static readonly String configFileLocation =
            AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        private static System.IO.DirectoryInfo applicationLocation =
            new System.IO.DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        private static readonly Dictionary<String, String>
            defaultConfigValues = new Dictionary<String, String>()
            {
                { DIGEST_HASH_KEY, "" },
                { PASSWORD_SALT_KEY, "" },

                { DB_ENCR_IV_KEY, "" },
                { DB_ENCR_SALT_KEY, "" },


                { DATABASE_ENGINE_LOCATION, "" },
                { LOCAL_DATABASE_LOCATION, "" },
                { EXTERNAL_DATABASES_LOCATION, "" },

                { PROGRAM_LOCATION, "" },
                { OUTPUT_LOCATION, ""},

                { TCP_SECONDS_TO_TIMEOUT, "5" }
            };

        private static void determineDirectoryStructure()
        {
            if(applicationLocation.Name == "Debug" || applicationLocation.Name == "Release")
            {
                //Program jest uruchamiany z folderu projektowego visual studio - macierzysty folder dla programu jest dwa foldery wyżej.
                applicationLocation = applicationLocation.Parent.Parent;
            }
            else
            {
                //Program jest uruchamiany wedle ustalonej struktury programu - z katalogu bin, stąd idziemy tylko jeden folder do góry.
                applicationLocation = applicationLocation.Parent;
            }

            ConfigManager.WriteValue(DATABASE_ENGINE_LOCATION, applicationLocation.FullName.ToString() + @"\bin\firebird_server\fbclient.dll");
            ConfigManager.WriteValue(LOCAL_DATABASE_LOCATION, applicationLocation.FullName.ToString() + @"\db\catalog.fdb");
            ConfigManager.WriteValue(EXTERNAL_DATABASES_LOCATION, applicationLocation.FullName.ToString() + @"\db\externals\");

            ConfigManager.WriteValue(PROGRAM_LOCATION, applicationLocation.FullName.ToString());
            ConfigManager.WriteValue(OUTPUT_LOCATION, applicationLocation.FullName.ToString() + @"\output\");
        }

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
            String valueString = ReadString(key);
            return StringToBytes(valueString);
        }

        /// <summary>
        /// Returns int representation of value from config file,
        /// or default value if the value is not set.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int ReadInt(String key)
        {
            String valueString = ReadString(key);
            return Int32.Parse(valueString);
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

            foreach (KeyValuePair<String, String> entry in defaultConfigValues)
            {
                if (!ValueExists(entry.Key, settings))
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
            foreach (KeyValuePair<String, String> entry in defaultConfigValues)
            {
                WriteValue(
                    entry.Key,
                    entry.Value);
            }
            determineDirectoryStructure();
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
