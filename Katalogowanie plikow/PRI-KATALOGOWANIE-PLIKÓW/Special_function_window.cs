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
        public List<ListViewItem> items_to_work_on;
        public List<Tuple<int,string>> database_tables;
        public string DB_connection_string;
        public string program_path;
        public int working_directory;

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

        private List<string> prepare_data_for_multimedia_extraction()
        {
            List<string> result = new List<string>();
            List<ListViewItem> file_list = new List<ListViewItem>();
            List<ListViewItem> folder_list = new List<ListViewItem>();

            folder_list = items_to_work_on.FindAll(x => x.SubItems[1].Text.Equals("Folder"));
            file_list = items_to_work_on.FindAll((x => !x.SubItems[1].Text.Equals("Folder")));

            while (file_list.Count != 0)
            {
                ListViewItem current = file_list.First();

                if (current.SubItems[1].Text.Equals(".avi") || current.SubItems[1].Text.Equals(".wmv"))
                {
                    DataTable file_path_container = new DataTable();
                    FbDataAdapter file_path_grabber = new FbDataAdapter("SELECT PATH " +
                                                                        "FROM " + current.ToolTipText + " " +
                                                                        "WHERE ID = " + int.Parse(current.Name) + ";"
                                                                        ,
                                                                        new FbConnection(DB_connection_string));

                    file_path_container.Clear();
                    file_path_grabber.Fill(file_path_container);

                    if (file_path_container.Rows.Count == 1)
                    {
                        result.Add((string)file_path_container.Rows[0].ItemArray[0]);
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
                                                                                         "WHERE DIR_ID = " + int.Parse(current.Name) + ";"
                                                                                         ,
                                                                                         new FbConnection(DB_connection_string));

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
                    FbDataAdapter database_grab_directory_content = new FbDataAdapter("SELECT PATH,EXTENSION " +
                                                                    "FROM " + database_tables[i].Item2 + " " +
                                                                    "WHERE DIR_ID = " + int.Parse(current.Name) + ";"
                                                                    ,
                                                                    new FbConnection(DB_connection_string));

                    database_folder_content.Clear();
                    database_grab_directory_content.Fill(database_folder_content);

                    for (int j = 0; j < database_folder_content.Rows.Count; j++)
                    {
                        if(database_folder_content.Rows[j].ItemArray[1].Equals(".avi") ||
                           database_folder_content.Rows[j].ItemArray[1].Equals(".wmv"))
                        result.Add((string)database_folder_content.Rows[j].ItemArray[0]);
                    }
                }
                // Po wyłuskaniu zawartości usuwamy folder z listy do przeszukiwania.
                folder_list.Remove(current);
            }
            return result;
        }

        private void BT_extract_from_images_Click(object sender, EventArgs e)
        {
            List<string> data_to_feed = new List<string>();
            data_to_feed = prepare_data_for_multimedia_extraction();
            if (data_to_feed.Count == 0) MessageBox.Show("Wybrany zakres nie zawiera plików, które można poddawać tej analizie (tylko dla plików .avi)!");
            else
            {
                Karol_main extractor_window = new Karol_main();
                extractor_window.filenames = data_to_feed;
                extractor_window.program_path = program_path;
                extractor_window.refresh_display();
                extractor_window.Show();
            }
            // Wywołuje część Karola przekazując do niej odpowiednio przygotowaną listę argumentów z plików wybranych w katalogu.
        }

        private void BT_compare_audio_files_Click(object sender, EventArgs e)
        {

        }

        private void BT_search_catalog_Click(object sender, EventArgs e)
        {

        }
    }
}
