using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PRI_KATALOGOWANIE_PLIKÓW.classes;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class DownloadProgressWindow : Form
    {
        private string file_name;
        private long total_file_size;
        private System.IO.FileInfo filesize_checker;

        public DownloadProgressWindow(string file_name, long total_file_size)
        {
            this.file_name = file_name; 
            this.total_file_size = total_file_size;
            this.filesize_checker = new System.IO.FileInfo(ConfigManager.ReadString(ConfigManager.DOWNLOAD_LOCATION) + file_name);

            InitializeComponent();
        }

        private void DownloadProgressWindow_Load(object sender, EventArgs e)
        {
            // Pobierz maksymalny rozmiar pliku w bajtach i zeskaluj progress bar do jego wartości.
            LB_file_name.Text = "Pobierany plik: " + file_name;

            LB_download_numbers.Text = "Pobrano " + filesize_checker.Length + " z " + total_file_size + " bajtów.";

            PB_download_progress.Maximum = 100;
            PB_download_progress.Value = 0;
        }

        public void DownloadProgressWindow_UpdateValue()
        {
            filesize_checker = new System.IO.FileInfo(ConfigManager.ReadString(ConfigManager.DOWNLOAD_LOCATION) + file_name);
            PB_download_progress.Value = (int)(filesize_checker.Length * 100 / total_file_size);
            LB_download_numbers.Text = "Pobrano " + filesize_checker.Length + " z " + total_file_size + " bajtów.";
        }

        public void DownloadProgressWindow_CheckIfDone()
        {
            filesize_checker = new System.IO.FileInfo(ConfigManager.ReadString(ConfigManager.DOWNLOAD_LOCATION) + file_name);
            if (total_file_size == filesize_checker.Length)
            {
                PB_download_progress.Value = 100;
                LB_download_numbers.Text = "Pobrano cały plik (" + total_file_size + " bajtów).";
            }
        }

        public void DownloadProgressWindow_Kill()
        {
            DownloadProgressWindow_CheckIfDone();

            Timer kill_timer = new Timer();
            kill_timer.Interval = 1000;
            kill_timer.Tick += kill_timer_Tick;

            kill_timer.Start();
        }

        private void kill_timer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
