using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Special_function_window : Form
    {
        public event EventHandler OnDataAvalible;

    // Deklaracje okien potomnych i danych przekazanych z Main_form'u
        // Okna potomne do Special_function_window
        private Karol_main extractor_window;
        private Janek_main audio_sorter_window;
        private Obrazy_main image_processing_window;
        private Wyszukiwarka_main searcher_window;

        // Dane przekazywane przez Main_form do Special_function_window
        public List<Tuple<int, string>> database_tables;
        public List<Tuple<int, string>> extends;
        public string DB_connection_string;
        public string program_path;
        public int working_directory;

    // Przechowywanie danych wejściowych do podprogramów
        public List<ListViewItem> items_to_work_on;
        public List<string> names;

    // Przechowywanie danych wyjściowych z podprogramów.
        // Część Janka - badanie podobieństwa plików audio:
        public List<Tuple<int, string>> audio_sorter_folders_returned;
        public List<Tuple<int, string>> audio_sorter_files_returned;

        // Część Karola - ekstrakcja napisów z filmów:
        public List<Tuple<int, string, string>> extractor_texts_returned;

        // Część Kuby - przeszukiwanie obrazków:
        public List<Tuple<int, string>> image_processor_folders_returned;
        public List<Tuple<int, string>> image_processor_files_returned;

        // Wyszukiwarka w katalogu - zwrot polecenia z parametrami do wykonania:
        // Dla pierwszego powrotu do Special_function_window
        public int target_table;
        public List<string> search_parameters;
        public List<string> search_payload;
        public List<Tuple<string, int, string>> search_parameters_values;
        // Dla zwrotu z wyszukiwania w Special_function_window do Main_form'u
        public List<Tuple<int, string>> searcher_folders_returned;
        public List<Tuple<int, string>> searcher_files_returned;

        // Parametr informujący o tym co zwracamy do programu głównego.
        public int return_index = 0;

    // Obsługa zdarzeń okien podrzędnych i pośredniczenie w ich kontakcie z Main_form
        // Obsługa zdarzenia powrotu z formu Janka - dostajemy listę Tuple<int,string> zawierającą ID przydzielone nowemu folderowi (w celach porządkowych)
        // i jego nową nazwę, jak i listę Tuple<int,string> zawierającą indeks do którego przydzielamy plik i jego ścieżkę.
        void Janek_main_OnDataAvalible(object sender, EventArgs e)
        {
            audio_sorter_folders_returned = new List<Tuple<int, string>>();
            audio_sorter_files_returned = new List<Tuple<int, string>>();

            audio_sorter_folders_returned = audio_sorter_window.result_directories.ToList();
            audio_sorter_files_returned = audio_sorter_window.result_files.ToList();

            return_index = 1;

            // Informujemy Main_form o tym że któraś z opcji zwróciła wynik.
            OnDataAvalible(this, EventArgs.Empty);
            this.Close();
            this.Dispose();
        }

        // Obsługa zdarzenia powrotu z formu Karola - dostajemy listę Tuple<string,string> zawierającą nazwę pliku dla którego przeprowadzaliśmy analizę i
        // tekst wyekstrachowany z filmu przydzielony plikowi.
        void Karol_main_OnDataAvalible(object sender, EventArgs e)
        {
            

            extractor_texts_returned = new List<Tuple<int, string, string>>();

            extractor_texts_returned = extractor_window.text_extraction_results.ToList();

            return_index = 0;

            // Informujemy Main_form o tym że któraś z opcji zwróciła wynik.
            OnDataAvalible(this, EventArgs.Empty);
            this.Close();
            this.Dispose();
        }

        // Obsługa zdarzenia powrotu z formu Kuby - dostajemy listę Tuple<int,string> zawierającą ID przydzielone nowemu folderowi (w celach porządkowych)
        // i jego nową nazwę, jak i listę Tuple<int,string> zawierającą indeks do którego przydzielamy plik i jego ścieżkę.
        void Obrazy_main_OnDataAvalible(object sender, EventArgs e)
        {
            image_processor_folders_returned = new List<Tuple<int, string>>();
            image_processor_files_returned = new List<Tuple<int, string>>();

            image_processor_folders_returned = image_processing_window.result_directories.ToList();
            image_processor_files_returned = image_processing_window.result_files.ToList();

            return_index = 2;

            // Informujemy Main_form o tym że któraś z opcji zwróciła wynik.
            OnDataAvalible(this, EventArgs.Empty);
            this.Close();
            this.Dispose();
        }

        // Obsługa zdarzenia powrotu z wyszukiwarki - przesyła polecenie stworzone w Wyszukiwarka_main do Main_form celem wykonania.
        void Wyszukiwarka_main_OnDataAvalible(object sender, EventArgs e)
        {
            Tuple<int, string> result_directory = new Tuple<int, string>(0,"Wyniki wyszukiwania");
            List<Tuple<int, string>> result_files = new List<Tuple<int, string>>();

            target_table = searcher_window.target_table_returned;
            search_parameters = searcher_window.target_query_parameters_returned;
            search_payload = searcher_window.target_query_payload_returned;
            search_parameters_values = searcher_window.parameters_values_returned;

            return_index = 3;

            // Wywołujemy specjalną wersję prepare_data, ma za zadanie zwrócić nam listę plików do dodania przez Main_form. 
            List<string> extensions_to_use = new List<string>();
            if(target_table < 6)
            {
                foreach (var text_extends in extends.FindAll(x => x.Item1 == target_table - 1))
                {
                    extensions_to_use.Add(text_extends.Item2);
                }
            }
            else
            {
                foreach (var extention in extends.FindAll(x => x.Item1 != 5))
                {
                    extensions_to_use.Add(extention.Item2);
                }
            }

            result_files = prepare_data(extensions_to_use, 3) as List<Tuple<int,string>>;

            searcher_folders_returned = new List<Tuple<int, string>>() { result_directory };
            searcher_files_returned = result_files;

            // Informujemy Main_form o tym że któraś z opcji zwróciła wynik.
            OnDataAvalible(this, EventArgs.Empty);
            this.Close();
            this.Dispose();
        }

    // Konstruktor standardowy dla Special_function_window
        public Special_function_window()
        {
            names = new List<string>();

            InitializeComponent();
        }

    // Obsługa blokowania dostępu do funkcji zaawansowanych wymagających danych z katalogu przy starcie Special_function_window
        private void Special_function_window_Load(object sender, EventArgs e)
        {
            if (items_to_work_on.Count == 0)
            {
                BT_extract_from_images.Enabled = false;
                BT_compare_audio_files.Enabled = false;
                BT_process_image.Enabled = false;
                BT_search_catalog.Enabled = false;
            } else
            {
                BT_extract_from_images.Enabled = true;
                BT_compare_audio_files.Enabled = true;
                BT_process_image.Enabled = true;
                BT_search_catalog.Enabled = true;
            }
        }

    // Obsługa ładowania danych dla wybranej opcji, w zależności od wartości zm. index zmienia typ zwracanych danych.
        private object prepare_data(List<string> extensions, int index)
        {
            object result = null;

            // Tutaj przechowujemy kolejno części SELECT, FROM i WHERE dla naszego zapytania SQL wyłuskującego właściwości plików w bazie.
            string[] file_query = new string[3];
            
            // Definicje możliwych formatów zwrotu danych z funkcji:
            List<Tuple<int, string>> result_legacy = null;
            List<Tuple<int, string, long>> result_image = null;
            List<Tuple<int, string>> result_search = null;

            // Kod legacy dla części Karola i Janka
            if (index < 2)
            {
                file_query[0] = "SELECT ID,PATH,NAME,EXTENSION ";
                result_legacy = new List<Tuple<int, string>>();
            }
            // Kod dla części Kuby - P-hash.
            if (index == 2)
            {
                file_query[0] = "SELECT ID,PATH,NAME,EXTENSION,PHASH ";
                result_image = new List<Tuple<int, string, long>>();
            }
            // Kod dla częśći Kuby - przeszukiwanie katalogu.
            if (index == 3)
            {
                file_query[0] = "SELECT ID,PATH,NAME,EXTENSION,";
                foreach(var parameter in this.search_parameters)
                {
                    if(!((parameter.Equals("NAME") || parameter.Equals("EXTENSION")))) file_query[0] += parameter + ",";
                }
                file_query[0] = file_query[0].TrimEnd(',');

                result_search = new List<Tuple<int, string>>();
            }

            List<ListViewItem> file_list = new List<ListViewItem>();
            List<ListViewItem> folder_list = new List<ListViewItem>();

            folder_list = items_to_work_on.FindAll(x => x.SubItems[1].Text.Equals("Folder"));
            file_list = items_to_work_on.FindAll((x => !x.SubItems[1].Text.Equals("Folder")));

            while (file_list.Count != 0)
            {
                ListViewItem current = file_list.First();

                if (extensions.Exists(x => x.Equals(current.SubItems[1].Text)))
                {
                    DataTable file_path_container = new DataTable();

                    file_query[1] = " FROM " + current.ToolTipText + " ";
                    if (index != 3) file_query[2] = "WHERE ID = @Id;";
                    else
                    {
                        // Podczas przeszukiwania katalogu dajemy własny file_query[2].
                        file_query[2] = "WHERE ID = @Id";
                        foreach (var query_component in search_payload)
                        {
                            file_query[2] += query_component;
                        }
                    }

                    FbDataAdapter file_path_grabber = new FbDataAdapter(file_query[0] +
                                                                        file_query[1] +
                                                                        file_query[2]
                                                                        ,
                                                                        new FbConnection(DB_connection_string));

                    file_path_grabber.SelectCommand.Parameters.AddWithValue("@Id", int.Parse(current.Name));

                    if (index == 3)
                    {
                        foreach (var parameter_value in search_parameters_values)
                        {
                            if (parameter_value.Item2 == 1) file_path_grabber.SelectCommand.Parameters.AddWithValue(parameter_value.Item1, parameter_value.Item3);
                            else file_path_grabber.SelectCommand.Parameters.AddWithValue(parameter_value.Item1, int.Parse(parameter_value.Item3));
                        }
                    }
                   

                    file_path_container.Clear();
                    file_path_grabber.Fill(file_path_container);

                    if (file_path_container.Rows.Count == 1)
                    {
                        // Kod legacy dla części Karola i Janka
                        if (index < 2)
                        {
                            result_legacy.Add(new Tuple<int, string>((int)file_path_container.Rows[0].ItemArray[0],
                                                                     (string)file_path_container.Rows[0].ItemArray[1]));
                        }
                        // Kod dla części Kuby - P-hash.
                        if (index == 2)
                        {
                            result_image.Add(new Tuple<int, string, long>((int)file_path_container.Rows[0].ItemArray[0],
                                                                          (string)file_path_container.Rows[0].ItemArray[1],
                                                                          (Int64)file_path_container.Rows[0].ItemArray[4]));
                        }
                        // Kod dla częśći Kuby - przeszukiwanie katalogu.
                        if(index == 3)
                        {
                            result_search.Add(new Tuple<int, string>(0,
                                                                     (string)file_path_container.Rows[0].ItemArray[2] + (string)file_path_container.Rows[0].ItemArray[3]));
                        }

                        names.Add((string)file_path_container.Rows[0].ItemArray[2] +
                                  (string)file_path_container.Rows[0].ItemArray[3]);
                    }
                }
                file_list.Remove(current);
            }

            while (folder_list.Count != 0)
            {
                ListViewItem current = folder_list.First();

                DataTable database_folder_subdirectories = new DataTable();
                DataTable database_folder_content = new DataTable();
                // Tworzymy adapter i podpinamy do niego zapytanie SQL wyłuskujące nam wszystkie tabele w bazie (oprócz domyślnych tabel systemu bazodanowego)
                FbDataAdapter database_grab_directory_subdirectories = new FbDataAdapter("SELECT ID,NAME " +
                                                                                         "FROM virtual_folder " +
                                                                                         "WHERE DIR_ID = @Target_directory_id;"
                                                                                         ,
                                                                                         new FbConnection(DB_connection_string));

                database_grab_directory_subdirectories.SelectCommand.Parameters.AddWithValue("@Target_directory_id", int.Parse(current.Name));

                database_grab_directory_subdirectories.Fill(database_folder_subdirectories);

                if (database_folder_subdirectories.Rows.Count != 0)
                // Znaleziono co najmniej jeden podfolder w przeszukiwanym folderze!
                {
                    for (int i = 0; i < database_folder_subdirectories.Rows.Count; i++)
                    {
                        ListViewItem item_to_add = new ListViewItem();
                        item_to_add.Name = ((int)database_folder_subdirectories.Rows[i].ItemArray[0]).ToString();
                        item_to_add.Text = (string)database_folder_subdirectories.Rows[i].ItemArray[1];
                        item_to_add.SubItems.Add("Folder");
                        item_to_add.SubItems.Add("---");
                        item_to_add.SubItems.Add("---");
                        item_to_add.SubItems.Add("---");
                        item_to_add.SubItems.Add("---");
                        folder_list.Add(item_to_add);
                    }
                }

                // Bierzemy teraz zawartość foldera przeszukiwanego.
                for (int i = 1; i < database_tables.Count; i++)
                {
                    // Specjalny przypadek dla obrazów - patrzymy tylko do tabeli, gdzie są pliki graficzne.
                    if (index == 2)
                    {
                        if (i <= database_tables.Find(x => x.Item2.Equals("metadata_image")).Item1) i = database_tables.Find(x => x.Item2.Equals("metadata_image")).Item1;
                        else break;
                    }

                    // W wszystkim oprócz przeszukiwania katalogu chcemy mieć przesiew po wartości rozszerzeń
                    if (index < 3)
                    {
                        string extensions_part = "WHERE DIR_ID = @Target_directory_id AND (";

                        foreach (string extension in extensions)
                        {
                            extensions_part = extensions_part + "EXTENSION = \'" + extension + "\' OR ";
                        }

                        extensions_part = extensions_part.Remove(extensions_part.Count() - 4);
                        extensions_part = extensions_part + ");";

                        file_query[2] = extensions_part;
                    }

                    // Podczas przeszukiwania katalogu skaczemy od razu do interesującej tabeli
                    if (index == 3)
                    {
                        if (target_table != 6)
                        {
                            if (i <= database_tables.Find(x => x.Item1 == target_table).Item1)
                            {
                                i = database_tables.Find(x => x.Item1 == target_table).Item1;
                            }
                            else break;
                        }

                        if (file_query[2] == null)
                        {
                            file_query[2] += "WHERE DIR_ID = @Target_directory_id";

                            foreach (var query_component in search_payload)
                            {
                                file_query[2] += query_component;
                            }
                        }
                    }

                    file_query[1] = " FROM " + database_tables[i].Item2 + " ";


                    FbDataAdapter database_grab_directory_content = new FbDataAdapter(file_query[0] +
                                                                                      file_query[1] +
                                                                                      file_query[2]
                                                                                      ,
                                                                                      new FbConnection(DB_connection_string));

                    database_grab_directory_content.SelectCommand.Parameters.AddWithValue("@Target_directory_id", int.Parse(current.Name));

                    if (index == 3)
                    {
                        foreach (var parameter_value in search_parameters_values)
                        {
                            if (parameter_value.Item2 == 1) database_grab_directory_content.SelectCommand.Parameters.AddWithValue(parameter_value.Item1, parameter_value.Item3);
                            else database_grab_directory_content.SelectCommand.Parameters.AddWithValue(parameter_value.Item1, int.Parse(parameter_value.Item3));
                        }
                    }

                    database_folder_content.Clear();
                    database_grab_directory_content.Fill(database_folder_content);

                    for (int j = 0; j < database_folder_content.Rows.Count; j++)
                    {
                        // Kod legacy dla części Karola i Janka
                        if (index < 2)
                        {
                            result_legacy.Add(new Tuple<int, string>((int)database_folder_content.Rows[j].ItemArray[0],
                                                                     (string)database_folder_content.Rows[j].ItemArray[1]));
                        }
                        // Kod dla części Kuby - P-hash.
                        if (index == 2)
                        {
                            result_image.Add(new Tuple<int, string, long>((int)database_folder_content.Rows[j].ItemArray[0],
                                                                          (string)database_folder_content.Rows[j].ItemArray[1],
                                                                          (Int64)database_folder_content.Rows[j].ItemArray[4]));
                        }
                        // Kod dla częśći Kuby - przeszukiwanie katalogu.
                        if (index == 3)
                        {
                            result_search.Add(new Tuple<int, string>(0,
                                                                     (string)database_folder_content.Rows[j].ItemArray[2] + (string)database_folder_content.Rows[j].ItemArray[3]));
                        }

                        names.Add((string)database_folder_content.Rows[j].ItemArray[2] +
                                  (string)database_folder_content.Rows[j].ItemArray[3]);
                        /*
                        result.Add(new Tuple<int, string>((int)database_folder_content.Rows[j].ItemArray[0],
                                                          (string)database_folder_content.Rows[j].ItemArray[1]));
                        
                        names.Add((string)database_folder_content.Rows[j].ItemArray[3] +
                                  (string)database_folder_content.Rows[j].ItemArray[2]);
                        */
                    }
                }
                // Po wyłuskaniu zawartości usuwamy folder z listy do przeszukiwania.
                folder_list.Remove(current);
            }

            if (index < 2) result = result_legacy;
            if (index == 2) result = result_image;
            if (index == 3) result = result_search;

            return result;
        }

    //Obsługa wywołania poszczególnych okien potomnych
        // Wywołanie części Karola i przekazanie do niej danych.
        private void BT_extract_from_images_Click(object sender, EventArgs e)
        {
            // Wywołuje część Karola przekazując do niej odpowiednio przygotowaną listę argumentów z plików wybranych w katalogu.
            List<Tuple<int, string>> data_to_feed = new List<Tuple<int, string>>();
            List<string> extensions_to_extract = new List<string>();
            extensions_to_extract.Add(".avi");
            extensions_to_extract.Add(".mp4");
            data_to_feed = prepare_data(extensions_to_extract, 0) as List<Tuple<int,string>>;
            if (data_to_feed.Count() == 0) MessageBox.Show("Wybrany zakres nie zawiera plików, które można poddawać tej analizie (tylko dla plików .avi)!");
            else
            {
                extractor_window = new Karol_main();
                extractor_window.Owner = this;
                extractor_window.names = names;
                extractor_window.filepaths = data_to_feed;
                extractor_window.program_path = program_path;
                extractor_window.display_refresh();
                extractor_window.OnDataAvalible += new EventHandler(Karol_main_OnDataAvalible);
                extractor_window.Show();
                Controls_set_lock(true);
            }
        }

        // Wywołanie części Janka i przekazanie do niej danych.
        private void BT_compare_audio_files_Click(object sender, EventArgs e)
        {
            // Wywołuje część Janka, przekazując do niej odpowiednio przygotowaną listę argumentów z plików wybranych w katalogu.
            List<Tuple<int, string>> data_to_feed = new List<Tuple<int, string>>();
            List<string> extensions_to_extract = new List<string>();
            extensions_to_extract.Add(".mp3");
            extensions_to_extract.Add(".wav");
            data_to_feed = prepare_data(extensions_to_extract, 1) as List<Tuple<int, string>>;
            switch(data_to_feed.Count)
            {
                case 0:
                    MessageBox.Show("Wybrany zakres nie zawiera plików, które można poddawać tej analizie (tylko dla plików .mp3 i .wav)!");
                    break;
                case 1:
                    MessageBox.Show("Wybrany zakres zawiera tylko jeden plik, który można poddawać tej analizie (tylko dla plików .mp3 i .wav)!");
                    break;
                default:
                    audio_sorter_window = new Janek_main();
                    audio_sorter_window.Owner = this;
                    audio_sorter_window.names = data_to_feed;
                    audio_sorter_window.OnDataAvalible += new EventHandler(Janek_main_OnDataAvalible);
                    audio_sorter_window.Show();
                    Controls_set_lock(true);
                    break;
            }
        }

        // Wywołanie części Kuby i przekazanie do niej danych.
        private void BT_process_image_Click(object sender, EventArgs e)
        {
            // Wywołuje wyszukiwarkę obrazów po P-hash'u, przekazując do niej odpowiednio przygotowaną listę argumentów z plików wybranych w katalogu.
            List<Tuple<int, string, long>> data_to_feed = new List<Tuple<int, string, long>>();
            List<string> extensions_to_extract = new List<string>();
            extensions_to_extract.Add(".jpg");
            extensions_to_extract.Add(".jpeg");
            extensions_to_extract.Add(".tiff");
            extensions_to_extract.Add(".bmp");
            data_to_feed = prepare_data(extensions_to_extract, 2) as List<Tuple<int, string, long>>;
            switch(data_to_feed.Count)
            {
                case 0:
                    MessageBox.Show("Wybrany zakres nie zawiera plików, które można poddawać tej analizie (tylko dla plików .jpg, .jpeg, .tiff, .bmp)!");
                    break;
                case 1:
                    MessageBox.Show("Wybrany zakres zawiera tylko jeden plik, który można poddawać tej analizie (tylko dla plików .jpg, .jpeg, .tiff, .bmp)!");
                    break;
                default:
                    image_processing_window = new Obrazy_main();
                    image_processing_window.Owner = this;
                    image_processing_window.names = names;
                    image_processing_window.data = data_to_feed;
                    image_processing_window.OnDataAvalible += new EventHandler(Obrazy_main_OnDataAvalible);
                    image_processing_window.Show();
                    Controls_set_lock(true);
                    break;
            }     
        }

        // Wywołanie wyszukiwarki i przekazanie do niej danych.
        private void BT_search_catalog_Click(object sender, EventArgs e)
        {
            List<Tuple<int, string>> data_to_feed = new List<Tuple<int, string>>();
            List<string> extensions_to_extract = new List<string>();

            searcher_window = new Wyszukiwarka_main();
            searcher_window.Owner = this;
            searcher_window.OnDataAvalible += new EventHandler(Wyszukiwarka_main_OnDataAvalible);
            searcher_window.Show();
            Controls_set_lock(true);
        }

    // Blokuje kontrolker okna, true powoduje zablokowanie, false odblokowanie.
        public void Controls_set_lock(bool lock_status)
        {
            Enabled = !lock_status;
            BT_compare_audio_files.Enabled = !lock_status;
            BT_extract_from_images.Enabled = !lock_status;
            BT_process_image.Enabled = !lock_status;
            BT_search_catalog.Enabled = !lock_status;
            if (lock_status == true) Hide();
            else Show();
        }

        private void Special_function_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Owner.Show();
        }
    }
}


