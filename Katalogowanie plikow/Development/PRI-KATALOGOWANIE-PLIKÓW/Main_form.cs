﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using PRI_KATALOGOWANIE_PLIKÓW.classes;
using PRI_KATALOGOWANIE_PLIKÓW.popup_forms;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Main_form : Form
    {
        #region Deklaracja zmiennych

        // Zmienne ekstraktora metadanych
        private Metadata_extractor metadata_extractor_window;
        // Tutaj wylądują po procesie ekstrakcji metadane.
        public List<string[]> metadata { get; set; }

        /* Tutaj wylądują po przeszukaniu katalogu wyswietlane podfoldery i pliki w nich zawarte
         * Struktura:
         * 1. directories_grabbed:
         * int Item1 - przechowuje ID folderu z tabeli virtual_folder.
         * string Item2 - przechowuje nazwę folderu wyciągniętą z tabeli virtual_folder.
         * trzy zmienne bool przechowujące odpowiednio widoczność, możliwość kopiowania i możliwość kopiowania bez pytania w funkcjonalności sieciowej.
         * 2. files_grabbed:
         * int Item1 - przechowuje ID pliku wyciągniętego z którejś z tabel zawierających metadane.
         * string Item2 - przechowuje nazwę tabeli, z której pochodzi dany plik.
         * string Item3 - przechowuje nazwę pliku.
         * string Item4 - przechowuje rozszerzenie pliku.
         * System.Datetime Item5 - przechowuje czas ostatniego zapisu dla danego pliku.
         * System.Datetime Item6 - przechowuje datę ostatniego katalogowanie dla pliku (czyt. kiedy został ostatnim razem zmieniony podczas katalogowania).
         * int Item7 - przechowuje rozmiar pliku.
         * Tupla z trzema boolami, analogiczna w funkcji do trzech zmiennych bool w directories_grabbed (C# wymaga, aby było to w Tupli, nie tworzy to problemów).
        */
        public List<Tuple<int, string, bool, bool, bool>> directories_grabbed { get; set; }
        public List<Tuple<int, string, string, string, DateTime, DateTime, long, Tuple< bool, bool, bool>>> files_grabbed { get; set; }

        // Tutaj przechowujemy konsktuktor connection string'a do bazy danych.
        private FbConnectionStringBuilder database_connection_string_builder { get; set; }

        /* Tutaj przechowujemy informację o strukturze bazy danych - jej tabelach i kolumnach w nich samych
         * 
         * Lista database_tables zbudowana jest następująco:
         * 1. Indeks tabeli, w zmiennej int - każdej tabeli w bazie nadajemy jakis indeks liczbowy, celem ich poprawnego stworzenia.
         * 2. Nazwa tabeli, w zmiennej string - każdej tabeli w bazie nadajemy także nazwę.
         * 
         * Lista database_columns zbudowana jest następująco:
         * 1. Indeks tabeli macierzystej, w zmiennej int - przyporządkowujemy kolumnie jej tabelę macierzystą.
         * 2. Nazwa kolumny, w zmiennej string - każdej kolumnie nadajemy także nazwę.
         * 3. Własności kolumny, w zmiennej string - każda kolumna może akceptować zadany typ danych i mieć szczególne atrybuty (np. NOT NULL).
         * 
         * Jako że struktura bazy nie powinna się zmieniać, to tutaj koduje je na sztywno. Przechowuje je w zmiennych bo są potrzebne w kilku miejscach na raz.
        */
        private List<Tuple<int, string>> database_tables { get; set; }
        private List<Tuple<int, string, string>> database_columns { get; set; }

        // Tutaj przechowujemy skrypty tworzące bazę danych, nie muszą być koniecznie globalne dla całego okna, ale chwilowo dla wygody takimi je uczyniłem.
        private List<string> creation_scripts { get; set; }
        
        // Lista z obsługiwanymi rozszerzeniami.
        public List<Tuple<int, string>> extends = new List<Tuple<int, string>>() {
                                                                               // Formaty plików tekstowych - identyfikator 0:
                                                                               new Tuple<int, string>(0, ".txt"),
                                                                               new Tuple<int, string>(0, ".csv"),
                                                                               new Tuple<int, string>(0, ".tsv"),
                                                                               new Tuple<int, string>(0, ".fb2"),
                                                                               // Formaty dokumentów - identyfikator 1:
                                                                               new Tuple<int, string>(1, ".docx"),
                                                                               new Tuple<int, string>(1, ".odt"),
                                                                               new Tuple<int, string>(1, ".ods"),
                                                                               new Tuple<int, string>(1, ".odp"),
                                                                               new Tuple<int, string>(1, ".xls"),
                                                                               new Tuple<int, string>(1, ".xlsx"),
                                                                               new Tuple<int, string>(1, ".pdf"),
                                                                               new Tuple<int, string>(1, ".ppt"),
                                                                               new Tuple<int, string>(1, ".pptx"),
                                                                               // Formaty plików złożonych - identyfikator 2:
                                                                               new Tuple<int, string>(2, ".htm"),
                                                                               new Tuple<int, string>(2, ".html"),
                                                                               new Tuple<int, string>(2, ".xml"),
                                                                               // Formaty plików graficznych - identyfikator 3:
                                                                               new Tuple<int, string>(3, ".jpg"),
                                                                               new Tuple<int, string>(3, ".jpeg"),
                                                                               new Tuple<int, string>(3, ".tiff"),
                                                                               new Tuple<int, string>(3, ".bmp"),
                                                                               // Formaty plików multimedialnych - identyfikator 4:
                                                                               new Tuple<int, string>(4, ".mp4"),
                                                                               new Tuple<int, string>(4, ".avi"),
                                                                               new Tuple<int, string>(4, ".mp3"),
                                                                               new Tuple<int, string>(4, ".wav"),
                                                                               // Identyfikator 5 służy dla plików .doc - są przypadkiem szczególnym:
                                                                               new Tuple<int, string>(5, ".doc")
                                                                               };

        
        

        
        // Ścieżki do poszczególnych plików niezbędnych do działania programu:
        private string program_path = null;
        private string output_path = null;
        //private string xml_path = null;
        //private string txt_path = null;
        private string database_file_path = null;
        private string database_engine_path = null;
        private string database_externals_path = null;

        // Zmienne stanu programu:
        private bool database_validated_successfully = false;

        // Zmienne nawigatora katalogu:
        private List<int> catalog_folder_id_list = new List<int>();
        private List<string> catalog_folder_path_list = new List<string>();

        // Ta zmienna przechowuje status wyświetlania w oknie eksploratora katalogu:
        // 0 - oznacza przeglądanie dostępnych katalogów,
        // 1 - oznacza przeglądanie katalogu lokalnego,
        // 2 - oznacza przeglądanie katalogu obiegowego.
        private int LV_catalog_display_status = 0;
        private string LV_catalog_display_current_catalog_alias = "";

        private ListViewItem[] LV_catalog_display_cache;
        private List<ListViewItem> LV_catalog_display_item_selection = new List<ListViewItem>();
        private List<ListViewItem> LV_catalog_display_data_to_manipulate = new List<ListViewItem>();
        private List<ListViewItem> LV_loaded_catalogs = new List<ListViewItem>();
        private int LV_catalog_display_data_to_manipulate_orgin_directory_id = 0;
        private bool cache_refresh = true, copy = false, cut = false;

        // Zmienne przesyłacza do opcji specialnych:
        private Special_function_window special_option_selector;
        private List<ListViewItem> sent_items;

        // Zmienne związane z funkcjonowaniem opcji sieciowych:
        private DownloadProgressWindow progress_window = null;
        private DistributedNetworkUser local_user = null;
        private DistributedNetwork distributedNetwork = null;
        private List<string> successful_downloads = new List<string>();
        private List<string> failed_downloads = new List<string>();
        private int total_selected = 0;
        private int total_to_download = 0;
        private int successful_downloads_count = 0;
        private int failed_downloads_count = 0;
        public bool networking_lock = true;
        private string recieved_external_catalog_name = "";
        private Timer catalog_download_finalizer = new Timer();
        private string used_IP_address_string = "";
        private string used_alias = ""; 


        #endregion

        #region Konstruktor i jego logika

        // Ładowanie z konfiga danych o użytkowniku lokalnym.
        private bool DetermineLocalUser(bool inital_check)
        {
            bool alias_determined = false, address_determined = false;

            string final_alias = "DEFAULT";
            System.Net.IPAddress final_IP_address = new System.Net.IPAddress(new byte[] { 0, 0, 0 ,0 });

            //Ładujemy dane z pliku konfiguracyjnego

            String grabbed_alias = ConfigManager.ReadString(ConfigManager.USER_ALIAS);
            String grabbed_IP_address_string = ConfigManager.ReadString(ConfigManager.TCP_COMM_IP_ADDRESS);

            if(grabbed_alias.Length == 0 || grabbed_IP_address_string.Length == 0 || grabbed_alias == null || grabbed_IP_address_string == null)
            {
                // Gdy coś poszło nie tak (adres albo alias użytkownika był niezdefiniowany)

                DialogResult dialogResult = DialogResult.Yes;
                if (inital_check)
                {
                    dialogResult = MessageBox.Show("Użytkownik programu jest albo niezdefiniowany albo zdefiniowany niepoprawnie.\n" +
                                                            "Czy chcesz zdefiniować nowego?\n" +
                                                            "Bez poprawnie zdefiniowanego użytkownika funkcjonalności sieciowe są niedostepne.", "Niepoprawny użytkownik"
                                                            , MessageBoxButtons.YesNo);
                }
                
                if(dialogResult==DialogResult.Yes || dialogResult==DialogResult.OK)
                {
                    // Jeżeli niezdefiniowany jest alias - poproś o nowy
                    if (grabbed_alias.Length == 0 || grabbed_alias == null)
                    {
                        MessageBox.Show("Prosimy podać nazwę użytkownika.");
                        String localUserAlias = "";
                        AliasInputForm AliasInputForm = new AliasInputForm(ref localUserAlias);
                        AliasInputForm.Owner = this;
                        AliasInputForm.ShowDialog();
                        localUserAlias = AliasInputForm.resultRef;
                        if (localUserAlias == null || localUserAlias.Equals(""))
                        {
                            MessageBox.Show("Pobieranie nazwy użytkownika zakonczyło się niepowodzeniem!");
                            alias_determined = false;
                        }
                        else
                        {
                            ConfigManager.WriteValue(ConfigManager.USER_ALIAS,
                                localUserAlias);
                            final_alias = localUserAlias;
                            alias_determined = true;
                        }
                    }
                    else
                    {
                        alias_determined = true;
                    }

                    // Jeżeli niezdefiniowany jest adres IP - poproś o nowy. Zakładamy sukces poprzedniego sprawdzenia
                    if ((grabbed_IP_address_string.Length == 0 || grabbed_IP_address_string == null) && alias_determined == true)
                    {
                        // Zmienić tutaj wywołanie z podstawowego podawania adresu na nowy form!
                        MessageBox.Show("Prosimy podać adres IP, pod którym widoczny ma być program.");
                        
                        System.Net.IPAddress localUserIPAddress = null;
                        UserDetermineAddressForm userDetermineAddressForm = new UserDetermineAddressForm(ref localUserIPAddress);
                        userDetermineAddressForm.Owner = this;
                        userDetermineAddressForm.ShowDialog();
                        localUserIPAddress = userDetermineAddressForm.resultRef;

                        if (localUserIPAddress == null)
                        {
                            MessageBox.Show("Pobieranie adresu IP zakonczyło się niepowodzeniem!");
                            ConfigManager.WriteValue(ConfigManager.USER_ALIAS,
                                "");
                            final_alias = "";
                            alias_determined = false;
                            address_determined = false;
                        }
                        else
                        {
                            ConfigManager.WriteValue(ConfigManager.TCP_COMM_IP_ADDRESS,
                                localUserIPAddress.ToString());
                            final_IP_address = localUserIPAddress;
                            address_determined = true;
                        }
                    }
                    if(address_determined == true && alias_determined == true)
                    {
                        local_user = new DistributedNetworkUser(false, final_alias, final_IP_address);
                        networking_lock = false;
                        distributedNetwork = new DistributedNetwork(this);
                        MessageBox.Show("Definicja nowego użytkownika zakończyła się sukcesem!");
                        return true;
                    }
                    else
                    {
                        local_user = new DistributedNetworkUser(false, final_alias, final_IP_address);
                        MessageBox.Show("Próba zdefiniowania nowego użytkownika zakończyła się niepowodzeniem!");
                        networking_lock = true;
                        return false;
                    }
                }
                else
                {
                    local_user = new DistributedNetworkUser(false, final_alias, final_IP_address);
                    networking_lock = true;
                    return false;
                }
            }
            else
            {
                // Wszystkie dane zostały załadowane - walidujemy i jeżeli walidacje są pomyślne - tworzymy użytkownika

                // Najpierw sprawdzamy alias użytkownika - czy nie zawiera nielegalnych znaków
                bool validation_result = true;
                char[] grabbed_alias_checker = grabbed_alias.ToArray();
                List<char> illegal_characters = System.IO.Path.GetInvalidFileNameChars().ToList();

                illegal_characters.Add('_');

                foreach (char illegal_character in illegal_characters)
                {
                    if (grabbed_alias_checker.Contains(illegal_character))
                    {
                        validation_result = false;
                        break;
                    }
                }

                if (validation_result == true)
                {
                    final_alias = grabbed_alias;
                    alias_determined = true;
                }
                else
                {
                    MessageBox.Show("Pobrany z pliku konfiguracyjnego alias zawiera niepoprawne znaki.\nPrzywracam mu wartość domyślną.");
                    ConfigManager.WriteValue(ConfigManager.USER_ALIAS, "");
                    alias_determined = false;
                }

                // Pobieramy adres IP i sprawdzamy czy da się go sparsować do czegoś użytecznego.

                System.Net.IPAddress ipAddress = null;
                try
                {
                    ipAddress = System.Net.IPAddress.Parse(grabbed_IP_address_string);
                    final_IP_address = ipAddress;
                    address_determined = true;
                }
                catch
                {
                    MessageBox.Show("Pobrany z pliku konfiguracyjnego adres nie może być uznany za poprawny!\nPrzywracam wartość domyślną.");
                    ConfigManager.WriteValue(ConfigManager.TCP_COMM_IP_ADDRESS, "");
                    address_determined = false;
                }

                // Teraz powinniśmy już mieć jasną sytuację - sprawdzamy wartości w flagach address_determined i alias_determined

                if (address_determined == true && alias_determined == true)
                {
                    local_user = new DistributedNetworkUser(false, final_alias, final_IP_address);
                    MessageBox.Show("Pomyślnie pobrano dane użytkownika " + final_alias);
                    networking_lock = false;
                    distributedNetwork = new DistributedNetwork(this);
                    return true;
                }
                else
                {
                    // robimy to samo co wcześniej, ale z wartościami ustalanymi na samym początku jako domyślne
                    local_user = new DistributedNetworkUser(false, final_alias, final_IP_address);
                    MessageBox.Show("Nie udało pobrać się danych użytkownika lub były one niepoprawne.\n" +
                                    "Funkcjonalności sieciowe są niedostępne.\n" +
                                    "Aby uzyskać do nich dostęp prosimy zredefiniować użytkownika.");
                    networking_lock = true;
                    return false;
                }
            }
        }

        // Ładowanie ścieżek do odpowiednich plików i folderów z pliku konfiguracyjnego.
        private void DetermineFilepaths()
        {
            if (ConfigManager.ConfigFileExists() == false || ConfigManager.ValidateConfigFile() == false)
            {
                // ERROR - jakimś cudem nie mieliśmy konfiga lub nie został on utworzony poprawnie - regenerujemy nasz plik konfiguracyjny!
                ConfigManager.CreateNewConfigFile();
            }

            if(ConfigManager.ConfigFileExists() == true && ConfigManager.ValidateConfigFile() == true)
            {
                // Wszystko w porządku, pobieramy wartości z konfigów do programu

                program_path = ConfigManager.ReadString(ConfigManager.PROGRAM_LOCATION);
                output_path = ConfigManager.ReadString(ConfigManager.OUTPUT_LOCATION);

                database_engine_path = ConfigManager.ReadString(ConfigManager.DATABASE_ENGINE_LOCATION);
                database_file_path = ConfigManager.ReadString(ConfigManager.LOCAL_DATABASE_LOCATION);
                database_externals_path = ConfigManager.ReadString(ConfigManager.EXTERNAL_DATABASES_LOCATION);
            }

            if(!Directory.Exists(program_path))
            {
                // FATAL ERROR - nigdy nie powinien zajść, ale jeżeli jakimś cudem jest - wyłącz program!
                Application.Exit();
                Environment.Exit(0);
            }

            if(!Directory.Exists(output_path))
            {
                // UNUSED
            }

            if (!Directory.Exists(database_externals_path))
            {
                // WARNING - jeżeli nie jest poprawnie zdefiniowany folder z katalogami obiegowymi to stwórz go!
                Directory.CreateDirectory(database_externals_path);
            }
            
            if (!File.Exists(database_file_path))
            {
                MessageBox.Show("Uwaga - lokacja katalogu lokalnego jest niepoprawna, albo nie został on jeszcze utworzony!");
            }

            if (!File.Exists(database_engine_path))
            {
                // FATAL ERROR - nie znaleziono .dll'ki do obsługi połączenia z bazą danych!
                Application.Exit();
                Environment.Exit(0);
            }
        }

        /* Tworzenie reprezentacji bazy danych (jej tabel i kolumn) w programie
         * 
         * Dodajemy do list: database_tables i database_columns; wartości wyznaczone empirycznie na podstawie danych testowych.
         * 
         * Gdy funkcja kończy działanie, mamy już wszystkie informacje o strukturze bazy załadowane i gotowe do korzystania.
         * 
        */
        // Procedura przechowująca konstrukcję bazy danych.
        private void DetermineDatabaseConstruction()
        {
            database_tables = new List<Tuple<int, string>>();
            database_columns = new List<Tuple<int, string, string>>();

            // Tutaj tworzymy dane o tabelach bazy danych:
            database_tables.Add(new Tuple<int, string>(0, @"virtual_folder"));
            database_tables.Add(new Tuple<int, string>(1, @"metadata_text"));
            database_tables.Add(new Tuple<int, string>(2, @"metadata_document"));
            database_tables.Add(new Tuple<int, string>(3, @"metadata_complex"));
            database_tables.Add(new Tuple<int, string>(4, @"metadata_image"));
            database_tables.Add(new Tuple<int, string>(5, @"metadata_multimedia"));

            // Tutaj tworzymy dane o kolumnach bazy danych:

            // Definicja kolumn tabeli virtual_folder:
            database_columns.Add(new Tuple<int, string, string>(0, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(0, @"NAME", @"VARCHAR(512) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(0, @"DIR_ID", @"INT")); // null dozwolony tylko dla katalogu nadrzędnego.
            database_columns.Add(new Tuple<int, string, string>(0, @"MODIFIABLE", @"BOOLEAN NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(0, @"VISIBLE_TO_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(0, @"REQUESTABLE_BY_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(0, @"COPIES_WITHOUT_CONFIRM", @"BOOLEAN DEFAULT FALSE"));

            // Definicja kolumn ogólnych tabeli metadata_text:
            database_columns.Add(new Tuple<int, string, string>(1, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(1, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(1, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(1, @"ORIGINAL_NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"EXTENSION", @"VARCHAR(64) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"PATH", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"ALTERNATE_PATHS", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(1, @"SIZE", @"BIGINT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"VISIBLE_TO_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(1, @"REQUESTABLE_BY_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(1, @"COPIES_WITHOUT_CONFIRM", @"BOOLEAN DEFAULT FALSE"));
            // Definicja kolumn specyficznych dla metadata_text:
            database_columns.Add(new Tuple<int, string, string>(1, @"CONTENT_TYPE", @"VARCHAR(80)"));
            database_columns.Add(new Tuple<int, string, string>(1, @"CONTENT_ENCODING", @"VARCHAR(64)"));

            // Definicja kolumn ogólnych tabeli metadata_document:
            database_columns.Add(new Tuple<int, string, string>(2, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(2, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(2, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(2, @"ORIGINAL_NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"EXTENSION", @"VARCHAR(64) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"PATH", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"ALTERNATE_PATHS", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"SIZE", @"BIGINT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"VISIBLE_TO_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(2, @"REQUESTABLE_BY_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(2, @"COPIES_WITHOUT_CONFIRM", @"BOOLEAN DEFAULT FALSE"));
            // Definicja kolumn specyficznych dla metadata_document:
            database_columns.Add(new Tuple<int, string, string>(2, @"CONTENT_TYPE",  @"VARCHAR(80)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"CREATION_TIME", @"TIMESTAMP"));
            database_columns.Add(new Tuple<int, string, string>(2, @"LAST_WRITE_TIME", @"TIMESTAMP"));
            database_columns.Add(new Tuple<int, string, string>(2, @"LANGUAGE", @"VARCHAR(128) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"TITLE", @"VARCHAR(512) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"SUBJECT", @"VARCHAR(512) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"DESCRIPTION", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"KEYWORDS", @"VARCHAR(512) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"COMMENTS", @"VARCHAR(512) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"PUBLISHER", @"VARCHAR(512) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"COMPANY", @"VARCHAR(512) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"AUTHOR", @"VARCHAR(512) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"CREATOR", @"VARCHAR(512) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"LAST_AUTHOR", @"VARCHAR(512) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"PAGE_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"TABLE_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"OBJECT_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"IMAGE_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"WORD_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"CHARACTER_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"APPLICATION_NAME", @"VARCHAR(64) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(2, @"APPLICATION_VERSION", @"VARCHAR(64) CHARACTER SET UTF8"));

            //Definicja kolumn ogólnych tabeli metadata_complex:
            database_columns.Add(new Tuple<int, string, string>(3, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(3, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(3, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(3, @"ORIGINAL_NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"EXTENSION", @"VARCHAR(64) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"PATH", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"ALTERNATE_PATHS", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(3, @"SIZE", @"BIGINT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"VISIBLE_TO_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(3, @"REQUESTABLE_BY_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(3, @"COPIES_WITHOUT_CONFIRM", @"BOOLEAN DEFAULT FALSE"));
            // Definicja kolumn specyficznych dla metadata_complex:
            database_columns.Add(new Tuple<int, string, string>(3, @"CONTENT_TYPE", @"VARCHAR(80)"));
            database_columns.Add(new Tuple<int, string, string>(3, @"CONTENT_ENCODING", @"VARCHAR(64)"));
            database_columns.Add(new Tuple<int, string, string>(3, @"TITLE", @"VARCHAR(512)"));
            // Dalsze kolumny przechowywałyby dane z pliku, nam zależy tylko na metadanych!
            // Jako że ekstraktor nie obsługuje wyłuskiwania metadanych (jego ekstraktor jest pusty), a pisanie własnego parsera jest poza budżetem czasowym
            // Niestety nie przechowujemy więcej niż tylko metadane dla plików html...
            
            /*
            database_columns.Add(new Tuple<int, string, string>(3, @"VM_COUNT", @"INT"));

            for (int i = 0; i < 64; i++)
            {
                database_columns.Add(new Tuple<int, string, string>(3, @"VM_" + i.ToString() + @"_NAME", @"VARCHAR(64) CHARACTER SET UTF8"));
                database_columns.Add(new Tuple<int, string, string>(3, @"VM_" + i.ToString() + @"_DATA", @"VARCHAR(128) CHARACTER SET UTF8"));
            }
            */

            // Definicja kolumn ogólnych tabeli metadata_image:
            database_columns.Add(new Tuple<int, string, string>(4, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(4, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(4, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(4, @"ORIGINAL_NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"EXTENSION", @"VARCHAR(64) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"PATH", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"ALTERNATE_PATHS", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(4, @"SIZE", @"BIGINT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"VISIBLE_TO_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(4, @"REQUESTABLE_BY_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(4, @"COPIES_WITHOUT_CONFIRM", @"BOOLEAN DEFAULT FALSE"));
            // Definicja kolumn specyficznych dla metadata_image:
            database_columns.Add(new Tuple<int, string, string>(4, @"CONTENT_TYPE", @"VARCHAR(80) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(4, @"COMMENT", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(4, @"COMPRESSION_TYPE", @"VARCHAR(80) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(4, @"WIDTH", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(4, @"HEIGHT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(4, @"PHASH", @"BIGINT"));

            //Definicja kolumn ogólnych tabeli metadata_multimedia
            database_columns.Add(new Tuple<int, string, string>(5, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(5, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(5, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(5, @"ORIGINAL_NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"NAME", @"VARCHAR(1024) CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"EXTENSION", @"VARCHAR(64) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"PATH", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8 NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"ALTERNATE_PATHS", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"SIZE", @"BIGINT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"VISIBLE_TO_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(5, @"REQUESTABLE_BY_OTHERS", @"BOOLEAN DEFAULT FALSE"));
            database_columns.Add(new Tuple<int, string, string>(5, @"COPIES_WITHOUT_CONFIRM", @"BOOLEAN DEFAULT FALSE"));
            // Definicja kolumn specyficznych dla metadata_multimedia:
            database_columns.Add(new Tuple<int, string, string>(5, @"CONTENT_TYPE", @"VARCHAR(80) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"TITLE", @"VARCHAR(256) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"TRACK_NUMBER", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(5, @"ALBUM", @"VARCHAR(256) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"RELEASE_DATE", @"VARCHAR(64) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"AUTHOR", @"VARCHAR(256) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"GENRE", @"VARCHAR(256) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"DURATION", @"VARCHAR(32) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"SAMPLING_RATE", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(5, @"AUDIO_COMPRESSOR", @"VARCHAR(128) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"WIDTH", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(5, @"HEIGHT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(5, @"FRAME_RATE", @"VARCHAR(6) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"VIDEO_COMPRESSOR", @"VARCHAR(128) CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"COMMENT", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8"));
            database_columns.Add(new Tuple<int, string, string>(5, @"EXTRACTED_TEXT", @"BLOB SUB_TYPE TEXT CHARACTER SET UTF8"));
        }

        // Ładowanie skryptów tworzących tabele bazy danych i ich kolumny, na podstawie danych w database_tables i database_rows
        private void LoadCreationScripts()
        {
            creation_scripts = new List<string>();

            Tuple<int, string> current_table;
            List<Tuple<int, string, string>> current_table_column_container;
            string current_creation_script = String.Empty;


            // Uniwersalny skrypt tworzący konfigi.
            for (int i = 0; i < database_tables.Count; i++)
            {
                current_table = database_tables.Find(x => x.Item1.Equals(i));
                current_table_column_container = database_columns.FindAll(x => x.Item1 == current_table.Item1);
                current_creation_script = "CREATE TABLE " + current_table.Item2 + " (";
                while (current_table_column_container.Count != 0)
                {
                    if (current_table_column_container.Count != 1)
                    {
                        current_creation_script += " " +
                                                           current_table_column_container.First().Item2 +
                                                           " " +
                                                           current_table_column_container.First().Item3 +
                                                           ",";
                    }
                    if (current_table_column_container.Count == 1)
                    {
                        current_creation_script += " " +
                                                           current_table_column_container.First().Item2 +
                                                           " " +
                                                           current_table_column_container.First().Item3 +
                                                           ");";
                    }
                    current_table_column_container.RemoveAt(current_table_column_container.IndexOf(current_table_column_container.First()));
                }
                creation_scripts.Add(current_creation_script);
            }
        }

        // Przygotowanie stringa do łączenia z bazą danych
        void PrepareConnectionString()
        {
            database_connection_string_builder = new FbConnectionStringBuilder();
            database_connection_string_builder.ServerType = FbServerType.Embedded;
            database_connection_string_builder.UserID = "SYSDBA"; // Defaultowy uzytkownik z najwyzszymi uprawnieniami do systemu bazodanowego.
            database_connection_string_builder.Password = ""; // Haslo nie jest sprawdzane w wersji embedded, można dać tu cokolwiek.
            database_connection_string_builder.Database = database_file_path;
            database_connection_string_builder.ClientLibrary = database_engine_path;
            database_connection_string_builder.Charset = "UTF8";
        }

        // Ładowanie lokalnego katalogu do listy wyświetlanych katalogów podczas jej wyświelania
        void LoadLocalCatalog()
        {
            ListViewItem local_catalog = new ListViewItem();

            local_catalog.Name = database_connection_string_builder.ConnectionString;
            local_catalog.Text = "Katalog lokalny";
            local_catalog.SubItems.Add("Katalog");
            local_catalog.SubItems.Add("---");
            local_catalog.SubItems.Add("---");
            local_catalog.SubItems.Add("---");
            local_catalog.SubItems.Add("---");
            local_catalog.SubItems.Add("---");
            local_catalog.SubItems.Add("---");

            LV_loaded_catalogs.Add(local_catalog);
        }

        // Obsługa logiki wymaganej przez WPF i resizing okna.
        public Main_form()
        {
            // Check if our config file does not exist, generate a new one!
            if(!ConfigManager.ConfigFileExists())
            {
                ConfigManager.CreateNewConfigFile();
            }
            
            // Only allow app to run after successful login 
            // or password setting
            new AppInstanceLoginManager().DisplayLoginRegisterForm();


            /*
            String ipAddressString = ConfigManager.ReadString(ConfigManager.TCP_COMM_IP_ADDRESS);
            if(ipAddressString.Length == 0)
            {
                MessageBox.Show("Prosimy podać adres IP, pod którym widoczny ma być program.");
                System.Net.IPAddress localUserIPAddress = null;
                IPAddressInputForm ipAddressInputForm = new IPAddressInputForm(ref localUserIPAddress);
                ipAddressInputForm.Owner = this;
                ipAddressInputForm.ShowDialog();
                localUserIPAddress = ipAddressInputForm.resultRef;
                if (localUserIPAddress == null)
                {
                    MessageBox.Show("Format podanego adresu IP nie może zostać uznany za poprawny!");
                    networking_lock = true;
                }
                else
                {
                    ConfigManager.WriteValue(ConfigManager.TCP_COMM_IP_ADDRESS,
                        localUserIPAddress.ToString());
                }
            } 
            else
            {
                System.Net.IPAddress ipAddress = null;
                try
                {
                    ipAddress = System.Net.IPAddress.Parse(ipAddressString);

                }
                catch
                {

                }
            }
            */

            AppCryptoDataStorage.UserAuthorized = true;
            new DatabaseEncryptor().DecryptDatabaseFile();
            InitializeComponent();

            DetermineFilepaths();
            DetermineDatabaseConstruction();
            LoadCreationScripts();
            PrepareConnectionString();
            LoadLocalCatalog();
            // zbedne? DetermineLocalAddress();
            DetermineLocalUser(true);

            metadata = new List<string[]>();
            directories_grabbed = new List<Tuple<int, string, bool, bool, bool>>();
            files_grabbed = new List<Tuple<int, string, string, string, System.DateTime, System.DateTime, long, Tuple<bool,bool,bool>>>();

            // Funkcjonalności sieciowe są włączane w kodzie DetermineLocalUser!
        }

        #endregion

        #region Logika zakładki Menu główne

        /*    Sprawdzanie czy istnieje baza danych (i jej walidacja jeżeli istnieje)
         *    
         *    Pierwsze polecenie przemieszcza nas do whereever\PRI-KATALOGOWANIE-PLIKÓW\PRI-KATALOGOWANIE-PLIKÓW\bin\Debug\ do katalogu nadrzedzego,
         *    drugie przemieszcza nas do whereever\PRI-KATALOGOWANIE-PLIKÓW\PRI-KATALOGOWANIE-PLIKÓW\bin do katalogu nadrzedzego.
         *    
         *    Efektem czego jestesmy w whereever\PRI-KATALOGOWANIE-PLIKÓW\PRI-KATALOGOWANIE-PLIKÓW\ do katalogu nadrzedzego, co poki co stanowi nasz
         *    folder działania (mamy tutaj .xml'ke, tekstowke itp. itd.)
         *    
         *    Wynikowo struktura programu ma wygladac w ten desen:
         *    bin\ - tutaj żyja nasze .dll'ki i .exe'c aplikacji
         *    db\ - tutaj żyje nasz katalog, w pliku catalog.fdb
         *    
         *    Tutaj zakladam ze program jest schowany glebiej przez Visual Studio, stad potrzebne są nam dodatkowe skoki. Przepisanie tej procedury na czysto
         *    nie jest problemem.
         *    
         *    UWAGA: Moze wygenerowac IOException jeżeli coś innego czyta nasze katalogi!
         */
        private void BT_test_database_click(object sender, EventArgs e)
        {
            if (File.Exists(database_file_path))
            {
                int[] validation_result = new int[database_tables.Count];
                string[] validation_return_string_container = new string[database_tables.Count];
                string validation_return_string = String.Empty;

                // Box ogłaszający znalezienie bazy.
                MessageBox.Show("Znalazłem istniejącą bazę, przechodzę do sprawdzenia jej poprawności!");

                validation_result = database_validate(database_connection_string_builder.ConnectionString);

                for (int i = 0; i < validation_result.Count(); i++)
                {
                    if (validation_result[i] == 0) validation_return_string_container[i] = "Tabela " + database_tables[i].Item2 + " nie istnieje w bazie! \n";
                    if (validation_result[i] == 1) validation_return_string_container[i] = "Tabela " + database_tables[i].Item2 + " ma niepoprawne pola! \n";
                    if (validation_result[i] == 2) validation_return_string_container[i] = "Tabela " + database_tables[i].Item2 + " jest skonstruowana poprawnie! \n";
                }

                if (validation_result.All(x => x.Equals(2)) == true)
                {
                    database_validated_successfully = true;
                    BT_extract_metadata.Enabled = true;
                }

                for (int i = 0; i < validation_return_string_container.Length; i++)
                {
                    validation_return_string += validation_return_string_container[i];
                }
                MessageBox.Show(validation_return_string);
            }
            else
            {
                MessageBox.Show("Nie znalazłem bazy, tworze nową w katalogu:\n" + program_path + @"\db");

                if (!Directory.Exists(program_path + @"\db"))
                {
                    //DEBUG MessageBox.Show("Tworze katalog db!");
                    Directory.CreateDirectory(program_path + @"\db");
                }

                FbConnection.CreateDatabase(database_connection_string_builder.ConnectionString);

                database_build(database_connection_string_builder.ConnectionString, false);

                MessageBox.Show("Zakonczono budowę bazy!");

                database_validated_successfully = true;
                BT_extract_metadata.Enabled = true;
            }
        }

        /*  Ekstracja metadanych - wywołanie okna odpowiedzialnego za jej obsługę
         *  
         *  Informacje o tym jak konkretnie robimy ekstrakcję metadanych znajdują się w formularzu Metadata_extractor.cs
         * 
         */
        private void BT_extract_metadata_click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog open = new FolderBrowserDialog())
            {
                if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    DirectoryInfo selected_directory = new DirectoryInfo(open.SelectedPath);

                    metadata_extractor_window = new Metadata_extractor();
                    metadata_extractor_window.Owner = this;
                    metadata_extractor_window.extends = extends;
                    metadata_extractor_window.target_directory = selected_directory;
                    metadata_extractor_window.OnDataAvalible += metadata_extractor_OnDataAvalible;
                    metadata_extractor_window.Show();
                }
            }
        }

        // Działa nie do końca poprawnie, czasem zjada dane w ALTERNATE_PATHS... no time to debug!
        private void metadata_extractor_OnDataAvalible(object sender, EventArgs e)
        {
            // Tutaj końcowo przeprowadzimy katalogowanie, a nie przez checkbox'a.

            // Zmienne globalne katalogowania
            bool first_cataloging = false, further_cataloging = false;
            List<string[]> metadata_working_set = new List<string[]>();
            

            // Najpierw sprawdzamy z którym katalogowaniem mamy do czynienia - de facto czy jest cos w katalogu
            if (metadata.Count != 0)
            {
                DataTable metadata_text_container = new DataTable();
                DataTable metadata_document_container = new DataTable();
                DataTable metadata_complex_container = new DataTable();
                DataTable metadata_image_container = new DataTable();
                DataTable metadata_multimedia_container = new DataTable();

                // Pobierz zawartośc bazy danych - dokładniej wszystko to co zostało już skatalogowane
                for (int i = 1; i < database_tables.Count(); i++)
                {
                    DataTable current_worked_table = new DataTable();
                    FbDataAdapter table_grabber = new FbDataAdapter("SELECT * " +
                                                                    "FROM " + database_tables[i].Item2 + ";"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

                    table_grabber.Fill(current_worked_table);

                    // Wyrzucamy z katalogu wszystko, co już nie ma przełożenia na żadny plik rzeczywisty
                    if (current_worked_table.Rows.Count != 0)
                    {
                        int current_worked_id = 0;
                        FileInfo file_check;
                        foreach (DataRow row in current_worked_table.Rows)
                        {
                            bool file_found = false;
                            string current_filepath = string.Empty;
                            current_worked_id = (int)row.ItemArray[0];
                            current_filepath = (string)row.ItemArray[6];
                            file_check = new FileInfo(current_filepath);
                            if (file_check.Exists == false)
                            {
                                List<string> alternate_paths;
                                if (row.ItemArray[7].GetType().Name.Equals("DBNull") || row.ItemArray[7].Equals(string.Empty)) alternate_paths = new List<string>();
                                else alternate_paths = ((string)row.ItemArray[7]).Split('|').ToList();
                                if (alternate_paths.Count != 0)
                                {
                                    foreach (string alternate_filepath in alternate_paths)
                                    {
                                        current_filepath = alternate_filepath;
                                        file_check = new FileInfo(alternate_filepath);
                                        if (file_check.Exists == true)
                                        {
                                            // Znaleźliśmy co chcieliśmy, modyfikujemy zmienna path w naszej zmiennej aby wskazywała na nasz działający path z ALTERNATE_PATHS
                                            FbCommand new_path_setter = new FbCommand("UPDATE " + database_tables[i].Item2 + " " +
                                                                                      "SET PATH = @New_path, ALTERNATE_PATH = @New_alternate_paths " +
                                                                                      "WHERE ID = @Id;"
                                                                                      ,
                                                                                      new FbConnection(database_connection_string_builder.ConnectionString));

                                            alternate_paths.Remove(alternate_filepath);

                                            new_path_setter.Parameters.AddWithValue("@Id", current_worked_id);
                                            new_path_setter.Parameters.AddWithValue("@New_path", current_filepath);
                                            new_path_setter.Parameters.AddWithValue("@New_alternate_paths", alternate_paths);

                                            new_path_setter.Connection.Open();
                                            new_path_setter.ExecuteNonQuery();
                                            new_path_setter.Connection.Close();

                                            file_found = true;
                                            break;
                                        }
                                    }
                                }
                                if (file_found == false)
                                {
                                    // Nie znalazł pliku we wszystkich możliwych miejscach - usuwamy go z bazy
                                    FbCommand metadata_remover = new FbCommand("DELETE FROM " + database_tables[i].Item2 + " " +
                                                                               "WHERE ID = @Id;"
                                                                               ,
                                                                               new FbConnection(database_connection_string_builder.ConnectionString));

                                    metadata_remover.Parameters.AddWithValue("@Id", current_worked_id);

                                    metadata_remover.Connection.Open();
                                    metadata_remover.ExecuteNonQuery();
                                    metadata_remover.Connection.Close();
                                }
                            }
                        }

                        FbDataAdapter table_after_cleanup_grabber = new FbDataAdapter("SELECT * " +
                                                                                      "FROM " + database_tables[i].Item2 + ";"
                                                                                      ,
                                                                                      new FbConnection(database_connection_string_builder.ConnectionString));
                        
                        switch (i)
                        {
                            case 1:
                                table_after_cleanup_grabber.Fill(metadata_text_container);
                                break;
                            case 2:
                                table_after_cleanup_grabber.Fill(metadata_document_container);
                                break;
                            case 3:
                                table_after_cleanup_grabber.Fill(metadata_complex_container);
                                break;
                            case 4:
                                table_after_cleanup_grabber.Fill(metadata_image_container);
                                break;
                            case 5:
                                table_after_cleanup_grabber.Fill(metadata_multimedia_container);
                                break;
                            default:
                                MessageBox.Show("ERROR: Przy czytaniu danych z tabeli wyszliśmy poza zakres database_tables!");
                                break;
                        }
                    }
                }

                // Przejście do decyzji odnoście danych zawartych w metadata - w zależności od tego czy katalog jest pusty, czy coś już w nim jest.
                if (metadata_text_container.Rows.Count == 0 &&
                    metadata_document_container.Rows.Count == 0 &&
                    metadata_complex_container.Rows.Count == 0 &&
                    metadata_image_container.Rows.Count == 0 &&
                    metadata_multimedia_container.Rows.Count == 0) first_cataloging = true;
                else further_cataloging = true;
            }

            metadata_working_set = metadata.ToList();

            for (int i = 0; i < metadata_working_set.Count; i++)
            {
                List<string> current_metadata_expander = new List<string>();
                current_metadata_expander = metadata_working_set[i].ToList();
                current_metadata_expander.Insert(1, metadata_working_set[i].ElementAt(1));
                current_metadata_expander.Insert(5, string.Empty);
                metadata_working_set[i] = current_metadata_expander.ToArray();
            }

            int iter_count = metadata_working_set.Count;
            for (int i = 0; i < iter_count; i++)
            {
                string[] current_metadata = metadata_working_set.ElementAt(i);
                string alternate_path = string.Empty;
                List<string[]> matches = metadata_working_set.FindAll(x =>
                                                                      x.ElementAt(2) == current_metadata.ElementAt(2) &&
                                                                      x.ElementAt(3) == current_metadata.ElementAt(3) &&
                                                                      x.ElementAt(6) == current_metadata.ElementAt(6));

                if (matches.Count > 0) matches.Remove(current_metadata); // Check coby nie zwinął samego siebie :P

                foreach (string[] match in matches)
                {
                    if (alternate_path == string.Empty || alternate_path =="") alternate_path += match.ElementAt(4);
                    else alternate_path += '|' + match.ElementAt(4);
                    metadata_working_set.Remove(match);
                }

                current_metadata[5] = alternate_path;
                iter_count = metadata_working_set.Count;
            }
            // Po przejściu poprzedniej pętli powinniśmy mieć listę metadanych zderukowaną o powtórzenia tego samego pliku.
            // Jak i listę alternatywnych ścieżek odpowiadająca 1:1 liście w metadata_working_set


            //Teraz sprawdzamy czy wystąpiły w naszej liście pliki o tej samej nazwie, ale innej zawartości
            for (int i = 0; i < metadata_working_set.Count; i++)
            {
                string[] current_metadata = metadata_working_set.ElementAt(i);
                List<string[]> differences = metadata_working_set.FindAll(x =>
                                                                          x.ElementAt(2) == current_metadata.ElementAt(2) &&
                                                                          x.ElementAt(3) == current_metadata.ElementAt(3) &&
                                                                          x.ElementAt(6) != current_metadata.ElementAt(6));

                for (int j = 0; j < differences.Count; j++)
                {
                    string[] current_difference = differences[j];
                    int where_in_parent = metadata_working_set.IndexOf(current_difference);
                    string name_new = current_difference.ElementAt(1) + " " + (j + 1).ToString();
                    current_difference[2] = name_new;
                    metadata_working_set[where_in_parent] = current_difference.ToArray();
                }
            }

            // Mamy pierwsze katalogowanie - w bazie nic nie ma, możemy dodać nasz wyredukowany zbiór!
            if (first_cataloging == true)
            {
                cataloging_add_data(metadata_working_set);
            }

            // Mamy dalsze katalogowanie - w bazie coś jest, trzeba wyszukać powtarzające się encje i rozwiązać konflikty.
            if (further_cataloging == true)
            {
                iter_count = metadata_working_set.Count;
                for (int i = 0; i < iter_count; i++)
                {
                    DataTable search_result_container = new DataTable();

                    // Zwraca wszystkie metadane o tej samej nazwie pliku macierzystego - trzeba je potem przefiltrować po zawartości path i alt_path.
                    FbDataAdapter content_search_result_grabber = new FbDataAdapter("SELECT ID,ORIGINAL_NAME,EXTENSION,SIZE,PATH,ALTERNATE_PATHS " +
                                                                                    "FROM " + metadata_working_set[i].ElementAt(0) + " " +
                                                                                    "WHERE ORIGINAL_NAME = @Original_name " +
                                                                                    "AND EXTENSION = @Extension;"
                                                                                    ,
                                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

                    content_search_result_grabber.SelectCommand.Parameters.AddWithValue("@Original_name", metadata_working_set[i].ElementAt(1));
                    content_search_result_grabber.SelectCommand.Parameters.AddWithValue("@Extension", metadata_working_set[i].ElementAt(3));
                    content_search_result_grabber.Fill(search_result_container);

                    if(search_result_container.Rows.Count == 0)
                    {
                        // Nie znaleźliśmy w bazie pliku o takiej nazwie pierwotnej i rozszerzeniu, musi on zostać dodany. Nic nie robimy - jest już w zbiorze do dodania.
                    }
                    else
                    {
                        // Mamy co najmniej jeden plik, który miał taką samą nazwę pierwotną i rozszerzenie podczas katalogowania. Sprawdzamy czy to takie same, czy inne pliki.
                        bool copy_found = false, path_new_added = false;
                        for (int j = 0; j < search_result_container.Rows.Count; j++)
                        {
                            // Sprawdzamy rozmiary plików z bazy z parametrami metadanej do dodania
                            if (long.Parse(metadata_working_set[i].ElementAt(6)) == (long)search_result_container.Rows[j].ItemArray[3])
                            {
                                // Znaleźliśmy duplikat, sprawdzamy czy ma z sobą nową lokację (w stosunku do już przechowywanych w bazie).
                                List<string> filepaths_all_from_DB = new List<string>();
                                string[] filepaths_alternate_from_DB = new string[0];

                                filepaths_all_from_DB.Add((string)search_result_container.Rows[j].ItemArray[4]);
                                filepaths_alternate_from_DB = ((string)search_result_container.Rows[j].ItemArray[5]).Split('|').ToArray();
                                if (!filepaths_alternate_from_DB.Contains(string.Empty))
                                {
                                    foreach (string filepath in filepaths_alternate_from_DB)
                                    {
                                        filepaths_all_from_DB.Add(filepath);
                                    }
                                }

                                if (filepaths_all_from_DB.Contains(metadata_working_set[i].ElementAt(4)))
                                {
                                    // Plik jest już w bazie i jest w nowym miejscu, wyrzucamy go z listy plików do dodania.
                                    copy_found = true;
                                    break;
                                }
                                else
                                {
                                    // Plik jest już w bazie, ale w nowym miejscu - musimy zmodyfikować zawartość alt_pathu dla danego rekordu.
                                    string alternate_paths_new = string.Empty;
                                    if (!(filepaths_alternate_from_DB.Contains(string.Empty)))
                                    {
                                        foreach (string filepath in filepaths_alternate_from_DB)
                                        {
                                            alternate_paths_new += filepath + '|';
                                        }
                                    }

                                    alternate_paths_new += metadata_working_set[i].ElementAt(4);

                                    FbCommand new_path_adder = new FbCommand("UPDATE " + metadata_working_set[i].ElementAt(0) + " " +
                                                                             "SET ALTERNATE_PATHS = @New_alternate_paths " +
                                                                             "WHERE ID = @Id;"
                                                                             ,
                                                                             new FbConnection(database_connection_string_builder.ConnectionString));

                                    new_path_adder.Parameters.AddWithValue("@Id", search_result_container.Rows[j].ItemArray[0]);
                                    new_path_adder.Parameters.AddWithValue("@New_alternate_paths", alternate_paths_new);

                                    new_path_adder.Connection.Open();
                                    new_path_adder.ExecuteNonQuery();
                                    new_path_adder.Connection.Close();

                                    path_new_added = true;
                                    break;
                                }
                            }
                            else
                            {
                                // Pudło, przeszukujemy dalej.
                            }
                        }
                        if(copy_found == false && path_new_added == false)
                        {
                            // Nie znaleźliśmy pliku o takim samym rozmiarze, stąd jest to kompletnie nowy plik, ale o tej samej nazwie. Trzeba mu wygenerować nową nazwę
                            // i zmienić ją w metadata_working_set przed jego dodaniem.
                            string[] element_new = metadata_working_set[i];
                            string name_new = metadata_working_set[i].ElementAt(1) + " " + search_result_container.Rows.Count;

                            element_new[2] = name_new;
                            metadata_working_set[i] = element_new;
                        }
                        else
                        {
                            metadata_working_set.RemoveAt(i);
                            iter_count = metadata_working_set.Count;
                            i--;
                        }
                    }
                }
                if (metadata_working_set.Count != 0)
                {
                    cataloging_add_data(metadata_working_set);
                }
                else
                {
                    MessageBox.Show("Wskazany folder nie zawierał nowych plików, katalog pozostał bez zmian.");
                }
            } 
        }

        // Odpowiada za dodanie przetworzonej listy plików do katalogu
        private void cataloging_add_data(List<string[]> working_set)
        {
            for (int i = 0; i < working_set.Count; i++)
            // Dopisywanie zawartości do bazy danych
            {
                string destination = String.Empty;
                string datafields = String.Empty;
                string values = String.Empty;
                string[] values_passed = new string[0];
                int parent_directory_ID = 0;

                if (working_set.Count != 0)
                {
                    destination = working_set[i].ElementAt(0);
                    var datatable_index = database_tables.Find(x => x.Item2.Equals(destination)).Item1;

                    var columns_to_populate = database_columns.FindAll(x => x.Item1 == datatable_index);
                    columns_to_populate.RemoveAll(x => x.Item2.Equals("CATALOGING_DATE")); // wyrzucamy z automatycznego wypelniania kolumnę CATALOGING_DATE
                    columns_to_populate.RemoveAll(x => x.Item2.Equals("VISIBLE_TO_OTHERS"));
                    columns_to_populate.RemoveAll(x => x.Item2.Equals("REQUESTABLE_BY_OTHERS"));
                    columns_to_populate.RemoveAll(x => x.Item2.Equals("COPIES_WITHOUT_CONFIRM"));

                    values_passed = new string[columns_to_populate.Count - 1];

                    // ID nie wypełniamy z ręki (robi to automatycznie silnik bazodanowy), stąd startujemy tutaj przejście od 1.
                    for (int j = 1; j < columns_to_populate.Count; j++)
                    {
                        datafields += columns_to_populate[j].Item2 + ",";
                        values += "@" + columns_to_populate[j].Item2 + ",";
                        values_passed[j - 1] = columns_to_populate[j].Item2;
                    }

                    datafields = datafields.TrimEnd(',');
                    values = values.TrimEnd(',');

                    FbCommand add_data = new FbCommand("INSERT INTO " + destination + "(" + datafields + ") VALUES (" + values + ")", new FbConnection(database_connection_string_builder.ConnectionString));

                    // Uwaga na warości ID, DIR_ID i CATALOGING_DATE, trzeba je wypełniać inaczej niż automatem.
                    // ID i CATALOGING_DATE wypelnia baza danych.
                    // Wypełniamy tutaj wartość dla DIR_ID na podstawie wcześniej stworzonych folderów wirtualnych w katalogu.

                    if (datatable_index == 1) parent_directory_ID = 2;
                    if (datatable_index == 2) parent_directory_ID = 3;
                    if (datatable_index == 3) parent_directory_ID = 4;
                    if (datatable_index == 4) parent_directory_ID = 5;
                    if (datatable_index == 5) parent_directory_ID = 6;
                    add_data.Parameters.AddWithValue(values_passed[0], parent_directory_ID);

                    for (int j = 1; j < working_set[i].Count(); j++)
                    {
                        if (!(working_set[i].ElementAt(j).Equals("NULL")))
                        {
                            add_data.Parameters.AddWithValue(values_passed[j], working_set[i].ElementAt(j));
                            //add_data.Parameters[j - 1].Charset = FbCharset.Utf8;
                        }
                        else
                        {
                            add_data.Parameters.AddWithValue(values_passed[j], null);
                            //add_data.Parameters[j - 1].Charset = FbCharset.Utf8;
                        }
                    }

                    if (working_set[i].Count() < values_passed.Length)
                    {
                        for (int j = working_set[i].Count(); j < values_passed.Length; j++)
                        {
                            add_data.Parameters.AddWithValue(values_passed[j], null);
                            //add_data.Parameters[j - 1].Charset = FbCharset.Utf8;
                        }
                    }

                    add_data.Parameters.AddWithValue(@"VISIBLE_TO_OTHERS", false);
                    add_data.Parameters.AddWithValue(@"REQUESTABLE_BY_OTHERS", false);
                    add_data.Parameters.AddWithValue(@"COPIES_WITHOUT_CONFIRM", false);

                    add_data.Connection.Open();
                    add_data.ExecuteNonQuery();
                    add_data.Connection.Close();
                }
            }
        }

        #endregion

        #region Operacje na bazie danych

        // Tworzenie plików w wirtualnych folderach
        private void database_virtual_item_make(int parent_id, string type, List<string> data)
        {
            string destination = String.Empty;
            string datafields = String.Empty;
            string values = String.Empty;
            string[] values_passed = new string[0];

            if (data.Count != 0)
            {
                destination = type;
                var datatable_index = database_tables.Find(x => x.Item2.Equals(destination)).Item1;

                var columns_to_populate = database_columns.FindAll(x => x.Item1 == datatable_index);
                values_passed = new string[columns_to_populate.Count - 1];

                // ID nie wypełniamy z ręki (robi to automatycznie silnik bazodanowy), stąd startujemy tutaj przejście od 1.
                for (int j = 1; j < columns_to_populate.Count; j++)
                {
                    datafields += columns_to_populate[j].Item2 + ",";
                    values += "@" + columns_to_populate[j].Item2 + ",";
                    values_passed[j - 1] = columns_to_populate[j].Item2;
                }
                datafields = datafields.TrimEnd(',');
                values = values.TrimEnd(',');

                FbCommand add_data = new FbCommand("INSERT INTO " + destination + "(" + datafields + ") VALUES (" + values + ")", new FbConnection(database_connection_string_builder.ConnectionString));

                
                add_data.Parameters.AddWithValue(values_passed[0], parent_id);

                for (int j = 1; j < data.Count(); j++)
                {
                    if (!(data.ElementAt(j).Equals(""))) add_data.Parameters.AddWithValue(values_passed[j], data.ElementAt(j));
                    else add_data.Parameters.AddWithValue(values_passed[j], null);
                }

                if (data.Count() < values_passed.Length)
                {
                    for (int j = data.Count(); j < values_passed.Length; j++) add_data.Parameters.AddWithValue(values_passed[j], null);
                }

                add_data.Connection.Open();
                add_data.ExecuteNonQuery();
                add_data.Connection.Close();
            }
        }

        // Tworzenie wirtualnych folderów
        private void database_virtual_folder_make(string name, int parent_index, bool modifiable_flag, string connection_string)
        {
            var columns_to_populate = database_columns.FindAll(x => x.Item1 == 0); // szukamy struktury tabeli virtual_folder
            string datafields = string.Empty;
            string values = string.Empty;
            string folder_creation_script = string.Empty;
            
            string[] values_passed = new string[columns_to_populate.Count - 1];

            FbConnection database_connection = new FbConnection(connection_string);
            FbCommand folder_creator = null;

            // ID nie wypełniamy z ręki (robi to automatycznie silnik bazodanowy), stąd startujemy tutaj przejście od 1.
            for (int j = 1; j < columns_to_populate.Count; j++)
            {
                datafields += columns_to_populate[j].Item2 + ",";
                values += "@" + columns_to_populate[j].Item2 + ",";
                values_passed[j - 1] = columns_to_populate[j].Item2;
            }
            datafields = datafields.TrimEnd(',');
            values = values.TrimEnd(',');

            folder_creation_script = "INSERT INTO " + database_tables.Find(x => x.Item1.Equals(0)).Item2 + "(" + datafields + ") VALUES (" + values + ")";
            folder_creator= new FbCommand(folder_creation_script, database_connection);

            folder_creator.Parameters.AddWithValue(values_passed[0], name);
            if(parent_index != -1) folder_creator.Parameters.AddWithValue(values_passed[1], parent_index); // -1 używany tylko dla roota!
            else folder_creator.Parameters.AddWithValue(values_passed[1], null);

            // Dodawanie flag widoczności dla folderu
            folder_creator.Parameters.AddWithValue(values_passed[2], modifiable_flag);
            folder_creator.Parameters.AddWithValue(@"VISIBLE_TO_OTHERS", false);
            folder_creator.Parameters.AddWithValue(@"REQUESTABLE_BY_OTHERS", false);
            folder_creator.Parameters.AddWithValue(@"COPIES_WITHOUT_CONFIRM", false);

            database_connection.Open();
            folder_creator.ExecuteNonQuery();
            database_connection.Close();
        }

        // Budowanie odpowiednich kolumn bazy danych
        private void database_build(string database_connection_string, bool external)
        {
            FbConnection database_connection = new FbConnection(database_connection_string);

            database_connection.Open();
            // Budujemy wszystkie tabele bazy:
            foreach (var script in creation_scripts)
            {
                FbCommand catalog_builder = new FbCommand(script, database_connection);
                catalog_builder.ExecuteNonQuery();
            }

            database_connection.Close();

            database_virtual_folder_make("root", -1, false, database_connection_string);

            // Jeżeli katalog ma być katalogiem obiegowym nie dodajemy do niego domyślnych folderów.
            if (external == false)
            {
                database_virtual_folder_make("Pliki tekstowe", 1, false, database_connection_string);
                database_virtual_folder_make("Dokumenty", 1, false, database_connection_string);
                database_virtual_folder_make("Pliki .htm i .html", 1, false, database_connection_string);
                database_virtual_folder_make("Obrazki i zdjęcia", 1, false, database_connection_string);
                database_virtual_folder_make("Pliki multimedialne", 1, false, database_connection_string);
            }
        }

        // Generowanie katalogu obiegowego na podstawie katalogu lokalnego
        // Alias nie może zawierać w nazwie znaku '_', oprócz tego wszystkie chwyty dozwolone.
        private bool external_catalog_build(string alias)
        {
            bool clear = false;

            if (alias.Contains('_'))
            {
                MessageBox.Show("ERROR! Nazwa aliasu nie może zawierać znaku: _");
                return false;
            }
            else
            {
                if (File.Exists(database_externals_path + alias + "_CATALOG.FDB"))
                {
                    try
                    {
                        File.Delete(database_externals_path + alias + "_CATALOG.FDB");
                        clear = true;
                    }
                    catch
                    {
                        MessageBox.Show("Generowanie katalogu obiegowego zakonczyło się niepowodzeniem!");
                        return false;
                    }
                }
                else
                {
                    clear = true;
                }

                if (clear)
                {
                    // Na samym początku pobieramy z bazy wszystkie foldery wirtualne oznaczone jako widoczne dla katalogu obiegowego
                    DataTable source_folder_structure_container = new DataTable();
                    FbDataAdapter source_folder_structure_grabber = new FbDataAdapter("SELECT * " +
                                                                            "FROM " + database_tables.Find(x => x.Item1 == 0).Item2 + " " +
                                                                            "WHERE VISIBLE_TO_OTHERS = TRUE;"
                                                                            ,
                                                                            new FbConnection(database_connection_string_builder.ConnectionString));

                    source_folder_structure_grabber.Fill(source_folder_structure_container);

                    if (source_folder_structure_container.Rows.Count == 0)
                    {
                        // Jezeli takowych nie było - zwracamy informację o próbie stworzenia pustego katalogu obiegowego i konczymy działanie procedury!
                        MessageBox.Show("Uwaga - próba wygenerowania pustego katalogu obiegowego!\nKatalog nie został wygenerowany.");
                        source_folder_structure_grabber.Dispose();
                        return false;
                    }
                    else
                    {
                        // Jeżeli takowe są to tworzymy string połączeniowy do nowej bazy danych:
                        FbConnectionStringBuilder external_catalog_connecton_string_builder = new FbConnectionStringBuilder();

                        external_catalog_connecton_string_builder = new FbConnectionStringBuilder();
                        external_catalog_connecton_string_builder.ServerType = FbServerType.Embedded;
                        external_catalog_connecton_string_builder.UserID = "SYSDBA"; // Defaultowy uzytkownik z najwyzszymi uprawnieniami do systemu bazodanowego.
                        external_catalog_connecton_string_builder.Password = ""; // Haslo nie jest sprawdzane w wersji embedded, można dać tu cokolwiek.
                        external_catalog_connecton_string_builder.Database = database_externals_path + alias + "_CATALOG.FDB";
                        external_catalog_connecton_string_builder.ClientLibrary = database_engine_path;
                        external_catalog_connecton_string_builder.Charset = "UTF8";

                        // Tworzymy nową bazę danych:
                        FbConnection.CreateDatabase(external_catalog_connecton_string_builder.ConnectionString);
                        // I budujemy ją tak samo jak katalog, z tą różnicą że mówimy funkcji że jest to katalog obiegowy:
                        database_build(external_catalog_connecton_string_builder.ConnectionString, true);

                        // Poprawne zaimplementowanie struktury folderów wirtualnych w katalogu obiegowym wymaga trzymania listy folderów wirtualnych
                        // i ich nowych identyfikatorów (nie można ufać DIR_ID z katalogu macierzystego!).
                        // Ma ona postać listy tupli złożonej z:
                        // 1. Item1 - string - nazwa folderu z katalogu macierzystego (taka sama jest w katalogu obiegowym)
                        // 2. Item2 - int - ID folderu macierzystego z katalogu macierzystego (czyt. DIR_ID przed korektą)
                        // 3. Item3 - int - ID folderu roboczego z katalogu macierzystego (czyt. ID przed korektą)
                        // 4. Item4 - int - ID folderu macierzystego w katalogu obiegowym (czyt. DIR_ID już po korekcie)
                        // 5. Item5 - int - ID folderu roboczego w katalogu obiegowym (czyt. ID po korekcie)
                        List<Tuple<string, int, int, int, int>> id_tracker_list = new List<Tuple<string, int, int, int, int>>();

                        for (int i = 0; i < source_folder_structure_container.Rows.Count; i++)
                        {
                            // Jako że tworzymy katalog obiegowy tylko z folderem root możemy bez problemu kopiować wszystkie znalezione foldery.
                            // Tworzymy im nowy odpowiednik, pobieramy jego ID i ID jego rodzica, dodajemy te informacje do listy.
                            // Na koniec chcemy mieć uzupełnioną listę folderów w katalogu obiegowym, z DIR_ID i ID folderu zarówno z katalogu macierzystego, jak i przydzielone w katalogu obiegowym.

                            Tuple<string, int, int, int, int> initial_data = new Tuple<string, int, int, int, int>("", 0, 0, 0, 0);
                            int parent_id = 0;
                            try
                            {
                                parent_id = (int)source_folder_structure_container.Rows[i].ItemArray[2];

                                initial_data = new Tuple<string, int, int, int, int>(
                                                                             (string)source_folder_structure_container.Rows[i].ItemArray[1],
                                                                             parent_id,
                                                                             (int)source_folder_structure_container.Rows[i].ItemArray[0],
                                                                             0,
                                                                             0);
                            }
                            catch
                            {
                                parent_id = -1;
                            }

                            if (parent_id != -1)
                            {
                                // Tworzymy nowy folder w odpowienim katalogu
                                Tuple<string, int, int, int, int> redirect_parent = id_tracker_list.Find(x => x.Item3 == parent_id);
                                if (redirect_parent == null)
                                {
                                    database_virtual_folder_make(initial_data.Item1, initial_data.Item2, false, external_catalog_connecton_string_builder.ConnectionString);
                                }
                                else
                                {
                                    database_virtual_folder_make(initial_data.Item1, redirect_parent.Item5, false, external_catalog_connecton_string_builder.ConnectionString);
                                }


                                // I tworzymy zapytanie o jego nowy ID i DIR_ID
                                DataTable new_folder_container = new DataTable();
                                FbDataAdapter new_folder_grabber = new FbDataAdapter("SELECT ID,DIR_ID " +
                                                                                     "FROM " + database_tables.Find(x => x.Item1 == 0).Item2 + " " +
                                                                                     "WHERE NAME = @Name AND DIR_ID = @Parent_directory_ID"
                                                                                     ,
                                                                                     new FbConnection(external_catalog_connecton_string_builder.ConnectionString));
                                if (redirect_parent == null)
                                {
                                    new_folder_grabber.SelectCommand.Parameters.AddWithValue("@Parent_directory_ID", initial_data.Item2);
                                }
                                else
                                {
                                    new_folder_grabber.SelectCommand.Parameters.AddWithValue("@Parent_directory_ID", redirect_parent.Item5);
                                }
                                new_folder_grabber.SelectCommand.Parameters.AddWithValue("@Name", initial_data.Item1);

                                new_folder_grabber.Fill(new_folder_container);

                                if (new_folder_container.Rows.Count > 0)
                                {
                                    // Kompletujemy dane w tupli
                                    Tuple<string, int, int, int, int> new_parent = id_tracker_list.Find(x => x.Item3 == (int)new_folder_container.Rows[0].ItemArray[1]);

                                    if (new_parent != null)
                                    {
                                        initial_data = new Tuple<string, int, int, int, int>(initial_data.Item1,
                                                                                     initial_data.Item2,
                                                                                     initial_data.Item3,
                                                                                     new_parent.Item5,
                                                                                     (int)new_folder_container.Rows[0].ItemArray[0]
                                                                                    );
                                    }
                                    else
                                    {
                                        initial_data = new Tuple<string, int, int, int, int>(initial_data.Item1,
                                                                                         initial_data.Item2,
                                                                                         initial_data.Item3,
                                                                                         (int)new_folder_container.Rows[0].ItemArray[1],
                                                                                         (int)new_folder_container.Rows[0].ItemArray[0]
                                                                                        );
                                    }

                                    // I dodajemy ją do listy
                                    id_tracker_list.Add(initial_data);
                                }
                                new_folder_grabber.Dispose();
                            }
                        }

                        for (int i = 0; i < id_tracker_list.Count; i++)
                        {
                            // Z folderów w liście kopiujemy oznaczoną zawartość.
                            for (int j = 1; j < database_tables.Count; j++)
                            {
                                Tuple<int, string> table = database_tables[j];

                                // Po stworzeniu bazy danych kopiujemy do niej wszystko z bazy macierzystej:
                                string datafields = String.Empty;
                                string values = String.Empty;
                                string[] values_passed = new string[0];

                                // Pobieranie informacji o strukturze tabeli, którą chcemy wypełnić:
                                int datatable_index = table.Item1;

                                var columns_to_populate = database_columns.FindAll(x => x.Item1 == datatable_index);
                                values_passed = new string[columns_to_populate.Count - 1];

                                // ID wypełnia silnik bazodanowy - stąd startujemy od 1!
                                for (int k = 1; k < columns_to_populate.Count; k++)
                                {
                                    datafields += columns_to_populate[k].Item2 + ",";
                                    values += "@" + columns_to_populate[k].Item2 + ",";
                                    values_passed[k - 1] = columns_to_populate[k].Item2;
                                }

                                datafields = datafields.TrimEnd(',');
                                values = values.TrimEnd(',');

                                DataTable source_content_container = new DataTable();
                                FbDataAdapter source_content_grabber = new FbDataAdapter("SELECT * " +
                                                                                        "FROM " + table.Item2 + " " +
                                                                                        "WHERE DIR_ID = @Parent_directory_id " +
                                                                                        "AND VISIBLE_TO_OTHERS = TRUE;"
                                                                                        ,
                                                                                        new FbConnection(database_connection_string_builder.ConnectionString));

                                source_content_grabber.SelectCommand.Parameters.AddWithValue("@Parent_directory_id", id_tracker_list[i].Item3);

                                source_content_grabber.Fill(source_content_container);

                                if (source_content_container.Rows.Count > 0)
                                {
                                    for (int k = 0; k < source_content_container.Rows.Count; k++)
                                    {
                                        FbCommand add_data = new FbCommand("INSERT INTO " + table.Item2 + "(" + datafields + ") VALUES (" + values + ")", new FbConnection(external_catalog_connecton_string_builder.ConnectionString));

                                        for (int l = 0; l < values_passed.Length; l++)
                                        {
                                            switch (values_passed[l])
                                            {
                                                case "DIR_ID":
                                                    {
                                                        add_data.Parameters.AddWithValue("@" + values_passed[l], id_tracker_list[i].Item5.ToString());
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        add_data.Parameters.AddWithValue("@" + values_passed[l], source_content_container.Rows[k].ItemArray[l + 1]);
                                                        break;
                                                    }
                                            }
                                        }
                                        add_data.Connection.Open();
                                        add_data.ExecuteNonQuery();
                                        add_data.Connection.Close();

                                        add_data.Dispose();
                                    }
                                }
                            }
                        }
                        source_folder_structure_grabber.Dispose();
                        MessageBox.Show("Generowanie katalogu obiegowego przebiegło pomyślnie!");
                        return true;
                    }
                }
            }
            return true;
        }

        // Pobranie indeksu, folderu macierzystego, nazwy, typu (z której tabeli pochodzi) i rozszerzenia każdego pliku w zadanym folderze
        private List<Tuple<int, int, string, string, string>> database_virtual_folder_get_all_files (int id)
        {
            List<Tuple<int, int, string, string, string>> results = new List<Tuple<int, int, string, string, string>>();

            DataTable subfolders_container = new DataTable();
            FbDataAdapter subfolders_grabber = new FbDataAdapter("SELECT ID " +
                                                                 "FROM " + database_tables[0].Item2 + " " +
                                                                 "WHERE DIR_ID = @Target_directory_id;"
                                                                 ,
                                                                 new FbConnection(database_connection_string_builder.ConnectionString));

            subfolders_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", id);

            subfolders_grabber.Fill(subfolders_container);

            // Uwaga - wywołanie rekurencyjne dla każdego znalezionego podfolderu!
            if(subfolders_container.Rows.Count >= 1)
            {
                for(int i = 0; i < subfolders_container.Rows.Count; i++)
                {
                    results.AddRange(database_virtual_folder_get_all_files((int)subfolders_container.Rows[i].ItemArray[0]));
                }
            }

            // Teraz pobieramy wszystkie pliki obecne w folderze
            for(int i = 1; i < database_tables.Count; i++)
            {
                DataTable files_container = new DataTable();
                FbDataAdapter files_grabber = new FbDataAdapter("SELECT ID,DIR_ID,NAME,EXTENSION " +
                                                                "FROM " + database_tables[i].Item2 + " " +
                                                                "WHERE DIR_ID = @Target_directory_id;"
                                                                ,
                                                                new FbConnection(database_connection_string_builder.ConnectionString));

                files_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", id);

                files_container.Clear();
                files_grabber.Fill(files_container);

                if(files_container.Rows.Count >= 1)
                {
                    for(int j = 0; j < files_container.Rows.Count; j++)
                    {
                        results.Add(new Tuple<int, int, string, string, string>((int)files_container.Rows[j].ItemArray[0],
                                                                (int)files_container.Rows[j].ItemArray[1],
                                                                (string)files_container.Rows[j].ItemArray[2],
                                                                database_tables[i].Item2,
                                                                (string)files_container.Rows[j].ItemArray[3]
                                                                ));
                    }
                }
            }
            
            return results;
        }

        // Pobieranie ścieżki rzeczywistej pliku z bazy danych.
        private string database_virtual_filepath_get(int id, string target_table)
        {
            string result = string.Empty;

            DataTable file_location_container = new DataTable();
            FbDataAdapter file_location_grabber = new FbDataAdapter("SELECT ID,PATH " +
                                                                    "FROM " + target_table + " " +
                                                                    "WHERE ID = @Id;"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            file_location_grabber.SelectCommand.Parameters.AddWithValue("@Id", id);

            file_location_grabber.Fill(file_location_container);

            if (file_location_container.Rows.Count == 1)
            {
                result = (string)file_location_container.Rows[0].ItemArray[1];
            }
            else
            {
                // Błąd - nie znaleziono w podanej tabeli pliku o takim ID.
                MessageBox.Show("Błąd! Nie znaleziono pliku w tabeli.");
            }

            return result;
        }

        // Sprawdzenie czy zadany folder jest modyfikowalny (de facto pobranie flagi MODIFIABLE z kolumny virtual_folder i zwrócenie jej do programu)
        private bool database_virtual_folder_modifiable_check(int id_to_check)
        {
            bool is_modifiable = true;
            DataTable database_folder_modifiable_verifier_container = new DataTable();
            FbDataAdapter database_folder_modifiable_verifier = new FbDataAdapter("SELECT ID,NAME,MODIFIABLE " +
                                                                                  "FROM " + database_tables[0].Item2 + " " +
                                                                                  "WHERE ID = @Id;"
                                                                                  ,
                                                                                  new FbConnection(database_connection_string_builder.ConnectionString));
            
            database_folder_modifiable_verifier.SelectCommand.Parameters.AddWithValue("@Id", id_to_check);

            database_folder_modifiable_verifier.Fill(database_folder_modifiable_verifier_container);

            if (database_folder_modifiable_verifier_container.Rows.Count == 1)
            {
                // Wszystko dobrze, znalazł wartość
                is_modifiable = bool.Parse(database_folder_modifiable_verifier_container.Rows[0].ItemArray[2].ToString());
                if (is_modifiable == false)
                {
                    MessageBox.Show(database_folder_modifiable_verifier_container.Rows[0].ItemArray[1].ToString() + " jest folderem domyślnym, nie może on zostać zmodyfikowany!");
                }
            }
            else
            {
                MessageBox.Show("Bład podczas pobierania flagi MODIFIABLE folderu!");
            }
            return is_modifiable;
        }

        // Procedura wywołująca kopiowanie pliku wirtualnego do folderu, patrzymy czy nie występuje kolizja i zmieniamy nazwę jeżeli zaszła.
        private void database_virtual_file_copy(int source_folder_id, int destination_folder_id, string type, string name, string extension)
        {
            List<string> passed_data = new List<string>();
            string name_new = name;
            // Sprawdzamy czy folder do którego kopiujemy nie ma już pliku o tej samej nazwie
            DataTable file_exists_container = new DataTable();
            FbDataAdapter file_exists_checker = new FbDataAdapter("SELECT NAME " +
                                                                    "FROM " + type + " " +
                                                                    "WHERE NAME = @Name " +
                                                                    "AND DIR_ID = @Target_directory_id " +
                                                                    "AND EXTENSION = @Extension;"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            file_exists_checker.SelectCommand.Parameters.AddWithValue("@Name", name_new);
            file_exists_checker.SelectCommand.Parameters.AddWithValue("@Target_directory_id", destination_folder_id);
            file_exists_checker.SelectCommand.Parameters.AddWithValue("@Extension", extension);

            file_exists_checker.Fill(file_exists_container);

            if (file_exists_container.Rows.Count > 0)
            {
                // Kolizja - mamy już plik o takiej nazwie w folderze do którego kopiujemy!
                // Doczepiamy do nazwy folderu cyfrę z ilością kopii i jest ok dopóki nie zrobimy sizeof(int) takich samych plików w jednym folderze.
                name_new = name_new + " " + file_exists_container.Rows.Count;
            }

            DataTable file_content_container = new DataTable();
            FbDataAdapter file_content_grabber = new FbDataAdapter("SELECT * " +
                                                                    "FROM " + type + " " +
                                                                    "WHERE NAME = @Name " +
                                                                    "AND DIR_ID = @Target_directory_id " +
                                                                    "AND EXTENSION = @Extension;"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            file_content_grabber.SelectCommand.Parameters.AddWithValue("@Name", name_new);
            file_content_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", source_folder_id);
            file_content_grabber.SelectCommand.Parameters.AddWithValue("@Extension", extension);

            file_content_grabber.Fill(file_content_container);

            if (file_content_container.Rows.Count == 1)
            {
                for (int j = 1; j < file_content_container.Rows[0].ItemArray.Count(); j++)
                {
                    passed_data.Add("" + file_content_container.Rows[0].ItemArray[j]);
                }
            }

            database_virtual_item_make(destination_folder_id, type, passed_data);
        }

        // Procedura wywołująca kopiowanie folderu wraz z jego zawartością do innego katalogu. Też sprawdza czy nie zaszła kolizja nazw.
        private void database_virtual_folder_copy(int source_folder_id, int destination_folder_id, string name)
        {
            string name_new = name;
            int id_new = 0;
            string error_message = string.Empty;
            bool error = false;
            bool[] flags = new bool[3];

            // 1. Tworzymy folder, który będzie naszą kopią

            // Sprawdzamy czy folder do którego kopiujemy nie ma już folderu o tej samej nazwie
            DataTable folder_exists_container = new DataTable();
            FbDataAdapter folder_exists_checker = new FbDataAdapter("SELECT ID,DIR_ID " +
                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                    "WHERE NAME = @Name " +
                                                                    "AND DIR_ID = @Target_directory_id;"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            folder_exists_checker.SelectCommand.Parameters.AddWithValue("@Name", name_new);
            folder_exists_checker.SelectCommand.Parameters.AddWithValue("@Target_directory_id", destination_folder_id);

            folder_exists_checker.Fill(folder_exists_container);
            if (folder_exists_container.Rows.Count >= 1)
            {
                // Kolizja - mamy już folder o takiej nazwie w folderze do którego kopiujemy!
                // Doczepiamy do nazwy folderu cyfrę z ilością kopii i jest ok dopóki nie zrobimy sizeof(int) folderów w jednym folderze.
                name_new = name_new + " " + folder_exists_container.Rows.Count;
            }

            database_virtual_folder_make(name_new, destination_folder_id, true, database_connection_string_builder.ConnectionString);

            // 2. Szukamy go w katalogu - chcemy jego ID żeby wrzucić do niego wszystkie rzeczy które były w folderze źródłowym

            DataTable new_folder_id_container = new DataTable();
            FbDataAdapter new_folder_id_grabber = new FbDataAdapter("SELECT ID,DIR_ID,NAME" + " " +
                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                    "WHERE NAME = @Name " +
                                                                    "AND DIR_ID = @Target_directory_id;"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            new_folder_id_grabber.SelectCommand.Parameters.AddWithValue("@Name", name_new);
            new_folder_id_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", destination_folder_id);

            new_folder_id_grabber.Fill(new_folder_id_container);

            if (new_folder_id_container.Rows.Count == 1)
            {
                // Wszystko ok - pobieram ID nadane nowemu folderowi!
                id_new = (int)new_folder_id_container.Rows[0].ItemArray[0];
            }
            else
            {
                // Błąd! Nie znalazł nowo-stworzonego folderu, albo znalazł ich za dużo!
                error = true;
                error_message += "Nie mogłem znaleść ID nowo-utworzonego folderu, albo znalazłem ich więcej niż jeden.\n";
            }

            // Także modyfikujemy jego flagi tak, aby pokrywały się z flagami folderu źródłowego


            // Pobieramy flagi folderu macierzystego
            DataTable parent_folder_flags_container = new DataTable();
            FbDataAdapter parent_folder_flags_grabber = new FbDataAdapter("SELECT VISIBLE_TO_OTHERS,REQUESTABLE_BY_OTHERS,COPIES_WITHOUT_CONFIRM " +
                                                                          "FROM " + database_tables[0].Item2 + " " +
                                                                          "WHERE ID = @Id "
                                                                          ,
                                                                          new FbConnection(database_connection_string_builder.ConnectionString));

            parent_folder_flags_grabber.SelectCommand.Parameters.AddWithValue("@Id", source_folder_id);

            parent_folder_flags_grabber.Fill(parent_folder_flags_container);

            if(parent_folder_flags_container.Rows.Count == 0)
            {
                // Error - nie znalazł folderu macierzystego!
                MessageBox.Show("ERROR - nie znaleziono flag folderu macierzystego!");
                error = true;
            }
            else
            {
                flags = new bool[3] { (bool)parent_folder_flags_container.Rows[0].ItemArray[0],
                                      (bool)parent_folder_flags_container.Rows[0].ItemArray[1],
                                      (bool)parent_folder_flags_container.Rows[0].ItemArray[2] };
            }

            // 3. Gdy mamy już id kopiujemy całą zawartość folderu źródłowego do naszego nowoutworzonego folderu

            if (error == false)
            {
                // Przeszukujemy zawartość folderu źródłowego i odtwarzamy obiekty z niego w naszym nowym folderze:
                // Najpierw tworzymy jego podfoldery. Uwaga bo funkcja wywołuje tutaj samą siebie rekurencyjnie.
                DataTable new_folder_subfolders_to_create_container = new DataTable();
                FbDataAdapter new_folder_subfolders_to_create_grabber = new FbDataAdapter("SELECT ID,NAME,DIR_ID " +
                                                                        "FROM " + database_tables[0].Item2 + " " +
                                                                        "WHERE DIR_ID = @Target_directory_id;"
                                                                        ,
                                                                        new FbConnection(database_connection_string_builder.ConnectionString));

                new_folder_subfolders_to_create_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", source_folder_id);

                new_folder_subfolders_to_create_grabber.Fill(new_folder_subfolders_to_create_container);

                for (int i = 0; i < new_folder_subfolders_to_create_container.Rows.Count; i++)
                {
                    if (!new_folder_subfolders_to_create_container.Rows[i].ItemArray[1].Equals(name_new))
                        database_virtual_folder_copy((int)new_folder_subfolders_to_create_container.Rows[i].ItemArray[0],
                                                     id_new,
                                                     (string)new_folder_subfolders_to_create_container.Rows[i].ItemArray[1]);
                }

                // Przekopiowaliśmy podfoldery, teraz trzeba wziąść całą pozostałą zawartośc folderu źródłowego
                for (int i = 1; i < database_tables.Count; i++)
                {
                    DataTable new_folder_content_container = new DataTable();
                    FbDataAdapter new_folder_content_grabber = new FbDataAdapter("SELECT *" +
                                                                            "FROM " + database_tables[i].Item2 + " " +
                                                                            "WHERE DIR_ID = @Target_directory_id;"
                                                                            ,
                                                                            new FbConnection(database_connection_string_builder.ConnectionString));

                    new_folder_content_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", source_folder_id);

                    new_folder_content_grabber.Fill(new_folder_content_container);
                    for (int j = 0; j < new_folder_content_container.Rows.Count; j++)
                    {
                        List<string> data_passed = new List<string>();
                        for (int k = 1; k < new_folder_content_container.Rows[j].ItemArray.Count(); k++)
                        {
                            string value = new_folder_content_container.Rows[j].ItemArray[k].ToString();
                            data_passed.Add(value);
                        }
                        database_virtual_item_make(id_new, database_tables[i].Item2, data_passed);
                    }
                }

                database_virtual_folder_modify_rights(id_new, name_new, "VISIBLE_TO_OTHERS", flags[0]);
                database_virtual_folder_modify_rights(id_new, name_new, "REQUESTABLE_BY_OTHERS", flags[1]);
                database_virtual_folder_modify_rights(id_new, name_new, "COPIES_WITHOUT_CONFIRM", flags[2]);
            }
            else
            {
                //Error handling - tutaj zwracamy co poszło nie tak.
                MessageBox.Show(error_message);
            }
        }

        // Procedura zmieniająca uprawnienia pliku w katalogu wirtualnym
        private void database_virtual_file_modify_rights(int source_folder_id, string type, string name, string extension, string right_to_modify, bool new_value)
        {
            // Zmieniamy uprawnienia pliku:
            FbCommand file_rights_modifier = new FbCommand("UPDATE " + type + " " +
                                                           "SET " + right_to_modify + "=@" + right_to_modify + "_VALUE " +
                                                           "WHERE NAME = @Name " +
                                                           "AND DIR_ID = @Target_directory_id " +
                                                           "AND EXTENSION = @Extension;"
                                                           ,
                                                           new FbConnection(database_connection_string_builder.ConnectionString));

            file_rights_modifier.Parameters.AddWithValue("@Name", name);
            file_rights_modifier.Parameters.AddWithValue("@Target_directory_id", source_folder_id);
            file_rights_modifier.Parameters.AddWithValue("@Extension", extension);
            file_rights_modifier.Parameters.AddWithValue("@"+ right_to_modify +"_VALUE", new_value);

            file_rights_modifier.Connection.Open();
            file_rights_modifier.ExecuteNonQuery();
            file_rights_modifier.Connection.Close();
        }

        // Procedura zmieniająca uprawnienia folderu i całej jego zawartości w katalogu wirtualnym
        // Zwraca liczbę elementów dla których zmiana przeszła pomyślnie w Item1 i tych, dla których zawiodła w Item2
        private void database_virtual_folder_modify_rights(int source_folder_id, string name, string right_to_modify, bool new_value)
        {
            DataTable subfolders_to_modify = new DataTable();
            FbDataAdapter subfolders_to_modify_grabber = new FbDataAdapter("SELECT ID,NAME,DIR_ID " +
                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                    "WHERE DIR_ID = @Target_directory_id;"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            subfolders_to_modify_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", source_folder_id);

            subfolders_to_modify_grabber.Fill(subfolders_to_modify);

            // Pobieramy tutaj wszystkie podfoldery naszego wskazanego folderu
            for (int i = 0; i < subfolders_to_modify.Rows.Count; i++)
            {
                if (!subfolders_to_modify.Rows[i].ItemArray[0].Equals(source_folder_id))
                {
                    database_virtual_folder_modify_rights((int)subfolders_to_modify.Rows[i].ItemArray[0],
                                                          (string)subfolders_to_modify.Rows[i].ItemArray[1],
                                                          right_to_modify,
                                                          new_value);
                }

            }

            // Wyłuskujemy całą możliwą zawartość z podfolderów i zmieniamy jej prawa:
            for (int i = 1; i < database_tables.Count; i++)
            {
                DataTable folder_content_to_modify_container = new DataTable();
                FbDataAdapter folder_content_to_modify_grabber = new FbDataAdapter("SELECT NAME,EXTENSION,VISIBLE_TO_OTHERS,REQUESTABLE_BY_OTHERS,COPIES_WITHOUT_CONFIRM " +
                                                                        "FROM " + database_tables[i].Item2 + " " +
                                                                        "WHERE DIR_ID = @Target_directory_id;"
                                                                        ,
                                                                        new FbConnection(database_connection_string_builder.ConnectionString));

                folder_content_to_modify_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", source_folder_id);

                folder_content_to_modify_grabber.Fill(folder_content_to_modify_container);
                for (int j = 0; j < folder_content_to_modify_container.Rows.Count; j++)
                {
                    // Czyszczenie wartości dla flag podrzędnych w przypadku wyłączenia flagi nadrzędnej:
                    if (right_to_modify.Equals("VISIBLE_TO_OTHERS") && new_value == false)
                    {
                        if ((bool)(folder_content_to_modify_container.Rows[j].ItemArray[3]) == true)
                            database_virtual_file_modify_rights(source_folder_id,
                                                                database_tables[i].Item2,
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[0],
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[1],
                                                                "REQUESTABLE_BY_OTHERS",
                                                                false);
                        if ((bool)(folder_content_to_modify_container.Rows[j].ItemArray[4]) == true)
                            database_virtual_file_modify_rights(source_folder_id,
                                                                database_tables[i].Item2,
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[0],
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[1],
                                                                "COPIES_WITHOUT_CONFIRM",
                                                                false);
                    }

                    if (right_to_modify.Equals("REQUESTABLE_BY_OTHERS") && new_value == false)
                    {
                        if ((bool)(folder_content_to_modify_container.Rows[j].ItemArray[4]) == true)
                            database_virtual_file_modify_rights(source_folder_id,
                                                                database_tables[i].Item2,
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[0],
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[1],
                                                                "COPIES_WITHOUT_CONFIRM",
                                                                false);
                    }

                    // Sprawdzanie, czy ustawiamy flagi nadrzędne i ustawienie podrzędnych na prawidłowe wartości jeżeli tak:
                    if (right_to_modify.Equals("COPIES_WITHOUT_CONFIRM") && new_value == true)
                    {
                        if ((bool)(folder_content_to_modify_container.Rows[j].ItemArray[3]) == false)
                            database_virtual_file_modify_rights(source_folder_id,
                                                                database_tables[i].Item2,
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[0],
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[1],
                                                                "REQUESTABLE_BY_OTHERS",
                                                                true);
                        if ((bool)(folder_content_to_modify_container.Rows[j].ItemArray[2]) == false)
                            database_virtual_file_modify_rights(source_folder_id,
                                                                database_tables[i].Item2,
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[0],
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[1],
                                                                "VISIBLE_TO_OTHERS",
                                                                true);
                    }

                    if (right_to_modify.Equals("REQUESTABLE_BY_OTHERS") && new_value == true)
                    {
                        if ((bool)(folder_content_to_modify_container.Rows[j].ItemArray[2]) == false)
                            database_virtual_file_modify_rights(source_folder_id,
                                                                database_tables[i].Item2,
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[0],
                                                                (string)folder_content_to_modify_container.Rows[j].ItemArray[1],
                                                                "VISIBLE_TO_OTHERS",
                                                                true);
                    }

                    // W pozostałych przypadkach postępujemy standardowo:
                    database_virtual_file_modify_rights(source_folder_id,
                                                        database_tables[i].Item2,
                                                        (string)folder_content_to_modify_container.Rows[j].ItemArray[0],
                                                        (string)folder_content_to_modify_container.Rows[j].ItemArray[1],
                                                        right_to_modify,
                                                        new_value);
                }
            }

            // Ostatecznie zmieniamy prawa dla folderu na którym pracowaliśmy:

            string modifications = "";

            if (new_value == true) modifications = "SET " + right_to_modify + "=@" + right_to_modify + "_VALUE ";
            else
            {
                if (right_to_modify.Equals("VISIBLE_TO_OTHERS"))
                {
                    modifications = "SET VISIBLE_TO_OTHERS = FALSE, REQUESTABLE_BY_OTHERS = FALSE, COPIES_WITHOUT_CONFIRM = FALSE ";
                }
                if (right_to_modify.Equals("REQUESTABLE_BY_OTHERS"))
                {
                    modifications = "SET REQUESTABLE_BY_OTHERS = FALSE, COPIES_WITHOUT_CONFIRM = FALSE ";
                }
                if (right_to_modify.Equals("COPIES_WITHOUT_CONFIRM"))
                {
                    modifications = "SET COPIES_WITHOUT_CONFIRM = FALSE ";
                }
            }

            FbCommand folder_rights_modifier = new FbCommand("UPDATE " + database_tables[0].Item2 + " " +
                                                       modifications +
                                                       "WHERE NAME = @Name " +
                                                       "AND ID = @Id "
                                                       ,
                                                       new FbConnection(database_connection_string_builder.ConnectionString));

            folder_rights_modifier.Parameters.AddWithValue("@Name", name);
            folder_rights_modifier.Parameters.AddWithValue("@Id", source_folder_id);
            if (new_value == true) folder_rights_modifier.Parameters.AddWithValue("@" + right_to_modify + "_VALUE", new_value);

            folder_rights_modifier.Connection.Open();
            folder_rights_modifier.ExecuteNonQuery();
            folder_rights_modifier.Connection.Close();
        }

        // Procedura zmieniająca TYLKO uprawnienia folderu.
        private void database_virtual_folder_simple_modify_rights(int source_folder_id, string name, string right_to_modify, bool new_value)
        {
            string modifications = "";

            if (new_value == true) modifications = "SET " + right_to_modify + "=@" + right_to_modify + "_VALUE ";
            else
            {
                if (right_to_modify.Equals("VISIBLE_TO_OTHERS"))
                {
                    modifications = "SET VISIBLE_TO_OTHERS = FALSE, REQUESTABLE_BY_OTHERS = FALSE, COPIES_WITHOUT_CONFIRM = FALSE ";
                }
                if (right_to_modify.Equals("REQUESTABLE_BY_OTHERS"))
                {
                    modifications = "SET REQUESTABLE_BY_OTHERS = FALSE, COPIES_WITHOUT_CONFIRM = FALSE ";
                }
                if (right_to_modify.Equals("COPIES_WITHOUT_CONFIRM"))
                {
                    modifications = "SET COPIES_WITHOUT_CONFIRM = FALSE ";
                }
            }

            FbCommand folder_rights_modifier = new FbCommand("UPDATE " + database_tables[0].Item2 + " " +
                                                       modifications +
                                                       "WHERE NAME = @Name " +
                                                       "AND ID = @Id "
                                                       ,
                                                       new FbConnection(database_connection_string_builder.ConnectionString));

            folder_rights_modifier.Parameters.AddWithValue("@Name", name);
            folder_rights_modifier.Parameters.AddWithValue("@Id", source_folder_id);
            if (new_value == true) folder_rights_modifier.Parameters.AddWithValue("@" + right_to_modify + "_VALUE", new_value);

            folder_rights_modifier.Connection.Open();
            folder_rights_modifier.ExecuteNonQuery();
            folder_rights_modifier.Connection.Close();
        }

        // Procedura usuwająca zadany folder wirtualny, w zależności od użytkownika usuwa foldery z zawartością.
        private void database_virtual_folder_delete(int id_to_delete, string target_table, bool is_file, bool surpress_pop_ups)
        {
            // Sprawdzamy czy to co usuwamy jest plikiem - jeżeli nie jest to mamy kilka rzeczy do sprawdzenia...
            if (is_file == false)
            {
                // Tutaj mamy folder.

                // Sprawdzamy najpierw czy folder jest usuwalny, potem czy nie ma nic w folderze który możemy usunąć
                // Obsługa sytuacji nieedytowalnego folderu jest już w funkcji database_virtual_folder_modifiable_check
                if (database_virtual_folder_modifiable_check(id_to_delete) == true)
                {
                    bool has_something_inside = false, found_file = false;
                    // W tej liście przechowujemy wszystkie informacje niezbędne do rekurencyjnego odpalenia instrukcji usuwania.
                    List<Tuple<int, string, string, bool>> detected_files = new List<Tuple<int, string, string, bool>>();

                    for (int i = 0; i < database_tables.Count; i++)
                    {
                        DataTable folder_content_checker_container = new DataTable();
                        FbDataAdapter folder_content_checker = new FbDataAdapter("SELECT ID,NAME,DIR_ID " +
                                                                                 "FROM " + database_tables[i].Item2 + " " +
                                                                                 "WHERE DIR_ID = @Target_directory_id;"
                                                                                 ,
                                                                                 new FbConnection(database_connection_string_builder.ConnectionString));

                        folder_content_checker.SelectCommand.Parameters.AddWithValue("@Target_directory_id", id_to_delete);

                        folder_content_checker.Fill(folder_content_checker_container);
                        if (folder_content_checker_container.Rows.Count != 0)
                        {
                            for (int j = 0; j < folder_content_checker_container.Rows.Count; j++)
                            {
                                if (i == 0) found_file = true;
                                else found_file = false;
                                detected_files.Add(new Tuple<int, string, string, bool>
                                                  (int.Parse(folder_content_checker_container.Rows[j].ItemArray[0].ToString()),
                                                  (string)folder_content_checker_container.Rows[j].ItemArray[1],
                                                  database_tables[i].Item2,
                                                  !found_file));
                            }
                            has_something_inside = true;
                        }
                    }
                    if (has_something_inside == true)
                    {
                        DialogResult subfolder_found = DialogResult.Yes;
                        if (surpress_pop_ups == false) subfolder_found = MessageBox.Show("Folder nie jest pusty!\n" + "Czy mam go usunąć mimo to?",
                                                                                         "Usuń folder z zawartością",
                                                                                         MessageBoxButtons.YesNo);
                        if (subfolder_found == DialogResult.Yes)
                        {
                            foreach (var thing_inside in detected_files)
                            {
                                database_virtual_folder_delete(thing_inside.Item1, thing_inside.Item3, thing_inside.Item4, true);
                            }
                            // Po usunięciu zawartości z potomków przyszedł czas na sam folder - wiemy że teraz musi być już pusty.
                            FbCommand database_directory_remover = new FbCommand("DELETE FROM " + target_table + " " +
                                                                                 "WHERE ID = @Id;"
                                                                                 ,
                                                                                 new FbConnection(database_connection_string_builder.ConnectionString));

                            database_directory_remover.Parameters.AddWithValue("@Id", id_to_delete);

                            database_directory_remover.Connection.Open();
                            database_directory_remover.ExecuteNonQuery();
                            database_directory_remover.Connection.Close();
                        }
                        if (subfolder_found == DialogResult.No)
                        {
                            //Nie robimy nic - folder nie zostanie usunięty.
                        }
                    }
                    else
                    {
                        //Wszystko ok, nie ma nic w srodku, możemy usuwać.
                        FbCommand database_directory_remover = new FbCommand("DELETE FROM " + target_table + " " +
                                                                             "WHERE ID = @Id;"
                                                                             ,
                                                                             new FbConnection(database_connection_string_builder.ConnectionString));

                        database_directory_remover.Parameters.AddWithValue("@Id", id_to_delete);

                        database_directory_remover.Connection.Open();
                        database_directory_remover.ExecuteNonQuery();
                        database_directory_remover.Connection.Close();
                    }
                }
            }
            else
            // Tutaj mamy plik
            {
                //Logika usuwania pliku - po prostu usuń z odpowiedniej tabeli.
                FbCommand database_file_remover = new FbCommand("DELETE FROM " + target_table + " " +
                                                                "WHERE ID = @Id;"
                                                                ,
                                                                new FbConnection(database_connection_string_builder.ConnectionString));

                database_file_remover.Parameters.AddWithValue("@Id", id_to_delete);

                database_file_remover.Connection.Open();
                database_file_remover.ExecuteNonQuery();
                database_file_remover.Connection.Close();
            }
        }

        /* Walidacja struktury bazy
         * 
         * Tutaj trzeba należy sprawdzić istniejący plik bazy danych pod kątem zawartości poprawnych tabel, same tabele sprawdzić pod kątem posiadania
         * prawidłowych kolumn (do samej zawartości nie schodzimy, tutaj silnik bazodanowy nie da nam wpisać niepoprawnych danych).
         *  
         * W zasadzie to: 
         * Najpierw łączymy się do bazy,
         * potem ściągamy wszystkie zdefiniowane przez użytkownika tabele. 
         * Następnie, mając informację jaka to tabela ściągamy jej kolumny.
         * I sprawdzamy czy ma wszystkie kolumny które powinna mieć
         * 
         * Funkcja ta zwraca tabele intów rozmiaru ilości tabel w bazie metadanych, oznaczają one stwierdzony "stan" każdej tabeli w załadowanej bazie.
         * Indeksy tablicy zwracanej tutaj odpowiadają indeksom nadanym w metadata_tables.
         * Możliwe stany to:
         * 0 - tabela nie istnieje,
         * 1 - tabela istnieje, ale brak jej którejś kolumny,
         * 2 - tabela istnieje i wszystkie kolumny są obecne.
         * 
         * Informacja o wyniku walidacji przechowywana jest w bool'u database_validated_successfully.
         * 
         */
        private int[] database_validate(string database_connection_string)
        {
            // Tworzymy tablicę 6-elementową przechowującą wynik walidacji dla każdej tabeli bazy programu. Zawiera true gdy są poprawne, false gdy są błędne.
            
            int[] result = new int[database_tables.Count];
            for (int i = 0; i < result.Length; i++) result[i] = 0;

            // Tworzymy database_tables, przechowa on wynik zapytania wyłapującego wszystkie tabele w bazie (oprócz domyślnych tabel systemu bazodanowego)
            DataTable database_tables_grabbed = new DataTable();
            DataTable database_columns_grabbed = new DataTable();
            // Tworzymy adapter i podpinamy do niego zapytanie SQL wyłuskujące nam wszystkie tabele w bazie (oprócz domyślnych tabel systemu bazodanowego)
            FbDataAdapter database_grab_tables = new FbDataAdapter("SELECT rdb$relation_name " +
                                                                   "FROM rdb$relations " +
                                                                   "WHERE rdb$view_blr IS NULL AND (rdb$system_flag is NULL OR rdb$system_flag = 0);"
                                                                   ,
                                                                   new FbConnection(database_connection_string));

            // Kopiujemy wyniki zapytania do naszej database_tables żeby móc z nich korzystać wewnątrz programu
            database_grab_tables.Fill(database_tables_grabbed);

            //Przechodzimy do samego działania programu, jeżeli wyłuskaliśmy metadane.
            if (database_tables_grabbed.Rows.Count != 0)
            {
                // Idziemy po wszystkich znalezionych tabelach
                for(int i = 0; i < database_tables_grabbed.Rows.Count; i++)
                {
                    bool[] correct;
                    string table_name_extracted = (string)database_tables_grabbed.Rows[i].ItemArray[0];
                    table_name_extracted = table_name_extracted.TrimEnd(' '); // dane pobrane z bazy mają duzo spacji na końcu, trimujemy je.
                                                                              // jest też przechowywany w bazie dużymi literami.

                    var table_search_result = database_tables.Find(x => x.Item2.Equals(table_name_extracted.ToLower()));
                    if (table_search_result != null) //gdy znaleźliśmy tabelę o imieniu istniejącym w database_tables
                    {

                        var column_search_result = database_columns.FindAll(x => x.Item1.Equals(table_search_result.Item1));
                        correct = new bool[column_search_result.Count];
                        for (int j = 0; j < correct.Length; j++) correct[j] = false;

                        FbDataAdapter table_grab_columns = new FbDataAdapter("SELECT rdb$field_name " +
                                                                         "FROM rdb$relation_fields " +
                                                                         "WHERE rdb$relation_name = '" + table_name_extracted + "';"
                                                                         ,
                                                                         new FbConnection(database_connection_string));

                        table_grab_columns.Fill(database_columns_grabbed);

                        for (int j = 0; j < database_columns_grabbed.Rows.Count; j++)
                        {
                            string column_name_extracted = (string)database_columns_grabbed.Rows[j].ItemArray[0];
                            column_name_extracted = column_name_extracted.TrimEnd(' ');

                            var matcher = column_search_result.Find(x => x.Item2.Equals(column_name_extracted));
                            if (matcher != null) correct[column_search_result.IndexOf(matcher)] = true;
                        }
                        if (correct.All(x => x.Equals(true) == true)) result[i] = 2;
                        if (correct.Any(x => x.Equals(false))) result[i] = 1;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Logika zakładki Wyświetlanie katalogów

        // Ładowanie wyświetlania katalogów w widoku nadrzędnym.
        private void LV_catalog_display_catalogs_load()
        {
            ListViewItem local_catalog = LV_loaded_catalogs[0];
            LV_loaded_catalogs.Clear();

            List<string> external_catalogs_names = new List<string>();
            List<string> external_catalogs_connection_strings = new List<string>();

            foreach (string filepath in Directory.EnumerateFiles(database_externals_path))
            {
                // Tutaj powinniśmy rozszyfrowywać dane z załadowanych plików

                // Tutaj zakładam, że rozszyfrowaliśmy plik i jest on bazą danych w formacie .fdb
                // Dodatkowo ma on nazwę w formacie [alias użytkownika, który wygenerował bazę]_catalog.fdb
                FileInfo grabbed_file = new FileInfo(filepath);
                if(grabbed_file.Name.Equals(ConfigManager.ReadString(ConfigManager.USER_ALIAS)+"_CATALOG.FDB") ||
                   grabbed_file.Name.Equals("EXTERNAL_CATALOG.FDB"))
                {
                    Console.WriteLine("Próba załadowania niedozwolonego katalogu " + grabbed_file.Name);
                }
                else
                {
                    FbConnectionStringBuilder connection_string_builder = new FbConnectionStringBuilder();
                    if (grabbed_file.Extension.Equals(".fdb") || grabbed_file.Extension.Equals(".FDB"))
                    {
                        // Walidujemy czy katalogi obiegowe są skonstruowane odpowiednio z szablonem bazy.
                        int[] validation_result = new int[database_columns.Count];

                        connection_string_builder.ServerType = FbServerType.Embedded;
                        connection_string_builder.UserID = "SYSDBA"; // Defaultowy uzytkownik z najwyzszymi uprawnieniami do systemu bazodanowego.
                        connection_string_builder.Password = ""; // Haslo nie jest sprawdzane w wersji embedded, można dać tu cokolwiek.
                        connection_string_builder.Database = filepath;
                        connection_string_builder.ClientLibrary = database_engine_path;
                        connection_string_builder.Charset = "UTF8";

                        try
                        {
                            validation_result = database_validate(connection_string_builder.ConnectionString);
                            if (validation_result.All(x => x.Equals(2)) == true)
                            {
                                external_catalogs_names.Add(grabbed_file.Name.Split('_')[0]);
                                external_catalogs_connection_strings.Add(connection_string_builder.ConnectionString);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Połączenie z katalogiem " + grabbed_file.Name + " nie powidło się.");
                        }
                    }
                }
            }

            LV_loaded_catalogs.Add(local_catalog);

            for (int i = 0; i < external_catalogs_names.Count; i++)
            {
                ListViewItem item_to_add = new ListViewItem();

                item_to_add.Name = external_catalogs_connection_strings[i];
                item_to_add.Text = "Katalog użytkownika " + external_catalogs_names[i];
                item_to_add.Tag = external_catalogs_names[i];
                item_to_add.SubItems.Add("Katalog");
                item_to_add.SubItems.Add("---");
                item_to_add.SubItems.Add("---");
                item_to_add.SubItems.Add("---");
                item_to_add.SubItems.Add("---");
                item_to_add.SubItems.Add("---");
                item_to_add.SubItems.Add("---");

                LV_loaded_catalogs.Add(item_to_add);
            }

            FbConnection.ClearAllPools();
        }

        // Wyświetlanie zawartości całego folderu.
        private void LV_catalog_display_folder_content_display(int id_to_display)
        {
            // Reset danych do wyswietlenia, na wypadek gdyby funkcja byla wywolana drugi i dalszy raz.
            LV_catalog_display.Items.Clear();
            directories_grabbed.Clear();
            files_grabbed.Clear();
            LV_catalog_display_cache = null;
            
            DataTable database_folder_subdirectories = new DataTable();
            DataTable database_folder_content = new DataTable();
            // Tworzymy adapter i podpinamy do niego zapytanie SQL wyłuskujące nam wszystkie tabele w bazie (oprócz domyślnych tabel systemu bazodanowego)
            FbDataAdapter database_grab_directory_subdirectories = new FbDataAdapter("SELECT ID,NAME,VISIBLE_TO_OTHERS,REQUESTABLE_BY_OTHERS,COPIES_WITHOUT_CONFIRM " +
                                                                                     "FROM virtual_folder " +
                                                                                     "WHERE DIR_ID = @Target_directory_id;"
                                                                                     ,
                                                                                     new FbConnection(database_connection_string_builder.ConnectionString));

            database_grab_directory_subdirectories.SelectCommand.Parameters.AddWithValue("@Target_directory_id", id_to_display);

            // Zeby zrobic go uniwersalnie, trzeba przekazać jako parametr DIR_ID i DEPTH, DIR_ID bedziemy mieli bezposrednio z folders_grabbed.

            database_grab_directory_subdirectories.Fill(database_folder_subdirectories);

            for (int i = 0; i < database_folder_subdirectories.Rows.Count; i++)
            {
                directories_grabbed.Add(new Tuple<int, string, bool, bool, bool>(
                                       (int)database_folder_subdirectories.Rows[i].ItemArray[0],
                                       (string)database_folder_subdirectories.Rows[i].ItemArray[1],
                                       (bool)database_folder_subdirectories.Rows[i].ItemArray[2],
                                       (bool)database_folder_subdirectories.Rows[i].ItemArray[3],
                                       (bool)database_folder_subdirectories.Rows[i].ItemArray[4]));
        }

            for (int i = 1; i < database_tables.Count; i++) // bierzemy wszystkie tabele oprócz virtual_folder
            {
                FbDataAdapter database_grab_directory_content = new FbDataAdapter("SELECT ID,NAME,EXTENSION,FS_LAST_WRITE_TIME,CATALOGING_DATE,SIZE,VISIBLE_TO_OTHERS,REQUESTABLE_BY_OTHERS,COPIES_WITHOUT_CONFIRM " +
                                                                "FROM " + database_tables[i].Item2 + " " +
                                                                "WHERE DIR_ID = @Target_directory_id;"
                                                                ,
                                                                new FbConnection(database_connection_string_builder.ConnectionString));

                database_grab_directory_content.SelectCommand.Parameters.AddWithValue("@Target_directory_id", id_to_display);

                database_folder_content.Clear();
                database_grab_directory_content.Fill(database_folder_content);

                for (int j = 0; j < database_folder_content.Rows.Count; j++)
                {
                    files_grabbed.Add(new Tuple<int, string, string, string, System.DateTime, System.DateTime, long, Tuple<bool, bool, bool>>(
                                     (int)database_folder_content.Rows[j].ItemArray[0],
                                     database_tables[i].Item2,
                                     (string)database_folder_content.Rows[j].ItemArray[1],
                                     (string)database_folder_content.Rows[j].ItemArray[2],
                                     (System.DateTime)database_folder_content.Rows[j].ItemArray[3],
                                     (System.DateTime)database_folder_content.Rows[j].ItemArray[4],
                                     (long)database_folder_content.Rows[j].ItemArray[5],
                                     new Tuple<bool,bool,bool>(
                                     (bool)database_folder_content.Rows[j].ItemArray[6],
                                     (bool)database_folder_content.Rows[j].ItemArray[7],
                                     (bool)database_folder_content.Rows[j].ItemArray[8])));
                }
            }
            LV_catalog_display.VirtualListSize = directories_grabbed.Count + files_grabbed.Count;
            if (LV_catalog_display.FocusedItem != null)
            {
                LV_catalog_display.FocusedItem.Selected = false;
                LV_catalog_display.FocusedItem.Focused = false;
            }
            if (LV_catalog_display.VirtualListSize >=1) LV_catalog_display.RedrawItems(0, LV_catalog_display.VirtualListSize-1, false);
        }

        // Wywołuje się przed LV_catalog_display_item_selected, służy do obsługi SHIFT+LMB w wybieraniu przedziałów.
        private void LV_catalog_display_item_range_select(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            ListView parent = (ListView)sender;
            if (e.IsSelected == true)
            {
                for (int i = e.StartIndex; i <= e.EndIndex; i++) LV_catalog_display_item_selection.Add(parent.Items[i]);
            }
            else LV_catalog_display_item_selection.Clear();
        }

        // Wywołuje się po LV_catalog_display_item_range_select, służy do obsługi CTRL+LMB w wybiraniu poszczególnych rzeczy.
        private void LV_catalog_display_item_selected(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView parent = (ListView)sender;
            LV_catalog_display_item_selection.Clear();
            for (int i = 0; i < parent.SelectedIndices.Count; i++)
            {
                LV_catalog_display_item_selection.Add(parent.Items[parent.SelectedIndices[i]]);
            }
        }

        // Obsługa zdarzenia wejścia w zakładkę, w zależności od wyniku testu bazy uaktywnia lub dezaktywuje podgląd katalogu.
        private void LV_catalog_display_visible_changed(object sender, EventArgs e)
        {
            if (database_validated_successfully == false)
            {
                MessageBox.Show("Niepoprawny wynik walidacji połączenia i zawartości bazy danych, nie można wyświetlić katalogu!");
                LV_catalog_display.Enabled = false;
                BT_previous.Enabled = false;
                BT_specials.Enabled = false;
                TB_catalog_path_current.Enabled = false;
            }
            else
            {
                if(LV_catalog_display_status == 0)
                {
                    catalog_folder_path_list.Add(@"\");
                    LV_catalog_display_catalogs_load();
                    LV_catalog_display_cache = LV_loaded_catalogs.ToArray();
                    LV_catalog_display.VirtualListSize = LV_catalog_display_cache.Count();
                    LV_catalog_display.RedrawItems(0, LV_catalog_display.VirtualListSize - 1, false);

                    TB_catalog_path_current.Text = catalog_folder_path_list.Last();
                    LV_catalog_display.Enabled = true;
                    BT_previous.Enabled = false;
                    cache_refresh = false;
                    BT_specials.Enabled = false;
                    TB_catalog_path_current.Enabled = false;
                }
                if(LV_catalog_display_status == 1)
                {
                    TB_catalog_path_current.Text = "";
                    foreach (string component in catalog_folder_path_list)
                    {
                        TB_catalog_path_current.Text += component;
                    }
                    LV_catalog_display.Enabled = true;
                    BT_previous.Enabled = true;
                    BT_specials.Enabled = true;
                    TB_catalog_path_current.Enabled = false;
                }
                if(LV_catalog_display_status == 2)
                {
                    TB_catalog_path_current.Text = "";
                    foreach (string component in catalog_folder_path_list)
                    {
                        TB_catalog_path_current.Text += component;
                    }
                    LV_catalog_display.Enabled = true;
                    BT_previous.Enabled = true;
                    BT_specials.Enabled = true;
                    TB_catalog_path_current.Enabled = false;
                }
            }
        }

        // Obsługa zdarzenia cachowania, potrzebna dla ListView w trybie wirtualnym. Odpowiada za konstrukcję i uaktualnianie cache'u.
        private void LV_catalog_display_cache_items(object sender, CacheVirtualItemsEventArgs e)
        {
            if (LV_catalog_display_cache != null && e.StartIndex >= 0 && e.EndIndex <= LV_catalog_display_cache.Length && cache_refresh==false)
            {
                //If the newly requested cache is a subset of the old cache, 
                //no need to rebuild everything, so do nothing.
                return;
            }

            //Now we need to rebuild the cache.
            LV_catalog_display_cache = new ListViewItem[e.EndIndex - e.StartIndex];

            //Fill the cache with the appropriate ListViewItems.
            for (int i = 0; i < LV_catalog_display_cache.Length; i++)
            {
                ListViewItem item_to_add = new ListViewItem();
                if (i < directories_grabbed.Count)
                {
                    item_to_add.Name = directories_grabbed[i].Item1.ToString();
                    item_to_add.Text = directories_grabbed[i].Item2;
                    item_to_add.SubItems.Add("Folder");
                    item_to_add.SubItems.Add("---");
                    item_to_add.SubItems.Add("---");
                    item_to_add.SubItems.Add("---");
                    
                    if (directories_grabbed[i].Item3 == false) item_to_add.SubItems.Add("NIE");
                    else item_to_add.SubItems.Add("TAK");

                    if (directories_grabbed[i].Item4 == false) item_to_add.SubItems.Add("NIE");
                    else item_to_add.SubItems.Add("TAK");

                    if (directories_grabbed[i].Item5 == false) item_to_add.SubItems.Add("NIE");
                    else item_to_add.SubItems.Add("TAK");

                    LV_catalog_display_cache[i] = item_to_add;
                }
                if(i >= directories_grabbed.Count)
                {
                    item_to_add.Name = files_grabbed[i - directories_grabbed.Count].Item1.ToString();
                    item_to_add.ToolTipText = files_grabbed[i - directories_grabbed.Count].Item2;
                    item_to_add.Text = files_grabbed[i - directories_grabbed.Count].Item3;
                    item_to_add.SubItems.Add(files_grabbed[i - directories_grabbed.Count].Item4);
                    item_to_add.SubItems.Add(files_grabbed[i - directories_grabbed.Count].Item5.ToLongTimeString() + " " +
                                             files_grabbed[i - directories_grabbed.Count].Item5.ToShortDateString());
                    item_to_add.SubItems.Add(files_grabbed[i - directories_grabbed.Count].Item6.ToLongTimeString() + " " +
                                             files_grabbed[i - directories_grabbed.Count].Item6.ToShortDateString());
                    item_to_add.SubItems.Add(files_grabbed[i - directories_grabbed.Count].Item7.ToString());

                    if (files_grabbed[i - directories_grabbed.Count].Rest.Item1 == false) item_to_add.SubItems.Add("NIE");
                    else item_to_add.SubItems.Add("TAK");

                    if (files_grabbed[i - directories_grabbed.Count].Rest.Item2 == false) item_to_add.SubItems.Add("NIE");
                    else item_to_add.SubItems.Add("TAK");

                    if (files_grabbed[i - directories_grabbed.Count].Rest.Item3 == false) item_to_add.SubItems.Add("NIE");
                    else item_to_add.SubItems.Add("TAK");

                    LV_catalog_display_cache[i] = item_to_add;
                }
            }

            cache_refresh = false;
        }

        /* Obsługa zdarzenia pobrania przedmiotu do wyświetlenia, wymagana dla ListView w trybie wirtualnym. 
         * Pobiera z cache'u przedmiot albo wydaje polecenie jego rekonstrukcji jeżeli nie ma w nim wymaganego przedmiotu.
        */
        private void LV_catalog_display_retrieve_item(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (LV_catalog_display_cache != null && e.ItemIndex >= 0 && e.ItemIndex < LV_catalog_display_cache.Length)
            {
                // Cache hit, zwracamy wartość z odpowiedniego indeksu.
                e.Item = LV_catalog_display_cache[e.ItemIndex];
            }
            else
            {
                // Cache miss, dajemy polecenie regeneracji cache'u.
                LV_catalog_display_cache_items(this, new CacheVirtualItemsEventArgs(0, directories_grabbed.Count + files_grabbed.Count));
                e.Item = LV_catalog_display_cache[e.ItemIndex];
            }
        }

        // Logika tworzenia folderu z menu kontekstowego.
        private bool context_menu_folder_make(int parent_dir_id, string new_folder_name)
        {
            bool folder_exists_already = false, result = false;
            DataTable database_folder_already_exists_container = new DataTable();
            FbDataAdapter database_folder_already_exists_verifier = new FbDataAdapter("SELECT NAME,DIR_ID " +
                                                                                      "FROM " + database_tables[0].Item2 + " " +
                                                                                      "WHERE DIR_ID = @Target_directory_id " + 
                                                                                      "AND NAME = @Name;"
                                                                                      ,
                                                                                      new FbConnection(database_connection_string_builder.ConnectionString));

            database_folder_already_exists_verifier.SelectCommand.Parameters.AddWithValue("@Target_directory_id", parent_dir_id.ToString());
            database_folder_already_exists_verifier.SelectCommand.Parameters.AddWithValue("@Name", new_folder_name);

            database_folder_already_exists_container.Clear();
            database_folder_already_exists_verifier.Fill(database_folder_already_exists_container);

            if(database_folder_already_exists_container.Rows.Count > 0)
            {
                MessageBox.Show(new_folder_name + " jest już w tym folderze!");
                folder_exists_already = true;
                result = false;
            }

            if(!folder_exists_already)
            {
                // Nie znaleźliśmy duplikatu o tym samym imieniu i o tym samym rodzicu
                // W przypadku nowego folderu z listy kontekstowej wywołujemy po prostu z sztywnym stringiem "Nowy folder"
                database_virtual_folder_make(new_folder_name, parent_dir_id, true, database_connection_string_builder.ConnectionString);
                result = true;
                // Zlecamy programowi ponowne wyświetlenie folderu - jako że przybył nam nowy folder.
                cache_refresh = true;
                LV_catalog_display_folder_content_display(parent_dir_id);
            }
            return result;
        }

        // Obsługa kliknięcia w ListView gdy nie klika się w żadny z wyświetlanych przez niego element.
        private void LV_catalog_display_click_no_selection(object sender, MouseEventArgs e)
        {
            ListView parent = (ListView)sender;
            var temp = parent.HitTest(e.Location).Item;
            if (e.Button == MouseButtons.Right && parent.HitTest(e.Location).Item == null)
            {
                if (LV_catalog_display_status == 0) // przeglądanie katalogów - daj możliwość ściągnięcia katalogu obiegowego jeżeli funkcje sieciowe są włączone.
                {
                    if (!networking_lock)
                    {
                        Point context_menu_position = PointToScreen(e.Location);
                        context_menu_position.X = context_menu_position.X + parent.Location.X + 2 * parent.Margin.Left;
                        context_menu_position.Y = context_menu_position.Y + parent.Location.Y + 30; // kolumna ma rozmiar 30px

                        ContextMenuStrip background_context_menu_strip = new ContextMenuStrip();
                        background_context_menu_strip.Items.Add("Ściągnij katalog obiegowy");
                        background_context_menu_strip.ItemClicked += element_context_menu_item_select;
                        parent.ContextMenuStrip = background_context_menu_strip;
                    }
                    else parent.ContextMenuStrip = null;
                }

                if (LV_catalog_display_status == 1) // przeglądanie katalogu lokalnego.
                {
                    Point context_menu_position = PointToScreen(e.Location);
                    context_menu_position.X = context_menu_position.X + parent.Location.X + 2 * parent.Margin.Left;
                    context_menu_position.Y = context_menu_position.Y + parent.Location.Y + 30; // kolumna ma rozmiar 30px
                
                    ContextMenuStrip background_context_menu_strip = new ContextMenuStrip();
                    background_context_menu_strip.Items.Add("Nowy folder");
                    background_context_menu_strip.Items.Add("Wklej");
                    if (copy == false && cut == false) background_context_menu_strip.Items[1].Enabled = false;
                    background_context_menu_strip.ItemClicked += element_context_menu_item_select;
                    parent.ContextMenuStrip = background_context_menu_strip;
                }
                if (LV_catalog_display_status == 2) // przeglądanie katalogu obiegowego.
                {
                    parent.ContextMenuStrip = null;
                }
            }
        }

        // Obsługa wybierania elementów z menu kontekstowego (oba typy obsługiwane jednym zdarzeniem).
        private void element_context_menu_item_select(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip parent = (ContextMenuStrip)sender;
            ListView super_parent = (ListView)parent.SourceControl;
            
            // Menu kontekstowe bez zaznaczenia obiektu w folderze
            if (e.ClickedItem.Text.Equals("Nowy folder"))
            {
                if(context_menu_folder_make(catalog_folder_id_list.Last(),"Nowy folder"))
                {
                    for(int i = 0; i < super_parent.Items.Count; i++)
                    {
                        if (super_parent.Items[i].Text.Equals("Nowy folder"))
                        {
                            super_parent.LabelEdit = true;
                            super_parent.Items[i].BeginEdit();
                            break;
                        }
                    }
                }
            }
            if (e.ClickedItem.Text.Equals("Wklej"))
            {
                if(copy == true)
                {
                    bool anything_visible = false;
                    for (int i = 0; i < LV_catalog_display_data_to_manipulate.Count; i++)
                    {
                        if(LV_catalog_display_data_to_manipulate[i].SubItems[1].Text.Equals("Folder"))
                        {
                            if (LV_catalog_display_data_to_manipulate[i].SubItems[5].Text == "TAK") anything_visible = true;
                            database_virtual_folder_copy(int.Parse(LV_catalog_display_data_to_manipulate[i].Name),
                                                 catalog_folder_id_list.Last(),
                                                 LV_catalog_display_data_to_manipulate[i].Text);
                        }
                        else
                        {
                            if (LV_catalog_display_data_to_manipulate[i].SubItems[5].Text == "TAK") anything_visible = true;
                            database_virtual_file_copy(LV_catalog_display_data_to_manipulate_orgin_directory_id,
                                                       catalog_folder_id_list.Last(),
                                                       LV_catalog_display_data_to_manipulate[i].ToolTipText,
                                                       LV_catalog_display_data_to_manipulate[i].Text,
                                                       LV_catalog_display_data_to_manipulate[i].SubItems[1].Text);
                        }
                    }
                    if(anything_visible == true)
                    {
                        for(int i = 0; i < catalog_folder_id_list.Count; i++)
                        {
                            if(catalog_folder_id_list[i] == 1) database_virtual_folder_simple_modify_rights(catalog_folder_id_list[i], "root", "VISIBLE_TO_OTHERS", true);
                            else database_virtual_folder_simple_modify_rights(catalog_folder_id_list[i], catalog_folder_path_list[i+1].Remove(catalog_folder_path_list[i+1].Length-1,1), "VISIBLE_TO_OTHERS", true);
                        }
                    }
                }
                if(cut == true)
                {

                }
                LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());
            }

            if (e.ClickedItem.Text.Equals("Ściągnij katalog obiegowy"))
            {
                if (new FileInfo(database_externals_path + "EXTERNAL_CATALOG.FDB").Exists)
                {
                    File.Delete(database_externals_path + "EXTERNAL_CATALOG.FDB");
                }

                used_alias = "";
                System.Net.IPAddress ipAddr = null;
                String alias = null;

                AliasOrAddressInputForm inputForm = new AliasOrAddressInputForm(ref ipAddr, ref alias);
                inputForm.Owner = this;
                inputForm.ShowDialog();
                alias = inputForm.alias_result;
                ipAddr = inputForm.address_result;

                if (alias == null && ipAddr == null)
                {
                    MessageBox.Show("Nie podano ani adresu, ani aliasu.");
                }
                else
                {
                    String requested_filename = "EXTERNAL_CATALOG.FDB";
                    String requested_filepath = "TO_DETERMINE";

                    // Czytamy plik konfiguracyjny w poszukuwaniu wprowadzonej danej
                    string grabbed_aliases = "";
                    bool error_on_aliases_read = false;

                    string provided_alias = "";
                    string provided_address_string = "";

                    if (alias != null) provided_alias = alias;
                    if (ipAddr != null) provided_address_string = ipAddr.ToString();

                    try
                    {
                        grabbed_aliases = ConfigManager.ReadString(ConfigManager.KNOWN_ALIASES);

                        string[] alias_address_pairs = new string[0];

                        if (!grabbed_aliases.Equals(""))
                        {
                            try
                            {
                                alias_address_pairs = grabbed_aliases.Split('|');
                                string[] pair_split = new string[2] { "", "" };

                                List<string> aliases_list = new List<string>();
                                List<string> addresses_list = new List<string>();

                                foreach (string pair in alias_address_pairs)
                                {
                                    try
                                    {
                                        pair_split = pair.Split('_');
                                        aliases_list.Add(pair_split[0]);
                                        addresses_list.Add(pair_split[1]);
                                    }
                                    catch
                                    {
                                        ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                                        error_on_aliases_read = true;
                                        break;
                                    }
                                }

                                if (error_on_aliases_read == false)
                                {
                                    if (alias != null)
                                    {
                                        if(alias.Equals(ConfigManager.ReadString(ConfigManager.USER_ALIAS)))
                                        {
                                            MessageBox.Show("Pobieranie własnego katalogu jest niemożliwe!");
                                            return;
                                        }
                                        
                                        // Szukamy w pliku konfiguracyjnym aliasu
                                        string alias_to_find = provided_alias;
                                        int pair_index = aliases_list.IndexOf(alias_to_find.ToUpper());
                                        if (pair_index != -1)
                                        {
                                            // Znaleźliśmy alias w liście - ustalamy used_IP_address_string.
                                            try
                                            {
                                                ipAddr = System.Net.IPAddress.Parse(addresses_list[pair_index]);

                                                alias = provided_alias;
                                                provided_address_string = ipAddr.ToString();
                                            }
                                            catch
                                            {
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            // Nie znaleźliśmy aliasu - konczymy błędem próbę ściągnięcia katalogu obiegowego!
                                            MessageBox.Show("Nie znaleziono w pliku konfiguracyjnym użytkownika o aliasie: " + alias);
                                            return;
                                        }
                                    }
                                    if (ipAddr != null)
                                    {
                                        if(provided_address_string.Equals(ConfigManager.ReadString(ConfigManager.TCP_COMM_IP_ADDRESS)))
                                        {
                                            MessageBox.Show("Pobieranie własnego katalogu jest niemożliwe!");
                                            return;
                                        }
                                        
                                        // Szukamy w pliku konfiguracyjnym adresu
                                        string address_to_find = provided_address_string;
                                        int pair_index = addresses_list.IndexOf(address_to_find);
                                        if (pair_index != -1)
                                        {
                                            // Znaleźliśmy adres w liście - ustalamy alias.
                                            try
                                            {
                                                provided_alias = aliases_list[pair_index];
                                                used_IP_address_string = provided_address_string;
                                            }
                                            catch
                                            {
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            // Nie znaleźliśmy adresu w liście - przekazujemy tylko wartość adresu do used_IP_address_string
                                            provided_alias = "UNKNOWN";
                                            used_alias = "UNKNOWN";
                                            used_IP_address_string = provided_address_string;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                            }
                        }
                        else
                        {
                            // Nasza lista aliasów jest pusta - idziemy dalej przez program.
                            used_IP_address_string = ipAddr.ToString();
                            provided_address_string = ipAddr.ToString();
                            used_alias = "UNKNOWN";
                            provided_alias = "UNKNOWN";
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Błąd odczytu znanych aliasów z pliku konfiguracyjnego!\n" +
                                        "Resetuję miejsce przechowywania aliasów!");
                        try
                        {
                            ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                        }
                        catch
                        {
                            MessageBox.Show("Błąd resetowania miejsca przechowywania aliasów!\n" +
                                            "Prawdopodobne uszkodzenie/błąd dostępu do pliku konfiguracyjnego!");
                            return;
                        }
                    }

                    if (!provided_alias.Equals("UNKNOWN")) 
                    {
                        if (new FileInfo(database_externals_path + provided_alias + "_CATALOG.FDB").Exists)
                        {
                            try
                            {
                                File.Delete(database_externals_path + provided_alias + "_CATALOG.FDB");
                            }
                            catch
                            {

                            }
                        }
                    }
                    
                    DistributedNetworkUser targetUser = new DistributedNetworkUser(false, provided_alias, System.Net.IPAddress.Parse(provided_address_string));

                    DistributedNetworkFile distributedNetworkFile =
                        new DistributedNetworkFile(requested_filename, requested_filepath, 0, true, true);

                    List<DistributedNetworkFile> distributedNetworkFiles = new List<DistributedNetworkFile>();
                    distributedNetworkFiles.Add(distributedNetworkFile);

                    distributedNetwork.RequestFile(targetUser, distributedNetworkFiles);
                }
            }


            // Menu kontekstowe dla obiektów w folderze
            if (e.ClickedItem.Text.Equals("Otwórz"))
            {
                if(super_parent.FocusedItem.SubItems[1].Text.Equals("Folder"))
                {
                    // Otwieramy folder
                    LV_catalog_display_change_directory(super_parent.FocusedItem);
                }
                else
                {
                    // Otwieramy plik
                    string target_location = database_virtual_filepath_get(int.Parse(super_parent.FocusedItem.Name), super_parent.FocusedItem.ToolTipText);
                    if (target_location != string.Empty)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(target_location);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            if (e.ClickedItem.Text.Equals("Kopiuj"))
            {
                LV_catalog_display_data_to_manipulate = new List<ListViewItem>(LV_catalog_display_item_selection);
                LV_catalog_display_data_to_manipulate_orgin_directory_id = catalog_folder_id_list.Last();
                copy = true;
                if (cut == true) cut = false;
            }
            if (e.ClickedItem.Text.Equals("Usuń"))
            {
                foreach(var item in LV_catalog_display_item_selection)
                {   
                        // wysylamy do usuniecia folder
                        if (item.SubItems[1].Text.Equals("Folder"))
                        database_virtual_folder_delete(int.Parse(item.Name),
                                                       database_tables[0].Item2,
                                                       false,
                                                       false);
                        // wysylamy do usuniecia plik
                        else database_virtual_folder_delete(int.Parse(item.Name),
                                                            item.ToolTipText,
                                                            true,
                                                            false);
                }

                cache_refresh = true;
                LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());

                //Tutaj trzeba dac polecenie regenerowanie cache'u.
            }
            if (e.ClickedItem.Text.Equals("Zmień nazwę"))
            {
                // parent.Text - ID obiektu który wysłał polecenie zmiany nazwy
                super_parent.LabelEdit = true;
                super_parent.Items[int.Parse(parent.Text)].BeginEdit();
            }
            if (e.ClickedItem.Text.Equals("Zmień widoczność w katalogu obiegowym") || 
                e.ClickedItem.Text.Equals("Zmień dostępność do pobrania") || 
                e.ClickedItem.Text.Equals("Zmień dostępność do pobrania (bez pytania)"))
            {
                string right_to_modify = "";
                int right_to_modify_index = 0, correct_values = 0, incorrect_values = 0;
                bool[] current_state = new bool[3] { false, false, false };
                bool anything_made_visible = false;

                // Określenie na której zmiennej wykonujemy zmianę uprawnień
                if (e.ClickedItem.Text.Equals("Zmień widoczność w katalogu obiegowym"))
                {
                    right_to_modify = "VISIBLE_TO_OTHERS";
                    right_to_modify_index = 5;
                }
                if (e.ClickedItem.Text.Equals("Zmień dostępność do pobrania"))
                {
                    right_to_modify = "REQUESTABLE_BY_OTHERS";
                    right_to_modify_index = 6;
                }
                if (e.ClickedItem.Text.Equals("Zmień dostępność do pobrania (bez pytania)"))
                {
                    right_to_modify = "COPIES_WITHOUT_CONFIRM";
                    right_to_modify_index = 7;
                }

                // Teraz zmieniamy uprawnienia obiektów wewnątrz selekcji:
                foreach (ListViewItem selected_item in LV_catalog_display_item_selection)
                {
                    bool new_value = false, skip = false;

                    // Pobieramy bierzący stan uprawnień dla obiektu:
                    // Current state reprezentuje nasze trzy flagi w kolejności
                    // 1. current_state[0] to VISIBLE_TO_OTHERS
                    // 2. current_state[0] to REQUESTABLE_BY_OTHERS
                    // 3. current_state[0] to COPIES_WITHOUT_CONFIRM
                    if (selected_item.SubItems[5].Text.Equals("TAK")) current_state[0] = true;
                    else current_state[0] = false;

                    if (selected_item.SubItems[6].Text.Equals("TAK")) current_state[1] = true;
                    else current_state[1] = false;

                    if (selected_item.SubItems[7].Text.Equals("TAK")) current_state[2] = true;
                    else current_state[2] = false;

                    // Tutaj wybieramy jak zmienimy wartość naszej zmiennej.
                    if (selected_item.SubItems[right_to_modify_index].Text.Equals("NIE")) new_value = true;
                    else new_value = false;

                    // Sprawdzamy czy jakakolwiek wartość w danym folderze zostaje uwidoczniona - jeżeli tak to wszystkie foldery macierzyste
                    // dla folderu, z którego pochodzi selekcja muszą także zostać uwidocznione!
                    if(current_state[0] == false && right_to_modify_index == 5 && new_value == true)
                    {
                        anything_made_visible = true;
                    }

                    // Czyszczenie wartości dla flag podrzędnych w przypadku wyłączenia flagi nadrzędnej
                    if(right_to_modify_index == 5 && new_value == false && !selected_item.SubItems[1].Text.Equals("Folder"))
                    {
                        if(current_state[1] == true)
                            database_virtual_file_modify_rights(catalog_folder_id_list.Last(),
                                                       selected_item.ToolTipText,
                                                       selected_item.Text,
                                                       selected_item.SubItems[1].Text,
                                                       "REQUESTABLE_BY_OTHERS",
                                                       false);
                        if (current_state[2] == true)
                            database_virtual_file_modify_rights(catalog_folder_id_list.Last(),
                                                       selected_item.ToolTipText,
                                                       selected_item.Text,
                                                       selected_item.SubItems[1].Text,
                                                       "COPIES_WITHOUT_CONFIRM",
                                                       false);
                    }

                    if (right_to_modify_index == 6 && new_value == false && !selected_item.SubItems[1].Text.Equals("Folder"))
                    {
                        if (current_state[2] == true)
                            database_virtual_file_modify_rights(catalog_folder_id_list.Last(),
                                                       selected_item.ToolTipText,
                                                       selected_item.Text,
                                                       selected_item.SubItems[1].Text,
                                                       "COPIES_WITHOUT_CONFIRM",
                                                       false);
                    }

                    // Sprawdzanie, czy flagi nadrzędne są ustawione na prawdę przy próbie zmiany wartości flagi podrzędnej
                    if (right_to_modify_index != 5)
                    {
                        if (current_state[0] == false) skip = true;
                        if (right_to_modify_index == 7)
                        {
                            if (current_state[1] == false) skip = true;
                        }
                        if (skip == true) incorrect_values++;
                    }

                    if (skip == false)
                    {
                        if (selected_item.SubItems[1].Text.Equals("Folder"))
                        {
                            //Zmieniamy uprawnienia dla folderu
                            database_virtual_folder_modify_rights(int.Parse(selected_item.Name),
                                                 selected_item.Text,
                                                 right_to_modify,
                                                 new_value);
                            correct_values++;
                        }
                        else
                        {
                            //Zmieniamy uprawnienia dla pliku
                            database_virtual_file_modify_rights(catalog_folder_id_list.Last(),
                                                       selected_item.ToolTipText,
                                                       selected_item.Text,
                                                       selected_item.SubItems[1].Text,
                                                       right_to_modify,
                                                       new_value);
                            correct_values++;
                        }
                    }
                }

                if (anything_made_visible == true)
                {
                    int source_folder_id = catalog_folder_id_list.Last();

                    // Wyłapujemy wszystkie foldery macierzyste dla folderu, z którego pracujemy
                    List<int> parents_ids_to_modify = new List<int>();

                    DataTable parents_to_modify = new DataTable();
                    FbDataAdapter parents_to_modify_grabber = new FbDataAdapter("SELECT ID,DIR_ID " +
                                                                            "FROM " + database_tables[0].Item2 + " " +
                                                                            "WHERE ID = @Target_directory_id AND VISIBLE_TO_OTHERS = FALSE;"
                                                                            ,
                                                                            new FbConnection(database_connection_string_builder.ConnectionString));

                    parents_to_modify_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", source_folder_id);

                    parents_to_modify_grabber.Fill(parents_to_modify);

                    while (parents_to_modify.Rows.Count > 0)
                    {
                        parents_ids_to_modify.Add((int)parents_to_modify.Rows[parents_to_modify.Rows.Count - 1].ItemArray[0]);
                        try
                        {
                            int next_parent_id = (int)parents_to_modify.Rows[parents_to_modify.Rows.Count - 1].ItemArray[1];

                            parents_to_modify_grabber.SelectCommand.Parameters[0].Value = next_parent_id;

                            parents_to_modify.Clear();
                            parents_to_modify_grabber.Fill(parents_to_modify);
                        }
                        catch
                        {
                            break;
                        }
                    }

                    // I zmieniamy ich widoczność jeżeli zachodzi taka potrzeba

                    FbCommand parents_rights_modifier = new FbCommand("UPDATE " + database_tables[0].Item2 + " " +
                                                               "SET VISIBLE_TO_OTHERS = TRUE " +
                                                               "WHERE ID = @Id; "
                                                               ,
                                                               new FbConnection(database_connection_string_builder.ConnectionString));

                    parents_rights_modifier.Parameters.Add(@"Id", FbDbType.Integer);

                    foreach (int id in parents_ids_to_modify)
                    {
                        parents_rights_modifier.Parameters[0].Value = id;

                        parents_rights_modifier.Connection.Open();
                        parents_rights_modifier.ExecuteNonQuery();
                        parents_rights_modifier.Connection.Close();
                    }
                }


                MessageBox.Show("Ilość plików, którym zmieniono uprawnienia: " + correct_values.ToString() + "\n" +
                                "Ilość plików, dla których zmiana nie była możliwa: " + incorrect_values.ToString());
                LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());

            }
            if (e.ClickedItem.Text.Equals("Właściwości"))
            {
                ListViewItem current_file = super_parent.FocusedItem;
                if (current_file.SubItems[1].Text.Equals("Folder"))
                {
                    // Otwieramy folder
                    MessageBox.Show("Foldery nie obsługują wyświetlania właściwości.");
                }
                else
                {
                    int index_at = 0;
                    List<string> file_metadata_names = new List<string>();
                    DataTable file_metadata_content_container = new DataTable();

                    FbDataAdapter file_metadata_content_extractor = new FbDataAdapter("SELECT * " +
                                                                          "FROM " + current_file.ToolTipText + " " +
                                                                          "WHERE ID = @Id;"
                                                                          ,
                                                                          new FbConnection(database_connection_string_builder.ConnectionString));

                    file_metadata_content_extractor.SelectCommand.Parameters.AddWithValue("@Id", int.Parse(current_file.Name));
                    file_metadata_content_extractor.Fill(file_metadata_content_container);

                    index_at = database_tables.FindIndex(x => x.Item2.Equals(current_file.ToolTipText));
                    var relevant_columns = database_columns.FindAll(x => x.Item1.Equals(index_at));
                    foreach (var column in relevant_columns)
                    {
                        file_metadata_names.Add(column.Item2);
                    }
                    
                    Properties_window item_properties = new Properties_window();
                    item_properties.data_passed = file_metadata_content_container;
                    item_properties.names_passed = file_metadata_names;
                    item_properties.Show();
                }
            }
            if (e.ClickedItem.Text.Equals("Ściągnij"))
            {
                //Console.WriteLine("Ściągnij clicked!");
                
                total_selected = LV_catalog_display_item_selection.Count();
                bool folders_in_selection = false;

                // Resetujemy informację o poprawnie pobranych plikach:
                total_to_download = 0;
                successful_downloads = new List<string>();
                successful_downloads_count = 0;
                failed_downloads = new List<string>();
                failed_downloads_count = 0;

                List<DistributedNetworkFile> distributedNetworkFiles = new List<DistributedNetworkFile>();
                DistributedNetworkUser targetUser = null;
                System.Net.IPAddress ipAddr = null;

                // Przeszukiwanie przechowywanych par alias adres:
                // Przechowujemy je w formacie ALIAS0_ADRES IP0|ALIAS1_ADRES IP1|...

                string grabbed_aliases = "";
                bool error_on_aliases_read = false;

                try
                {
                    grabbed_aliases = ConfigManager.ReadString(ConfigManager.KNOWN_ALIASES);

                    string[] alias_address_pairs = new string[0];

                    if (!grabbed_aliases.Equals(""))
                    {
                        try
                        {
                            alias_address_pairs = grabbed_aliases.Split('|');
                            string[] pair_split = new string[2] { "", "" };

                            List<string> aliases_list = new List<string>();
                            List<string> addresses_list = new List<string>();

                            foreach (string pair in alias_address_pairs)
                            {
                                try
                                {
                                    pair_split = pair.Split('_');
                                    aliases_list.Add(pair_split[0]);
                                    addresses_list.Add(pair_split[1]);
                                }
                                catch
                                {
                                    MessageBox.Show("Błąd dzielenia pary alias-adres na składowe!\n" +
                                                    "Czyszczę zawartość znanych aliasów!");
                                    ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                                    error_on_aliases_read = true;
                                    break;
                                }
                            }

                            if (error_on_aliases_read == false)
                            {
                                string alias_to_find = LV_catalog_display_current_catalog_alias;
                                int pair_index = aliases_list.IndexOf(alias_to_find.ToUpper());
                                if(pair_index != -1)
                                {
                                    // Znaleźliśmy alias w liście - ustalamy ipAddr
                                    try
                                    {
                                        ipAddr = System.Net.IPAddress.Parse(addresses_list[pair_index]);
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Błąd przy parsowaniu adresu z znalezionej pary!");
                                    }
                                }
                                else
                                {
                                    // Nie znaleźliśmy aliasu w liście - nic nie robimy.
                                }
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Błąd dzielenia kontenera znanych aliasów na pary alias-adres!\n" +
                                            "Czyszczę zawartość znanych aliasów!");
                            ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                            error_on_aliases_read = true;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Błąd odczytu znanych aliasów z pliku konfiguracyjnego!\n" +
                                    "Resetuję miejsce przechowywania aliasów!");
                    try
                    {
                        ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                    }
                    catch
                    {
                        MessageBox.Show("Błąd resetowania miejsca przechowywania aliasów!\n" +
                                        "Prawdopodobne uszkodzenie/błąd dostępu do pliku konfiguracyjnego!");
                        return;
                    }
                    error_on_aliases_read = true;
                }

                // Nie znaleźliśmy w konfigu podanego aliasu - tworzymy nowy rekord w pliku konfiguracyjnym.
                if (ipAddr == null)
                {
                    IPAddressInputForm ipAddressInputForm = new IPAddressInputForm(ref ipAddr);
                    ipAddressInputForm.Owner = this;
                    ipAddressInputForm.ShowDialog();
                    ipAddr = ipAddressInputForm.resultRef;

                    if (ipAddr == null)
                    {
                        // Podany adres IP jest niepoprawny - okno o tym informujące jest wywoływane gdzie indziej, tutaj po prostu przerywamy funkcję!
                        return;
                    }
                    else
                    {
                        // Podano adres IP zwalidowany - pobieramy zawartość grabbed_aliases i badamy co w nim jest
                        if(grabbed_aliases.Length == 0)
                        {
                            // Grabbed aliases był pusty albo próba jego wzięcia zakonczyła się błędem - nadpisz nowym stringiem z pojedynczym aliasem.
                            string entity_to_add = "";
                            entity_to_add = LV_catalog_display_current_catalog_alias;
                            entity_to_add += '_' + ipAddr.ToString();
                            ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, entity_to_add);
                        }
                        else
                        {
                            // Grabbed aliases zawiera już zawartość, która przeszła poprawnie walidację - pobieramy zawartość dotychczasową i dodajemy nowy.
                            string entity_to_add = grabbed_aliases;
                            entity_to_add += '|' + LV_catalog_display_current_catalog_alias;
                            entity_to_add += '_' + ipAddr.ToString();
                            ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, entity_to_add);
                        }
                    }
                }

                foreach (ListViewItem selected_item in LV_catalog_display_item_selection)
                {
                    if (selected_item.SubItems[1].Text.Equals("Folder"))
                    {
                        // obsługa folderów w wybieraniu obiektów do wysłania, zwraca pod koniec działania funkcji okno z informacją że jest unsupported.
                        folders_in_selection = true;
                    }
                    else
                    {
                        // obsługa plików w wybieraniu obiektów do wysyłania - pobieramy ścieżkę i alias użytkownika, od którego chcemy dane.
                        // ID w tabeli, z której pochodzi metadana - selected_item.Name
                        // tabela z której pochodzi metadana - selected_item.ToolTipText
                        if (selected_item.SubItems[6].Text.Equals("TAK"))
                        {
                            // Inkrementujemy licznik rzeczy do ściągnięcia.
                            total_to_download++;

                            // Sprawdzamy czy plik można ściągnąć bez pytania.
                            bool requested_downloads_without_question = false;
                            if (selected_item.SubItems[7].Text.Equals("TAK")) requested_downloads_without_question = true;

                            // Bierzemy ścieżkę do pliku z bazy danych:
                            string requested_filepath = "", requested_filename = "", requested_alias = "";
                            long requested_filesize = 0;

                            DataTable filepath_container = new DataTable();
                            FbDataAdapter filepath_grabber = new FbDataAdapter("SELECT ORIGINAL_NAME,PATH " +
                                                                              "FROM " + selected_item.ToolTipText + " " +
                                                                              "WHERE ID = @Id;"
                                                                              ,
                                                                              new FbConnection(database_connection_string_builder.ConnectionString));

                            filepath_grabber.SelectCommand.Parameters.AddWithValue("@Id", int.Parse(selected_item.Name));

                            filepath_grabber.Fill(filepath_container);

                            if (filepath_container.Rows.Count == 1)
                            {
                                // Znalazł nasz plik
                                requested_filepath = (string)filepath_container.Rows[0].ItemArray[1];
                                requested_filename = (string)filepath_container.Rows[0].ItemArray[0];
                            }

                            // Bierzemy alias użytkownika, którego katalogo obiegowy czytamy:
                            requested_alias = LV_catalog_display_current_catalog_alias;

                            // Bierzemy rozmiar pliku, który chcemy ściągnąć:
                            try
                            {
                                requested_filesize = long.Parse(selected_item.SubItems[4].Text);
                            }
                            catch
                            {
                                requested_filesize = 0;
                            }
                            FileInfo file_already_downloaded = new FileInfo(ConfigManager.ReadString(ConfigManager.DOWNLOAD_LOCATION) + Path.GetFileName(requested_filepath));
                            // Sprawdzamy czy plik, który chcemy sciągnąć nie jest już w folderze zapisu
                            if (file_already_downloaded.Exists)
                            {
                                // Jest - usuwamy go bo zakładamy że jest stary!
                                File.Delete(ConfigManager.ReadString(ConfigManager.DOWNLOAD_LOCATION) + Path.GetFileName(requested_filepath));
                            }

                            /* Takim sposobem mamy:
                             * Ścieżkę do pobieranego pliku - w zm. requested_filepath.
                             * Alias użytkownika, od którego pobieramy - w zm. requested_alias.
                             * Bool'a czy możemy pobrać go bez pytania - w zm. requested_downloads_without_question.
                             * Pobieramy je tylko gdy plik może być pobieraly, co sprawdzamy na samym początku.
                            */

                            // Tutaj wywołujemy przesyłanie plików:
                            targetUser = new DistributedNetworkUser(false, requested_alias, ipAddr);

                            DistributedNetworkFile distributedNetworkFile =
                                new DistributedNetworkFile(requested_filename, requested_filepath, requested_filesize, true, requested_downloads_without_question);
                            distributedNetworkFiles.Add(distributedNetworkFile);
                        }
                    }
                }
                distributedNetwork.RequestFile(targetUser, distributedNetworkFiles);
                if (folders_in_selection == true)
                {
                    MessageBox.Show("W wybranych plikach znalazł się folder. Program obsługuje tylko przesyłanie plików.");
                }
                if(total_to_download == 0)
                {
                    MessageBox.Show("W wybranych plikach nie było żadnego pliku, który możnaby ściągnąć.");
                }

            }
            if (e.ClickedItem.Text.Equals("Opcje specjalne"))
            {
                BT_specials_Click(this, new EventArgs());
            }
        }

        // Sprawdzanie możliwości edycji nazwy pliku przed jej modyfikacją - sprawdzamy czy nazwa jest edytowalna.
        private void LV_catalog_display_before_label_edit (object sender, LabelEditEventArgs e)
        {
            ListView parent = (ListView)sender;

            // e.Label - nowa nazwa
            // parent.Items[e.Item].Name - ID obiektu do zmiany nazwy.
            // parent.Items[e.Item].Text - nazwa obiektu do zmiany nazwy.
            // parent.Items[e.Item].Subitems[1].Text - typ obiektu do zmiany nazwy.
            
            if (parent.Items[e.Item].SubItems[1].Text.Equals("Folder"))
            {
                if (database_virtual_folder_modifiable_check(int.Parse(parent.Items[e.Item].SubItems[0].Name)) == true) e.CancelEdit = false;
                else e.CancelEdit = true;
            }
            else
            {
                // Edytujemy nazwę pliku - wszystko dozwolone tutaj, sprawdzimy dopiero po tym jak cos wpisze
            }
        }
        
        // Sprawdzanie możliwości edycji nazwy pliku po wpisaniu nowej - tutaj sprawdzamy kolizje z już istniejącymi nazwami.
        private void LV_catalog_display_after_label_edit (object sender, LabelEditEventArgs e)
        {
            // tutaj użytkownik skonczył już edytować nazwę - sprawdzamy czy nie ma juz czegoś o tej samej nazwie i dopiero gdy wiemy że nie ma dajemy mu zrobić swoje.
            // potem updateujemy rekord w bazie danych.

            ListView parent = (ListView)sender;

            if (e.Label == null) e.CancelEdit = true;
            else
            {
                if (e.Label == "") e.CancelEdit = true;
            }
            if(e.CancelEdit != true)
            {
                if (parent.Items[e.Item].SubItems[1].Text.Equals("Folder"))
                {
                    // Edytujemy nazwę folderu - sprawdzić czy dozwolona jest modyfikacja nazwy dla tego folderu!
                    bool folder_exists_already = false;
                    DataTable database_folder_already_exists_container = new DataTable();
                    FbDataAdapter database_folder_already_exists_verifier = new FbDataAdapter("SELECT NAME,DIR_ID " +
                                                                                              "FROM " + database_tables[0].Item2 + " " +
                                                                                              "WHERE DIR_ID = @Target_directory_ID " +
                                                                                              "AND NAME = @Name;"
                                                                                              ,
                                                                                              new FbConnection(database_connection_string_builder.ConnectionString));

                    database_folder_already_exists_verifier.SelectCommand.Parameters.AddWithValue("@Target_directory_ID", catalog_folder_id_list.Last());
                    database_folder_already_exists_verifier.SelectCommand.Parameters.AddWithValue("@Name", e.Label);


                    database_folder_already_exists_container.Clear();
                    database_folder_already_exists_verifier.Fill(database_folder_already_exists_container);
                    if (database_folder_already_exists_container.Rows.Count > 0)
                    {
                        MessageBox.Show(e.Label + " jest już w tym folderze!");
                        folder_exists_already = true;
                        e.CancelEdit = true;
                    }

                    if (!folder_exists_already)
                    {
                        // Nie znaleźliśmy duplikatu o tym samym imieniu i o tym samym rodzicu, czas zmienić jego pierwotną zawartość w bazie danych:
                        FbCommand database_folder_renamer = new FbCommand("UPDATE " + database_tables[0].Item2 + " " +
                                                                          "SET NAME = @Name " +
                                                                          "WHERE ID = @Id;"
                                                                          ,
                                                                          new FbConnection(database_connection_string_builder.ConnectionString));

                        database_folder_renamer.Parameters.AddWithValue("@Name", e.Label);
                        database_folder_renamer.Parameters.AddWithValue("@Id", parent.Items[e.Item].Name);

                        database_folder_renamer.Connection.Open();
                        database_folder_renamer.ExecuteNonQuery();
                        database_folder_renamer.Connection.Close();
                    }

                }
                else
                {
                    // Edytujemy nazwę pliku - wszystko dozwolone, byle tylko nie było dwóch plikach o tych samych nazwach i rozszerzeniach
                    // Edytujemy nazwę pliku - sprawdzamy czy nie pojawił się już plik o tej samej nazwie i rozszerzeniu.
                    string target_table = string.Empty;
                    bool file_exists_already = false;
                    DataTable database_file_already_exists_container = new DataTable();

                    target_table = parent.Items[e.Item].ToolTipText;

                    FbDataAdapter database_file_already_exists_verifier = new FbDataAdapter("SELECT NAME,DIR_ID,EXTENSION " +
                                                                                            "FROM " + target_table + " " +
                                                                                            "WHERE DIR_ID = @Target_directory_ID " +
                                                                                            "AND NAME = @Name " +
                                                                                            "AND EXTENSION = @Extension;"
                                                                                            ,
                                                                                            new FbConnection(database_connection_string_builder.ConnectionString));

                    database_file_already_exists_verifier.SelectCommand.Parameters.AddWithValue("@Target_directory_ID", catalog_folder_id_list.Last());
                    database_file_already_exists_verifier.SelectCommand.Parameters.AddWithValue("@Name", e.Label);
                    database_file_already_exists_verifier.SelectCommand.Parameters.AddWithValue("@Extension", parent.Items[e.Item].SubItems[1].Text);

                    database_file_already_exists_container.Clear();
                    database_file_already_exists_verifier.Fill(database_file_already_exists_container);
                    if (database_file_already_exists_container.Rows.Count > 0)
                    {
                        MessageBox.Show(e.Label + " jest już w tym folderze!");
                        file_exists_already = true;
                        e.CancelEdit = true;
                    }
                    if (!file_exists_already)
                    {
                        // Nie znaleźliśmy duplikatu o tym samym imieniu i o tym samym rodzicu, czas zmienić jego pierwotną zawartość w bazie danych.
                        FbCommand database_file_renamer = new FbCommand("UPDATE " + target_table + " " +
                                                                        "SET NAME = @Name " +
                                                                        "WHERE ID = @Id " +
                                                                        "AND NAME = @Name_to_find;"
                                                                        ,
                                                                        new FbConnection(database_connection_string_builder.ConnectionString));

                        database_file_renamer.Parameters.AddWithValue("@Name", e.Label);
                        database_file_renamer.Parameters.AddWithValue("@Id", parent.Items[e.Item].Name);
                        database_file_renamer.Parameters.AddWithValue("@Name_to_find", parent.Items[e.Item].Text);


                        database_file_renamer.Connection.Open();
                        database_file_renamer.ExecuteNonQuery();
                        database_file_renamer.Connection.Close();
                    }
                }
            }
            parent.LabelEdit = false;   
        }

        // Obsługuje zdarzenie pojedynczego kliknięcia myszą.
        private void LV_catalog_display_single_click (object sender, MouseEventArgs e)
        {
            ListView parent = (ListView)sender;
            if (e.Button == MouseButtons.Right)
            {
                if (parent.HitTest(e.Location).Item.SubItems[1].Text.Equals("Folder"))
                {
                    //opcje dla folderów
                    Point context_menu_position = PointToScreen(e.Location);
                    context_menu_position.X = context_menu_position.X + parent.Location.X + 2 * parent.Margin.Left;
                    context_menu_position.Y = context_menu_position.Y + parent.Location.Y + 30; // kolumna ma rozmiar 30px
                    ContextMenuStrip folder_context_menu_strip = new ContextMenuStrip();
                    folder_context_menu_strip.Name = parent.HitTest(e.Location).Item.SubItems[1].Text; // typ obiektu na którym jesteśmy
                    folder_context_menu_strip.Text = parent.HitTest(e.Location).Item.Index.ToString(); // indeks na liście obiektu na ktorym jestesmy
                    if (LV_catalog_display_status == 0)
                    {
                        folder_context_menu_strip.Items.Add("Otwórz");
                    }
                    if (LV_catalog_display_status == 1)
                    {
                        folder_context_menu_strip.Items.Add("Otwórz");
                        folder_context_menu_strip.Items.Add("Kopiuj");
                        folder_context_menu_strip.Items.Add("Usuń");
                        folder_context_menu_strip.Items.Add("Zmień nazwę");
                        folder_context_menu_strip.Items.Add("Zmień widoczność w katalogu obiegowym");
                        folder_context_menu_strip.Items.Add("Zmień dostępność do pobrania");
                        folder_context_menu_strip.Items.Add("Zmień dostępność do pobrania (bez pytania)");
                        folder_context_menu_strip.Items.Add("Opcje specjalne");
                    }
                    if(LV_catalog_display_status == 2)
                    {
                        folder_context_menu_strip.Items.Add("Otwórz");
                        folder_context_menu_strip.Items.Add("Usuń");
                        folder_context_menu_strip.Items.Add("Opcje specjalne");
                    }
                    folder_context_menu_strip.ItemClicked += element_context_menu_item_select;
                    if (context_menu_position.Y < context_menu_position.Y) context_menu_position.Y = context_menu_position.Y - folder_context_menu_strip.Height - 25;
                    parent.ContextMenuStrip = folder_context_menu_strip;
                }
                else
                {
                    //opcje dla plików
                    Point context_menu_position = PointToScreen(e.Location);
                    context_menu_position.X = context_menu_position.X + parent.Location.X + 2 * parent.Margin.Left;
                    context_menu_position.Y = context_menu_position.Y + parent.Location.Y + 30; // kolumna ma rozmiar 30px
                    ContextMenuStrip file_context_menu_strip = new ContextMenuStrip();
                    file_context_menu_strip.Name = parent.HitTest(e.Location).Item.SubItems[1].Text; // typ obiektu na którym jesteśmy
                    file_context_menu_strip.Text = parent.HitTest(e.Location).Item.Index.ToString(); // indeks na liście obiektu na ktorym jestesmy
                    if (LV_catalog_display_status == 1)
                    {
                        file_context_menu_strip.Items.Add("Otwórz");
                        file_context_menu_strip.Items.Add("Kopiuj");
                        file_context_menu_strip.Items.Add("Usuń");
                        file_context_menu_strip.Items.Add("Zmień nazwę");
                        file_context_menu_strip.Items.Add("Zmień widoczność w katalogu obiegowym");
                        file_context_menu_strip.Items.Add("Zmień dostępność do pobrania");
                        file_context_menu_strip.Items.Add("Zmień dostępność do pobrania (bez pytania)");
                        file_context_menu_strip.Items.Add("Właściwości");
                        file_context_menu_strip.Items.Add("Opcje specjalne");
                    }
                    if (LV_catalog_display_status == 2)
                    {
                        if(!networking_lock) file_context_menu_strip.Items.Add("Ściągnij");
                        file_context_menu_strip.Items.Add("Właściwości");
                        file_context_menu_strip.Items.Add("Opcje specjalne");
                    }
                    file_context_menu_strip.ItemClicked += element_context_menu_item_select;
                    if (context_menu_position.Y < context_menu_position.Y) context_menu_position.Y = context_menu_position.Y - file_context_menu_strip.Height - 25;
                    parent.ContextMenuStrip = file_context_menu_strip;
                }
            }
        }

        // Zmienia wyswietlany folder.
        private void LV_catalog_display_change_directory (ListViewItem target_directory)
        {
            if (target_directory != null && target_directory.SubItems.Count != 0)
            {
                if (target_directory.SubItems[1].Text.Equals("Folder"))
                {
                    if (catalog_folder_path_list.Count > 1 && catalog_folder_path_list[1].Equals(@"Katalog lokalny\"))
                    {
                        // Jesteśmy w katalogu lokalnym
                        catalog_folder_id_list.Add(int.Parse(target_directory.Name));
                        catalog_folder_path_list.Add(target_directory.Text + @"\");
                        TB_catalog_path_current.Text = string.Empty;
                        for (int i = 0; i < catalog_folder_path_list.Count; i++)
                        {
                            TB_catalog_path_current.Text += catalog_folder_path_list[i];
                        }

                        cache_refresh = true;
                        LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());
                    }
                    if (catalog_folder_path_list.Count > 1 && !catalog_folder_path_list[1].Equals(@"Katalog lokalny\"))
                    {
                        // Jesteśmy w którymś katalogu obiegowym
                        catalog_folder_id_list.Add(int.Parse(target_directory.Name));
                        catalog_folder_path_list.Add(target_directory.Text + @"\");
                        TB_catalog_path_current.Text = string.Empty;
                        for (int i = 0; i < catalog_folder_path_list.Count; i++)
                        {
                            TB_catalog_path_current.Text += catalog_folder_path_list[i];
                        }

                        cache_refresh = true;
                        LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());
                    }
                }
                else
                {
                    if (target_directory.SubItems[1].Text.Equals("Katalog"))
                    {
                        database_connection_string_builder = new FbConnectionStringBuilder(target_directory.Name);
                    }
                    else
                    {
                        // Błąd - przekazano do funkcji plik.
                        MessageBox.Show("Błąd! Przekazany przedmiot wskazuje na coś innego niż folder");
                    }   
                }
            }
            else
            {
                // Błąd - przekazany obiekt jest niepoprawnie skonstruowany lub pusty
                MessageBox.Show("Błąd! Przekazany obiekt jest nieprawidłowo skonstruowany");
            }
        }

        // Obsługuje zdarzenie podwójnego kliknięcia.
        private void LV_catalog_display_double_click (object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ListView parent = (ListView)sender;
                ListViewItem target = parent.HitTest(e.Location).Item;

                if (target.SubItems[1].Text.Equals("Folder"))
                {
                    // Otwieramy folder
                    LV_catalog_display_change_directory(target);
                    LV_catalog_display_item_selection.Clear();
                }
                else
                {
                    if (target.SubItems[1].Text.Equals("Katalog"))
                    {
                        // Otwieramy katalog, przydałoby się sprawdzić czy dalej istnieje...
                        database_connection_string_builder = new FbConnectionStringBuilder(target.Name);

                        if (target.Text.Equals("Katalog lokalny"))
                        {
                            catalog_folder_path_list.Add(@"Katalog lokalny\");
                            LV_catalog_display_current_catalog_alias = local_user.Alias;
                            LV_catalog_display_status = 1;
                            LV_catalog_display_folder_content_display(1);
                            catalog_folder_id_list.Add(1);
                            LV_catalog_display_visible_changed(this, new EventArgs());
                        }
                        else
                        {
                            catalog_folder_path_list.Add(target.Text + @"\");
                            LV_catalog_display_current_catalog_alias = (string)target.Tag;
                            LV_catalog_display_status = 2;
                            LV_catalog_display_folder_content_display(1);
                            catalog_folder_id_list.Add(1);
                            LV_catalog_display_visible_changed(this, new EventArgs());
                        }
                    }
                    else
                    {
                        if (LV_catalog_display_status == 1)
                        {
                            // Otwieramy plik
                            string target_location = database_virtual_filepath_get(int.Parse(target.Name), target.ToolTipText);
                            if (target_location != string.Empty)
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(target_location);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Obsługuje guzik do cofania się pomiędzy folderami.
        private void BT_previous_click (object sender, EventArgs e)
        {
            if (catalog_folder_id_list.Count == 1)
            {
                LV_catalog_display_status = 0;
                TB_catalog_path_current.Text = "";
                catalog_folder_id_list = new List<int>();
                catalog_folder_path_list = new List<string>();
                LV_catalog_display_visible_changed(this, new EventArgs());
            }
            if (catalog_folder_id_list.Count > 1)
            {
                catalog_folder_id_list.Remove(catalog_folder_id_list.Last());
                catalog_folder_path_list.Remove(catalog_folder_path_list.Last());
                TB_catalog_path_current.Text = string.Empty;
                for (int i = 0; i < catalog_folder_path_list.Count; i++)
                {
                    TB_catalog_path_current.Text += catalog_folder_path_list[i];
                }

                if (catalog_folder_path_list.Count == 1)
                {
                    LV_catalog_display_status = 0;
                }
                if (catalog_folder_path_list.Count == 2 && catalog_folder_path_list[1].Equals(@"Katalog lokalny\"))
                {
                    LV_catalog_display_status = 1;
                }
                if (catalog_folder_path_list.Count == 2 && !catalog_folder_path_list[1].Equals(@"Katalog lokalny\"))
                {
                    LV_catalog_display_status = 2;
                }
                LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());
            }
        }

        // Obsługuje zmianę długości TextBox'a do wyświetlania ścieżki w przeglądanym katalogu
        private void Catalog_page_top_table_layout_SizeChanged(object sender, EventArgs e)
        {
            int length_new = 0;
            TableLayoutPanel caller = (TableLayoutPanel)sender;
            length_new = caller.Size.Width;

            TB_catalog_path_current.Width = length_new - 2 * 72;
        }

        // Obsługa zdarzenia powrotu z wyboru opcji specjalnych - używana przez wszystkie okna specjalne, głównie zajmują się dodawaniem
        // wyniku do bazy danych w odpowiadający dla okna zwracającego sposób.
        void Special_function_window_OnDataAvalible(object sender, EventArgs e)
        {
            
            int operation_index = 0, counter = 0, working_directory = 0, iter_count;

            // Dane zwracane z części Janka:
            List<Tuple<int, string>> audio_sorter_folders_to_add;
            List<Tuple<int, string>> audio_sorter_files_to_add;

            // Dane zwracane z części Karola:
            List<Tuple<int, string, string>> extractor_texts_to_add;

            // Dane zwracane z części Kuby:
            List<Tuple<int, string>> image_processor_folders_to_add;
            List<Tuple<int, string>> image_processor_files_to_add;
            List<Tuple<int, string>> searcher_folders_to_add;
            List<Tuple<int, string>> searcher_files_to_add;


            List<Tuple<int, int, string, string, string>> all_files_selected = new List<Tuple<int, int, string, string, string>>();

            var folders_in_selection = LV_catalog_display_item_selection.Where(x => x.SubItems[1].Text.Equals("Folder"));
            var files_in_selection = LV_catalog_display_item_selection.Where(x => !x.SubItems[1].Text.Equals("Folder"));
            working_directory = special_option_selector.working_directory;
            for (int i = 0; i < folders_in_selection.Count(); i++)
            {
                all_files_selected.AddRange(database_virtual_folder_get_all_files(int.Parse(folders_in_selection.ElementAt(i).Name)));
                counter++;
            }
            for (int i = 0; i < files_in_selection.Count(); i++)
            {
                Tuple<int, int, string, string, string> tuple_to_add = 
                new Tuple<int, int, string, string, string>(
                    counter,
                    catalog_folder_id_list.Last(),
                    files_in_selection.ElementAt(i).Text,
                    files_in_selection.ElementAt(i).ToolTipText,
                    files_in_selection.ElementAt(i).SubItems[1].Text
                    );
                counter++;
                
                all_files_selected.Add(tuple_to_add);
            }

            operation_index = special_option_selector.return_index;
            switch (operation_index)
            {
                case 0:
                    extractor_texts_to_add = new List<Tuple<int, string, string>>();
                    extractor_texts_to_add = special_option_selector.extractor_texts_returned;

                    int texts_added = 0, texts_empty = 0;
                    for(int i = 0; i < extractor_texts_to_add.Count; i++)
                    {
                        if (!extractor_texts_to_add[i].Item3.Equals(string.Empty))
                        {
                            // Wyszukujemy w bazie pliki o indeksach podanych w tupli, wiemy że są w metadata_multimedia
                            texts_added++;
                            FbCommand database_record_manipulator = new FbCommand("UPDATE metadata_multimedia " +
                                                                              "SET EXTRACTED_TEXT = @Extracted_text " +
                                                                              "WHERE ID = @Id;"
                                                                              ,
                                                                              new FbConnection(database_connection_string_builder.ConnectionString));

                            string extracted_text = System.IO.File.ReadAllText(extractor_texts_to_add[i].Item3);
                            if(extracted_text.Equals("\r\n"))
                            {
                                texts_empty++;
                                database_record_manipulator.Parameters.AddWithValue("@Extracted_text", string.Empty);
                            }
                            else
                            {
                                database_record_manipulator.Parameters.AddWithValue("@Extracted_text", extracted_text);
                            }

                            database_record_manipulator.Parameters.AddWithValue("@Id", extractor_texts_to_add[i].Item1);

                            database_record_manipulator.Connection.Open();
                            database_record_manipulator.ExecuteNonQuery();
                            database_record_manipulator.Connection.Close();
                        }
                    }
                    if(texts_added == 0)
                    {
                        MessageBox.Show("Ekstrakcja tekstu z zaznaczonego zbioru nie zwróciła wyników (wszystkie ekstrakcje zostały anulowane).");
                    }
                    else
                    {
                        string report_line_1st = string.Empty;
                        string report_line_2nd = string.Empty;

                        if (texts_added >= 5) report_line_1st = "Wyekstrachowano " + texts_added.ToString() + " tekstów z zaznaczonego zbioru.\n";
                        else
                        {
                            if (texts_added > 1 && texts_added < 5) report_line_1st = "Wyekstrachowano " + texts_added.ToString() + " teksty z zaznaczonego zbioru.\n";
                            else if (texts_added == 1) report_line_1st = "Wyekstrachowano " + texts_added.ToString() + " tekst z zaznaczonego zbioru.\n";
                        }
                        
                        if (texts_empty >= 5) report_line_2nd = "Do bazy dodano " + (texts_added - texts_empty).ToString() + " z nich, jako że " + texts_empty.ToString() + " było pustych.";
                        else
                        {
                            if (texts_empty > 1 && texts_empty < 5) report_line_2nd = "Do bazy dodano " + (texts_added - texts_empty).ToString() + " z nich, jako że " + texts_empty.ToString() + " były puste.";
                            else
                            {
                                if (texts_empty == 1) report_line_2nd = "Do bazy dodano " + (texts_added - texts_empty).ToString() + " z nich, jako że " + texts_empty.ToString() + " był pusty.";
                                else if (texts_empty == 0) report_line_2nd = "Do bazy dodano wszystkie z nich.";
                            }
                        }
                        
                        MessageBox.Show(report_line_1st + report_line_2nd, "Raport z ekstrakcji");
                    }
                    break;
                case 1:
                    audio_sorter_folders_to_add = new List<Tuple<int, string>>();
                    audio_sorter_files_to_add = new List<Tuple<int, string>>();

                    audio_sorter_folders_to_add = special_option_selector.audio_sorter_folders_returned;
                    audio_sorter_files_to_add = special_option_selector.audio_sorter_files_returned;
                    
                    iter_count = audio_sorter_folders_to_add.Count;
                    for(int i = 0; i<iter_count; i++)
                    {
                        Tuple<int, string> processed_folder = audio_sorter_folders_to_add.ElementAt(i);
                        List<Tuple<int,string>> found_repeats = audio_sorter_folders_to_add.FindAll(x => x.Item2.Equals(processed_folder.Item2));
                        found_repeats.Remove(processed_folder);
                        foreach(var repeat in found_repeats)
                        {
                            List<Tuple<int, string>> files_to_correct = audio_sorter_files_to_add.FindAll(x => x.Item1.Equals(repeat.Item1));
                            foreach (var file_to_correct in files_to_correct)
                            {
                                Tuple<int, string> corrected_value = new Tuple<int, string>(processed_folder.Item1, file_to_correct.Item2);
                                audio_sorter_files_to_add[audio_sorter_files_to_add.FindIndex(x => x.Equals(file_to_correct))] = corrected_value;
                            }
                            audio_sorter_folders_to_add.Remove(repeat);
                            iter_count = audio_sorter_folders_to_add.Count;
                        }
                    }

                    audio_sorter_files_to_add = audio_sorter_files_to_add.Distinct().ToList();

                    foreach (Tuple<int, string> folder in audio_sorter_folders_to_add)
                    {
                        if (audio_sorter_files_to_add.FindAll(x => x.Item1.Equals(folder.Item1)).Count() > 0)
                        {
                            int new_id = 0;

                            database_virtual_folder_make(folder.Item2, catalog_folder_id_list.Last(), true, database_connection_string_builder.ConnectionString);
                            DataTable new_folder_ID_container = new DataTable();
                            FbDataAdapter new_folder_ID_grabber = new FbDataAdapter("SELECT ID " +
                                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                                    "WHERE NAME = @Name " + 
                                                                                    "AND DIR_ID = @Target_directory_id;"
                                                                                    ,
                                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

                            new_folder_ID_grabber.SelectCommand.Parameters.AddWithValue("@Name", folder.Item2);
                            new_folder_ID_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", working_directory);


                            new_folder_ID_grabber.Fill(new_folder_ID_container);
                            if(new_folder_ID_container.Rows.Count == 1)
                            {
                                new_id = (int)new_folder_ID_container.Rows[0].ItemArray[0];
                            }
                        
                            foreach (Tuple<int, string> file in audio_sorter_files_to_add.FindAll(x => x.Item1.Equals(folder.Item1)))
                            {
                                    if (new_id == 0) new_id = catalog_folder_id_list.Last();
                                    Tuple<int, int, string, string, string> found_file = all_files_selected.Find(x => (x.Item3 + x.Item5).Equals(file.Item2));
                                    if (found_file != null) database_virtual_file_copy(found_file.Item2, new_id, found_file.Item4, found_file.Item3, found_file.Item5);
                            }
                        }
                    }
                    LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());   
                    break;
                case 2:
                    image_processor_folders_to_add = new List<Tuple<int, string>>();
                    image_processor_files_to_add = new List<Tuple<int, string>>();

                    image_processor_folders_to_add = special_option_selector.image_processor_folders_returned;
                    image_processor_files_to_add = special_option_selector.image_processor_files_returned;

                    //Nie sprawdzamy czy nie ma repeatów w folderach, jako że są one tworzone inaczej niż w częśći Janka.

                    //Nie usuwamy także powtórzeń z listy plików - po prostu nie ma tam żadnych.

                    foreach (Tuple<int, string> folder in image_processor_folders_to_add)
                    {
                        if (image_processor_folders_to_add.FindAll(x => x.Item1.Equals(folder.Item1)).Count() > 0)
                        {
                            int new_id = 0;
                            string folder_name = folder.Item2;
                            //Sprawdzamy czy w bazie nie ma już folderu o naszej wynikowej nazwie:
                            DataTable folder_exists_container = new DataTable();
                            FbDataAdapter folder_exists_checker = new FbDataAdapter("SELECT ID " +
                                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                                    "WHERE NAME LIKE @Name " +
                                                                                    "AND DIR_ID = @Target_directory_id;"
                                                                                    ,
                                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

                            folder_exists_checker.SelectCommand.Parameters.AddWithValue("@Name", "%"+folder.Item2+"%");
                            folder_exists_checker.SelectCommand.Parameters.AddWithValue("@Target_directory_id", working_directory);

                            folder_exists_checker.Fill(folder_exists_container);

                            if (folder_exists_container.Rows.Count != 0)
                            {
                                // Znależliśmy powtórzenie folderu!
                                folder_name = folder.Item2 + " " + folder_exists_container.Rows.Count.ToString();
                            }

                            database_virtual_folder_make(folder_name, catalog_folder_id_list.Last(), true, database_connection_string_builder.ConnectionString);
                            DataTable new_folder_ID_container = new DataTable();
                            FbDataAdapter new_folder_ID_grabber = new FbDataAdapter("SELECT ID " +
                                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                                    "WHERE NAME = @Name " +
                                                                                    "AND DIR_ID = @Target_directory_id;"
                                                                                    ,
                                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

                            new_folder_ID_grabber.SelectCommand.Parameters.AddWithValue("@Name", folder_name);
                            new_folder_ID_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", working_directory);


                            new_folder_ID_grabber.Fill(new_folder_ID_container);
                            if (new_folder_ID_container.Rows.Count == 1)
                            {
                                new_id = (int)new_folder_ID_container.Rows[0].ItemArray[0];
                            }

                            foreach (Tuple<int, string> file in image_processor_files_to_add.FindAll(x => x.Item1.Equals(folder.Item1)))
                            {
                                if (new_id == 0) new_id = catalog_folder_id_list.Last();
                                Tuple<int, int, string, string, string> found_file = all_files_selected.Find(x => (x.Item3 + x.Item5).Equals(file.Item2));
                                if (found_file != null) database_virtual_file_copy(found_file.Item2, new_id, found_file.Item4, found_file.Item3, found_file.Item5);
                            }
                        }
                    }
                    LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());
                    break;
                case 3:
                    searcher_folders_to_add = new List<Tuple<int, string>>();
                    searcher_files_to_add = new List<Tuple<int, string>>();

                    searcher_folders_to_add = special_option_selector.searcher_folders_returned;
                    searcher_files_to_add = special_option_selector.searcher_files_returned;

                    MessageBox.Show("Przeszukanie wskazanego zbioru danych zakonczone!\n Liczba znalezionych plików: " + searcher_files_to_add.Count);

                    //Nie sprawdzamy czy nie ma repeatów w folderach, jako że są one tworzone inaczej niż w częśći Janka.

                    //Nie usuwamy także powtórzeń z listy plików - po prostu nie ma tam żadnych.

                    foreach (Tuple<int, string> folder in searcher_folders_to_add)
                    {
                        if (searcher_folders_to_add.FindAll(x => x.Item1.Equals(folder.Item1)).Count() > 0)
                        {
                            int new_id = 0;
                            string folder_name = folder.Item2;
                            //Sprawdzamy czy w bazie nie ma już folderu o naszej wynikowej nazwie:
                            DataTable folder_exists_container = new DataTable();
                            FbDataAdapter folder_exists_checker = new FbDataAdapter("SELECT ID " +
                                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                                    "WHERE NAME LIKE @Name " +
                                                                                    "AND DIR_ID = @Target_directory_id;"
                                                                                    ,
                                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

                            folder_exists_checker.SelectCommand.Parameters.AddWithValue("@Name", "%"+folder.Item2+"%");
                            folder_exists_checker.SelectCommand.Parameters.AddWithValue("@Target_directory_id", working_directory);

                            folder_exists_checker.Fill(folder_exists_container);

                            if(folder_exists_container.Rows.Count != 0)
                            {
                                // Znależliśmy powtórzenie folderu!
                                folder_name = folder.Item2 + " przeprowadzonego " + DateTime.Now;
                            }

                            database_virtual_folder_make(folder_name, catalog_folder_id_list.Last(), true, database_connection_string_builder.ConnectionString);
                            DataTable new_folder_ID_container = new DataTable();
                            FbDataAdapter new_folder_ID_grabber = new FbDataAdapter("SELECT ID " +
                                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                                    "WHERE NAME = @Name " +
                                                                                    "AND DIR_ID = @Target_directory_id;"
                                                                                    ,
                                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

                            new_folder_ID_grabber.SelectCommand.Parameters.AddWithValue("@Name", folder_name);
                            new_folder_ID_grabber.SelectCommand.Parameters.AddWithValue("@Target_directory_id", working_directory);


                            new_folder_ID_grabber.Fill(new_folder_ID_container);
                            if (new_folder_ID_container.Rows.Count == 1)
                            {
                                new_id = (int)new_folder_ID_container.Rows[0].ItemArray[0];
                            }

                            foreach (Tuple<int, string> file in searcher_files_to_add.FindAll(x => x.Item1.Equals(folder.Item1)))
                            {
                                if (new_id == 0) new_id = catalog_folder_id_list.Last();
                                Tuple<int, int, string, string, string> found_file = all_files_selected.Find(x => (x.Item3 + x.Item5).Equals(file.Item2));
                                if (found_file != null) database_virtual_file_copy(found_file.Item2, new_id, found_file.Item4, found_file.Item3, found_file.Item5);
                            }
                        }
                    }
                    LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());
                    break;
                case 4:
                    // Budowanie katalogu obiegowego.
                    if(local_user != null)
                    {
                        external_catalog_build(local_user.Alias);
                    }
                    break;
                default:
                    break;
            }
        }

        // Wejscie do podokna wyboru opcji specjalnych.
        private void BT_specials_Click(object sender, EventArgs e)
        {
            sent_items = new List<ListViewItem>();
            sent_items = LV_catalog_display_item_selection.ToList();

            // Wywołujemy nowego Forma w którym mamy wybór poszczególnych opcji specjalnych i przekazujemy do niego listę naszych wybranych obiektów.
            special_option_selector = new Special_function_window();
            special_option_selector.Owner = this;
            special_option_selector.mode = LV_catalog_display_status;
            // Z data to manipulate wyciagamy wszystkie pliki - potrzebujemy ich nazwy, rozszerzenia i ścieżki fizyczne.
            special_option_selector.DB_connection_string = database_connection_string_builder.ConnectionString;
            special_option_selector.database_tables = database_tables;
            special_option_selector.items_to_work_on = sent_items;
            special_option_selector.working_directory = catalog_folder_id_list.Last();
            special_option_selector.program_path = program_path;
            special_option_selector.extends = extends;
            special_option_selector.OnDataAvalible += new EventHandler(Special_function_window_OnDataAvalible);
            special_option_selector.Show();
            if (networking_lock == true)
            {
                var BT_external_catalog_create_grabber = special_option_selector.Controls.Find("BT_external_catalog_create", true);
                if(BT_external_catalog_create_grabber != null)
                {
                    if(BT_external_catalog_create_grabber.Count() != 0)
                    {
                        Control BT_external_catalog_create = BT_external_catalog_create_grabber[0];
                        if (BT_external_catalog_create.Enabled) BT_external_catalog_create.Enabled = false;
                    }
                }
            }
            else
            {
                if (LV_catalog_display_status != 2)
                {
                    var BT_external_catalog_create_grabber = special_option_selector.Controls.Find("BT_external_catalog_create", true);
                    if (BT_external_catalog_create_grabber != null)
                    {
                        if (BT_external_catalog_create_grabber.Count() != 0)
                        {
                            Control BT_external_catalog_create = BT_external_catalog_create_grabber[0];
                            if (!BT_external_catalog_create.Enabled) BT_external_catalog_create.Enabled = true;
                        }
                    }
                }
            }
            this.Hide();
        }

        #endregion
        
        #region Logika związana z sieciowością

        public bool RequestFileTransferPermission(
            DistributedNetworkFile distributedNetworkFile)
        {
            DialogResult dialogResult = MessageBox.Show(
                "User requests file: " +
                distributedNetworkFile.realFilePath + 
                //distributedNetworkFile.filePathInCatalogue +
                "\nAllow upload?",
                "File request",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if(dialogResult == DialogResult.Yes || 
                dialogResult == DialogResult.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DisplayMessageBoxFromAnotherThread(String msg)
        {
                MessageBox.Show(msg);
        }

        public void AddSuccessfulDownloadNameFromAnotherThread(String filename)
        {
            this.successful_downloads.Add(filename);
        }

        public void AddFailedDownloadNameFromAnotherThread(String filename)
        {
            this.failed_downloads.Add(filename);
        }

        public void IncrementSuccessfulDownloadCountFromAnotherThread()
        {
            this.successful_downloads_count++;
        }

        public void IncrementFailedDownloadCountFromAnotherThread()
        {
            this.failed_downloads_count++;
        }

        public void SpawnProgressWindowFromAnotherThread(String filename, int file_size)
        {
            progress_window = new DownloadProgressWindow(filename, file_size);
            progress_window.Show();
        }

        public void UpdateProgressWindowFromAnotherThread()
        {
            if(progress_window != null)
            {
                progress_window.DownloadProgressWindow_UpdateValue();
            }
        }

        public void KillProgressWindowFromAnotherThread()
        {
            if (progress_window != null)
            {
                progress_window.DownloadProgressWindow_Kill();
            }
        }

        public void CheckIfDoneFromAnotherThread()
        {
            if(failed_downloads_count + successful_downloads_count == total_to_download)
            {
                // Czas reportować o ściągnięciu:

                MessageBox.Show("Ściąganie plików zakonczone!\n" +
                                "Liczba wszystkich plików do ściągnięcia: " + total_to_download + "\n" +
                                "Liczba plików, które udało się ściągnąć: " + successful_downloads_count + "\n" +
                                "Liczba niepowodzeń: " + failed_downloads_count + "\n");

                if(failed_downloads_count > 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Czy chcesz wyświelić nazwy plików, dla których pobieranie nie powidło się?", 
                                                                "",
                                                                MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.OK || dialogResult == DialogResult.Yes)
                    {
                        String reporter_content = "";
                        int failures_to_report_count = failed_downloads_count, i = 0;

                        // Przechodzimy przez wszystkie porażki w ściągnięciu pliku.
                        while (failures_to_report_count != 0)
                        {
                            reporter_content += failed_downloads[i] + "\n";

                            // Raportujemy 16 niepowodzeń za jednym razem.
                            if (failures_to_report_count > 16)
                            {
                                if (i % 15 == 0 && i != 0)
                                {
                                    MessageBox.Show(reporter_content);
                                    reporter_content = "";
                                }
                            }

                            i++;
                            failures_to_report_count--;
                        }

                        // Wszystkie które zostały po wcześniejszej pętli tutaj wyświetlamy jako ostatnie.
                        if(!reporter_content.Equals(""))
                        {
                            MessageBox.Show(reporter_content);
                            reporter_content = "";
                        }
                    }
                }
            }
        }

        public string GrabExternalCatalogPathFromAnotherThread()
        {
            string filepath = "";

            filepath = ConfigManager.ReadString(ConfigManager.EXTERNAL_DATABASES_LOCATION) +
                       ConfigManager.ReadString(ConfigManager.USER_ALIAS).ToUpper() + "_CATALOG.FDB";

            return filepath;
        }

        public void TerminateDBConnectionsFromAnotherThread()
        {
            FbConnection.ClearAllPools();
        }

        public void AddAliasFromAnotherThread(String alias, String address)
        {
            string grabbed_aliases = ConfigManager.ReadString(ConfigManager.KNOWN_ALIASES);
            bool error_on_aliases_read = false, change_made = false;

            string[] alias_address_pairs = new string[0];
            string final_message = "";

            try
            {
                alias_address_pairs = grabbed_aliases.Split('|');
                string[] pair_split = new string[2] { "", "" };

                List<string> aliases_list = new List<string>();
                List<string> addresses_list = new List<string>();

                foreach (string pair in alias_address_pairs)
                {
                    try
                    {
                        if(!pair.Equals(""))
                        {
                            pair_split = pair.Split('_');
                            aliases_list.Add(pair_split[0]);
                            addresses_list.Add(pair_split[1]);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Błąd dzielenia pary alias-adres na składowe!\n" +
                                        "Czyszczę zawartość znanych aliasów!");
                        ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                        error_on_aliases_read = true;
                        break;
                    }
                }

                if (error_on_aliases_read == false)
                {
                    string alias_to_find = alias;
                    int pair_index = aliases_list.IndexOf(alias_to_find.ToUpper());
                    if (pair_index != -1)
                    {
                        // Znaleźliśmy alias w liście - jeżeli adresy się różnią, to zastąp teraźniejszym!
                        if (addresses_list[pair_index] != address)
                        {
                            addresses_list[pair_index] = address;
                            final_message = "Odświeżono adres IP użytkownika o aliasie " + aliases_list[pair_index];
                            change_made = true;
                        }
                    }
                    else
                    {
                        // Nie znaleźliśmy aliasu w liście - dodajemy kompletnie nowy
                        aliases_list.Add(alias);
                        addresses_list.Add(address);
                        change_made = true;
                        final_message = "Dodano użytkownika o aliasie " + alias;
                    }
                }

                if (change_made == true)
                {
                    // Zmienialiśmy wartości w aliasach - stąd musimy przegenerować ich string i zapisać go do konfiga
                    String substitution_string = "";

                    for (int i = 0; i < aliases_list.Count; i++)
                    {
                        substitution_string += aliases_list[i] + '_';
                        substitution_string += addresses_list[i] + '|';
                    }

                    substitution_string = substitution_string.TrimEnd('|');

                    ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, substitution_string);
                    MessageBox.Show(final_message + '.');
                }
            }
            catch
            {
                MessageBox.Show("Błąd dzielenia kontenera znanych aliasów na pary alias-adres!\n" +
                                "Czyszczę zawartość znanych aliasów!");
                ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                error_on_aliases_read = true;
            }
        }

        public void AttemptToFinalizeFromAnotherThread(String target_filename)
        {
            recieved_external_catalog_name = target_filename;

            //Catalog_download_finalizer_Tick(this, new EventArgs());

            catalog_download_finalizer = new Timer();
            catalog_download_finalizer.Interval = 1000;
            catalog_download_finalizer.Tick += Catalog_download_finalizer_Tick;

            catalog_download_finalizer.Start();
        }

        private void Catalog_download_finalizer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (used_alias.Equals("UNKNOWN"))
                {
                    // Nie mieliśmy użytkownika o takim aliasie - wydobywamy go z nowej nazwy katalogu i dodajemy do pliku konfiguracyjnego:

                    string[] alias_grabber = Path.GetFileName(recieved_external_catalog_name).Split('_');

                    used_alias = alias_grabber[0];

                    AddAliasFromAnotherThread(used_alias, used_IP_address_string);
                }
                if (new FileInfo(database_externals_path + "EXTERNAL_CATALOG.FDB").Exists)
                {
                    File.Move(database_externals_path + "EXTERNAL_CATALOG.FDB", recieved_external_catalog_name);
                    catalog_download_finalizer.Stop();
                    MessageBox.Show("Pobranie katalogu zakonczone sukcesem!");

                    LV_catalog_display_visible_changed(this, new EventArgs());
                }
            }
            catch
            {

            }
        }

        private void BT_define_user_Click(object sender, EventArgs e)
        {
            if(networking_lock == false)
            {
                // Pobraliśmy już działającego użytkownika. Pytamy użytkownika czy chce zastąpić starego użytkownika.
                DialogResult dialogResult = MessageBox.Show("Użytkownik jest już zdefiniowany\nCzy mam nadpisać go nowymi danymi?", "Próba nadpisania użytkownika", MessageBoxButtons.YesNo); ;
                if (dialogResult == DialogResult.Yes)
                {
                    ConfigManager.WriteValue(ConfigManager.USER_ALIAS, "");
                    ConfigManager.WriteValue(ConfigManager.TCP_COMM_IP_ADDRESS, "");
                    ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                    distributedNetwork.Shutdown();
                    DetermineLocalUser(false);
                }
                else if (dialogResult == DialogResult.No)
                {
                    //Nie robimy nic.
                }
            }
            else
            {
                DetermineLocalUser(false);
            }
        }

        private void BT_display_known_aliases_Click(object sender, EventArgs e)
        {
            string grabbed_aliases = ConfigManager.ReadString(ConfigManager.KNOWN_ALIASES);

            if(grabbed_aliases.Equals(""))
            {
                MessageBox.Show("Nie zostały jeszcze zapamiętane żadne aliasy");
            }
            else
            {
                bool error_on_aliases_read = false;

                string[] alias_address_pairs = new string[0];

                try
                {
                    alias_address_pairs = grabbed_aliases.Split('|');
                    string[] pair_split = new string[2] { "", "" };

                    List<string> aliases_list = new List<string>();
                    List<string> addresses_list = new List<string>();

                    foreach (string pair in alias_address_pairs)
                    {
                        try
                        {
                            if (!pair.Equals(""))
                            {
                                pair_split = pair.Split('_');
                                aliases_list.Add(pair_split[0]);
                                addresses_list.Add(pair_split[1]);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Błąd dzielenia pary alias-adres na składowe!\n" +
                                            "Czyszczę zawartość znanych aliasów!");
                            ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                            error_on_aliases_read = true;
                            break;
                        }
                    }

                    if (error_on_aliases_read == false)
                    {
                        int aliases_count = aliases_list.Count, i = 0;
                        string reporter_content = "";

                        // Przechodzimy przez wszystkie porażki w ściągnięciu pliku.
                        while (aliases_count != 0)
                        {
                            reporter_content += aliases_list[i] + " ma adres " + addresses_list[i] + '\n';

                            // Raportujemy 16 niepowodzeń za jednym razem.
                            if (aliases_count > 16)
                            {
                                if (i % 15 == 0 && i != 0)
                                {
                                    MessageBox.Show(reporter_content);
                                    reporter_content = "";
                                }
                            }

                            i++;
                            aliases_count--;
                        }

                        // Wszystkie które zostały po wcześniejszej pętli tutaj wyświetlamy jako ostatnie.
                        if (!reporter_content.Equals(""))
                        {
                            MessageBox.Show(reporter_content);
                            reporter_content = "";
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Błąd dzielenia kontenera znanych aliasów na pary alias-adres!\n" +
                                    "Czyszczę zawartość znanych aliasów!");
                    ConfigManager.WriteValue(ConfigManager.KNOWN_ALIASES, "");
                    error_on_aliases_read = true;
                }
            }
        }

        #endregion
    }
}
