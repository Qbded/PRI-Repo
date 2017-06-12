﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using TikaOnDotNet;
using TikaOnDotNet.TextExtraction;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Xml.Linq;
using FirebirdSql.Data.FirebirdClient;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Main_form : Form
    {
        // Tutaj wylądują po procesie ekstrakcji metadane.
        public List<string[]> metadata { get; set; }

        /* Tutaj wylądują po przeszukaniu katalogu wyswietlane podfoldery i pliki w nich zawarte
         * Struktura:
         * 1. directories_grabbed:
         * int Item1 - przechowuje ID folderu z tabeli virtual_folder.
         * string Item2 - przechowuje nazwę folderu wyciągniętą z tabeli virtual_folder.
         * 2. files_grabbed:
         * int Item1 - przechowuje ID pliku wyciągniętego z którejś z tabel zawierających metadane.
         * string Item2 - przechowuje nazwę tabeli, z której pochodzi dany plik.
         * string Item3 - przechowuje nazwę pliku.
         * string Item4 - przechowuje rozszerzenie pliku.
         * System.Datetime Item5 - przechowuje czas ostatniego zapisu dla danego pliku.
         * System.Datetime Item6 - przechowuje datę ostatniego katalogowanie dla pliku (czyt. kiedy został ostatnim razem zmieniony podczas katalogowania).
         * int Item7 - przechowuje rozmiar pliku.
        */
        public List<Tuple<int, string>> directories_grabbed { get; set; } 
        public List<Tuple<int, string, string, string, System.DateTime, System.DateTime, int>> files_grabbed { get; set; }

        // Tutaj przechowujemy connection string do bazy danych.
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

        // Wcześniejsze zmienne globalne jeszcze z kodu Janka, jedynie dodałem w extends kropki przed nazwami rozszerzeń.
        public string[] extends = { ".txt", ".csv", ".doc", ".docx", ".odt", ".ods", ".odp", ".xls", ".xlsx", ".pdf", ".ppt", ".pptx", ".pps", ".fb2", ".htm", ".html", ".tsv", ".xml", ".jpg", ".jpeg", ".tiff", ".bmp", ".mp4", ".avi", ".mp3", ".wav"};
        
        // Z tych zmiennych nie korzystałem nigdy...
        private string regex = @"((u|s|wy){0,1}(tw((órz)|(orzyć)|(orzenie))))(\s)(etykiet(y|ę){0,1})(\s)((<[a-ząęśćżźół\#]+\$>)+)(\s)((dla){0,1})(\s)((każde){0,1}((go)|j){0,1})(\s){0,1}((grupy){0,1})(\s){0,1}((((plik)|(obiekt))(u|ów))|(lokacji))(\s)(((\*{0,1})\.{0,1}[a-z0-9]{3,4}\s{0,1})+)";
        private string regexCreate = @"(create)(\s)(label)(\s)((<[a-ząęśćżźół\#]+\$>)+)(\s)((for){0,1})(\s)((every)|(all)|(each){0,1})(\s){0,1}((group of files)|(files' group){0,1})(\s)(((\*{0,1})\.{0,1}[a-z0-9]{3,4}\s{0,1})+)";
        private string[] exampleCommands = { "utwórz etykietę <x$> dla pliku *.mp3", "utwórz etykietę <x$> dla obiektu *.mp3", "utwórz etykietę <x$> dla lokacji *.mp3", "utwórz etykiety <x$><y$> dla grupy plików *.mp3 *wav", "utwórz etykiety <x$><y$> dla plików *.mp3 *wav", "utwórz etykiety <x$><y$> plików *.mp3 *wav", "utwórz etykiety <x$><y$> dla obiektów *.mp3 *wav", "utwórz etykiety <x$><y$> dla lokacji *.mp3 *wav" };
        private string[] exampleCommandsCreate = { "create label <x$> for every file *.mp3", "create label <x$> for each file *.mp3", "create label <x$> for all file *.mp3", "create label <x$> for file *.mp3", "create label <x$><y$> for files' group *.mp3 *wav", "create label <x$><y$> for group of file *.mp3 *wav", };
        int randResult = 0;
        int randResultCreate = 0;
        List<string> excludedMetadata;

        // Ścieżki do poszczególnych plików niezbędnych do działania programu:
        private string program_path = null;
        private string output_path = null;
        private string xml_path = null;
        private string txt_path = null;
        private string database_path = null;

        // Zmienne stanu programu:
        private bool database_validated_successfully = false;

        // Zmienne nawigatora katalogu:
        private List<int> catalog_folder_id_list = new List<int>();
        private List<string> catalog_folder_path_list = new List<string>();
        private ListViewItem[] LV_catalog_display_cache;
        private List<ListViewItem> LV_catalog_display_item_selection = new List<ListViewItem>();
        private List<ListViewItem> LV_catalog_display_data_to_manipulate = new List<ListViewItem>();
        private int LV_catalog_display_data_to_manipulate_orgin_directory_id = 0;
        private bool cache_refresh = true, copy = false, cut = false;

        // Zmienne przesyłacza do opcji specialnych:
        private List<Tuple<string,string>> files_to_work_on;

        /*   Oduzależnienie programu od statycznych stringów - pobieranie lokacji programu
         *    
         *    Na początku tworzymy grabber dający nam obiekt DirectoryInfo zawiarajacy lokacje fizyczna programu na dysku twardym.
         *    Potem sprawdzamy czy jest w katalogu Debug/Release (co swiadczy o tym ze dalej jest w konfiguracji testowej, a nie roboczej),
         *    jezeli jest to musimy pobrać lokację dwóch katalogów w górę, jeżeli nie - tylko jeden
         *    
         *    Strutura programu na dzien dzisiejszy wygląda tak (i będzie tak wyglądała aż do zakonczenia prac z VS):
         *    bin\Release - zawiera .exe'c zbudowany przez VS w wersji release i biblioteki .dll programu
         *                *    bin\Debug - zawiera .exe'c zbudowany przez VS w wersji debug i biblioteki .dll programu
         *    db\ - tutaj żyje nasz katalog, w pliku catalog.fdb
         *    
         *    Wynikowo struktura programu ma wygladac w ten desen:
         *    bin\ - tutaj żyja nasze .dll'ki i .exe'c aplikacji
         *    db\ - tutaj żyje nasz katalog, w pliku catalog.fdb
         *    output\ - tutaj żyją wyniki tego co zwraca Karol swoją obrabiarką do pl. multimedialnych.
         *    
         *    Tutaj zakladam ze program jest schowany glebiej przez Visual Studio, stad potrzebne są nam dodatkowe skoki. Przepisanie tej procedury na czysto
         *    nie jest problemem.
         *    
         *    UWAGA: Moze wygenerowac IOException jeżeli coś innego czyta nasze podfoldery!
        */
        private void DetermineFilepaths()
        {
            DirectoryInfo directory_grabber = new DirectoryInfo(Application.StartupPath);
            string target_directory;

            if (directory_grabber.Name == "Release" || directory_grabber.Name == "Debug")
            {
                MessageBox.Show("Program w wersji roboczej!");
                target_directory = directory_grabber.Parent.Parent.FullName.ToString();
            }
            else
            {
                MessageBox.Show("Program w wersji ostatecznej!");
                target_directory = directory_grabber.Parent.FullName.ToString();
            }

            this.program_path = target_directory;
            this.output_path = target_directory + @"\output\";
            this.xml_path = target_directory + @"\metadata.xml"; // XML'ka Janka,
            this.txt_path = target_directory + @"\$$$.txt"; // Plik testowy Janka,
            this.database_path = target_directory + @"\db\catalog.fdb"; // Lokacja katalogu tworzonego przez program
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

            // Kolumny tabeli virtual_folder:
            database_columns.Add(new Tuple<int, string, string>(0, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(0, @"NAME", @"VARCHAR(128) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(0, @"DIR_ID", @"INT")); // null dozwolony tylko dla katalogu nadrzędnego.
            database_columns.Add(new Tuple<int, string, string>(0, @"MODIFIABLE", @"BOOLEAN NOT NULL"));

            // Kolumny tabeli metadata_text:
            database_columns.Add(new Tuple<int, string, string>(1, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(1, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(1, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(1, @"NAME", @"VARCHAR(128) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"EXTENSION", @"VARCHAR(5) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"PATH", @"VARCHAR(512) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"SIZE", @"INT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"CONTENT_TYPE", @"VARCHAR(80)"));
            database_columns.Add(new Tuple<int, string, string>(1, @"CONTENT_ENCODING", @"VARCHAR(64)"));

            // Kolumny tabeli metadata_document
            database_columns.Add(new Tuple<int, string, string>(2, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(2, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(2, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(2, @"NAME", @"VARCHAR(128) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"EXTENSION", @"VARCHAR(5) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"PATH", @"VARCHAR(512) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"SIZE", @"INT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(2, @"CONTENT_TYPE",  @"VARCHAR(80)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"CREATION_TIME", @"VARCHAR(32)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"LAST_WRITE_TIME", @"VARCHAR(32)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"LANGUAGE", @"VARCHAR(32)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"TITLE", @"VARCHAR(256)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"SUBJECT", @"VARCHAR(256)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"DESCRIPTION", @"VARCHAR(512)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"KEYWORDS", @"VARCHAR(128)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"COMMENTS", @"VARCHAR(512)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"PUBLISHER", @"VARCHAR(256)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"COMPANY", @"VARCHAR(256)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"AUTHOR", @"VARCHAR(64)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"CREATOR", @"VARCHAR(64)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"LAST_AUTHOR", @"VARCHAR(64)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"PAGE_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"TABLE_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"OBJECT_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"IMAGE_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"WORD_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"CHARACTER_COUNT", @"INT"));
            database_columns.Add(new Tuple<int, string, string>(2, @"APPLICATION_NAME", @"VARCHAR(64)"));
            database_columns.Add(new Tuple<int, string, string>(2, @"APPLICATION_VERSION", @"VARCHAR(64)"));

            //Kolumny tabeli metadata_complex
            database_columns.Add(new Tuple<int, string, string>(3, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(3, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(3, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(3, @"NAME", @"VARCHAR(128) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"EXTENSION", @"VARCHAR(5) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"PATH", @"VARCHAR(512) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"SIZE", @"INT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(3, @"CONTENT_TYPE", @"VARCHAR(80)"));
            database_columns.Add(new Tuple<int, string, string>(3, @"CONTENT_ENCODING", @"VARCHAR(64)"));
            database_columns.Add(new Tuple<int, string, string>(3, @"TITLE", @"VARCHAR(128)"));
            database_columns.Add(new Tuple<int, string, string>(3, @"VM_COUNT", @"INT"));

            for (int i = 0; i < 64; i++)
            {
                database_columns.Add(new Tuple<int, string, string>(3, @"VM_" + i.ToString() + @"_NAME", @"VARCHAR(64)"));
                database_columns.Add(new Tuple<int, string, string>(3, @"VM_" + i.ToString() + @"_DATA", @"VARCHAR(128)"));
            }

            //Kolumny tabeli metadata_image
            database_columns.Add(new Tuple<int, string, string>(4, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(4, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(4, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(4, @"NAME", @"VARCHAR(128) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"EXTENSION", @"VARCHAR(5) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"PATH", @"VARCHAR(512) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"SIZE", @"INT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            //Kolumny tabeli metadata_multimedia
            database_columns.Add(new Tuple<int, string, string>(5, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(5, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(5, @"CATALOGING_DATE", @"TIMESTAMP DEFAULT CURRENT_TIME"));
            database_columns.Add(new Tuple<int, string, string>(5, @"NAME", @"VARCHAR(128) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"EXTENSION", @"VARCHAR(5) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"PATH", @"VARCHAR(512) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"SIZE", @"INT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(5, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
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
            database_connection_string_builder.DataSource = "localhost"; // Identyfikator sieciowy - do kogo sie laczymy. Moze byc postaci adres IP+Port.
            database_connection_string_builder.UserID = "SYSDBA"; // Defaultowy uzytkownik z najwyzszymi uprawnieniami do systemu bazodanowego, tworzony podczas instalacji
            database_connection_string_builder.Password = "5o9orjoh"; // Haslo nadane podczas instalacji Firebird'a użytkownikowi SYSDBA, uzupełnić w zależności u kogo jest jakie
            database_connection_string_builder.Database = database_path;
            database_connection_string_builder.ServerType = FbServerType.Default;
        }

    // Obsługa logiki wymaganej przez WPF i resizing okna.
        public Main_form()
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            InitializeComponent();

            DetermineFilepaths();
            DetermineDatabaseConstruction();
            LoadCreationScripts();
            PrepareConnectionString();

            this.txtCommand.LostFocus += TxtCommand_LostFocus;
            this.chkMetadata.LostFocus += ChkMetadata_LostFocus;
            this.Load += Form1_Load;
            this.KeyDown += Form1_KeyDown;

            excludedMetadata = new List<string>();
            metadata = new List<string[]>();
            directories_grabbed = new List<Tuple<int, string>>();
            files_grabbed = new List<Tuple<int, string, string, string, System.DateTime, System.DateTime, int>>();
        }

        /* Zmiana rozmiaru okna głównego
         * 
        *  Zakładam minimalny rozmiar okna na poziomie tego, co zrobił na początku Janek
        *  
        *  Gdy zmieniamy rozmiar okna głównego niektóre elementy powinny zmienić swój rozmiar lub pozycję, są to:
        *  - tabpage tabPage1 (Kryteria katalogowania) - rozmiar
        *  - groupbox groupBox1 (Użyj polecenie) - rozmiar
        *       - textbox txtCommand (za Polecenie:) - rozmiar
        *       - button BT_text_database (Test bazy) - pozycja
        *       - button BT_extract_metadata (Kataloguj) - pozycja
        *  - groupbox z metadanymi - rozmiar
        *       - checkedlistbox chkMetadata - rozmiar 
        *  
        *  I tyle, reszta w kodzie.
        *  
        */
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Ustawiamy rozdzielczosc minimalna na poziomie tego, co zrobił Janek
            this.MinimumSize = new Size(471, 465);

            // A jezeli jest wieksza, to reskalujemy
            if (this.Size.Width >= 471 && this.Size.Height >= 465)
            {
                // Najpierw TabPage tabPage1:
                tabPage1.Width = this.Size.Width - 24;
                tabPage1.Height = this.Size.Height - 71;

                // Przechodzimy do jego subkomponentów:
                    // Groupbox użyj opcje:
                    groupBox1.Width = tabPage1.Width - 9;
                    // Dla niego zostawiamy wysokość stałą, jako ze pod nim jeszcze cos jest.
                    // Przechodzimy do jego subkomponentów (nie wszystkie wymagaja skalowania)
                        // Textbox txtCommand:
                        txtCommand.Width = tabPage1.Width - 126;

                        // Button BT_test_database:
                        BT_test_database.Location = new Point(groupBox1.Width - 135, 99);
                        // Tez zostawiamy wysokość stałą, jego rodzic nie zmienia swojej wysokosci.

                        // Button BT_extract_metadata:
                        BT_extract_metadata.Location = new Point(groupBox1.Width - 135, 138);
                        // Tez zostawiamy wysokość stałą, jego rodzic nie zmienia swojej wysokosci.
                    // Groupbox metadane:
                    groupBox2.Width = tabPage1.Width - 13;
                    groupBox2.Height = tabPage1.Height - groupBox1.Height - 15;
                        // CheckedListBox chkMetadata:
                            chkMetadata.Width = groupBox2.Width - 12;
                            chkMetadata.Height = groupBox2.Height - 51;
                // I to na tyle, jak bedzie przybywalo komponentów trzeba bedzie je tutaj dodawać
                // Liczby do odejmowania obliczam biorąc za podstawę okno w rozmiarze minimalnym,
                // odejmuje od rozmiaru kontenera macierzystego albo rozmiar jego subkomponentu
                // (jezeli chcemy go skalowac) lub jego lokację (jeżeli chcemy zmienić pozycję)
            }
        }

        private void TB_catalog_path_current_resizer(object sender, EventArgs e)
        {
            TableLayoutPanel parent = (TableLayoutPanel)sender;
            TextBox target = (TextBox)parent.Controls[1];
            target.Width = parent.Size.Width - 150;
        }



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
            if (File.Exists(database_path))
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

                if (validation_result.All(x => x.Equals(2)) == true) database_validated_successfully = true;

                for (int i = 0; i < validation_return_string_container.Length; i++)
                {
                    validation_return_string += validation_return_string_container[i];
                }
                MessageBox.Show(validation_return_string);
            }
            else
            {
                MessageBox.Show("Nie znalazłem bazy, tworze nową w katalogu:\n" + database_path);

                if (!Directory.Exists(program_path + @"\db"))
                {
                    //DEBUG MessageBox.Show("Tworze katalog db!");
                    Directory.CreateDirectory(program_path + @"\db");
                }

                FbConnection.CreateDatabase(database_connection_string_builder.ConnectionString);

                database_build(database_connection_string_builder.ConnectionString);

                MessageBox.Show("Zakonczono budowę bazy!");
                database_validated_successfully = true;
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

                    Metadata_extractor metadata_extractor_window = new Metadata_extractor();
                    metadata_extractor_window.Owner = this;
                    metadata_extractor_window.extends = extends;
                    metadata_extractor_window.target_directory = selected_directory;
                    metadata_extractor_window.Show();
                }
            }
        }

        // Stary kawałek logiki do wyświetlania zawatrości zmiennej metadata, teraz już nie jest potrzebny
        private void chkExcludeMetadata_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkExcludeMetadata.Checked)
            {
                this.chkMetadata.Enabled = true;
                for (int i = 0; i < metadata.Count(); i++)
                {
                    string display = String.Empty;
                    for (int j = 0; j < metadata[i].Length; j++) display += " " + metadata[i].ElementAt(j) + " ";
                    this.chkMetadata.Items.Add(display);
                }

                //DEBUG - zapisujemy wyniki katalogowania do pliku $$$.txt
                FileInfo txt_dump = new FileInfo(txt_path);
                if (txt_dump.Exists)
                {
                    StreamWriter txt_dumper = new StreamWriter(txt_path);

                    for (int i = 0; i < metadata.Count(); i++)
                    {
                        string display = String.Empty;
                        for (int j = 0; j < metadata[i].Length; j++) display += " " + metadata[i].ElementAt(j) + " ";
                        txt_dumper.WriteLine(display);
                    }
                    txt_dumper.Close();
                }

                //DEBUG - wrzucamy wyniki katalogowania do bazy danych.
                for (int i = 0; i < metadata.Count; i++)
                {
                        string destination = String.Empty;
                        string datafields = String.Empty;
                        string values = String.Empty;
                        string[] values_passed = new string[0];
                        int parent_directory_ID = 0;

                        if (metadata[i].Length != 0)
                        {
                            destination = metadata[i].ElementAt(0);
                            var datatable_index = database_tables.Find(x => x.Item2.Equals(destination)).Item1;

                            var columns_to_populate = database_columns.FindAll(x => x.Item1 == datatable_index);
                            columns_to_populate.RemoveAll(x => x.Item2.Equals("CATALOGING_DATE")); // wyrzucamy z automatycznego wypelniania kolumne CATALOGING_DATE
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

                            for (int j = 1; j < metadata[i].Count(); j++)
                            {
                                if (!(metadata[i].ElementAt(j).Equals("NULL"))) add_data.Parameters.AddWithValue(values_passed[j], metadata[i].ElementAt(j));
                                else add_data.Parameters.AddWithValue(values_passed[j], null);
                            }

                            if (metadata[i].Count() < values_passed.Length)
                            {
                                for (int j = metadata[i].Count(); j < values_passed.Length; j++) add_data.Parameters.AddWithValue(values_passed[j], null);
                            }

                            add_data.Connection.Open();
                            add_data.ExecuteNonQuery();
                            add_data.Connection.Close();
                        }
                    }
            }
            else
            {
                this.chkMetadata.Enabled = false;
                this.chkMetadata.Items.Clear();
            }
        }

    // Operacje na bazie danych

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
        private void database_virtual_folder_make(string name, int parent_index, bool modifiable_flag)
        {
            var columns_to_populate = database_columns.FindAll(x => x.Item1 == 0); // szukamy struktury tabeli virtual_folder
            string datafields = string.Empty;
            string values = string.Empty;
            string folder_creation_script = string.Empty;
            
            string[] values_passed = new string[columns_to_populate.Count - 1];

            FbConnection database_connection = new FbConnection(database_connection_string_builder.ConnectionString);
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
            folder_creator.Parameters.AddWithValue(values_passed[2], modifiable_flag);

            database_connection.Open();
            folder_creator.ExecuteNonQuery();
            database_connection.Close();
        }

        // Budowanie odpowiednich kolumn bazy danych
        private void database_build(string database_connection_string)
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

            database_virtual_folder_make("root", -1, false);
            database_virtual_folder_make("Pliki tekstowe", 1, false);
            database_virtual_folder_make("Dokumenty", 1, false);
            database_virtual_folder_make("Pliki .htm i .html", 1, false);
            database_virtual_folder_make("Obrazki i zdjęcia", 1, false);
            database_virtual_folder_make("Pliki multimedialne", 1, false);
        }

        // Wstawianie pojedynczej tabeli do bazy danych - jeżeli wykryjemy jej brak!
        private void database_table_insert(string database_connection_string, int missing_table_index)
        {
            FbConnection database_connection = new FbConnection(database_connection_string);
            string script_used = String.Empty;

            script_used = creation_scripts[missing_table_index];
            database_connection.Open();
            
            FbCommand table_adder = new FbCommand(script_used, database_connection);
            table_adder.ExecuteNonQuery();
            
            database_connection.Close();
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


    // Zakładka Katalog i jej logika:

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
            FbDataAdapter database_grab_directory_subdirectories = new FbDataAdapter("SELECT ID,NAME " +
                                                                                     "FROM virtual_folder " +
                                                                                     "WHERE DIR_ID = " + id_to_display + ";"
                                                                                     ,
                                                                                     new FbConnection(database_connection_string_builder.ConnectionString));
            // Zeby zrobic go uniwersalnie, trzeba przekazać jako parametr DIR_ID i DEPTH, DIR_ID bedziemy mieli bezposrednio z folders_grabbed.

            database_grab_directory_subdirectories.Fill(database_folder_subdirectories);

            for (int i = 0; i < database_folder_subdirectories.Rows.Count; i++)
            {
                directories_grabbed.Add(new Tuple<int, string>(
                                       (int)database_folder_subdirectories.Rows[i].ItemArray[0],
                                       (string)database_folder_subdirectories.Rows[i].ItemArray[1])); 
            }

            for (int i = 1; i < database_tables.Count; i++) // bierzemy wszystkie tabele oprócz virtual_folder
            {
                FbDataAdapter database_grab_directory_content = new FbDataAdapter("SELECT ID,NAME,EXTENSION,FS_LAST_WRITE_TIME,CATALOGING_DATE,SIZE " +
                                                                "FROM " + database_tables[i].Item2 + " " +
                                                                "WHERE DIR_ID = " + id_to_display + ";"
                                                                ,
                                                                new FbConnection(database_connection_string_builder.ConnectionString));

                database_folder_content.Clear();
                database_grab_directory_content.Fill(database_folder_content);

                for (int j = 0; j < database_folder_content.Rows.Count; j++)
                {
                    files_grabbed.Add(new Tuple<int, string, string, string, System.DateTime, System.DateTime, int>(
                                     (int)database_folder_content.Rows[j].ItemArray[0],
                                     database_tables[i].Item2,               
                                     (string)database_folder_content.Rows[j].ItemArray[1],
                                     (string)database_folder_content.Rows[j].ItemArray[2],
                                     (System.DateTime)database_folder_content.Rows[j].ItemArray[3],
                                     (System.DateTime)database_folder_content.Rows[j].ItemArray[4],
                                     (int)database_folder_content.Rows[j].ItemArray[5]));
                }
            }
            LV_catalog_display.VirtualListSize = directories_grabbed.Count + files_grabbed.Count;
            if(LV_catalog_display.VirtualListSize >=1) LV_catalog_display.RedrawItems(0, LV_catalog_display.VirtualListSize-1, false);
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
            }
            else
            {
                if (LV_catalog_display.Enabled == false)
                {
                    LV_catalog_display_folder_content_display(1);
                    catalog_folder_id_list.Add(1);
                    catalog_folder_path_list.Add(@"\");
                    TB_catalog_path_current.Text = catalog_folder_path_list.Last();
                    LV_catalog_display.Enabled = true;
                }
                else
                {
                    // Wyczyść wcześniejszą zawartość i zregeneruj
                    TB_catalog_path_current.Text = string.Empty;
                    if (LV_catalog_display_cache != null) Array.Clear(LV_catalog_display_cache, 0, LV_catalog_display_cache.Length);
                    catalog_folder_id_list.Clear();
                    catalog_folder_path_list.Clear();
                    LV_catalog_display_folder_content_display(1);
                    catalog_folder_id_list.Add(1);
                    catalog_folder_path_list.Add(@"\");
                    TB_catalog_path_current.Text = catalog_folder_path_list.Last();
                    LV_catalog_display.Enabled = true;
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
                    item_to_add.SubItems.Add("---");
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
        private void context_menu_folder_make(int parent_dir_id, string new_folder_name)
        {
            bool folder_exists_already = false;
            DataTable database_folder_already_exists_container = new DataTable();
            FbDataAdapter database_folder_already_exists_verifier = new FbDataAdapter("SELECT NAME,DIR_ID " +
                                                                                      "FROM " + database_tables[0].Item2 + " " +
                                                                                      "WHERE DIR_ID = " + parent_dir_id.ToString() + ";"
                                                                                      ,
                                                                                      new FbConnection(database_connection_string_builder.ConnectionString));

            database_folder_already_exists_container.Clear();
            database_folder_already_exists_verifier.Fill(database_folder_already_exists_container);
            for (int i = 0; i < database_folder_already_exists_container.Rows.Count; i++)
            {
                if(database_folder_already_exists_container.Rows[i].ItemArray[0].Equals(new_folder_name)) 
                {
                    MessageBox.Show(new_folder_name + " jest już w tym folderze!");
                    folder_exists_already = true;
                    break;
                }
            }

            if(!folder_exists_already)
            {
                // Nie znaleźliśmy duplikatu o tym samym imieniu i o tym samym rodzicu
                // W przypadku nowego folderu z listy kontekstowej wywołujemy po prostu z sztywnym stringiem "Nowy folder"
                database_virtual_folder_make(new_folder_name, parent_dir_id, true);
                // Zlecamy programowi ponowne wyświetlenie folderu - jako że przybył nam nowy folder.
                cache_refresh = true;
                LV_catalog_display_folder_content_display(parent_dir_id);
            }
        }

        // Obsługa kliknięcia w ListView gdy nie klika się w żadny z wyświetlanych przez niego element.
        private void LV_catalog_display_click_no_selection(object sender, MouseEventArgs e)
        {
            ListView parent = (ListView)sender;
            if (e.Button == MouseButtons.Right && parent.HitTest(e.Location).Item == null)
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
        }

        // Obsługa wybierania elementów z menu kontekstowego (oba typy obsługiwane jednym zdarzeniem.
        private void element_context_menu_item_select(object sender, ToolStripItemClickedEventArgs e)
        {
            ContextMenuStrip parent = (ContextMenuStrip)sender;
            ListView super_parent = (ListView)parent.SourceControl;
            
            // Menu kontekstowe bez zaznaczenia obiektu w folderze
            if (e.ClickedItem.Text.Equals("Nowy folder"))
            {
                context_menu_folder_make(catalog_folder_id_list.Last(),"Nowy folder");
                super_parent.Items[super_parent.Items.Count-1].BeginEdit();
            }
            if (e.ClickedItem.Text.Equals("Wklej"))
            {
                if(copy == true)
                {
                    for(int i = 0; i < LV_catalog_display_data_to_manipulate.Count; i++)
                    {
                        if(LV_catalog_display_data_to_manipulate[i].SubItems[1].Text.Equals("Folder")) {
                            database_virtual_folder_copy(int.Parse(LV_catalog_display_data_to_manipulate[i].Name),
                                                 catalog_folder_id_list.Last(),
                                                 LV_catalog_display_data_to_manipulate[i].Text);
                        }
                        else
                        {
                            database_virtual_file_copy(LV_catalog_display_data_to_manipulate_orgin_directory_id,
                                                       catalog_folder_id_list.Last(),
                                                       LV_catalog_display_data_to_manipulate[i].ToolTipText,
                                                       LV_catalog_display_data_to_manipulate[i].Text);
                        }
                    }
                    
                }
                if(cut == true)
                {

                }
                LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());
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
                    if(target_location != string.Empty)
                    {
                        System.Diagnostics.Process.Start(target_location);
                    }
                }
            }
            if (e.ClickedItem.Text.Equals("Wytnij"))
            {
                LV_catalog_display_data_to_manipulate = LV_catalog_display_item_selection;
                LV_catalog_display_data_to_manipulate_orgin_directory_id = catalog_folder_id_list.Last();
                cut = true;
                if (copy == true) copy = false;
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
                super_parent.Items[int.Parse(parent.Text)].BeginEdit();
            }
            if (e.ClickedItem.Text.Equals("Właściwości"))
            {

            }
        }

        // Pobieranie ścieżki rzeczywistej pliku z bazy danych.
        private string database_virtual_filepath_get (int id, string target_table)
        {
            string result = string.Empty;

            DataTable file_location_container = new DataTable();
            FbDataAdapter file_location_grabber = new FbDataAdapter("SELECT ID,PATH " +
                                                                    "FROM " + target_table + " " +
                                                                    "WHERE ID = " + id + ";"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            file_location_grabber.Fill(file_location_container);

            if(file_location_container.Rows.Count == 1)
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
        private bool database_virtual_folder_modifiable_check (int id_to_check)
        {
            bool is_modifiable = true;
            DataTable database_folder_modifiable_verifier_container = new DataTable();
            FbDataAdapter database_folder_modifiable_verifier = new FbDataAdapter("SELECT ID,NAME,MODIFIABLE " +
                                                                                  "FROM " + database_tables[0].Item2 + " " +
                                                                                  "WHERE ID = " + id_to_check + ";"
                                                                                  ,
                                                                                  new FbConnection(database_connection_string_builder.ConnectionString));

            database_folder_modifiable_verifier.Fill(database_folder_modifiable_verifier_container);

            if (database_folder_modifiable_verifier_container.Rows.Count == 1)
            {
                // Wszystko dobrze, znalazł wartość
                is_modifiable = bool.Parse(database_folder_modifiable_verifier_container.Rows[0].ItemArray[2].ToString());
                if (is_modifiable == false)
                {
                    MessageBox.Show(database_folder_modifiable_verifier_container.Rows[0].ItemArray[1].ToString()+" jest folderem domyślnym, nie może on zostać zmodyfikowany!");
                }
            }
            else
            {
                MessageBox.Show("Bład podczas pobierania flagi MODIFIABLE folderu!");
            }
            return is_modifiable;
        }

        // Procedura wywołująca kopiowanie pliku wirtualnego do folderu, patrzymy czy nie występuje kolizja i zmieniamy nazwę jeżeli zaszła.
        private void database_virtual_file_copy (int source_folder_id, int destination_folder_id, string type, string name)
        {
            string name_new = name;
            List<string> passed_data = new List<string>();

            // Sprawdzamy czy folder do którego kopiujemy nie ma już pliku o tej samej nazwie
            DataTable file_exists_container = new DataTable();
            FbDataAdapter file_exists_checker = new FbDataAdapter("SELECT NAME,DIR_ID" +
                                                                    "FROM " + type + " " +
                                                                    "WHERE NAME = " + name_new +
                                                                    "AND DIR_ID = " + destination_folder_id + ";"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            file_exists_checker.Fill(file_exists_container);
            if (file_exists_container.Rows.Count >= 1)
            {
                // Kolizja - mamy już plik o takiej nazwie w folderze do którego kopiujemy!
                // Doczepiamy do nazwy folderu cyfrę z ilością kopii i jest ok dopóki nie zrobimy sizeof(int) takich samych plików w jednym folderze.
                name_new = name_new + " " + file_exists_container.Rows.Count;
            }

            DataTable file_content_container = new DataTable();
            FbDataAdapter file_content_grabber = new FbDataAdapter("SELECT *" +
                                                                    "FROM " + type + " " +
                                                                    "WHERE NAME = " + name_new +
                                                                    "AND DIR_ID = " + source_folder_id + ";"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            file_content_grabber.Fill(file_content_container);
            if(file_content_container.Rows.Count == 1)
            {
                for (int i = 1; i < file_content_container.Rows[0].ItemArray.Count(); i++)
                    passed_data.Add((string)file_content_container.Rows[0].ItemArray[i]);
            }

            database_virtual_item_make(destination_folder_id, type, passed_data);
        }

        // Procedura wywołująca kopiowanie folderu wraz z jego zawartością do innego katalogu. Też sprawdza czy nie zaszła kolizja nazw.
        private void database_virtual_folder_copy (int source_folder_id,int destination_folder_id, string name)
        {
            string name_new = name;
            int id_new = 0;
            string error_message = string.Empty;
            bool error = false;

            // 1. Tworzymy folder, który będzie naszą kopią

            // Sprawdzamy czy folder do którego kopiujemy nie ma już folderu o tej samej nazwie
            DataTable folder_exists_container = new DataTable();
            FbDataAdapter folder_exists_checker = new FbDataAdapter("SELECT ID,DIR_ID " +
                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                    "WHERE NAME = '" + name_new + "' " +
                                                                    "AND DIR_ID = " + destination_folder_id + ";"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            folder_exists_checker.Fill(folder_exists_container);
            if (folder_exists_container.Rows.Count >= 1)
            {
                // Kolizja - mamy już folder o takiej nazwie w folderze do którego kopiujemy!
                // Doczepiamy do nazwy folderu cyfrę z ilością kopii i jest ok dopóki nie zrobimy sizeof(int) folderów w jednym folderze.
                name_new = name_new + " " + folder_exists_container.Rows.Count;
            }

            database_virtual_folder_make(name_new, destination_folder_id, true);

            // 2. Szukamy go w katalogu - chcemy jego ID żeby wrzucić do niego wszystkie rzeczy które były w folderze źródłowym

            DataTable new_folder_id_container = new DataTable();
            FbDataAdapter new_folder_id_grabber = new FbDataAdapter("SELECT ID,DIR_ID,NAME" + " " + 
                                                                    "FROM " + database_tables[0].Item2 + " " +
                                                                    "WHERE NAME = '" + name_new + "' " +
                                                                    "AND DIR_ID = " + destination_folder_id + ";"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

            new_folder_id_grabber.Fill(new_folder_id_container);

            if(new_folder_id_container.Rows.Count == 1)
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

            // 3. Gdy mamy już id kopiujemy całą zawartość folderu źródłowego do naszego nowoutworzonego folderu

            if (error == false)
            {
                // Przeszukujemy zawartość folderu źródłowego i odtwarzamy obiekty z niego w naszym nowym folderze:
                // Najpierw tworzymy jego podfoldery. Uwaga bo funkcja wywołuje tutaj samą siebie rekurencyjnie.
                DataTable new_folder_subfolders_to_create_container = new DataTable();
                FbDataAdapter new_folder_subfolders_to_create_grabber = new FbDataAdapter("SELECT ID,NAME,DIR_ID " +
                                                                        "FROM " + database_tables[0].Item2 + " " +
                                                                        "WHERE DIR_ID = " + source_folder_id + ";"
                                                                        ,
                                                                        new FbConnection(database_connection_string_builder.ConnectionString));

                new_folder_subfolders_to_create_grabber.Fill(new_folder_subfolders_to_create_container);

                for (int i = 0; i < new_folder_subfolders_to_create_container.Rows.Count; i++)
                {
                    if(!new_folder_subfolders_to_create_container.Rows[i].ItemArray[1].Equals(name_new))
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
                                                                            "WHERE DIR_ID = " + source_folder_id + ";"
                                                                            ,
                                                                            new FbConnection(database_connection_string_builder.ConnectionString));

                    new_folder_content_grabber.Fill(new_folder_content_container);
                    for (int j = 0; j < new_folder_content_container.Rows.Count; j++)
                    {
                        List<string> data_passed = new List<string>();
                        for(int k = 1; k < new_folder_content_container.Rows[j].ItemArray.Count(); k++)
                        {
                            string value = new_folder_content_container.Rows[j].ItemArray[k].ToString();
                            data_passed.Add(value);
                        }
                        database_virtual_item_make(id_new, database_tables[i].Item2, data_passed);
                    }
                }

            }
            else
            {
                //Error handling - tutaj zwracamy co poszło nie tak.
                MessageBox.Show(error_message);
            }
        }

        // Procedura usuwająca zadany folder wirtualny, w zależności od użytkownika usuwa foldery z zawartością.
        private void database_virtual_folder_delete (int id_to_delete, string target_table, bool is_file, bool surpress_pop_ups)
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
                                                                                 "WHERE DIR_ID = " + id_to_delete + ";"
                                                                                 ,
                                                                                 new FbConnection(database_connection_string_builder.ConnectionString));
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
                                database_virtual_folder_delete(thing_inside.Item1, thing_inside.Item3, thing_inside.Item4,true);
                            }
                            // Po usunięciu zawartości z potomków przyszedł czas na sam folder - wiemy że teraz musi być już pusty.
                            FbCommand database_directory_remover = new FbCommand("DELETE FROM " + target_table + " " +
                                                                                 "WHERE ID = " + id_to_delete + ";"
                                                                                 ,
                                                                                 new FbConnection(database_connection_string_builder.ConnectionString));
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
                                                                             "WHERE ID = " + id_to_delete + ";"
                                                                             ,
                                                                             new FbConnection(database_connection_string_builder.ConnectionString));
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
                                                                "WHERE ID = " + id_to_delete + ";"
                                                                ,
                                                                new FbConnection(database_connection_string_builder.ConnectionString));
                database_file_remover.Connection.Open();
                database_file_remover.ExecuteNonQuery();
                database_file_remover.Connection.Close();
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

            if (parent.Items[e.Item].SubItems[1].Text.Equals("Folder"))
            {
                // Edytujemy nazwę folderu - sprawdzić czy dozwolona jest modyfikacja nazwy dla tego folderu!
                bool folder_exists_already = false;
                DataTable database_folder_already_exists_container = new DataTable();
                FbDataAdapter database_folder_already_exists_verifier = new FbDataAdapter("SELECT NAME,DIR_ID " +
                                                                                          "FROM " + database_tables[0].Item2 + " " +
                                                                                          "WHERE DIR_ID = " + catalog_folder_id_list.Last() + " " +
                                                                                          "AND NAME = '" + e.Label + "' ;"
                                                                                          ,
                                                                                          new FbConnection(database_connection_string_builder.ConnectionString));

                database_folder_already_exists_container.Clear();
                database_folder_already_exists_verifier.Fill(database_folder_already_exists_container);
                if(database_folder_already_exists_container.Rows.Count > 0)
                {
                        MessageBox.Show(e.Label + " jest już w tym folderze!");
                        folder_exists_already = true;
                        e.CancelEdit = true;
                }

                if (!folder_exists_already)
                {
                    // Nie znaleźliśmy duplikatu o tym samym imieniu i o tym samym rodzicu, czas zmienić jego pierwotną zawartość w bazie danych:
                    FbCommand database_folder_renamer = new FbCommand("UPDATE " + database_tables[0].Item2 + " " +
                                                                      "SET NAME = '" + e.Label + "' " +
                                                                      "WHERE ID = " + parent.Items[e.Item].Name + ";"
                                                                      ,
                                                                      new FbConnection(database_connection_string_builder.ConnectionString));

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
                                                                                        "WHERE DIR_ID = " + catalog_folder_id_list.Last() + 
                                                                                        "AND NAME = '" + e.Label + "' " +
                                                                                        "AND EXTENSION = '" + parent.Items[e.Item].SubItems[1].Text + "';"
                                                                                        ,
                                                                                        new FbConnection(database_connection_string_builder.ConnectionString));

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
                                                                    "SET NAME = '" + e.Label + "' " +
                                                                    "WHERE ID = " + parent.Items[e.Item].Name + " " +
                                                                    "AND NAME = '" + parent.Items[e.Item].Text + "' ;"
                                                                    ,
                                                                    new FbConnection(database_connection_string_builder.ConnectionString));

                    database_file_renamer.Connection.Open();
                    database_file_renamer.ExecuteNonQuery();
                    database_file_renamer.Connection.Close();
                }
            }
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
                    folder_context_menu_strip.Items.Add("Otwórz");
                    folder_context_menu_strip.Items.Add("Wytnij");
                    folder_context_menu_strip.Items.Add("Kopiuj");
                    folder_context_menu_strip.Items.Add("Usuń");
                    folder_context_menu_strip.Items.Add("Zmień nazwę");
                    folder_context_menu_strip.Items.Add("Właściwości");
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
                    file_context_menu_strip.Items.Add("Otwórz");
                    file_context_menu_strip.Items.Add("Wytnij");
                    file_context_menu_strip.Items.Add("Kopiuj");
                    file_context_menu_strip.Items.Add("Usuń");
                    file_context_menu_strip.Items.Add("Zmień nazwę");
                    file_context_menu_strip.Items.Add("Właściwości");
                    file_context_menu_strip.ItemClicked += element_context_menu_item_select;
                    if (context_menu_position.Y < context_menu_position.Y) context_menu_position.Y = context_menu_position.Y - file_context_menu_strip.Height - 25;
                    parent.ContextMenuStrip = file_context_menu_strip;
                }
            }
        }

        // Zmienia wyswietlany folder.
        private void LV_catalog_display_change_directory (ListViewItem target_directory)
        {
            if (target_directory != null && target_directory.SubItems.Count != 0) {
                if (target_directory.SubItems[1].Text.Equals("Folder"))
                {
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
                else
                {
                    // Błąd - przekazano do funkcji plik.
                    MessageBox.Show("Błąd! Przekazany przedmiot wskazuje na coś innego niż folder");
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
            if(e.Button == MouseButtons.Left)
            {
                ListView parent = (ListView)sender;
                ListViewItem target = parent.HitTest(e.Location).Item;

                if (target.SubItems[1].Text.Equals("Folder"))
                {
                    // Otwieramy folder
                    LV_catalog_display_change_directory(target);
                }
                else
                {
                    // Otwieramy plik
                    string target_location = database_virtual_filepath_get(int.Parse(target.Name), target.ToolTipText);
                    if (target_location != string.Empty)
                    {
                        System.Diagnostics.Process.Start(target_location);
                    }
                }
            }
        }

        // Obsługuje guzik do cofania się pomiędzy folderami.
        private void BT_previous_click (object sender, EventArgs e)
        {
            if (catalog_folder_id_list.Count > 1)
            {
                catalog_folder_id_list.Remove(catalog_folder_id_list.Last());
                catalog_folder_path_list.Remove(catalog_folder_path_list.Last());
                TB_catalog_path_current.Text = string.Empty;
                for (int i = 0; i < catalog_folder_path_list.Count; i++)
                {
                    TB_catalog_path_current.Text += catalog_folder_path_list[i];
                }
                LV_catalog_display_folder_content_display(catalog_folder_id_list.Last());

            }
            else MessageBox.Show("Jestem już w folderze głównym.");
        }

    // Obsługa opcji specjalnych:

        // Wejscie do podokna wyboru opcji specjalnych.
        private void BT_specials_Click(object sender, EventArgs e)
        {
            // Wywołujemy nowego Forma w którym mamy wybór poszczególnych opcji specjalnych i przekazujemy do niego listę naszych wybranych obiektów.
            Special_function_window special_option_selector = new Special_function_window();
            special_option_selector.Owner = this;
            // Z data to manipulate wyciagamy wszystkie pliki - potrzebujemy ich nazwy, rozszerzenia i ścieżki fizyczne.
            special_option_selector.DB_connection_string = database_connection_string_builder.ConnectionString;
            special_option_selector.database_tables = database_tables;
            special_option_selector.items_to_work_on = LV_catalog_display_item_selection;
            special_option_selector.working_directory = catalog_folder_id_list.Last();
            special_option_selector.program_path = program_path;
            special_option_selector.Show();
        }

        // Dalej znajduje się kod legacy...

































































        // Kod legacy, w zasadzie nie widziałem nigdzie jego użycia
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }

       
        private void ChkMetadata_LostFocus(object sender, EventArgs e)
        {
            /*
            var diff = metadata.Where(x => !excludedMetadata.Contains(x)).ToList();
            this.AppendXML(diff);
            */
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            Random rand = new Random();
            randResult = rand.Next(0, exampleCommands.Length - 1);
            randResultCreate = rand.Next(0, exampleCommandsCreate.Length - 1);
            this.lbExampleCommand.Text += exampleCommands[randResult];
        }

        private void TxtCommand_LostFocus(object sender, EventArgs e)
        {
            Regex rx = new Regex(regex, RegexOptions.IgnoreCase);
            Match mtxtCommand = Regex.Match(this.txtCommand.Text, regex);

            foreach (var item in this.GroupRegex(rx, mtxtCommand))
                _group.Add(item);

            var notSupported = String.Empty;
            try
            {
                var split = _group[_group.Count - 2].Split(new char[] { ' ', '*', '.' });
                for (int i = 0; i < extends.Length; i++)
                    foreach (var s in split)
                        if (!extends.Contains(s))
                            notSupported += " " + s;

                MessageBox.Show("Nieobsługiwana grupa rozszerzeń: " + this.RemoveDuplicates(notSupported), "Grupa rozszerzeń", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Tłumaczy każde słowo tekstu podanego na wejściu
        /// </summary>
        /// <param name="normal">lista słów języka wejściowego tekstu</param>
        /// <param name="create">lista słów "języka 'create'"</param>
        /// <returns>Słownik słowo-tłumaczenie</returns>
        private Dictionary<string, string> Groups(List<string> normal, List<string> create)
        {
            Dictionary<string, string> groups = new Dictionary<string, string>();
            string[] filesGroup = { "files' group", "group of files" };
            string[] forEach = { "each", "all", "every" };
            Random rand = new Random();
            int index = rand.Next(1, 2);
            int i = rand.Next(0, forEach.Length - 1);
            try
            {
                groups.Add(normal[0], create[0]);
                groups.Add(normal[index], create[1]);
                groups.Add("każdego", forEach[i]);
                groups.Add("każdej", forEach[i]);
                groups.Add("dla", "for");
                groups.Add("grupy plików", filesGroup[index]);
            }
            catch { }

            return groups;
        }

        /// <summary>
        /// Grupuje tekst podany na wejściu względem wyrażenia regularnego
        /// </summary>
        /// <param name="rx">Wyrażenie regularne, do którego tekst wejściowy będzie dopasowywany</param>
        /// <param name="match">Dopasowanie zawierające tekst podany na wejściu</param>
        /// <returns>Dopasowane grupy tekstu podanego na wejściu</returns>
        private List<string> GroupRegex(Regex rx, Match match)
        {
            List<string> groups = new List<string>();

            while (match.Success)
            {
                for (int i = 1; i <= 50; i++)
                {
                    Group g = match.Groups[i];
                    string gToString = g.ToString();
                    if (!gToString.Equals("") && !gToString.Equals(" ") && gToString.Length != 1 && !gToString.Equals("o"))
                        groups.Add(gToString);

                }
                match = match.NextMatch();
            }
            return groups;
        }

        private void txtCommand_TextChanged(object sender, EventArgs e)
        {
            if (this.txtCommand.Text.Contains("<"))
                this.toolTip1.SetToolTip(txtCommand, "nazwa_etykiety$");
            if (this.txtCommand.Text.Contains(">"))
                this.toolTip1.RemoveAll();
            if (this.txtCommand.Text.Contains(".") || this.txtCommand.Text.Contains("*"))
            {
                string text = String.Empty;
                for (int i = 0; i < extends.Length; i++)
                {
                    text += extends[i] + "\n";
                }
                this.toolTip1.SetToolTip(txtCommand, text);
            }
        }

        List<string> _group = new List<string>();
        private void chkUseCreteRule_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkUseCreteRule.Checked)
            {
                this.lbExampleCommand.Text = "";
                this.lbExampleCommand.Text += "E.g. " + exampleCommandsCreate[randResultCreate];
            }
            else
            {
                this.lbExampleCommand.Text = "";
                this.lbExampleCommand.Text += "Np.: " + exampleCommands[randResult];
            }

            this.chkUseEquality.Enabled = true;

            Regex rx = new Regex(regex, RegexOptions.IgnoreCase);
            Match m = Regex.Match(exampleCommands[randResult], regex);

            Regex rxCreate = new Regex(regexCreate, RegexOptions.IgnoreCase);
            Match mCreate = Regex.Match(exampleCommandsCreate[randResultCreate], regexCreate);
            var groupRegex = GroupRegex(rx, m);
            var groupRegexCreate = GroupRegex(rxCreate, mCreate);
            var split = txtCommand.Text.Split(' ');
            bool equals = false;

            if (this.txtCommand.Text.Length != 0)
                foreach (var group in this.Groups(groupRegex, groupRegexCreate))
                    foreach (var s in split)
                        if (s == group.Key)
                            equals = true;

            Match mtxtCommand = Regex.Match(this.txtCommand.Text, regex);

            foreach (var item in this.GroupRegex(rx, mtxtCommand))
                _group.Add(item);

            if (this.chkUseCreteRule.Checked)
            {
                this.txtCommand.Text = String.Empty;
                if (equals)
                    foreach (var group in this.Groups(groupRegex, groupRegexCreate))
                        this.txtCommand.Text += " " + group.Value;

                if (this.txtCommand.Text.Length != 0)
                    this.txtCommand.Text += " " + _group[5] + " " + _group[_group.Count - 2];
                this.txtCommand.Text = RemoveDuplicates(txtCommand.Text);
            }
        }

        private string RemoveDuplicates(string p)
        {
            var distinct = string.Join(" ",

                Regex.Matches(p, @"([^\s]+)")
                         .OfType<Match>()
                     .Select(m => m.Groups[0].Value)
                     .Distinct()

            );

            return distinct;

        }

        private void chkUseEquality_CheckedChanged(object sender, EventArgs e)
        {
            Regex rx = new Regex(@"((each)|(all)|(every))(\s)(for)(\s)(((group of files)|(files' group)){0,1})");
            this.txtCommand.Text = rx.Replace(this.txtCommand.Text, "=");
        }

        private void chooseMetadata_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //MessageBox.Show(m.Key + " => " + m.Value);
        }

        private void PostXML(string v, string requestData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tworzy plik XML na podst. metadanych oraz nazwy maszyny i jej użytkownika
        /// </summary>
        /// <param name="metadata">lista metadanych pozostałych po wykluczeniu tych niechcianych przez użytkownika</param>
        private void AppendXML(List<string> metadata)
        {
            Random rand = new Random();
            List<XmlNode> node = new List<XmlNode>();
            char randChar = (char)rand.Next(65, 90);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xml_path);
            XmlElement _metadata = xDoc.CreateElement("metadata");

            string strMachineUserName = Environment.MachineName + "_" + Environment.UserName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff");

            string strMd5 = this.Encrypt(strMachineUserName, true);
            string strMd5MachineUserName = this.Encrypt("machineUserName", true);

            string key = randChar + strMd5MachineUserName.Substring(1).Replace("=", string.Empty).Replace("+", string.Empty).Replace("/", string.Empty);

            _metadata.SetAttribute(key, strMd5);
            File.SetAttributes(txt_path, FileAttributes.Normal);
            File.AppendAllText(txt_path, key + "$" + strMd5MachineUserName + Environment.NewLine);


            foreach (var m in metadata)
                //if (m.Substring(0, m.IndexOf("=")).Equals("FilePath") || m.Substring(0, m.IndexOf("=")).Contains("Author") || m.Substring(0, m.IndexOf("=")).Contains("publisher") || m.Substring(0, m.IndexOf("=")).Contains("dc") || m.Substring(0, m.IndexOf("=")).Contains("author") || m.Substring(0, m.IndexOf("=")).Contains("version") || m.Substring(0, m.IndexOf("=")).Contains("date") || m.Substring(0, m.IndexOf("=")).Contains("Date") || m.Substring(0, m.IndexOf("=")).Contains("created") || m.Substring(0, m.IndexOf("=")).Contains("modified") || m.Substring(0, m.IndexOf("=")).Contains("Version") || m.Substring(0, m.IndexOf("=")).Contains("creator") || m.Substring(0, m.IndexOf("=")).Contains("Last"))
                //    node.Add(xDoc.CreateElement(this.Encrypt(m.Substring(0, m.IndexOf("=")), true).Replace("=", string.Empty).Replace("+", string.Empty).Replace("/", string.Empty)));
                //else
                node.Add(xDoc.CreateElement(m.Substring(0, m.IndexOf("="))));

            foreach (var m in metadata)
                foreach (var n in node)
                {
                    var mEncrypt = this.Encrypt(m.Substring(m.IndexOf("=") + 1), true);
                    n.InnerText = this.Encrypt(m.Substring(m.IndexOf("=") + 1), true).Replace("=", string.Empty).Replace("+", string.Empty).Replace("/", string.Empty);
                    File.AppendAllText(txt_path, n.InnerText + "$" + mEncrypt + Environment.NewLine);
                }


            foreach (var n in node)
                _metadata.AppendChild(n);

            xDoc.DocumentElement.AppendChild(_metadata);
            xDoc.Save(xml_path);

            File.SetAttributes(txt_path, FileAttributes.ReadOnly | FileAttributes.Hidden);
        }

        private string Encrypt(string v, bool isHashUsed)
        {
            byte[] keyArray;
            byte[] encrypted = UTF8Encoding.UTF8.GetBytes(v);

            System.Configuration.AppSettingsReader appSettings = new System.Configuration.AppSettingsReader();

            string key = appSettings.GetValue("SecurityKey", typeof(String)) as string;

            if (isHashUsed)
            {
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                keyArray = md5provider.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                md5provider.Clear();
            }
            else keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            tdes.Key = keyArray;
            tdes.Mode = CipherMode.CFB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cryptoTransform = tdes.CreateEncryptor();
            byte[] resultArray = cryptoTransform.TransformFinalBlock(encrypted, 0, encrypted.Length);

            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private void chkMetadata_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.chkMetadata.Items.Remove(this.chkMetadata.SelectedItem);
            this.toolTip1.SetToolTip(this.chkMetadata, this.chkMetadata.SelectedItem.ToString());
        }

        private void bnCatalogue_Click(object sender, EventArgs e)
        {
            this.contextMenuStrip1.Visible = true;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chkSelectAllListPositions_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.chkMetadata.Items.Count; i++)
            {
                this.chkMetadata.SetItemChecked(i, true);
            }
        }

        private void chkMetadata_DoubleClick(object sender, EventArgs e)
        {
            this.excludedMetadata.Remove(this.chkMetadata.SelectedItem.ToString());
        }

        private void chkMetadata_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.excludedMetadata.Add(this.chkMetadata.SelectedItem.ToString());
        }

        /*
        private void button1_Click_1(object sender, EventArgs e)
        {
            //MessageBox.Show(this.Decrypt("vlnsYsM7p0Ay16aH5m3IkzmvWncbGmC/mbUTzUity0h7y92SW6GONLYsqLYnDNKW", true).Substring(8));
            var extractor = new TikaOnDotNet.TextExtraction.TextExtractor().Extract(xml_path);
            string xmlNoSpaces = Regex.Replace(extractor.Text, @"\s+", string.Empty);

            Regex rx = new Regex("(<.*?>)", RegexOptions.IgnoreCase);
            Regex rxQuery = new Regex("(<.*?>)(.+)(<.*?>)", RegexOptions.IgnoreCase);
            Match mTxt = Regex.Match(extractor.Text, "(<.*?>)");
            File.SetAttributes(txt_path, FileAttributes.Normal);
            StreamReader reader = new StreamReader(txt_path);
            StreamReader reader2 = new StreamReader(txt_path);
            string line = String.Empty;
            string subgroup = String.Empty;
            string _subgroup = String.Empty;
            var regex = @"([A-Za-z]+)(_[A-Za-z]+)(_[0-9]+)$";

            foreach (var group in this.GroupRegex(rx, mTxt))
                if (group.Length > 22)
                {
                    subgroup = group.Substring(group.IndexOf("=")+2);
                    _subgroup = this.Decrypt(Regex.Replace(subgroup, @">$", string.Empty).TrimEnd('"'), true).Substring(8);
                }

            line = Regex.Replace(reader.ReadLine(), @"\$(.+)", String.Empty);
            
            XDocument xDoc = XDocument.Load(xml_path);
            var query = xDoc.Descendants("Metadata")
                            .Where(parent => parent.Elements("metadata")
                            .Any(child => (string)child.Attribute(line).Value == Regex.Replace(subgroup, @">$", string.Empty).TrimEnd('"')));

            bool realMachineUser = false;
            foreach (var q in query)
            {
                foreach (var group in this.GroupRegex(rx, mTxt))
                    if (group.Length > 22)
                    {
                        subgroup = group.Substring(group.IndexOf("=") + 2);
                        _subgroup = this.Decrypt(Regex.Replace(subgroup, @">$", string.Empty).TrimEnd('"'), true).Substring(8);

                        Regex rxMachineUser = new Regex(regex, RegexOptions.IgnoreCase);
                        Match m = Regex.Match(_subgroup, regex);
                        realMachineUser = Environment.MachineName.Contains(this.GroupRegex(rxMachineUser, m)[0]) && Environment.UserName.Equals(this.GroupRegex(rxMachineUser, m)[1].Replace("_", String.Empty));
                    }
            }
            List<string> metadataKey = new List<string>();
            if (realMachineUser)
                foreach (var group in this.GroupRegex(rx, mTxt))
                    if (group.Length <= 22 && !group.Contains("/") && !group.Equals("<Metadata>"))
                        metadataKey.Add(Regex.Replace(group, "<|>", String.Empty));

            var metadataKeyDist = metadataKey.Distinct();
        }
        */

        private string Decrypt(string v1, bool v2)
        {
            byte[] keyArray;
            byte[] encrypted = Convert.FromBase64String(v1);

            System.Configuration.AppSettingsReader appSettings = new System.Configuration.AppSettingsReader();

            string key = appSettings.GetValue("SecurityKey", typeof(String)) as string;

            if (v2)
            {
                MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
                keyArray = md5provider.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                md5provider.Clear();
            }
            else keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            tdes.Key = keyArray;
            tdes.Mode = CipherMode.CFB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cryptoTransform = tdes.CreateDecryptor();
            byte[] resultArray = cryptoTransform.TransformFinalBlock(encrypted, 0, encrypted.Length);

            tdes.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
