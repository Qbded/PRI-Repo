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
    public partial class Wyszukiwarka_main : Form
    {
        public event EventHandler OnDataAvalible;

    // Definicje używanych wewnętrznie danych:
        // Konstrukcja grup parametrów:
        // 1. Item1 (int) - identyfikator określający o której grupie parametrów mówimy
        // 2. Item2 (int) - określa liczbę parametrów wchodzących w tą grupę parametrów
        // 3. Item3 (string[]) - po kolei definiowane są w tej tablicy nazwy wewnętrzne dla poszczególnych parametrów
        // 4. Item4 (int[]) - po kolei definiowane są w tej tablicy typy danych dla poszczególnych parametrów, gdzie:
        //    0 to oznaczenie na int, stringa interpretować później jako int
        //    1 to oznaczenie na string, podawany as is,
        //    2 to oznaczenie na DateTime, podawane w stringu w formacie DD-MM-YYYY.
        // 5. Item5 (string[]) - po kolei definiowane są w tej tablicy wyświetlane użytkownikowi nazwy dla parametrów
        private List<Tuple<int, int, string[], int[], string[]>> parameter_group_templates;

        // Konstrukcja grup wartości:
        // 1. Item1 (int) - identyfikator określający o której grupie wartości mówimy
        // 2. Item2 (int) - określa liczbę parametrów wchodzących w tą grupę wartości
        // 3. Item3 (string[]) - po kolei definiowane są w tej tablicy nazwy wewnętrzne dla poszczególnych wartości
        // 4. Item4 (int[]) - po kolei definiowane są w tej tablicy typy kontrolek dla poszczególnych wartości, gdzie:
        //    0 to oznaczenie na CheckBox
        //    1 to oznaczenie na TextBox
        // 5. Item5 (string[]) - po kolei definiowane są w tej tablicy wyświetlane użytkownikowi nazwy dla wartości
        private List<Tuple<int, int, string[], int[], string[]>> values_group_templates;
        
    // Wartości zwracane do Special_function_window:
        
        // Zawiera indeks poszukiwanej tabeli, od 1 do 6, zgodnie z numeracją w database_tables, gdzie 6 daje rozkaz przeszukania wszystkich.
        public int target_table_returned;
        
        // Zawiera listę parametrów dla wyszukiwania w bazie.
        public List<string> target_query_parameters_returned;
        
        // Zawiera skonstruowane klauzule WHERE do zapytania SQL.
        public List<string> target_query_payload_returned;

        // Zawiera zwracane typy wartości i ich wartość.
        // 1. Item1 (string) - nazwa parametru, do którego odnoszą się te dane
        // 2. Item2 (int) - identyfikator do późniejszej interpretacji danych przekazywanych w stringu, gdzie:
        //    0 to int,
        //    1 to string,
        //    2 to dzień,
        //    3 to miesiąc,
        //    4 to rok.
        // 3. Item3 (string) - dane dla parametru uzupełnione przez użytkownika w GUI.
        public List<Tuple<string, int, string>> parameters_values_returned;


        private void TL_datasource_Initialise()
        {
            Control[] _items = new Control[6];

            CheckBox textfile_selector = new CheckBox();
            textfile_selector.Dock = DockStyle.Fill;
            textfile_selector.Name = "1";
            textfile_selector.Text = "W plikach tekstowych";
            textfile_selector.Checked = false;
            textfile_selector.CheckedChanged += datasource_selector_CheckedChanged;
            _items[0] = textfile_selector;

            CheckBox document_selector = new CheckBox();
            document_selector.Dock = DockStyle.Fill;
            document_selector.Name = "2";
            document_selector.Text = "W dokumentach";
            document_selector.Checked = false;
            document_selector.CheckedChanged += datasource_selector_CheckedChanged;
            _items[1] = document_selector;

            CheckBox complex_selector = new CheckBox();
            complex_selector.Dock = DockStyle.Fill;
            complex_selector.Name = "3";
            complex_selector.Text = "W plikach .htm i .xml";
            complex_selector.Checked = false;
            complex_selector.CheckedChanged += datasource_selector_CheckedChanged;
            _items[2] = complex_selector;

            CheckBox image_selector = new CheckBox();
            image_selector.Dock = DockStyle.Fill;
            image_selector.Name = "4";
            image_selector.Text = "W plikach graficznych";
            image_selector.Checked = false;
            image_selector.CheckedChanged += datasource_selector_CheckedChanged;
            _items[3] = image_selector;

            CheckBox multimedia_selector = new CheckBox();
            multimedia_selector.Dock = DockStyle.Fill;
            multimedia_selector.Name = "5";
            multimedia_selector.Text = "W plikach multimedialnych";
            multimedia_selector.Checked = false;
            multimedia_selector.CheckedChanged += datasource_selector_CheckedChanged;
            _items[4] = multimedia_selector;

            CheckBox global_selector = new CheckBox();
            global_selector.Dock = DockStyle.Fill;
            global_selector.Name = "6";
            global_selector.Text = "Wszystkich typach plików";
            global_selector.Checked = false;
            global_selector.CheckedChanged += datasource_selector_CheckedChanged;
            _items[5] = global_selector;

            TL_datasource.Controls.AddRange(_items);
        }

        // Tworzymy definicje w parameters_construction (możnaby tu zrobić linka do ładowania definicji z pliku zewnętrzego, gdyby takowy kiedykolwiek powstał)
        private void parameter_group_templates_Setup()
        {
            this.parameter_group_templates = new List<Tuple<int, int, string[], int[], string[]>>();
            
            // Definicja dla przeszukiwania prostego - można je wykonać dla każdego z podtypów.
            Tuple<int, int, string[], int[], string[]> simple_construction = new Tuple<int, int, string[], int[], string[]>(
            1,
            7,
            new string[7] { "NAME", "ORIGINAL_NAME", "SIZE", "CATALOGING_DATE", "FS_CREATION_TIME", "FS_LAST_WRITE_TIME", "EXTENSION" },
            new int[7] { 1, 1, 0, 2, 2, 2, 1 },
            new string[7] { "Nazwa w katalogu", "Nazwa na dysku", "Rozmiar", "Data katalogowania", "Data utworzenia", "Data modyfikacji", "Rozszerzenie" }
            );
            parameter_group_templates.Add(simple_construction);

            // Definicja dla przeszukiwania dokumentów - można je wykonać tylko dla dokumentów.
            Tuple<int, int, string[], int[], string[]> document_construction = new Tuple<int, int, string[], int[], string[]>(
            2,
            6,
            new string[6] { "LANGUAGE", "TITLE", "SUBJECT", "AUTHOR", "COMPANY", "PAGE_COUNT" },
            new int[6] { 1, 1, 1, 1, 1, 0 },
            new string[6] { "Język", "Tytuł dokumentu", "Temat dokumentu", "Autor", "Firma", "Liczba stron" }
            );
            parameter_group_templates.Add(document_construction);

            // Definicja dla przeszukiwania plików złożonych - można je wykonać tylko dla plików złożonych.
            Tuple<int, int, string[], int[], string[]> complex_construction = new Tuple<int, int, string[], int[], string[]>(
            3,
            2,
            new string[2] { "TITLE", "CONTENT_ENCODING" },
            new int[2] { 1, 1 },
            new string[2] { "Tytuł w pliku", "Kodowanie zawartości" }
            );
            parameter_group_templates.Add(complex_construction);

            // Definicja dla przeszukiwania obrazów - można je wykonać tylko dla obrazów.
            Tuple<int, int, string[], int[], string[]> image_construction = new Tuple<int, int, string[], int[], string[]>(
            4,
            3,
            new string[3] { "COMMENT", "WIDTH", "HEIGHT" },
            new int[3] { 1, 0, 0 },
            new string[3] { "Komentarz w obrazie", "Szerokość", "Wysokość" }
            );
            parameter_group_templates.Add(image_construction);

            // Definicja dla przeszukiwania plików mulimedialnych - można je wykonać tylko dla plików multimedialnych.
            Tuple<int, int, string[], int[], string[]> multimedia_construction = new Tuple<int, int, string[], int[], string[]>(
            5,
            11,
            new string[11] { "TITLE", "TRACK_NUMBER", "ALBUM", "RELEASE_DATE", "AUTHOR", "GENRE", "DURATION", "WIDTH", "HEIGHT", "COMMENT", "EXTRACTED_TEXT" },
            new int[11] { 1, 0, 1, 1, 1, 1, 1, 0, 0, 1, 1 },
            new string[11] { "Tytuł", "Numer utworu", "Album", "Data wydania", "Autor", "Gatunek", "Czas trwania", "Szerokość", "Wysokość", "Komentarz", "Wyekstrachowany tekst" }
            );
            parameter_group_templates.Add(multimedia_construction);
        }

        private void values_group_templates_Setup()
        {
            this.values_group_templates = new List<Tuple<int, int, string[], int[], string[]>>();

            // Opcje szukania dostępne dla liczb.
            Tuple<int, int, string[], int[], string[]> integer_values = new Tuple<int, int, string[], int[], string[]>(
            0,
            7,
            new string[7] { "CB_NOT_EMPTY", "CB_GREATER", "TB_GREATER", "CB_SMALLER", "TB_SMALLER", "CB_EQUAL", "TB_EQUAL" },
            new int[7] { 0, 0, 1, 0, 1, 0, 1 },
            new string[7] { "Nie dopuszczaj wartości pustej", "Ma być większa od", "" , "Ma być równa", "",  "Ma być mniejsza od", "" }
            );
            values_group_templates.Add(integer_values);

            // Opcje szukania dostępne dla stringów.
            Tuple<int, int, string[], int[], string[]> string_values = new Tuple<int, int, string[], int[], string[]>(
            1,
            5,
            new string[5] { "CB_NOT_EMPTY", "CB_CONTAINS", "TB_CONTAINS", "CB_EXACTLY", "TB_EXACTLY" },
            new int[5] { 0, 0, 1, 0, 1 },
            new string[5] { "Nie dopuszczaj wartości pustej", "Zawiera gdzieś w sobie", "", "Musi się w nim znaleść", "" }
            );
            values_group_templates.Add(string_values);

            // Opcje szukania dostępne dla dat.
            Tuple<int, int, string[], int[], string[]> datetime_values = new Tuple<int, int, string[], int[], string[]>(
            2,
            9,
            new string[9] { "CB_NOT_EMPTY", "CB_DAY", "TB_DAY", "CB_MONTH", "TB_MONTH", "CB_YEAR", "TB_YEAR", "CB_EARLIER", "CB_LATER" },
            new int[9] { 0, 0, 1, 0, 1, 0, 1, 0, 0 },
            new string[9] { "Nie dopuszczaj wartości pustej", "Dzień", "", "Miesiąc", "", "Rok", "" , "Wcześniej niż", "Później niż" }
            );
            values_group_templates.Add(datetime_values);

        }

        // Tworzy checkbox, ale UWAGA - nie jest on podpięty pod żadne zdarzenie!
        // Zdarzenia do zdefiniowania:
        // CheckedChanged - wyzwalane przy jakiejkolwiek zmianie statusu zaznaczenia checkbox'a
        private CheckBox Checkbox_Construct(int tag, string name, string text)
        {
            CheckBox constructed_checkbox = new CheckBox();
            constructed_checkbox.Dock = DockStyle.Fill;
            constructed_checkbox.Tag = tag;
            constructed_checkbox.Name = name;
            constructed_checkbox.Text = text;
            constructed_checkbox.Checked = false;
            
            return constructed_checkbox;
        }

        // Tworzy textbox, ale UWAGA - nie jest on podpięty pod żadne zdarzenie! max_length == 0 powoduje brak zdefiniowanego limitu długości.
        // Wartości do zdefiniowania:
        // MaxLength - maksymalna długość akceptowanego tekstu.
        // Zdarzenia do zdefiniowania:
        // EnabledChanged - wyzwalane przy włączeniu/wyłączeniu textbox'a,
        // TextChanged - wyzwalane przy KAŻDEJ zmianie tekstu,
        // KeyDown - służy do filtrowania możliwych do wprowadzenia rzeczy (tekst czy tylko liczby)
        private TextBox Textbox_Construct(int tag, string name, string text)
        {
            TextBox constructed_textbox = new TextBox();
            constructed_textbox.Dock = DockStyle.Fill;
            constructed_textbox.Tag = tag;
            constructed_textbox.Name = name;
            constructed_textbox.Text = text;
            constructed_textbox.Enabled = true;
            return constructed_textbox;
        }

        private Control[] parameters_Prepare(int type_index)
        {
            Control[] parameters = new Control[0];

            // Szukamy czy mamy taką definicję typu:
            var used_parameter_template = parameter_group_templates.Find(x => x.Item1 == type_index);

            if( used_parameter_template != null)
            {
                // Znaleźliśmy!
                parameters = new Control[used_parameter_template.Item2];
                for (int i = 0; i < used_parameter_template.Item2; i++)
                {
                    CheckBox constructed_checkbox = Checkbox_Construct(used_parameter_template.Item4[i], used_parameter_template.Item3[i], used_parameter_template.Item5[i]);
                    constructed_checkbox.CheckedChanged += parameter_selector_CheckedChanged;
                    parameters[i] = constructed_checkbox;
                }
            }
            else
            {
                // Nie znaleźliśmy - musi być gdzieś błąd
                MessageBox.Show("ERROR - podano nieprawidłowy type_index!");
            }

            return parameters;
        }
        
        private Control[] values_Prepare(int type_index)
        {
            Control[] values = new Control[0];
            
            // Szukamy czy mamy taką definicję typu:
            var used_value_template = values_group_templates.Find(x => x.Item1 == type_index);

            if (used_value_template != null)
            {
                // Znaleźliśmy!
                values = new Control[used_value_template.Item2];
                for (int i = 0; i < used_value_template.Item2; i++)
                {
                    if (used_value_template.Item4[i] == 0)
                    {
                        CheckBox constructed_checkbox = Checkbox_Construct(used_value_template.Item1, used_value_template.Item3[i], used_value_template.Item5[i]);
                        constructed_checkbox.CheckedChanged += value_selector_CheckedChanged;
                        values[i] = constructed_checkbox;
                    }
                    else
                    {
                        TextBox constructed_textbox = Textbox_Construct(used_value_template.Item1, used_value_template.Item3[i], used_value_template.Item5[i]);
                        constructed_textbox.Enabled = false;
                        constructed_textbox.EnabledChanged += value_selector_EnabledChanged;
                        switch (used_value_template.Item1)
                        {
                            case 0:
                                // Zmienna int - jedyne co robimy to zakazujemy tutaj KeyDown o wartościach nieliczbowych (chyba że to backspace!)
                                constructed_textbox.KeyDown += number_input_KeyDown;
                                break;

                            case 1:
                                // Zmienna string - można tu wpisać w zasadzie wszystko.
                                break;
                            case 2:
                                // Zmienna datetime - badamy czy wartości wpisane w dzień, miesiąc i rok są w rozsądnych zakresach.
                                switch(used_value_template.Item3[i])
                                {
                                    case "TB_DAY":
                                        // Sprawdzanie dla dni - tylko czy są w zakresie 1 do 31 obustronnie domkniętym.
                                        constructed_textbox.TextChanged += day_input_TextChanged;
                                        constructed_textbox.KeyDown += number_input_KeyDown;
                                        constructed_textbox.MaxLength = 2;
                                        break;
                                    case "TB_MONTH":
                                        // Sprawdzanie dla miesięcy - tylko czy są w zakresie 1 do 12 obustronnie domkniętym.
                                        constructed_textbox.TextChanged += month_input_TextChanged;
                                        constructed_textbox.KeyDown += number_input_KeyDown;
                                        constructed_textbox.MaxLength = 2;
                                        break;
                                    case "TB_YEAR":
                                        // Sprawdzanie dla lat - tylko czy są większe od zera.
                                        constructed_textbox.TextChanged += year_input_TextChanged;
                                        constructed_textbox.KeyDown += number_input_KeyDown;
                                        break;
                                    default:
                                        // Tutaj nigdy nie powinniśmy się znaleść!
                                        MessageBox.Show("ERROR - niepoprawna wartość dla tagów określających czas roku w użytym schemacie wartości!");
                                        break;
                                } 
                                break;
                            default:
                                MessageBox.Show("ERROR - id użytego schematu jest niezdefiniowane!");
                                break;
                        }
                        
                        
                        values[i] = constructed_textbox;
                    }
                    
                }
            }
            else
            {
                // Nie znaleźliśmy - musi być gdzieś błąd
                MessageBox.Show("ERROR - podano nieprawidłowy type_index!");
            }
            return values;
        }

        private void value_selector_EnabledChanged(object sender, EventArgs e)
        {
            TextBox caller = (TextBox)sender;
            
            if (caller.Enabled == false)
            {
                caller.BackColor = SystemColors.Control;
            }
            else
            {
                caller.BackColor = Color.White;
            }
        }

        private void day_input_TextChanged(object sender, EventArgs e)
        {
            TextBox caller = (TextBox)sender;
            int parsed_value;

            try
            {
                parsed_value = int.Parse(caller.Text);
            }
            catch
            {
                parsed_value = -1;
            }

            if (caller.Text.Length == 0) caller.BackColor = Color.White;
            else
            {
                if (parsed_value >= 1 && parsed_value <= 31) caller.BackColor = Color.Green;
            else caller.BackColor = Color.Red;
            }
        }

        private void month_input_TextChanged(object sender, EventArgs e)
        {
            TextBox caller = (TextBox)sender;
            int parsed_value;

            try
            {
                parsed_value = int.Parse(caller.Text);
            }
            catch
            {
                parsed_value = -1;
            }

            if (caller.Text.Length == 0) caller.BackColor = Color.White;
            else
            {
                if (parsed_value >= 1 && parsed_value <= 12) caller.BackColor = Color.Green;
                else caller.BackColor = Color.Red;
            }
        }

        private void year_input_TextChanged(object sender, EventArgs e)
        {
            TextBox caller = (TextBox)sender;
            int parsed_value;

            try
            {
                parsed_value = int.Parse(caller.Text);
            }
            catch
            {
                parsed_value = -1;
            }
            if (caller.Text.Length == 0) caller.BackColor = Color.White;
            else
            {
                if (parsed_value < 1 || parsed_value > 9999) { caller.BackColor = Color.Red;  }
                else caller.BackColor = Color.Green;
            }
        }

        private void number_input_KeyDown(object sender, KeyEventArgs e)
        {

            if ((e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) ||
                (e.KeyCode == Keys.D0 && e.Shift == false) ||
                (e.KeyCode == Keys.D1 && e.Shift == false) ||
                (e.KeyCode == Keys.D2 && e.Shift == false) ||
                (e.KeyCode == Keys.D3 && e.Shift == false) ||
                (e.KeyCode == Keys.D4 && e.Shift == false) ||
                (e.KeyCode == Keys.D5 && e.Shift == false) ||
                (e.KeyCode == Keys.D6 && e.Shift == false) ||
                (e.KeyCode == Keys.D7 && e.Shift == false) ||
                (e.KeyCode == Keys.D8 && e.Shift == false) ||
                (e.KeyCode == Keys.D9 && e.Shift == false) ||
                (e.KeyCode == Keys.Back))
            {
                // Wciśnięty klawisz jest liczbą lub backspace - przekaż go dalej.
                e.SuppressKeyPress = false;
            }
            else
            {
                // Wcisnięty klawisz nie jest ani liczbą ani backspacem - zatrzymaj go!
                e.SuppressKeyPress = true;
            }
        }

        private void TL_parameters_Draw(int type_index)
        {
            // Stałe określające ilość kolumn i ich zawartość dla zadanego typu wyszukiwanych metadanych:

            Control[] parameters_to_draw = new Control[0];

            int calculated_parameters_count = 0;

            bool error = false;

            switch (type_index)
            {
                case 1:
                    // Pliki tekstowe - bierzemy tylko podstawowe opcje
                    calculated_parameters_count = parameter_group_templates[0].Item2 + 1;
                    parameters_to_draw = new Control[calculated_parameters_count];
                    this.parameters_Prepare(1).CopyTo(parameters_to_draw, 0);
                    break;
                case 2:
                    // Dokumenty - bierzemy opcje podstawowe i opcje dokumentów
                    calculated_parameters_count = parameter_group_templates[0].Item2 + parameter_group_templates[1].Item2 + 1;
                    parameters_to_draw = new Control[calculated_parameters_count];
                    this.parameters_Prepare(1).CopyTo(parameters_to_draw,0);
                    this.parameters_Prepare(2).CopyTo(parameters_to_draw, parameter_group_templates[1].Item2 + 1);
                    break;
                case 3:
                    // Pliki złożone - bierzemy opcje podstawowe i opcje plików złożonych
                    calculated_parameters_count = parameter_group_templates[0].Item2 + parameter_group_templates[2].Item2 + 1;
                    parameters_to_draw = new Control[calculated_parameters_count];
                    this.parameters_Prepare(1).CopyTo(parameters_to_draw, 0);
                    this.parameters_Prepare(3).CopyTo(parameters_to_draw, parameter_group_templates[1].Item2 + 1);
                    break;
                case 4:
                    // Pliki graficzne - bierzemy opcje podstawowe i opcje plików graficznych
                    calculated_parameters_count = parameter_group_templates[0].Item2 + parameter_group_templates[3].Item2 + 1;
                    parameters_to_draw = new Control[calculated_parameters_count];
                    this.parameters_Prepare(1).CopyTo(parameters_to_draw, 0);
                    this.parameters_Prepare(4).CopyTo(parameters_to_draw, parameter_group_templates[1].Item2 + 1);
                    break;
                case 5:
                    // Pliki multimedialne - bierzemy opcje podstawowe i opcje plików multimedialnych
                    calculated_parameters_count = parameter_group_templates[0].Item2 + parameter_group_templates[4].Item2 + 1;
                    parameters_to_draw = new Control[calculated_parameters_count];
                    this.parameters_Prepare(1).CopyTo(parameters_to_draw, 0);
                    this.parameters_Prepare(5).CopyTo(parameters_to_draw, parameter_group_templates[1].Item2 + 1);
                    break;
                case 6:
                    // Wszystkie pliki - bierzemy tylko opcje podstawowe
                    calculated_parameters_count = parameter_group_templates[0].Item2 + 1;
                    parameters_to_draw = new Control[calculated_parameters_count];
                    this.parameters_Prepare(1).CopyTo(parameters_to_draw, 0);
                    break;
                default:
                    // Tutaj nigdy nie powinniśmy się znaleść!
                    MessageBox.Show("Błąd podczas tworzenia parametrów - podany type_index nie jest zdefiniowany!");
                    error = true;
                    break;
            }

            if(error == false)
            {
                TL_parameters.RowCount = calculated_parameters_count;
                for (int i = 0; i < calculated_parameters_count; i++)
                {
                    TL_parameters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
                }
                TL_parameters.Controls.AddRange(parameters_to_draw);
            }
            
        }

        private void TL_values_Draw(int type_of_value, string name)
        {
            Control[] new_controls_to_display;

            Control[] old_controls = new Control[TL_values.Controls.Count];
            TL_values.Controls.CopyTo(old_controls, 0);

            Control[] new_controls = values_Prepare(type_of_value);

            new_controls_to_display = new Control[old_controls.Length + new_controls.Length + 1];

            Label LB_value_name = new Label();
            LB_value_name.Name = name;
            LB_value_name.Dock = DockStyle.Fill;
            LB_value_name.Text = name;

            old_controls.CopyTo(new_controls_to_display, 0);
            new_controls_to_display[old_controls.Length] = LB_value_name;
            new_controls.CopyTo(new_controls_to_display, 1 + old_controls.Length);

            TL_values.RowCount = new_controls_to_display.Length + 1;

            for(int i = 0; i < new_controls_to_display.Length + 1; i++)
            {
                TL_parameters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            }

            TL_values.Controls.Clear();
            TL_values.Controls.AddRange(new_controls_to_display);
        }

        public Wyszukiwarka_main()
        {
            // Przygotowywanie danych statycznych
            parameter_group_templates_Setup();
            values_group_templates_Setup();

            // Inicjalizacja graficznych komponentów
            InitializeComponent();

            // Zaludnianie odpowiednich komponentów zawartością
            TL_datasource_Initialise();

        }

        private void datasource_selector_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox caller = (CheckBox)sender;
            if (caller.CheckState == CheckState.Checked)
            {
                foreach (CheckBox control in caller.Parent.Controls)
                {
                    if (control.Name != caller.Name) control.Enabled = false;
                }
                TL_parameters.AutoScroll = true;
                TL_parameters_Draw(int.Parse(caller.Name));
            }
            else
            {
                foreach (CheckBox control in caller.Parent.Controls)
                {

                    if (control.Name != caller.Name) control.Enabled = true;
                }
                TL_parameters.Controls.Clear();
                if (TL_values.Controls.Count > 0) TL_values.Controls.Clear();
                this.BT_execute.Enabled = false;
                TL_parameters.AutoScroll = false;

            }
        }

        private void parameter_selector_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox caller = (CheckBox)sender;
            if (caller.CheckState == CheckState.Checked)
            {
                // Dodajemy odpowiednie opcje w zależności od wybranych wartości.
                TL_values.AutoScroll = true;
                TL_values_Draw((int)caller.Tag, caller.Text);
            }
            else
            {
                // Usuwamy odpowiednie opcje w zależności od wybranych wartości.
                var deletion_template = values_group_templates.Find(x => x.Item1 == (int)caller.Tag);
                if(deletion_template != null)
                {
                    int delete_from = TL_values.Controls.IndexOfKey(caller.Text);

                    TL_values.AutoScroll = false;
                    for (int i = 0; i <= deletion_template.Item2; i++)
                    {
                        TL_values.Controls.RemoveAt(delete_from);
                    }
                }
                if (TL_values.Controls.Count == 0) this.BT_execute.Enabled = false;
                TL_values.AutoScroll = true;
            }              
        }

        private void value_selector_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox caller = (CheckBox)sender;

            int slave_index;
            bool slave_found = false;

            slave_index = caller.Parent.Controls.IndexOf(caller) + 1;
            TextBox slave = new TextBox();
            if (slave_index <= caller.Parent.Controls.Count)
            {
                try
                {
                    slave = (TextBox)caller.Parent.Controls[slave_index];
                    slave_found = true;
                }
                catch
                {
                    slave = null;
                    slave_found = false;
                }
            }

            if (caller.CheckState == CheckState.Checked)
            {
                // Włączamy podległegy mu TextBox gdy jest zaznaczany.
                if(slave_found == true) slave.Enabled = true;
                if (this.BT_execute.Enabled == false) BT_execute.Enabled = true;
            }
            else
            {
                bool everything_unset = true;

                for (int i = 0; i < caller.Parent.Controls.Count; i++)
                {
                    CheckBox checkbox_found;

                    try
                    {
                        checkbox_found = (CheckBox)caller.Parent.Controls[i];
                        if (checkbox_found.Checked == true) everything_unset = false;
                    }
                    catch
                    {
                        checkbox_found = null;
                    }
                }

                if (everything_unset) this.BT_execute.Enabled = false;

                if (slave_found == true)
                {
                    slave.ResetText();
                    slave.Enabled = false;
                }
            }
        }

        private void Wyszukiwarka_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((Special_function_window)this.Owner).Controls_set_lock(false);
        }

        // Przesyła dane do Special_function_window.
        private void BT_execute_Click(object sender, EventArgs e)
        {
            List<string> parameters_selected = new List<string>();

            target_query_payload_returned = new List<string>();
            target_query_parameters_returned = new List<string>();
            parameters_values_returned = new List<Tuple<string, int, string>>();

            // Pobieramy indeks tabeli, na której wywołamy zapytanie. Gdy wybraliśmy wszystkie zachodzi przypadek szczególny, ale to dopiero w Special_function_window
            foreach (CheckBox datasource in TL_datasource.Controls)
            {
                if (datasource.Checked == true) target_table_returned = datasource.TabIndex + 1;
            }

            // Pobieramy wszystkie wybrane przez użytkownika parametry
            foreach (CheckBox parameter in TL_parameters.Controls)
            {
                if (parameter.Checked == true)
                {
                    target_query_parameters_returned.Add(parameter.Name);
                    // Szukamy czy istnieje taki typ parametru w naszej definicji schematów grup wartości
                    var value_group_template = values_group_templates.Find(x => x.Item1 == (int)parameter.Tag);
                    if (value_group_template != null)
                    {
                        // Istnieje, przechodzimy do szukania go w GUI.
                        var value_group = TL_values.Controls.Find(parameter.Text, true);
                        if (value_group.Length != 0)
                        {
                            // Znaleźliśmy - wyciągamy wartości i dodajemy odpowiednio sformatowane stringi do target_query_parameters_returned
                            int start_index = TL_values.Controls.IndexOf(value_group[0]) + 1;
                            int end_index = start_index + value_group_template.Item2;

                            List<string> query_strings = new List<string>();
                            List<Tuple<string, int, string>> parameters_values = new List<Tuple<string, int, string>>();

                            CheckBox analysed_checkbox;
                            TextBox slaved_textbox;

                            for (int i = start_index; i < end_index; i++)
                            {
                                try
                                {
                                    analysed_checkbox = (CheckBox)TL_values.Controls[i];
                                    // Jeżeli tutaj jesteśmy, to znaleźliśmy checkbox. Przechodzimy do sprawdzenia czy ma podległego TextBox'a 
                                    if (analysed_checkbox.Checked == true)
                                    {
                                        try
                                        {
                                            slaved_textbox = (TextBox)TL_values.Controls[i + 1];
                                            // Jeżeli tutaj jesteśmy, to tak, ma podległy TextBox.
                                        }
                                        catch
                                        {
                                            slaved_textbox = null;
                                            // Podległego textbox'a nie było

                                        }

                                        switch (analysed_checkbox.Name)
                                        {
                                            case "CB_NOT_EMPTY":
                                                query_strings.Add("AND " + parameter.Name + " IS NOT NULL");
                                                break;
                                            case "CB_GREATER":
                                                query_strings.Add("AND " + parameter.Name + " > @" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'));
                                                parameters_values.Add(new Tuple<string, int, string>("@" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'), 0, slaved_textbox.Text));
                                                break;
                                            case "CB_SMALLER":
                                                query_strings.Add("AND " + parameter.Name + " < @" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'));
                                                parameters_values.Add(new Tuple<string, int, string>("@" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'), 0, slaved_textbox.Text));
                                                break;
                                            case "CB_EQUAL":
                                                query_strings.Add("AND " + parameter.Name + " = @" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'));
                                                parameters_values.Add(new Tuple<string, int, string>("@" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'), 0, slaved_textbox.Text));
                                                break;
                                            case "CB_CONTAINS":
                                                query_strings.Add("AND " + parameter.Name + " LIKE @" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'));
                                                parameters_values.Add(new Tuple<string, int, string>("@" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'), 1, "%" + slaved_textbox.Text + "%"));
                                                break;
                                            case "CB_EXACTLY":
                                                query_strings.Add("AND " + parameter.Name + " LIKE @" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'));
                                                parameters_values.Add(new Tuple<string, int, string>("@" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'), 1, "%" + slaved_textbox.Text + "%"));
                                                break;
                                            case "CB_DAY":
                                                query_strings.Add("AND EXTRACT(DAY FROM " + parameter.Name + ") = @" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'));
                                                parameters_values.Add(new Tuple<string, int, string>("@" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'), 2, slaved_textbox.Text));
                                                break;
                                            case "CB_MONTH":
                                                query_strings.Add("AND EXTRACT(MONTH FROM " + parameter.Name + ") = @" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'));
                                                parameters_values.Add(new Tuple<string, int, string>("@" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'), 2, slaved_textbox.Text));
                                                break;
                                            case "CB_YEAR":
                                                query_strings.Add("AND EXTRACT(YEAR FROM " + parameter.Name + ") = @" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'));
                                                parameters_values.Add(new Tuple<string, int, string>("@" + analysed_checkbox.Name + "_IN_" + value_group[0].Text.ToUpper().Replace(' ', '_'), 2, slaved_textbox.Text));
                                                break;
                                            case "CB_EARLIER":
                                                {
                                                    var index_to_work_on = query_strings.FindIndex(x => x.Contains("="));
                                                    while (index_to_work_on != -1)
                                                    {
                                                        query_strings[index_to_work_on] = query_strings[index_to_work_on].Replace("=", "<");
                                                        index_to_work_on = query_strings.FindIndex(x => x.Contains("="));
                                                    }
                                                }
                                                break;
                                            case "CB_LATER":
                                                {
                                                    var index_to_work_on = query_strings.FindIndex(x => x.Contains("="));
                                                    while (index_to_work_on != -1)
                                                    {
                                                        query_strings[index_to_work_on] = query_strings[index_to_work_on].Replace("=", ">");
                                                        index_to_work_on = query_strings.FindIndex(x => x.Contains("="));
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                                catch
                                {
                                    // Czasem wskakujemy tutaj gdy napotykamy na label
                                    analysed_checkbox = null;
                                }

                            }
                            // Po zebraniu wszystkich opcji konstruujemy finalny string zapytania dla danego parametru:
                            string query_string_constructed = "";
                            query_string_constructed += " AND (" + query_strings[0].Remove(0, 3);
                            for (int j = 1; j < query_strings.Count; j++)
                            {
                                query_string_constructed += " " + query_strings[j];
                            }
                            query_string_constructed += " )";
                            target_query_payload_returned.Add(query_string_constructed);

                            // Teraz przekazujemy parametry użyte w tym stringu do naszej listy:
                            foreach (var parameter_value in parameters_values)
                            {
                                parameters_values_returned.Add(parameter_value);
                            }
                        }
                        else
                        {
                            // W kontrolce nie było odpowiednich elementów, nigdy tak nie powinno się zdarzyć...
                            MessageBox.Show("ERROR - w TL_values nie znaleziono wskazanej grupy wartości!");
                        }
                    }
                    else
                    {
                        // Dany typ parametrów nie jest zdefiniowany!
                        MessageBox.Show("ERROR - podany typ parametrów nie został znaleziony w schematach grup wartości");
                    }
                } 
            }

            // Konczymy ostatni string zapytania średnikiem as per SQL standards
            target_query_payload_returned[target_query_payload_returned.Count - 1] += ";";

            // Wszystkie dane są załadowane w odpowiednie miejsca, dajemy znać Special_function_window że jesteśmy gotowi:
            OnDataAvalible(this, EventArgs.Empty);
            this.Close();
            this.Dispose();
        }
    }
}
