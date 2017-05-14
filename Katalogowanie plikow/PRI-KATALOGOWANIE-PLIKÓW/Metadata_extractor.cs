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
        public List<string[]> metadata_extracted { get; set; }
        public long max_file_size_bytes { get; set; }
        public int file_total_count { get; set; }
        public int file_supported_count { get; set; }
        public int current_file { get; set; }
        public int percent_done { get; set; }
        public float percent_value { get; set; }
        public List<Tuple<int, int, string>> text_ordering_set;
        public List<Tuple<int, int, string>> document_ordering_set;
        public List<Tuple<int, int, string>> complex_ordering_set;
        public List<Tuple<int, int, string>> image_ordering_set;
        public List<Tuple<int, int, string>> multimedia_ordering_set;

        public Metadata_extractor()
        {
            InitializeComponent();
            // Debug - najlepiej byłoby to przesyłać w trakcie startu Metadata_extractora z Form1, które brałoby je z jakiegoś pliku konfiguracyjnego/definicyjnego
            text_ordering_set = new List<Tuple<int, int, string>>();
            document_ordering_set = new List<Tuple<int, int, string>>();
            complex_ordering_set = new List<Tuple<int, int, string>>();
            image_ordering_set = new List<Tuple<int, int, string>>();
            multimedia_ordering_set = new List<Tuple<int, int, string>>();
            load_ordering_sets();
            // \Debug
            BGW_metadata_extractor.WorkerReportsProgress = true;
            BGW_metadata_extractor.WorkerSupportsCancellation = true;
            max_file_size_bytes = (64 * 1024 * 1024); // Maksymalny rozmiar w bajtach pliku poddawanego katalogowaniu, na razie ustaliłem na sztywno 64 MB.
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
                List<string[]> result = new List<string[]>();
                int file_total_count = 0, file_supported_count = 0, current_file = 0;

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
                            List<string> extracted_metadata_container = new List<string>();
                            string[] extracted_metadata_string_container = new string[0];
                            file_supported_count++;


                            // Podstawowe i uniwersalne metadane:
                            extracted_metadata_container.Add(file.Name);
                            extracted_metadata_container.Add(file.Extension);
                            extracted_metadata_container.Add(file.FullName);
                            extracted_metadata_container.Add("" + file.Length);
                            extracted_metadata_container.Add(file.CreationTime.ToString());
                            extracted_metadata_container.Add(file.LastWriteTime.ToString());
                            

                            // Metadane plików tekstowych ekstrahujemy tutaj
                            if (file.Extension == ".txt" ||
                                file.Extension == ".csv" ||
                                file.Extension == ".tsv" ||
                                file.Extension == ".fb2" ||
                                file.Extension == ".xml"
                                )
                            {
                                extracted_metadata_container.AddRange(extract_metadata(file, text_ordering_set));
                                extracted_metadata_string_container = new string[extracted_metadata_container.Count + 1];
                                extracted_metadata_string_container[0] = "metadata_text";
                                for(int j = 1; j <= extracted_metadata_container.Count; j++)
                                {
                                    extracted_metadata_string_container[j] = extracted_metadata_container[j - 1];
                                }
                            }
                            // Specjalny przypadek - pliki .doc - mogą być traktowane albo jak tekstówki, albo jak dokumenty
                            if (file.Extension == ".doc")
                            {
                                var test_extract = extract_metadata(file, document_ordering_set);
                                if (test_extract.First().Contains("text/plain;"))
                                {
                                    extracted_metadata_container.AddRange(extract_metadata(file, text_ordering_set));
                                    extracted_metadata_string_container = new string[extracted_metadata_container.Count + 1];
                                    extracted_metadata_string_container[0] = "metadata_text";
                                    for (int j = 1; j <= extracted_metadata_container.Count; j++)
                                    {
                                        extracted_metadata_string_container[j] = extracted_metadata_container[j - 1];
                                    }
                                }
                                else
                                {
                                    extracted_metadata_container.AddRange(test_extract);
                                    extracted_metadata_string_container = new string[extracted_metadata_container.Count + 1];
                                    extracted_metadata_string_container[0] = "metadata_document";
                                    for (int j = 1; j <= extracted_metadata_container.Count; j++)
                                    {
                                        extracted_metadata_string_container[j] = extracted_metadata_container[j - 1];
                                    }
                                }
                            }
                            // Metadane dokumentów ekstrahujemy tutaj
                            if (file.Extension == ".docx" ||
                                file.Extension == ".odt" ||
                                file.Extension == ".ods" ||
                                file.Extension == ".odp" ||
                                file.Extension == ".xls" ||
                                file.Extension == ".xlsx" ||
                                file.Extension == ".pdf" ||
                                file.Extension == ".ppt" ||
                                file.Extension == ".pptx"
                                )
                            {
                                extracted_metadata_container.AddRange(extract_metadata(file, document_ordering_set));
                                extracted_metadata_string_container = new string[extracted_metadata_container.Count + 1];
                                extracted_metadata_string_container[0] = "metadata_document";
                                for (int j = 1; j <= extracted_metadata_container.Count; j++)
                                {
                                    extracted_metadata_string_container[j] = extracted_metadata_container[j - 1];
                                }
                            }
                            // Metadane plików .htm i .html ekstrahujemy tutaj
                            if (file.Extension == ".htm" ||
                                file.Extension == ".html"
                                )
                            {
                                extracted_metadata_container.AddRange(extract_metadata(file, complex_ordering_set));
                                extracted_metadata_string_container = new string[extracted_metadata_container.Count + 1];
                                extracted_metadata_string_container[0] = "metadata_complex";
                                for (int j = 1; j <= extracted_metadata_container.Count; j++)
                                {
                                    extracted_metadata_string_container[j] = extracted_metadata_container[j - 1];
                                }
                            }
                            // Metadane obrazków/zdjęć ekstrahujemy tutaj
                            if (file.Extension == ".jpg" ||
                                file.Extension == ".jpeg" ||
                                file.Extension == ".tiff" ||
                                file.Extension == ".bmp"
                                )
                            {
                                extracted_metadata_container.AddRange(extract_metadata(file, image_ordering_set));
                                extracted_metadata_string_container = new string[extracted_metadata_container.Count + 1];
                                extracted_metadata_string_container[0] = "metadata_image";
                                for (int j = 1; j <= extracted_metadata_container.Count; j++)
                                {
                                    extracted_metadata_string_container[j] = extracted_metadata_container[j - 1];
                                }
                            }
                            // Metadane plików multimedialnych ekstrahujemy tutaj
                            if (file.Extension == ".mp4" ||
                                file.Extension == ".avi" ||
                                file.Extension == ".mp3" ||
                                file.Extension == ".wav"
                                )
                            {
                                extracted_metadata_container.AddRange(extract_metadata(file, multimedia_ordering_set));
                                extracted_metadata_string_container = new string[extracted_metadata_container.Count + 1];
                                extracted_metadata_string_container[0] = "metadata_multimedia";
                                for (int j = 1; j <= extracted_metadata_container.Count; j++)
                                {
                                    extracted_metadata_string_container[j] = extracted_metadata_container[j - 1];
                                }
                            }

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
                PB_extraction_progress.Value = (int)Math.Floor((float)current_file / percent_value);
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

        /* Ekstrakcja metadanych i parsowanie ich do postaci przyswajalnej przez SQL'a
         * 
         * Za pomocą wyznaczonych analizą przypadków zbiorów porządkujących przeczesujemy wygenerowane przez ekstraktor metadane pliku. 
         * Wyłuskujemy spośród nich te z nich które nas interesują (czyt. są zdefiniowane w zbiorze porządkującym) i patrzymy która wartość powtarza się w nich
         * najczęściej (z wyłączeniem wartości NULL, nie chcemy żeby nadpisały dane z ekstrakcji).
         * 
         * Używane struktury danych (niestandardowe):
         * 1. Tuple<int,int,string> - używany w parametrze ordering_set przekazywanym do funkcji extract_metadata.
         * 
         * Jest zbudowana następująco:
         * ID typu int, w kodzie oznaczony Item1 - jest to numer przyporządkowany zadanemu typowi metadanych unikatowo, stanowi informacje o tym, w której kolumnie tabeli bazy danych będzie.
         * Number_of_aliases typu int, w kodzie oznaczony Item2 - ilość aliasów (innych nazw na tą samą metadaną) pod którymi dana metadana występuje podczas ekstrakcji. 
         * Alias typu string, w kodzie oznaczony Item3 - zawiera alias który należy identyfikować z metadanymi o ID.
         * 
         * Przekazujemy za jej pomocą informację o tym jakie metadane należy ekstrahować, pod jakimi aliasami występują i jaki ma być ich porządek w wyjściowym
         * stringu.
         * 
         * 2. List<Tuple<int,string> - używana podczas nietrywialnej ekstrakcji metadanych (gdy ilość wyekstrahowanych metadanych > ilość kolumn tabeli, w której mają one wynikowo się znaleść.
         * 
         * Jest ona zbudowana następująco:
         * Jak każda lista, jest ona zbudowana z indywidualnie adresowalnych elementów, ma także parametr Count określający ilość jej elementów.
         * Same jej elementy składają się z:
         * ID typu int, w kodzie oznaczony Item1 - jest to wartość taka sama jak Item1 w Tuple<int,int,string>, identyfikuje przynależność do kolumny zadanej metadanej.
         * Content typu string, w kodzie jako Item2 - jest to wartość metadanej wyekstrahowana przez ekstraktor z pliku.
         * 
         * Lista ta jest absolutnie niezbędna do prawidłowego przebiegu procesu ekstrakcji danych, na jej podstawie konstruowany jest string wynikowy zwracany do Form1.
         * Po każdym przebiegu ekstrakcji dla pliku MUSI skonczyć pusta.
         * 
         * 
         * Proces ekstrakcji składa się z następujących kroków:
         * 
         * 1. Wygeneruj odpowiedniej wielkości kontener na wynik ekstrakcji wszystkich metadanych (tablicę stringów o ilości elementów równej ilości kolumn odpowiedniej tabeli bazy danych)
         * 2. Stwórz resolver używany podczas ekstrakcji nietrywialnej.
         * 3. Zacznij ekstrakcję, jeżeli dana metadana pojawia się więcej niż raz (ale pod innym aliasamem) dodaj ją do resolvera.
         * 4. Po przeczesaniu wszystkich metadanych i dodaniu do kontenera metadanych trywialnych przechodzimy do wybrania najlepszych danych z resolvera.
         * 5. Sortujemy tuple w resolverze po ID (Item1), następnie zdejmujemy je zgodnie z kolejnością (od najmniejszego do największego).
         * 6. Wybieramy jako ostateczną tą wartość metadanej, która powtórzyła się w największej ilości aliasów (z pominięciem wartości NULL, ta często jest najczęstsza).
         * 7. Dodajemy ją do kontenera i usuwamy z resolvera wszystkie tuple o ID które właśnie analizowaliśmy.
         * 8. Kontynuujemy kroki 6 i 7 az lista zostaje opróżniona - od teraz wszystkie dane które wyekstrahował ekstraktor mamy już w kontenerze.
         * 9. Przenosimy zawartość kontenera indeks po indeksie do stringa wynikowego.
         * 10. Zwracamy stringa wynikowego.
         * 
         * Dane w tej postaci są już gotowe do wgrania do bazy danych, ale to robi Form1 - tylko on ma połączenie z bazą!
        */

        private string[] extract_metadata(FileInfo file, List< Tuple<int,int,string> > ordering_set)
        {

            TikaOnDotNet.TextExtraction.TextExtractor extractor = new TikaOnDotNet.TextExtraction.TextExtractor();
            string[] result_container = new string[0];
            if (ordering_set.Count != 0) result_container = new string[ordering_set[ordering_set.Count - 1].Item1 + 1];

            for (int i = 0; i < result_container.Length; i++) result_container[i] = "NULL";

            List< Tuple<int,string> > metadata_final_value_resolver = new List<Tuple<int, string>>();

            int place = -1, position = 0;

            var extract = extractor.Extract(file.FullName);
            foreach (var extracted_metadata in extract.Metadata)
            {
                for(int i = 0; i < ordering_set.Count; i++)
                {
                    if (extracted_metadata.Key.Equals(ordering_set[i].Item3))
                    {
                        place = ordering_set[i].Item1;
                        position = i;
                    }
                }
                if (place != -1)
                {
                    if (ordering_set[position].Item2 > 1)
                    {
                        if (!(extracted_metadata.Value.Equals(""))) metadata_final_value_resolver.Add(new Tuple<int, string>
                                                                                                     (ordering_set[position].Item1,
                                                                                                      extracted_metadata.Value.ToString()));

                    }
                    else
                    {
                        if (!(extracted_metadata.Value.Equals(""))) result_container[place] = extracted_metadata.Value;
                    }
                    place = -1;
                }
            }

            if (metadata_final_value_resolver.Count != 0) {
                metadata_final_value_resolver.Sort();

                int current_index = 0;

                while(metadata_final_value_resolver.Count != 0)
                {
                    int options_avalible = 0;
                    string best_match = String.Empty;
                    int[] options_matches = new int[0];

                    current_index = metadata_final_value_resolver.Min().Item1;

                    List<Tuple<int,string>> options = metadata_final_value_resolver.FindAll(x => x.Item1 == current_index);
                    options.RemoveAll(x => x.Item2.Equals("NULL"));
                    if (options.Count == 0)
                    {
                        result_container[current_index] = "NULL";
                    }
                    else
                    {
                        options_avalible = options.Count;
                        options_matches = new int[options_avalible];

                        for (int i = 0; i < options_avalible; i++)
                        {
                            for (int j = 0; j < options_avalible; j++) if (options[i] == options[j]) options_matches[i]++;
                        }

                        best_match = options.Max().Item2;

                        result_container[current_index] = best_match;
                    }
                    metadata_final_value_resolver.RemoveAll(x => x.Item1 == current_index);
                }
            }

            return result_container;
        }

        /* Tworzenie zbiorów porządkowych
         * 
         * Zbiory porządkowe zostały opisane wcześniej.
         * 
         * Ich konkrenta struktura powinna stać się jasna po obejrzeniu struktury bazy danych zdefiniowanej w Form1.
         * Specjalny przypadek zachodzi gdy analizujemy plik .htm/html, u nich porządkowanie nie jest możliwe ze względu na losową kolejność losowych metadanych.
         * Nawet jeżeli w dwóch poszczególnych plikach mamy takie same typy metadanych, niestety nie można zagwarantować że będą one wyekstrahowane w tej samej kolejności.
         * Chociaż teraz wpadł mi do głowy pewien hack - nie zagwarantujemy co prawda kolejności, ale możemy zagwarantować sortowanie of sorts (zależy jak sortuje stringi).
        */

        private void load_ordering_sets()
        {
            if (this.text_ordering_set.Count == 0)
            {
                this.text_ordering_set.Clear();

                this.text_ordering_set.Add(new Tuple<int, int, string>(0, 1, "Content-Type"));

                this.text_ordering_set.Add(new Tuple<int, int, string>(1, 1, "Content-Encoding"));
            }

            if(this.document_ordering_set.Count == 0)
            {
                this.document_ordering_set.Clear();

                this.document_ordering_set.Add(new Tuple<int, int, string>(0, 1, "Content-Type"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(1, 4, "Creation-Date"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(1, 4, "meta:creation-date"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(1, 4, "dcterms:created"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(1, 4, "pdf:docinfo:created"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(2, 4, "Last-Modified"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(2, 4, "modified"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(2, 4, "dcterms:modified"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(2, 4, "pdf:docinfo:modified"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(3, 2, "language"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(3, 2, "dc:language"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(4, 3, "title"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(4, 3, "dc:title"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(4, 3, "pdf:docinfo:title"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(5, 4, "subject"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(5, 4, "cp:subject"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(5, 4, "dc:subject"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(5, 4, "pdf:docinfo:subject"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(6, 2, "description"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(6, 2, "dc:description"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(7, 3, "Keywords"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(7, 3, "meta:keyword"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(7, 3, "pdf:docinfo:keywords"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(8, 3, "Comments"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(8, 3, "w:comments"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(8, 3, "comment"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(9, 2, "publisher"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(9, 2, "dc:publisher"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(10, 2, "Company"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(10, 2, "extended-properties:Company"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(11, 2, "Author"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(11, 2, "meta:author"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(12, 3, "creator"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(12, 3, "dc:creator"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(12, 3, "pdf:docinfo:creator"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(13, 2, "Last-Author"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(13, 2, "meta:last-author"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(14, 6, "Page-Count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(14, 6, "meta:page-count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(14, 6, "Slide-Count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(14, 6, "meta:slide-count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(14, 6, "nbPage"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(14, 6, "xmpTPg:NPages"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(15, 3, "Table-Count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(15, 3, "meta:table-count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(15, 3, "nbTab"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(16, 3, "Object-Count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(16, 3, "meta:object-count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(16, 3, "nbObject"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(17, 3, "Image-Count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(17, 3, "meta:image-count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(17, 3, "nbImg"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(18, 3, "Word-Count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(18, 3, "meta:word-count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(18, 3, "nbWord"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(19, 3, "Character Count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(19, 3, "meta:character-count"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(19, 3, "nbCharacter"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(20, 4, "Application-Name"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(20, 4, "extended-properties:Application"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(20, 4, "xmp:CreatorTool"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(20, 4, "pdf:docinfo:creator_tool"));

                this.document_ordering_set.Add(new Tuple<int, int, string>(21, 2, "Application-Version"));
                this.document_ordering_set.Add(new Tuple<int, int, string>(21, 2, "extended-properties:AppVersion"));
            }

            //Work in progress
            if (this.complex_ordering_set.Count == 0)
            {
                this.complex_ordering_set.Clear();

                this.complex_ordering_set.Add(new Tuple<int, int, string>(0, 1, "Content-Type"));

                this.complex_ordering_set.Add(new Tuple<int, int, string>(1, 1, "Content-Encoding"));

                this.complex_ordering_set.Add(new Tuple<int, int, string>(2, 2, "title"));
                this.complex_ordering_set.Add(new Tuple<int, int, string>(2, 2, "dc:title"));

                //Trzeba jakoś zrobić łapanie 64 pierwszych metadanych, na razie nie beda brane pod uwage podczas katalogowania.
            }

            //Work in progress
            if (this.image_ordering_set.Count == 0)
            {
                this.image_ordering_set.Clear();
                /*
                this.image_ordering_set.Add(new Tuple<int, int, string>(0, 1, "Content-Type"));

                this.image_ordering_set.Add(new Tuple<int, int, string>(1, 1, "Content-Encoding"));

                this.image_ordering_set.Add(new Tuple<int, int, string>(2, 2, "title"));
                this.image_ordering_set.Add(new Tuple<int, int, string>(2, 2, "dc:title"));
                */
                
            }

            //Work in progress
            if (this.multimedia_ordering_set.Count == 0)
            {
                this.multimedia_ordering_set.Clear();
                /*
                this.multimedia_ordering_set.Add(new Tuple<int, int, string>(0, 1, "Content-Type"));

                this.multimedia_ordering_set.Add(new Tuple<int, int, string>(1, 1, "Content-Encoding"));

                this.multimedia_ordering_set.Add(new Tuple<int, int, string>(2, 2, "title"));
                this.multimedia_ordering_set.Add(new Tuple<int, int, string>(2, 2, "dc:title"));
                */
            }



        }
    }
}