/* Kod legacy
        private List<Tuple<int, string>> prepare_data_legacy(List<string> extensions)
        {
            List<Tuple<int, string>> result = new List<Tuple<int, string>>();

            List<ListViewItem> file_list = new List<ListViewItem>();
            List<ListViewItem> folder_list = new List<ListViewItem>();

            folder_list = items_to_work_on.FindAll(x => x.SubItems[1].Text.Equals("Folder"));
            file_list = items_to_work_on.FindAll((x => !x.SubItems[1].Text.Equals("Folder")));

            while (file_list.Count != 0)
            {
                ListViewItem current = file_list.First();

                if (extensions.Exists(x => x.Equals(current.SubItems[1].Text)))
                {
                    DataTable file_path_container = new DataTable();
                    FbDataAdapter file_path_grabber = new FbDataAdapter("SELECT ID,PATH,NAME,EXTENSION " +
                                                                        "FROM " + current.ToolTipText + " " +
                                                                        "WHERE ID = @Id;"
                                                                        ,
                                                                        new FbConnection(DB_connection_string));

                    file_path_grabber.SelectCommand.Parameters.AddWithValue("@Id", int.Parse(current.Name));

                    file_path_container.Clear();
                    file_path_grabber.Fill(file_path_container);

                    if (file_path_container.Rows.Count == 1)
                    {
                        result.Add(new Tuple<int, string>((int)file_path_container.Rows[0].ItemArray[0],
                                                          (string)file_path_container.Rows[0].ItemArray[1]));
                        names.Add((string)file_path_container.Rows[0].ItemArray[2] +
                                  (string)file_path_container.Rows[0].ItemArray[3]);
                    }
                }
                file_list.Remove(current);
            }

            while (folder_list.Count != 0)
            {
                ListViewItem current = folder_list.First();

                DataTable database_folder_subdirectories = new DataTable();
                DataTable database_folder_content = new DataTable();
                // Tworzymy adapter i podpinamy do niego zapytanie SQL wyłuskujące nam wszystkie tabele w bazie (oprócz domyślnych tabel systemu bazodanowego)
                FbDataAdapter database_grab_directory_subdirectories = new FbDataAdapter("SELECT ID,NAME " +
                                                                                         "FROM virtual_folder " +
                                                                                         "WHERE DIR_ID = @Target_directory_id;"
                                                                                         ,
                                                                                         new FbConnection(DB_connection_string));

                database_grab_directory_subdirectories.SelectCommand.Parameters.AddWithValue("@Target_directory_id", int.Parse(current.Name));

                database_grab_directory_subdirectories.Fill(database_folder_subdirectories);

                if (database_folder_subdirectories.Rows.Count != 0)
                // Znaleziono co najmniej jeden podfolder w przeszukiwanym folderze!
                {
                    for (int i = 0; i < database_folder_subdirectories.Rows.Count; i++)
                    {
                        ListViewItem item_to_add = new ListViewItem();
                        item_to_add.Name = ((int)database_folder_subdirectories.Rows[i].ItemArray[0]).ToString();
                        item_to_add.Text = (string)database_folder_subdirectories.Rows[i].ItemArray[1];
                        item_to_add.SubItems.Add("Folder");
                        item_to_add.SubItems.Add("---");
                        item_to_add.SubItems.Add("---");
                        item_to_add.SubItems.Add("---");
                        item_to_add.SubItems.Add("---");
                        folder_list.Add(item_to_add);
                    }
                }
                // Bierzemy teraz zawartość foldera przeszukiwanego.
                for (int i = 1; i < database_tables.Count; i++)
                {
                    string extensions_part = "";

                    foreach (string extension in extensions)
                    {
                        extensions_part = extensions_part + "EXTENSION = \'" + extension + "\' OR ";
                    }

                    extensions_part = extensions_part.Remove(extensions_part.Count() - 4);

                    FbDataAdapter database_grab_directory_content = new FbDataAdapter("SELECT ID,PATH,EXTENSION,NAME " +
                                                                    "FROM " + database_tables[i].Item2 + " " +
                                                                    "WHERE DIR_ID = @Target_directory_id AND (" +
                                                                    extensions_part +
                                                                    ");"
                                                                    ,
                                                                    new FbConnection(DB_connection_string));

                    database_grab_directory_content.SelectCommand.Parameters.AddWithValue("@Target_directory_id", int.Parse(current.Name));

                    database_folder_content.Clear();
                    database_grab_directory_content.Fill(database_folder_content);

                    for (int j = 0; j < database_folder_content.Rows.Count; j++)
                    {
                        result.Add(new Tuple<int, string>((int)database_folder_content.Rows[j].ItemArray[0],
                                                          (string)database_folder_content.Rows[j].ItemArray[1]));
                        names.Add((string)database_folder_content.Rows[j].ItemArray[3] +
                                  (string)database_folder_content.Rows[j].ItemArray[2]);
                    }
                }
                // Po wyłuskaniu zawartości usuwamy folder z listy do przeszukiwania.
                folder_list.Remove(current);
            }
            return result;
        }
        */
