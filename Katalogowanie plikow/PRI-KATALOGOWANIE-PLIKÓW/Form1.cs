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
    public partial class Form1 : Form
    {
        // Tutaj wylądują po procesie ekstrakcji metadane.
        public List<string[]> metadata { get; set; }

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
        private string regex = @"((u|s|wy){0,1}(tw((órz)|(orzyć)|(orzenie))))(\s)(etykiet(y|ę){0,1})(\s)((<[a-ząęśćżźół\#]+\$>)+)(\s)((dla){0,1})(\s)((każde){0,1}((go)|j){0,1})(\s){0,1}((grupy){0,1})(\s){0,1}((((plik)|(obiekt))(u|ów))|(lokacji))(\s)(((\*{0,1})\.{0,1}[a-z0-9]{3,4}\s{0,1})+)";
        private string regexCreate = @"(create)(\s)(label)(\s)((<[a-ząęśćżźół\#]+\$>)+)(\s)((for){0,1})(\s)((every)|(all)|(each){0,1})(\s){0,1}((group of files)|(files' group){0,1})(\s)(((\*{0,1})\.{0,1}[a-z0-9]{3,4}\s{0,1})+)";
        private string[] exampleCommands = { "utwórz etykietę <x$> dla pliku *.mp3", "utwórz etykietę <x$> dla obiektu *.mp3", "utwórz etykietę <x$> dla lokacji *.mp3", "utwórz etykiety <x$><y$> dla grupy plików *.mp3 *wav", "utwórz etykiety <x$><y$> dla plików *.mp3 *wav", "utwórz etykiety <x$><y$> plików *.mp3 *wav", "utwórz etykiety <x$><y$> dla obiektów *.mp3 *wav", "utwórz etykiety <x$><y$> dla lokacji *.mp3 *wav" };
        private string[] exampleCommandsCreate = { "create label <x$> for every file *.mp3", "create label <x$> for each file *.mp3", "create label <x$> for all file *.mp3", "create label <x$> for file *.mp3", "create label <x$><y$> for files' group *.mp3 *wav", "create label <x$><y$> for group of file *.mp3 *wav", };
        int randResult = 0;
        int randResultCreate = 0;
        List<string> excludedMetadata;

        // Ścieżki do poszczególnych plików niezbędnych do działania programu:
        private string program_path = null;
        private string xml_path = null;
        private string txt_path = null;
        private string database_path = null;

        // Zmienne stanu programu:
        private bool database_validated_successfully = false;

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
         *                *    db\ - tutaj żyje nasz katalog, w pliku catalog.fdb
         *    
         *    Tutaj zakladam ze program jest schowany glebiej przez Visual Studio, stad potrzebne są nam dodatkowe skoki. Przepisanie tej procedury na czysto
         *    nie jest problemem.
         *    
         *    UWAGA: Moze wygenerowac IOException jeżeli coś innego czyta nasze katalogi!
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
            database_columns.Add(new Tuple<int, string, string>(0, @"DIR_ID", @"CHAR(36)")); // null dozwolony tylko dla katalogu nadrzędnego.
            database_columns.Add(new Tuple<int, string, string>(0, @"DEPTH", @"INT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(0, @"MODIFIABLE", @"BOOLEAN NOT NULL"));

            // Kolumny tabeli metadata_text:
            database_columns.Add(new Tuple<int, string, string>(1, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(1, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
            database_columns.Add(new Tuple<int, string, string>(1, @"NAME", @"VARCHAR(128) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"EXTENSION", @"VARCHAR(5) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"PATH", @"VARCHAR(512) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"SIZE", @"INT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(1, @"CONTENT_TYPE", @"VARCHAR(80)"));
            database_columns.Add(new Tuple<int, string, string>(1, @"CONTENT_ENCODING", @"VARCHAR(64)"));

            // Kolumny tabeli metadata_document
            database_columns.Add(new Tuple<int, string, string>(2, @"ID ", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(2, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
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
            database_columns.Add(new Tuple<int, string, string>(4, @"NAME", @"VARCHAR(128) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"EXTENSION", @"VARCHAR(5) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"PATH", @"VARCHAR(512) NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"SIZE", @"INT NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"FS_CREATION_TIME", @"TIMESTAMP NOT NULL"));
            database_columns.Add(new Tuple<int, string, string>(4, @"FS_LAST_WRITE_TIME", @"TIMESTAMP NOT NULL"));
            //Kolumny tabeli metadata_multimedia
            database_columns.Add(new Tuple<int, string, string>(5, @"ID", @"INT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY"));
            database_columns.Add(new Tuple<int, string, string>(5, @"DIR_ID", @"INT NOT NULL REFERENCES virtual_folder"));
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

        void PrepareConnectionString()
        {
            database_connection_string_builder = new FbConnectionStringBuilder();
            database_connection_string_builder.DataSource = "localhost"; // Identyfikator sieciowy - do kogo sie laczymy. Moze byc postaci adres IP+Port.
            database_connection_string_builder.UserID = "SYSDBA"; // Defaultowy uzytkownik z najwyzszymi uprawnieniami do systemu bazodanowego, tworzony podczas instalacji
            database_connection_string_builder.Password = "5o9orjoh"; // Haslo nadane podczas instalacji Firebird'a użytkownikowi SYSDBA, uzupełnić w zależności u kogo jest jakie
            database_connection_string_builder.Database = database_path;
            database_connection_string_builder.ServerType = FbServerType.Default;
        }

        public Form1()
        {
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

                // Przebudowujemy bazę jeżeli cokolwiek było nie tak

                for (int i = 0; i < validation_result.Count(); i++)
                {
                    if (validation_result[i] == 0) validation_return_string_container[i] = "Tabela " + database_tables[i].Item2 + " nie istnieje w bazie! \n";
                    if (validation_result[i] == 1) validation_return_string_container[i] = "Tabela " + database_tables[i].Item2 + " ma niepoprawne pola \n!";
                    if (validation_result[i] == 2) validation_return_string_container[i] = "Tabela " + database_tables[i].Item2 + " jest skonstruowana poprawnie! \n";
                }

                if (validation_result.All(x => x.Equals(true)) == true) database_validated_successfully = true;

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

                        // Uwaga na warości ID i DIR_ID, trzeba je wypełniać inaczej niż automatem.
                        // Wypełniamy tutaj wartość dla DIR_ID na podstawie wcześniej stworzonych folderów wirtualnych w katalogu.

                        if (datatable_index == 1) parent_directory_ID = 2;
                        if (datatable_index == 2) parent_directory_ID = 3;
                        if (datatable_index == 3) parent_directory_ID = 4;
                        if (datatable_index == 4) parent_directory_ID = 5;
                        if (datatable_index == 5) parent_directory_ID = 6;
                        add_data.Parameters.AddWithValue(values_passed[0], parent_directory_ID);

                        for (int j = 1; j < metadata[i].Count(); j++)
                        {
                            if(!(metadata[i].ElementAt(j).Equals("NULL"))) add_data.Parameters.AddWithValue(values_passed[j], metadata[i].ElementAt(j));
                            else add_data.Parameters.AddWithValue(values_passed[j], null);
                        }

                        if(metadata[i].Count() < values_passed.Length)
                        {
                            for(int j = metadata[i].Count(); j < values_passed.Length; j++) add_data.Parameters.AddWithValue(values_passed[j], null);
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

            //Zbudowano bazę zgodnie z sztuką, teraz czas na stworzenie folderów domyślnych...
            var columns_to_populate = database_columns.FindAll(x => x.Item1 == 0);
            string datafields = String.Empty;
            string values = String.Empty;
            string[] values_passed = new string[columns_to_populate.Count-1];

            // ID nie wypełniamy z ręki (robi to automatycznie silnik bazodanowy), stąd startujemy tutaj przejście od 1.
            for (int j = 1; j < columns_to_populate.Count; j++)
            {
                datafields += columns_to_populate[j].Item2 + ",";
                values += "@" + columns_to_populate[j].Item2 + ",";
                values_passed[j-1] = columns_to_populate[j].Item2;
            }
            datafields = datafields.TrimEnd(',');
            values = values.TrimEnd(',');

            string folder_creation_script = "INSERT INTO " + database_tables.Find(x => x.Item1.Equals(0)).Item2 + "(" + datafields + ") VALUES (" + values + ")";

            FbCommand root_folder_creator = new FbCommand(folder_creation_script, database_connection);
            root_folder_creator.Parameters.AddWithValue(values_passed[0], "Root");
            root_folder_creator.Parameters.AddWithValue(values_passed[1], "NULL");
            root_folder_creator.Parameters.AddWithValue(values_passed[2], "0");
            root_folder_creator.Parameters.AddWithValue(values_passed[3], "FALSE");

            database_connection.Open();
            root_folder_creator.ExecuteNonQuery();
            database_connection.Close();

            // to tutaj jest napisane brzydko, ale chwilowo whackowane coby dobrze działało.

            FbCommand text_folder_creator = new FbCommand(folder_creation_script, database_connection);
            text_folder_creator.Parameters.AddWithValue(values_passed[0], "Pliki tekstowe");
            text_folder_creator.Parameters.AddWithValue(values_passed[1], "1");
            text_folder_creator.Parameters.AddWithValue(values_passed[2], "1");
            text_folder_creator.Parameters.AddWithValue(values_passed[3], "FALSE");

            database_connection.Open();
            text_folder_creator.ExecuteNonQuery();
            database_connection.Close();

            FbCommand document_folder_creator = new FbCommand(folder_creation_script, database_connection);
            document_folder_creator.Parameters.AddWithValue(values_passed[0], "Dokumenty");
            document_folder_creator.Parameters.AddWithValue(values_passed[1], "1");
            document_folder_creator.Parameters.AddWithValue(values_passed[2], "1");
            document_folder_creator.Parameters.AddWithValue(values_passed[3], "FALSE");

            database_connection.Open();
            document_folder_creator.ExecuteNonQuery();
            database_connection.Close();

            FbCommand complex_folder_creator = new FbCommand(folder_creation_script, database_connection);
            complex_folder_creator.Parameters.AddWithValue(values_passed[0], "Pliki .htm i .html");
            complex_folder_creator.Parameters.AddWithValue(values_passed[1], "1");
            complex_folder_creator.Parameters.AddWithValue(values_passed[2], "1");
            complex_folder_creator.Parameters.AddWithValue(values_passed[3], "FALSE");

            database_connection.Open();
            complex_folder_creator.ExecuteNonQuery();
            database_connection.Close();

            FbCommand image_folder_creator = new FbCommand(folder_creation_script, database_connection);
            image_folder_creator.Parameters.AddWithValue(values_passed[0], "Obrazki i zdjęcia");
            image_folder_creator.Parameters.AddWithValue(values_passed[1], "1");
            image_folder_creator.Parameters.AddWithValue(values_passed[2], "1");
            image_folder_creator.Parameters.AddWithValue(values_passed[3], "FALSE");

            database_connection.Open();
            image_folder_creator.ExecuteNonQuery();
            database_connection.Close();

            FbCommand multimedia_folder_creator = new FbCommand(folder_creation_script, database_connection);
            multimedia_folder_creator.Parameters.AddWithValue(values_passed[0], "Pliki multimedialne");
            multimedia_folder_creator.Parameters.AddWithValue(values_passed[1], "1");
            multimedia_folder_creator.Parameters.AddWithValue(values_passed[2], "1");
            multimedia_folder_creator.Parameters.AddWithValue(values_passed[3], "FALSE");

            database_connection.Open();
            multimedia_folder_creator.ExecuteNonQuery();
            database_connection.Close();
        }

        // Wstawianie pojedynczej tabeli do bazy danych - jeżeli wykryjemy jej brak!
        private void database_insert_table(string database_connection_string, int missing_table_index)
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
         * Dobrze byłoby gdzieś przechowywać informację o wyniku testu bazy żeby program nie mógł pracować na nie przetestowanej bazie.
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
