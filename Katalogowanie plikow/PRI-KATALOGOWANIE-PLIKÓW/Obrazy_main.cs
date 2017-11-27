using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Shipwreck.Phash;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Obrazy_main : Form
    {
        public event EventHandler OnDataAvalible;

        //Rozszerzenia przetwarzanych plików
        public static string[] extends = { "jpg", "jpeg", "tiff", "bmp" };

        //Nazwy plików do poddania analizie
        public List<string> names;

        //Dane wejściowe - int jako spadek z ładowania danych, string to ścieżka do pliku, ulong to p-hash.
        public List<Tuple<int, string, long>> data;

        //Zmienne przechowujące wyniki - zarówno foldery jak i pliki wynikowe
        public List<Tuple<int, string>> result_directories;
        public List<Tuple<int, string>> result_files;

        //Cache dla obrazków załadowanych przez program
        private List<Tuple<int,Image>> PB_image_cache;

        //Wybrany spośród wszystkich obraz, do którego będziemy przyrównywali inne
        private string image_selected;
        private int image_selected_index_in_data;

        public Obrazy_main()
        {
            InitializeComponent();

            names = new List<string>();
            data = new List<Tuple<int, string, long>>();

            PB_image_cache = new List<Tuple<int, Image>>();

            result_directories = new List<Tuple<int, string>>();
            result_files = new List<Tuple<int, string>>();

            image_selected = "";
        }

        private void Obrazy_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((Special_function_window)this.Owner).Controls_set_lock(false);
        }

        private void BT_execute_Click(object sender, EventArgs e)
        {
            if (LB_image_selected_name.Text != "")
            {
                int[] counters = new int[4];
                int total_count = 0;
                ulong image_selected_hash = (ulong)data[image_selected_index_in_data].Item3;
                int distance = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    //Nie chcemy porównywać obrazu samego ze sobą
                    if (i != image_selected_index_in_data)
                    {
                        ulong image_analysed_hash = (ulong)data[i].Item3;
                        distance = pHash.ph_hamming_distance(image_selected_hash, image_analysed_hash);

                        if(distance == 0)
                        {
                            result_files.Add(new Tuple<int, string>(0, names[i]));
                            counters[0]++;
                        }

                        if(distance == 1)
                        {
                            result_files.Add(new Tuple<int, string>(1, names[i]));
                            counters[1]++;
                        }

                        if (distance > 1 && distance <= 3)
                        {
                            result_files.Add(new Tuple<int, string>(2, names[i]));
                            counters[2]++;
                        }

                        if (distance > 4)
                        {
                            result_files.Add(new Tuple<int, string>(3, names[i]));
                            counters[3]++;
                        }
                    }
                }
                total_count = counters[0] + counters[1] + counters[2] + counters[3];

                if (counters[0] > 0) result_directories.Add(new Tuple<int, string>(0, "Kopie obrazu " + image_selected));
                if (counters[1] > 0) result_directories.Add(new Tuple<int, string>(1, "Obrazy bardzo podobne do " + image_selected));
                if (counters[2] > 0) result_directories.Add(new Tuple<int, string>(2, "Obrazy podobne do " + image_selected));
                if (counters[3] > 0) result_directories.Add(new Tuple<int, string>(3, "Obrazy niepodobne do " + image_selected));

                MessageBox.Show("Wyniki analizy obrazów w wybranym zbiorze do obrazu " + image_selected + ": \n" +
                                "Liczba zbadanych obrazów: " + total_count + " \n" +
                                "Obrazy takie same jak " + image_selected + ": " + counters[0] + " \n" +
                                "Obrazy bardzo podobne: " + counters[1] + " \n" +
                                "Obrazy w pewnym stopniu podobne: " + counters[2] + " \n" +
                                "Obrazy niepodobne: " + counters[3]);

                OnDataAvalible(this, EventArgs.Empty);
                this.Close();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Proszę wybrać obraz, który będzie porównany do pozostałych na liście.");
            }
        }

        private void Obrazy_main_Load(object sender, EventArgs e)
        {
            for(int i = 0; i < names.Count; i++)
            {
                LB_images.Items.Add(names[i]);
            }
        }

        private void PB_image_draw(int index)
        {
            Image image_to_draw = null;

            // Sprawdzam czy w cache jest poszukiwany obraz
            if (PB_image_cache.Count > 0)
            {
                var found_image = PB_image_cache.Find(x => x.Item1.Equals(index));
                if(found_image != null) image_to_draw = found_image.Item2;
            }    
            // Jeżeli nie ma w nim tego, czego szukam - ładuję obraz z odpowiedniej ścieżki w data i dodaje go do cache.
            if (image_to_draw == null)
            {
                    image_to_draw = Image.FromFile(data[index].Item2);
                    PB_image_cache.Add(new Tuple<int, Image>(index, image_to_draw));
            }
            PB_image_preview.Image = image_to_draw;
        }

        private void LB_images_SelectedIndexChanged(object sender, EventArgs e)
        {
            image_selected = (string)LB_images.Items[LB_images.SelectedIndex];
            image_selected_index_in_data = LB_images.SelectedIndex;
            LB_image_selected_name.Text = image_selected;
            PB_image_draw(LB_images.SelectedIndex);
        }

        private void PB_image_preview_SizeChanged(object sender, EventArgs e)
        {
            if (PB_image_preview.Image.PhysicalDimension.ToSize().Height <= PB_image_preview.Size.Height &&
               PB_image_preview.Image.PhysicalDimension.ToSize().Width <= PB_image_preview.Size.Width)
            {
                //Obraz ma rozmiary mniejsze lub równe rozmiarowi okna - wyłączamy skalowanie!
                PB_image_preview.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            else
            {
                //Obraz jest większy niż okno - skalujemy!
                PB_image_preview.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
    }
}
