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

        private Karol_main extractor_window;
        private Janek_main audio_sorter_window;

        // Dane wejściowe do podprogramów
        public List<ListViewItem> items_to_work_on;
        public List<Tuple<int,string>> database_tables;
        public string DB_connection_string;
        public string program_path;
        public int working_directory;

        // Przechowywanie danych wyjściowych z podprogramów.
        public List<Tuple<int, string>> audio_sorter_folders_returned;
        public List<Tuple<int, string>> audio_sorter_files_returned;
        public List<Tuple<int, string, string>> extractor_texts_returned;

        // Parametr informujący o tym co zwracamy do programu głównego.
        public int return_index = 0;

        void Janek_main_OnDataAvalible(object sender, EventArgs e)
        {
            // Obsługa zdarzenia powrotu z formu Janka - dostajemy listę Tuple<int,string> zawierającą ID przydzielone nowemu folderowi (w celach porządkowych)
            // i jego nową nazwę, jak i listę Tuple<int,string> zawierającą indeks do którego przydzielamy plik i jego ścieżkę.
            audio_sorter_folders_returned = new List<Tuple<int, string>>();
            audio_sorter_files_returned = new List<Tuple<int, string>>();

            audio_sorter_folders_returned = audio_sorter_window.result_directories.ToList();
            audio_sorter_files_returned = audio_sorter_window.result_files.ToList();

            return_index = 2;

            // Informujemy Main_form o tym że któraś z opcji zwróciła wynik.
            OnDataAvalible(this, EventArgs.Empty);
            this.Close();
            this.Dispose();
        }
        void Karol_main_OnDataAvalible(object sender, EventArgs e)
        {
            // Obsługa zdarzenia powrotu z formu Karola - dostajemy listę Tuple<string,string> zawierającą nazwę pliku dla którego przeprowadzaliśmy analizę i
            // tekst wyekstrachowany z filmu przydzielony plikowi.

            extractor_texts_returned = new List<Tuple<int, string, string>>();

            extractor_texts_returned = extractor_window.text_extraction_results.ToList();

            return_index = 1;

            // Informujemy Main_form o tym że któraś z opcji zwróciła wynik.
            OnDataAvalible(this, EventArgs.Empty);
            this.Close();
            this.Dispose();
        }

        

        public Special_function_window()
        {
            InitializeComponent();
        }

        private void Special_function_window_Load(object sender, EventArgs e)
        {
            if(items_to_work_on.Count == 0)
            {
                BT_extract_from_images.Enabled = false;
                BT_compare_audio_files.Enabled = false;
            } else
            {
                BT_extract_from_images.Enabled = true;
                BT_compare_audio_files.Enabled = true;
            }
        }

        private List<Tuple<int, string>> prepare_data(List<string> extensions)
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
                    FbDataAdapter file_path_grabber = new FbDataAdapter("SELECT ID,PATH " +
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
                    FbDataAdapter database_grab_directory_content = new FbDataAdapter("SELECT ID,PATH,EXTENSION " +
                                                                    "FROM " + database_tables[i].Item2 + " " +
                                                                    "WHERE DIR_ID = @Target_directory_id;"
                                                                    ,
                                                                    new FbConnection(DB_connection_string));

                    database_grab_directory_content.SelectCommand.Parameters.AddWithValue("@Target_directory_id", int.Parse(current.Name));

                    database_folder_content.Clear();
                    database_grab_directory_content.Fill(database_folder_content);

                    for (int j = 0; j < database_folder_content.Rows.Count; j++)
                    {
                        if(extensions.Exists(x => x.Equals(database_folder_content.Rows[j].ItemArray[2])))
                        result.Add(new Tuple<int, string>((int)database_folder_content.Rows[j].ItemArray[0],
                                                          (string)database_folder_content.Rows[j].ItemArray[1]));
                    }
                }
                // Po wyłuskaniu zawartości usuwamy folder z listy do przeszukiwania.
                folder_list.Remove(current);
            }
            return result;
        }

        private void BT_extract_from_images_Click(object sender, EventArgs e)
        {
            // Wywołuje część Karola przekazując do niej odpowiednio przygotowaną listę argumentów z plików wybranych w katalogu.
            List<Tuple<int, string>> data_to_feed = new List<Tuple<int, string>>();
            List<string> extensions_to_extract = new List<string>();
            extensions_to_extract.Add(".avi");
            extensions_to_extract.Add(".wmv");
            data_to_feed = prepare_data(extensions_to_extract);
            if (data_to_feed.Count() == 0) MessageBox.Show("Wybrany zakres nie zawiera plików, które można poddawać tej analizie (tylko dla plików .avi)!");
            else
            {
                extractor_window = new Karol_main();
                extractor_window.Owner = this;
                extractor_window.filepaths = data_to_feed;
                extractor_window.program_path = program_path;
                extractor_window.display_refresh();
                extractor_window.OnDataAvalible += new EventHandler(Karol_main_OnDataAvalible);
                extractor_window.Show();
                Controls_set_lock(true);
            }
        }

        private void BT_compare_audio_files_Click(object sender, EventArgs e)
        {
            // Wywołuje część Janka, przekazując do niej odpowiednio przygotowaną listę argumentów z plików wybranych w katalogu.
            List<Tuple<int, string>> data_to_feed = new List<Tuple<int, string>>();
            List<string> extensions_to_extract = new List<string>();
            extensions_to_extract.Add(".mp3");
            extensions_to_extract.Add(".wav");
            data_to_feed = prepare_data(extensions_to_extract);
            if (data_to_feed.Count == 0) MessageBox.Show("Wybrany zakres nie zawiera plików, które można poddawać tej analizie (tylko dla plików .mp3 i .wav)!");
            else
            {
                audio_sorter_window = new Janek_main();
                audio_sorter_window.Owner = this;
                audio_sorter_window.names = data_to_feed;
                audio_sorter_window.OnDataAvalible += new EventHandler(Janek_main_OnDataAvalible);
                audio_sorter_window.Show();
                Controls_set_lock(true);
            }
        }

        // Przeszukuje katalog.
        private void BT_search_catalog_Click(object sender, EventArgs e)
        {

        }

        // Blokuje kontrolki okna, true powoduje zablokowanie, false odblokowanie
        public void Controls_set_lock(bool lock_status)
        {
            Enabled = !lock_status;
            BT_compare_audio_files.Enabled = !lock_status;
            BT_extract_from_images.Enabled = !lock_status;
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
