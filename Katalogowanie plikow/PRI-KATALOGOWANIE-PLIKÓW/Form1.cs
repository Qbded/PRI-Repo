using System;
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
        public List<string> metadata { get; set; }
        public string[] extends = { ".txt", ".csv", ".doc", ".docx", ".odt", ".ods", ".odp", ".xls", ".xlsx", ".pdf", ".ppt", ".pptx", ".pps", ".fb2", ".htm", ".html", ".tsv", ".xml", ".jpg", ".jpeg", ".tiff", ".bmp", ".mp4", ".avi", ".mp3", ".wav"};
        private string regex = @"((u|s|wy){0,1}(tw((órz)|(orzyć)|(orzenie))))(\s)(etykiet(y|ę){0,1})(\s)((<[a-ząęśćżźół\#]+\$>)+)(\s)((dla){0,1})(\s)((każde){0,1}((go)|j){0,1})(\s){0,1}((grupy){0,1})(\s){0,1}((((plik)|(obiekt))(u|ów))|(lokacji))(\s)(((\*{0,1})\.{0,1}[a-z0-9]{3,4}\s{0,1})+)";
        private string regexCreate = @"(create)(\s)(label)(\s)((<[a-ząęśćżźół\#]+\$>)+)(\s)((for){0,1})(\s)((every)|(all)|(each){0,1})(\s){0,1}((group of files)|(files' group){0,1})(\s)(((\*{0,1})\.{0,1}[a-z0-9]{3,4}\s{0,1})+)";
        private string[] exampleCommands = { "utwórz etykietę <x$> dla pliku *.mp3", "utwórz etykietę <x$> dla obiektu *.mp3", "utwórz etykietę <x$> dla lokacji *.mp3", "utwórz etykiety <x$><y$> dla grupy plików *.mp3 *wav", "utwórz etykiety <x$><y$> dla plików *.mp3 *wav", "utwórz etykiety <x$><y$> plików *.mp3 *wav", "utwórz etykiety <x$><y$> dla obiektów *.mp3 *wav", "utwórz etykiety <x$><y$> dla lokacji *.mp3 *wav" };
        private string[] exampleCommandsCreate = { "create label <x$> for every file *.mp3", "create label <x$> for each file *.mp3", "create label <x$> for all file *.mp3", "create label <x$> for file *.mp3", "create label <x$><y$> for files' group *.mp3 *wav", "create label <x$><y$> for group of file *.mp3 *wav", };
        int randResult = 0;
        int randResultCreate = 0;
        List<string> excludedMetadata;

        string program_path = null;
        string xml_path = null;
        string txt_path = null;
        string database_path = null;

        private void DetermineFilepaths()
        {
            /*   Oduzależnienie programu od statycznych stringów - pobieranie lokacji programu
            *    
            *    Na początku tworzymy grabber dający nam obiekt DirectoryInfo zawiarajacy lokacje fizyczna programu na dysku twardym.
            *    Potem sprawdzamy czy jest w katalogu Debug/Release (co swiadczy o tym ze dalej jest w konfiguracji testowej, a nie roboczej),
            *    jezeli jest to musimy pobrać lokację dwóch katalogów w górę, jeżeli nie - tylko jeden
            *    
            *    Strutura programu na dzien dzisiejszy wygląda tak (i będzie tak wyglądała aż do zakonczenia prac z VS):
            *    bin\Release - zawiera .exe'c zbudowany przez VS w wersji release i biblioteki .dll programu
            *    bin\Debug - zawiera .exe'c zbudowany przez VS w wersji debug i biblioteki .dll programu
            *    db\ - tutaj żyje nasz katalog, w pliku catalog.fdb
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

        public Form1()
        {
            InitializeComponent();

            DetermineFilepaths();

            this.txtCommand.LostFocus += TxtCommand_LostFocus;
            this.chkMetadata.LostFocus += ChkMetadata_LostFocus;
            this.Load += Form1_Load;
            this.KeyDown += Form1_KeyDown;

            excludedMetadata = new List<string>();
            metadata = new List<string>();
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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void ChkMetadata_LostFocus(object sender, EventArgs e)
        {
            var diff = metadata.Where(x => !excludedMetadata.Contains(x)).ToList();
            this.AppendXML(diff);
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
            int i = rand.Next(0,forEach.Length - 1);
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
                using (var connection = new FbConnection(@"ServerType=0;User=SYSDBA;Password=;Database=" + database_path)) // w Password= wpisac hasło użytkownika SYSDBA
                {
                    MessageBox.Show("Znalazłem istniejącą bazę!");

                    connection.Open();
                    // House-keeping, zarwanie połączenia zaraz po jego nawiązaniu.
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("Nie znalazłem bazy, tworze nową w katalogu:\n" + database_path);

                if (!Directory.Exists(program_path + @"\db"))
                {
                    //DEBUG MessageBox.Show("Tworze katalog db!");
                    Directory.CreateDirectory(program_path + @"\db");
                }

                FbConnectionStringBuilder builder = new FbConnectionStringBuilder();
                builder.DataSource = "localhost"; //identyfikator sieciowy - do kogo sie laczymy. Moze byc postaci adres IP+Port.
                builder.UserID = "SYSDBA"; //defaultowy uzytkownik z najwyzszymi uprawnieniami do systemu bazodanowego, tworzony podczas instalacji
                builder.Password = ""; //haslo nadane podczas instalacji Firebird'a użytkownikowi SYSDBA, uzupełnić w zależności u kogo jest jakie
                builder.Database = database_path;
                builder.ServerType = FbServerType.Default;

                FbConnection.CreateDatabase(builder.ConnectionString);

                //Dalej instrukcje tworzenia tabel itp. itd.

            }
        }
        
        /*  Ekstracja metadanych - wywołanie okna odpowiedzialnego za jej obsługę
         *  
         *  Informacje o tym jak robimy ekstrakcję metadanych znajdują się w formularzu Metadata_extractor.cs
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
                this.lbExampleCommand.Text += "E.g. " +exampleCommandsCreate[randResultCreate];
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
                    this.txtCommand.Text += " " + _group[5] + " "  + _group[_group.Count - 2];
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

        private void chkExcludeMetadata_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkExcludeMetadata.Checked)
            {
                this.chkMetadata.Enabled = true;
                for(int i = 0; i < metadata.Count(); i++)
                {
                    this.chkMetadata.Items.Add(metadata[i]);
                }
                FileInfo txt_dump = new FileInfo(txt_path);
                if (txt_dump.Exists)
                {
                    StreamWriter txt_dumper = new StreamWriter(txt_path);

                    for(int i = 0; i < metadata.Count(); i++)
                    {
                        txt_dumper.WriteLine(metadata[i]);
                    }
                    txt_dumper.Close();
                }
            }
            else
            {
                this.chkMetadata.Enabled = false;
                this.chkMetadata.Items.Clear();
            }

            //this.AppendXML(metadata);
            //string requestData = AppendXML(metadata);
            //this.PostXML("http://localhost:53118/Service1.svc", requestData);
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
            File.AppendAllText(txt_path, key+"$"+strMd5MachineUserName+Environment.NewLine);
            

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
                    File.AppendAllText(txt_path, n.InnerText+"$" +mEncrypt+ Environment.NewLine);
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
