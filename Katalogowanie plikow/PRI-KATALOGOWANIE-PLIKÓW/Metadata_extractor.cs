using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Metadata_extractor : Form
    {
        public string[] extends { get; set; }
        public DirectoryInfo target_directory { get; set; }
        public List<string> metadata_extracted { get; set; }
        public long max_file_size_bytes { get; set; }
        public int file_total_count { get; set; }
        public int file_supported_count { get; set; }
        public int current_file { get; set; }
        public int percent_done { get; set; }
        public float percent_value { get; set; }

        public Metadata_extractor()
        {
            InitializeComponent();
            BGW_metadata_extractor.WorkerReportsProgress = true;
            BGW_metadata_extractor.WorkerSupportsCancellation = true;
            max_file_size_bytes = (64 * 1024 * 1024); // Maksymalny rozmiar pliku poddawanego katalogowaniu, na razie jest to 64 MB
            file_total_count = 0;
            file_supported_count = 0;
            current_file = 0;
            percent_done = 0;
            percent_value = 0.0f;
            BGW_metadata_extractor.RunWorkerAsync();
        }

        /*  Ekstracja metadanych przez BackgroundWorkera
         *  
         *  Zaczynamy tutaj podobnie jak w Jankowym kodzie, z tą różnicą że tworzymy tylko jeden TextExtractor().
         *  Okazuje się że Tika wymaga tylko jednego obiektu TextExtractor na analizowaną zmienna, automatycznie wybierze
         *  podczas jej analizy odpowiednie ekstraktory i przepuści przez nie plik.
         *  
         *  Niestety, za każdym razem ekstraktor otwiera plik, stąd potrzebne jest ograniczenie na ich wielkość. 
         *  
         *  Także niebezpieczna jest sytuacja gdy plik ma złe rozszerzenie - testowałem .zip 50-megowy z zamienioną na .odt nazwą,
         *  program po 80 sekundach ekstrakcji doszedł do tego że to plik .zip, ale niestety niemożliwe jest z tego co wiem najpierw
         *  sprawdzenie prawdziwego typu pliku przed ekstrakcją... Zanim Tika nie spróbuje nie wie jaki to plik, z kolei kiedy spróbuje
         *  jest już za późno.
         *  
         *  Na razie ekstrakcja jest z pojedynczego katalogu wybranego przez użytkownika, ale można dość szybko przerobić to na wiele katalogów,
         *  wystarczy przekazać List<DirectoryInfo> i foreachować dla niej proces wyszukiwania plików. Wymagałaby wtedy modyfikacji część kodu z
         *  liczeniem wszystkich analizowanych danych, ale reszta powinna zachowywać się przyzwoicie.
         * 
         *  Po zakonczeniu ekstrakcji planuje gotową listę zawierająco odpowiednio sformatowane dane zinsertować do bazy danych w odpowiednie tabele
         *  i pola w nich. Formatowanie metadanych chwilowo jest z jednej sztancy, dalsze planuje robić w osobnych funkcjach na koncu tego formularza.
         *  
         *  Samo wyrzucenie do bazy danych może odbywać się albo tutaj (czego nie radzę - Form1 będzie miał stałe połączenie z bazą, lepiej nie tworzyć
         *  nowego, tylko zwrócić do niego dane z których skorzysta), albo w Form1 (za czym z kolei jestem).
         * 
         *  Ekstrahuje tu metadane zarówno z systemu plików, jak i zwrócone z samego extractora.
        */

        private void BGW_extract_metadata(object sender, DoWorkEventArgs e)
        {
            {
                string extracted_metadata_string_container = String.Empty;
                List<string> result = new List<string>();
                int file_total_count = 0, file_supported_count = 0, current_file = 0;

                TikaOnDotNet.TextExtraction.TextExtractor extractor = new TikaOnDotNet.TextExtraction.TextExtractor();

                var files = target_directory.GetFiles("*", SearchOption.AllDirectories);
                file_total_count = files.Count();
                BGW_metadata_extractor.ReportProgress(file_total_count * -1);
                foreach (var file in files)
                {
                    current_file++;
                    BGW_metadata_extractor.ReportProgress(current_file);
                    for (int i = 0; i < extends.Length; i++)
                    {
                        if (file.Extension == extends[i] && file.Length <= max_file_size_bytes)
                        {
                            file_supported_count++;
                            // Podstawowe i uniwersalne metadane:
                            extracted_metadata_string_container = 
                                               "\"" + file.Name + "\" " +
                                               "\"" + file.FullName + "\" " +
                                               "\"" + file.Length + "\" " +
                                               "\"" + file.CreationTime.ToString() + "\" " +
                                               "\"" + file.LastWriteTime.ToString() + "\"";
                            var extract = extractor.Extract(file.FullName);
                            // Ekstrakcja detali z poszczególnych typów plików:
                            foreach (var extracted_metadata in extract.Metadata)
                            {
                                if (extracted_metadata.Key != "X-Parsed-By" &&
                                    extracted_metadata.Key != "FilePath"
                                    ) // Wywalamy tutaj niepotrzebne metadane wspólne dla wszystkich ekstraktorów
                                    extracted_metadata_string_container = extracted_metadata_string_container +
                                        " \"" + extracted_metadata.Key + "\" " +
                                        "\"" + extracted_metadata.Value + "\"";
                            }
                            // Zanim dodamy trzeba wybrać na podstawie rozszerzenia pliku funkcję czyszczącą string z nieporządanych metadanych
                            // Implementacja jest w toku u mnie na kompie, wrzucam tą wersję za Git'a żebyście wiedzieli co robię na teraz-zaraz-już.

                            result.Add(extracted_metadata_string_container);
                            BGW_metadata_extractor.ReportProgress(0);
                        }
                    }
                }
                this.metadata_extracted = result;
                if (BGW_metadata_extractor.CancellationPending == true)
                {
                    e.Cancel = true;
                }
            }
        }

        /*  Update w czasie ekstrakcji metadanych przez BackgroundWorkera
         * 
         *  Tutaj nasz worker zwraca do swojego okna informacje o przebiegu programu.
         *  Nadużywam tutaj trochę możliwości e.ProgressPercentage, w zależności od jego wartości
         *  wykonuje tutaj trzy rzeczy:
         *  1. Jeżeli jest mniejszy od zera to przekazuje w nim ilość wszystkich plików. Dzieje się tak tylko raz.
         *  2. Jeżeli jest większy od zera to ma on za zadanie przeliczyć nową wartość dla ProgressBar'a i ją uaktualnić.
         *  3. Jeżeli jest zerem, to oblicza ile było już poprawnych plików z których bierzemy metadane i zwraca tą wartość do odpowiedniego labela.
         * 
         */

        private void BGW_progress_updated(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 0)
            {
                file_total_count = -1 * e.ProgressPercentage;
                this.LB_file_count_container.Text = file_total_count.ToString();
            }
            if (e.ProgressPercentage > 0)
            {
                current_file = e.ProgressPercentage;
                percent_value = (float)file_total_count / 100;
                PB_extraction_progress.Value = (int)Math.Floor((float)current_file/percent_value);
            }
            if (e.ProgressPercentage == 0)
            {
                file_supported_count++;
                this.LB_file_supported_count_container.Text = file_supported_count.ToString();
            }
        }

        /*  Koniec ekstrakcji metadanych przez BackgroundWorkera
         *  
         *  Program przechodzi tutaj jeżeli zaszło jedno z trzech zdarzeń:
         *  1. Program zakonczył działanie normalnie - wtedy e.Error jest równy null i e.Cancelled to false
         *  2. Program chce zwrócić błąd - wtedy coś jest w e.Error
         *  3. Program został zakonczony przez przycisk - wtedy e.Cancelled jest prawdą.
         *  
         *  Program zwraca dane do Form1 gdy zachodzi przypadek pierwszy i drugi, zwraca albo wszystko, albo tylko częśc przed wystąpieniem błędu.
         *  Same błędy wyskakują jako okienka, nienajlepsze jeżeli miałoby ich być dużo, ale tak póki co jest w kodzie.
         */

        private void BGW_done(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
            }
            if (e.Cancelled == true) this.Close();
            else
            {
                this.LB_metadata_count_container.Text = metadata_extracted.Count().ToString();
                Form1 parent = (Form1)this.Owner;
                parent.metadata = metadata_extracted;
                MessageBox.Show("Ekstrakcja metadanych zakonczona!");
                this.Close();
            }
        }

        private void BT_interrupt_click(object sender, EventArgs e)
        {
            if (BGW_metadata_extractor.WorkerSupportsCancellation == true)
            {
                BGW_metadata_extractor.CancelAsync();
                this.Close();
            }
        }

        //WIP
        //Ekstrakcja poszczególnych typów metadanych
        /*
        private string extract_txt_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_csv_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_tsv_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_doc_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_docx_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_odt_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_ods_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_odp_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_xls_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_xlsx_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_pdf_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_ppt_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_pptx_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_fb2_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_html_metadata(FileInfo file) //obsluguje takze htm
        {
            string result = String.Empty;

            return result;
        }

        private string extract_xml_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_jpeg_metadata(FileInfo file) //obsluguje takze jpeg
        {
            string result = String.Empty;

            return result;
        }

        private string extract_tiff_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_txt_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_bmp_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_mp4_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_avi_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_mp3_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }

        private string extract_wav_metadata(FileInfo file)
        {
            string result = String.Empty;

            return result;
        }
        */
    }
}
