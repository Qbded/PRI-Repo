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
using PRI_KATALOGOWANIE_PLIKÓW;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Janek_main : Form
    {
        public event EventHandler OnDataAvalible;
        public static string[] extends = { "mp3", "wav"};
        private string regex = @"((u|s|wy){0,1}(tw((órz)|(orzyć)|(orzenie))))(\s)(etykiet(y|ę){0,1})(\s)((<[A-ZĄĘŚĆŻŹÓŁa-ząęśćżźół\#\s0-9%\.,]+\$>)+)(\s)((dla){0,1})(\s)((każde){0,1}((go)|j){0,1})(\s){0,1}((grupy){0,1})(\s){0,1}((((plik)|(obiekt))(u|ów))|(lokacji))(\s)(((\*{0,1})\.{0,1}[a-z0-9]{3,4}\s{0,1})+)";
        private string regexCreate = @"(create)(\s)(label)(\s)((<[A-ZĄĘŚĆŻŹÓŁa-ząęśćżźół\#\s0-9%\.,]+\$>)+)(\s)((for){0,1})(\s)((every)|(all)|(each){0,1})(\s){0,1}((group of files)|(files' group){0,1})(\s)(((\*{0,1})\.{0,1}[a-z0-9]{3,4}\s{0,1})+)";
        private string[] exampleCommands = { "utwórz etykietę <x$> dla pliku *.mp3", "utwórz etykietę <x$> dla obiektu *.mp3", "utwórz etykietę <x$> dla lokacji *.mp3", "utwórz etykiety <x$><y$> dla grupy plików *.mp3 *wav", "utwórz etykiety <x$><y$> dla plików *.mp3 *wav", "utwórz etykiety <x$><y$> plików *.mp3 *wav", "utwórz etykiety <x$><y$> dla obiektów *.mp3 *wav", "utwórz etykiety <x$><y$> dla lokacji *.mp3 *wav" };
        private string[] exampleCommandsCreate = { "create label <x$> for every file *.mp3", "create label <x$> for each file *.mp3", "create label <x$> for all file *.mp3", "create label <x$> for file *.mp3", "create label <x$><y$> for files' group *.mp3 *wav", "create label <x$><y$> for group of file *.mp3 *wav", };
        int randResult = 0;
        int randResultCreate = 0;
        Dictionary<Tuple<string, string>, string> metadata;
        Dictionary<string, string> fileLabels;
        List<string> excludedMetadata;
        public List<string> names;
        List<string> selectedLabels;
        List<int> length;
        bool equals;
        // const string path = @"C:\Users\lenovo\Documents\Visual Studio 2012\Projects\PRI-KATALOGOWANIE-PLIKÓW\PRI-KATALOGOWANIE-PLIKÓW\metadata.xml";
        // const string pathTxt = @"C:\Users\lenovo\Documents\Visual Studio 2012\Projects\PRI-KATALOGOWANIE-PLIKÓW\PRI-KATALOGOWANIE-PLIKÓW\$$$.txt";
        WaveOf wOf;
        AForge.Math.Metrics.CosineSimilarity sim;
        List<List<double>> nested;
        List<string> extensionsInLv;
        int _index;
        string activate;

        public List<Tuple<int, string>> result_directories;
        public List<Tuple<int, string>> result_files;

        private NAudio.Wave.WaveFileReader wave;
        private NAudio.Wave.DirectSoundOut output;
        private NAudio.Wave.BlockAlignReductionStream stream;

        public Janek_main()
        {
            InitializeComponent();

            this.txtCommand.LostFocus += TxtCommand_LostFocus;
            this.Load += Form1_Load;
            this.KeyDown += Form1_KeyDown;

            length = new List<int>();
            result_directories = new List<Tuple<int, string>>();
            result_files = new List<Tuple<int, string>>();
            names = new List<string>();
            excludedMetadata = new List<string>();
            metadata = new Dictionary<Tuple<string, string>, string>();
            fileLabels = new Dictionary<string, string>();
            equals = false;
            sim = new AForge.Math.Metrics.CosineSimilarity();
            nested = new List<List<double>>();
            extensionsInLv = new List<string>();
            selectedLabels = new List<string>();
            _index = 0;
            activate = String.Empty;
        }

        private void Increment(ref int _index)
        {
            _index++;
        }

        private void Decrement(ref int _index)
        {
            _index--;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.MediaPlayPause)
            {
                this.PlayPauseToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.MediaStop)
            {
                this.StopToolStripMenuItem_Click(sender, e);
            }
            else if (e.KeyCode == Keys.MediaNextTrack)
            {
                this.Increment(ref _index);
                if (_index == names.Count) _index = 0;
                activate = names[_index];
                this.StopToolStripMenuItem_Click(sender, e);
                this.Form1_TextChanged(sender, e);
                this.PlayPauseToolStripMenuItem_Click(sender, e);
            }

            else if (e.KeyCode == Keys.MediaPreviousTrack)
            {
                this.Decrement(ref _index);
                if (_index < 0) _index = names.Count-1;
                activate = names[_index];
                this.StopToolStripMenuItem_Click(sender, e);
                this.Form1_TextChanged(sender, e);
                this.PlayPauseToolStripMenuItem_Click(sender, e);
            }
            else if (Control.ModifierKeys == Keys.Control
                && e.KeyCode == Keys.O || e.KeyCode == Keys.BrowserSearch)
            {
                this.bnChooseFolder_Click(sender, e);
            }
            else if (Control.ModifierKeys == Keys.Alt
                && e.KeyCode == Keys.F4)
            {
                this.Close();
            }
            else if (Control.ModifierKeys == Keys.Alt
                && e.KeyCode == Keys.U)
            {
                this.chkUseCreteRule.Checked = true;
            }
            else if (Control.ModifierKeys == Keys.Alt
                && e.KeyCode == Keys.R)
            {
                this.chkUseEquality.Checked = true;
            }

            else if (Control.ModifierKeys == Keys.Alt
                && e.KeyCode == Keys.A)
            {
                this.bnCatalogue_Click(sender, e);
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ChkMetadata_LostFocus(object sender, EventArgs e)
        {

        }
              
        /// <summary>
        /// Przypisuje flagę do pliku
        /// </summary>
        /// <param name="to">Plik, do którego chcemy utworzyć przypisanie</param>
        /// <param name="flag">Flaga ustawiana podczas przypisania. Początkowo flaga jest ustawiona na NIEPRZYDZIELONY</param>
        private string Assign(string to)
        {
            string assign = "NIEPRZYDZIELONY$";
            return assign;
            /*
             * ToDo : Utworzyć przypisania dla poszczególnych typów plików
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
            if (_index > 0)
            {
                foreach (var item in fileLabels)
                    MessageBox.Show(item.Key + " => " + item.Value);
            }

            //}
            //else
            //{
            //    _group.Clear();
            //    Clipboard.SetText(this.txtCommand.Text);
            //}
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
            equals = false;

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
                    try
                    {
                        this.txtCommand.Text += " " + _group[5] + " " + _group[_group.Count - 2];
                    }
                    catch (Exception) { }
                this.txtCommand.Text = RemoveDuplicates(txtCommand.Text);
                foreach (var g in _group)
                    MessageBox.Show(g);
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
        
        private void bnCatalogue_Click(object sender, EventArgs e)
        {
            int _i = 0, _j = 1;

            double[] nToArray_i, nToArray_j;
            Dictionary<string, double> similarities = new Dictionary<string, double>();
            foreach (var n in nested.ToArray())
            {
                nToArray_i = nested[_i].ToArray();
                foreach (var m in nested.ToArray())
                {
                    if (_j >= nested.Count)
                    {
                        _j = 0;
                        break;
                    }
                    //if (_i == _j) continue;
                    nToArray_j = nested[_j].ToArray();
                    if (_i != _j)
                        //this.listView4.Items.Add("Podobieństwo pliku " + _i + ". do " + _j + ".=" 
                        //    + sim.GetSimilarityScore(nToArray_i, nToArray_j)*100+"%");
                    similarities.Add(_i + "," + _j, sim.GetSimilarityScore(nToArray_i, nToArray_j));
                    //MessageBox.Show(info);
                    _j++;
                }
                _i++;
            }
            string similarity = String.Empty;

            var newSim = similarities.Where(x => x.Value != 1);
            Dictionary<string,double> merged = new Dictionary<string, double>();
            Dictionary<string, double> differences = new Dictionary<string, double>(); 
            foreach (var sim in newSim)
            {
                var split = sim.Key.Split(',');
                differences.Add(sim.Key, Math.Abs(newSim.ToList()[Convert.ToInt16(split[0])].Value - newSim.ToList()[Convert.ToInt16(split[1])].Value));
            }

            while (differences.Count > 1)
            {
                Dictionary<string, double> oldSim = new Dictionary<string, double>();
                foreach (var difference in differences)
                    oldSim.Add(difference.Key, difference.Value);

                double min = differences.Min(x => x.Value);
                double value = differences.Aggregate((x, y) => x.Value < y.Value ? y : x).Value;
                string key = differences.Aggregate((x, y) => x.Value < y.Value ? y : x).Key;
                merged.Add(key, value);
                differences.Clear();
                oldSim.Add(key + "," + key, min);
                
                foreach (var old in oldSim)
                    if (old.Value != min)
                        differences.Add(old.Key, min - old.Value);
            }

            List<string> folded = new List<string>();
            similarity = String.Empty;
            foreach (var merge in merged.Keys)
                similarity += merge;
            for(int i = 0; i < merged.Count; i++)
            {
                Tuple<int, string> directory_to_create = new Tuple<int,string>(
                    i,
                    "Różnica podobieństw = " + merged.ElementAt(i).Value
                    );

                result_directories.Add(directory_to_create);

                foreach (var m in merged.ElementAt(i).Key.Split(','))
                {
                    var mdiv10 = (Convert.ToInt16(m) > names.Count - 1)
                        ? Convert.ToInt16(m) / 10 : Convert.ToInt16(m);
                    try
                    {
                        folded.Add(names[mdiv10]);
                        // tutaj zamieniamy polecenie przesunięcia pliku na dodanie do listy razem z ID folderu rodzicicielskiego:
                        string name_to_add = string.Empty;
                        name_to_add = names[mdiv10].Split('\\').Last();
                        Tuple<int, string> file_to_move = new Tuple<int, string>(
                            i,
                            name_to_add
                            );
                        result_files.Add(file_to_move);
                        /*
                        System.IO.File.Move(names[mdiv10],
                        names[mdiv10]
                            .Substring(0, names[mdiv10]
                            .LastIndexOf("\\") + 1) + "Różnica podobieństw = " + merged.ElementAt(i).Value + "\\"
                            + names[mdiv10]
                            .Substring(names[mdiv10].LastIndexOf("\\") + 1));
                        */
                    }
                    catch (Exception)
                    {

                    }
                }

                if(result_directories.Count != 0 && result_files.Count != 0)
                {
                    // Dajemy znać formowi wywołującemu (tutaj Special_function_window) że dane są gotowe do odbioru.
                    OnDataAvalible(this, EventArgs.Empty);
                    MessageBox.Show("Zakończono proces katalogowania.");
                    this.Close();
                }
            }
            /*
            foreach (var merge in merged)
            {
                
                DirectoryInfo di = Directory.CreateDirectory(
    names[Convert.ToInt16(merge.Key.Split(',')[0])]
    .Substring(0, names[Convert.ToInt16(merge.Key.Split(',')[0])]
    .LastIndexOf("\\") + 1) + "Różnica podobieństw = " + merge.Value);

                foreach (var m in merge.Key.Split(','))
                {
                    var mdiv10 = (Convert.ToInt16(m) > names.Count - 1)
                        ? Convert.ToInt16(m) / 10 : Convert.ToInt16(m);
                    try
                    {
                        folded.Add(names[mdiv10]);
                        System.IO.File.Move(names[mdiv10],
                        names[mdiv10]
                            .Substring(0, names[mdiv10]
                            .LastIndexOf("\\") + 1) + "Różnica podobieństw = " + merge.Value + "\\"
                            + names[mdiv10]
                            .Substring(names[mdiv10].LastIndexOf("\\") + 1));
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            */
            /*
            MessageBox.Show(this.RemoveDuplicates(similarity));
            MessageBox.Show("Test");
            //
            string name = String.Empty;
            string fileLabelStr = String.Empty;
            foreach (var item in fileLabels.Keys)
                fileLabelStr += item;
            name = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss-" + fileLabelStr + "[");
            var invalidChars = Path.GetInvalidFileNameChars();
            string newDateStr = String.Join("", name.Select(c => invalidChars.Contains(c) ? '-' : c));
            */
            //foreach (var merge in similarity.Split(new char[] { ',', ' '}))
            //{
            //    if (Convert.ToInt16(merge) <= names.Count - 1)
            //    {
            //        MessageBox.Show(merge);
            //        folded.Add(names[Convert.ToInt16(merge)]);
            //        System.IO.File.Move(names[Convert.ToInt16(merge)],
            //            names[Convert.ToInt16(similarity.Split(',')[0])]
            //    .Substring(0, names[Convert.ToInt16(similarity.Split(',')[0])]
            //    .LastIndexOf("\\") + 1) + newDateStr + "\\" + names[Convert.ToInt16(merge)]
            //                      .Substring(names[Convert.ToInt16(merge)].LastIndexOf("\\") + 1));
            //    }
            //}

            //System.Threading.Thread.Sleep(1000);
            //var diff = names.Where(x => !folded.Contains(x));
            /*
            foreach (var item in diff)
            {
                DirectoryInfo diRest = Directory.CreateDirectory(
                names[Convert.ToInt16(similarity.Split(',')[0])]
                .Substring(0, names[Convert.ToInt16(similarity.Split(',')[0])]
                .LastIndexOf("\\") + 1)
                + String.Join("", (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss-" + fileLabelStr))
                .Select(c => invalidChars.Contains(c) ? '-' : c)));

                System.IO.File.Move(item,
                    names[Convert.ToInt16(similarity.Split(',')[0])]
            .Substring(0, names[Convert.ToInt16(similarity.Split(',')[0])]
            .LastIndexOf("\\") + 1) + String.Join("", (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss-" + fileLabelStr))
                .Select(c => invalidChars.Contains(c) ? '-' : c)) + "\\" + item
                              .Substring(item.LastIndexOf("\\") + 1));
            }
            */
            //MessageBox.Show(similarity);
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            this.DisposeWave();
            activate = this.listView1.SelectedItems[0].Text;
            this.Text = "Katalogowanie plików dźwiękowych [odtwarzam [" + activate.Substring(activate.LastIndexOf("\\") + 1) + "]]";
            this.timer1.Start();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var name in names.Distinct())
            {
                List<double> parameters = new List<double>();
                if (name.EndsWith(".mp3"))
                {
                    Mp3ToWavConverter.Convert(name);
                }
                wOf = new WaveOf(name.Replace(".mp3", ".wav"));

                parameters.Add(wOf["Q1-Minimum"]);
                parameters.Add(wOf["Q1-Maximum"]);
                parameters.Add(wOf["Q2-Minimum"]);
                parameters.Add(wOf["Q2-Maximum"]);
                parameters.Add(wOf["Q3-Minimum"]);
                parameters.Add(wOf["Q3-Maximum"]);
                parameters.Add(wOf["QDev-Minimum"]);
                parameters.Add(wOf["QDev-Maximum"]);
                parameters.Add(wOf["Average"]);
                parameters.Add(wOf["Mode"]);
                parameters.Add(wOf["Total time"]);
                
                nested.Add(parameters);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //int index = 0;
            foreach (var name in names.Distinct())
            {
                if (name.EndsWith(".mp3"))
                {
                    //var _files = Directory.GetFiles(name.Substring(0, name.LastIndexOf("\\") + 1), "*", SearchOption.TopDirectoryOnly);
                    // Zmiana w wywalaniu plików wav na których pracowaliśmy - wywalać tylko te, które zostały stworzone do celów analizy!
                    if (System.IO.File.Exists(name.Replace(".mp3", ".wav"))) System.IO.File.Delete(name.Replace(".mp3", ".wav"));
                    
                    /*
                    foreach (var file in _files)
                        if (file.EndsWith(".wav"))
                            System.IO.File.Delete(file);
                    */
                }
            }
           
            this.Text = "Katalogowanie plików [można katalogować]";
            this.bnCatalogue.Enabled = true;
            this.bnPreview.Enabled = true;
            this.bnShare.Enabled = true;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //this.Text = e.UserState.ToString();
        }

        private void bnPreview_Click(object sender, EventArgs e)
        {
            int index = 0;
            foreach (var obj in nested)
            {
                List<string> summary = new List<string>();
                summary.Add(names[index]);
                foreach (var label in fileLabels.Keys)
                    summary.Add(label);
                foreach (var item in obj)
                {
                    summary.Add(item.ToString());
                }
                ListViewItem row = new ListViewItem();
                
                for (int i = 0; i < 13; i++)
                {
                    row.SubItems.Insert(i, new ListViewItem.ListViewSubItem(new ListViewItem(), summary[i]));
                }

                this.listView1.Items.Add(row);

                index++;    
            }       
        }

        private void bnChooseFolder_Click(object sender, EventArgs e)
        {

            if (names.Count == 0)
            {
                using (FolderBrowserDialog open = new FolderBrowserDialog())
                {
                    if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var files = Directory.GetFiles(open.SelectedPath, "*", SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            names.Add(file);
                        }

                        if (names.Count == 0) MessageBox.Show("Folder " + open.SelectedPath + " jest pusty", "Wybór plików", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }

            var extractors = new List<TikaOnDotNet.TextExtraction.TextExtractor>();

            foreach (var name in names.Distinct())
            {
                extractors.Add(new TikaOnDotNet.TextExtraction.TextExtractor());
                if (name.EndsWith(".mp3") && !System.IO.File.Exists(name.Replace(".mp3", ".wav")))
                        Mp3ToWavConverter.Convert(name);
            }

            foreach (var ext in extractors)

                foreach (var name in names.Distinct())
                {
                    try
                    {
                        var ex = ext.Extract(name);
                        foreach (var _e in ex.Metadata)
                        {
                            metadata.Add(new Tuple<string, string>(_e.Key, _e.Value), name);
                            foreach (var item in metadata.Values)
                                extensionsInLv.Add(item.Substring(item.LastIndexOf(".")));
                        }
                    }
                    catch (Exception) { }
                }

            string extensionsStr = String.Empty;
            string fileLabelsStr = String.Empty;
            foreach (var extension in extensionsInLv.Distinct())
            {
                extensionsStr += extension + " ";
                fileLabelsStr += "<nienazwany$>";
            }

            if (this.txtCommand.Text == String.Empty)
            {
                this.txtCommand.Text = "utwórz etykietę " + fileLabelsStr + " dla każdego pliku " + extensionsStr.TrimEnd(' ');
            }

            Regex rx = new Regex(regex, RegexOptions.IgnoreCase);
            Match m = Regex.Match(exampleCommands[randResult], regex);

            Regex rxCreate = new Regex(regexCreate, RegexOptions.IgnoreCase);
            Match mCreate = Regex.Match(exampleCommandsCreate[randResultCreate], regexCreate);

            var groupRegexCreate = GroupRegex(rxCreate, mCreate);

            Match mtxtCommand = Regex.Match(this.txtCommand.Text, regex);

            foreach (var item in this.GroupRegex(rx, mtxtCommand))
                _group.Add(item);
            var groupRegex = GroupRegex(rx, m);
            var split = txtCommand.Text.Split(' ');

            if (this.txtCommand.Text.Length != 0)
                foreach (var group in this.Groups(groupRegex, groupRegexCreate))
                    foreach (var s in split)
                        if (s == group.Key)
                            equals = true;
            string[] _split = null;
            var notSupported = String.Empty;
            try
            {
                _split = _group[_group.Count - 2].Split(new char[] { ' ', '*', '.' });
                
                foreach (var s in _split)
                    if (s != " " && s != "")
                        if (!extends.Contains(s))
                            notSupported += " " + s;

                if (notSupported.Length > 0) MessageBox.Show("Nieobsługiwana grupa rozszerzeń: " + this.RemoveDuplicates(notSupported), "Katalogowanie plików", MessageBoxButtons.OK, MessageBoxIcon.Hand);

            }
            catch (Exception) { }
            if (equals)
                foreach (var group in this.Groups(groupRegex, groupRegexCreate))
                    this.txtCommand.Text += " " + group.Value;

            if (this.txtCommand.Text.Length != 0)
                try
                {
                    this.txtCommand.Text += " " + _group[5] + " " + _group[_group.Count - 2];
                }
                catch (Exception) { }
            this.txtCommand.Text = RemoveDuplicates(txtCommand.Text);

            Regex rxLabel = new Regex(@"(<[A-ZĄĘŚĆŻŹÓŁa-ząęśćżźół\#\s0-9%\.,]+\$>)", RegexOptions.IgnoreCase);
            Match mLabel = Regex.Match(_group[5], @"(<[A-ZĄĘŚĆŻŹÓŁa-ząęśćżźół\#\s0-9%\.,]+\$>)");

            int i = 0;
            
            foreach (var group in this.GroupRegex(rxLabel, mLabel))
            {
                try
                {
                    if (group != " " && group != "")
                    {
                        fileLabels.Add(group.Trim('<').Trim('>').Replace("$", "(" + (i + 1).ToString() + ")"), _split[2 * i + 1]);
                        i++;
                    }
                }
                catch (Exception) { };
            }
            if (this.txtCommand.TextLength >= 89)
                this.chkUseCreteRule.Enabled = false;
            this.Text = "Katalogowanie plików dźwiękowych [obliczam...]";
            try
            {
                this.backgroundWorker1.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Katalogowanie plików", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void DisposeWave()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing
                    || output.PlaybackState == NAudio.Wave.PlaybackState.Paused)
                    output.Stop();
                output.Dispose();
                output = null;
            }

            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }

        private void Form1_TextChanged(object sender, EventArgs e)
        {
            this.PlayPauseToolStripMenuItem.Text = "&Wznów/Wstrzymaj [00:00]";
            if (!this.Text.Contains("obliczam...")
                && !this.Text.Contains("można katalogować"))
            {
                if (activate.EndsWith(".mp3"))
                {
                    NAudio.Wave.WaveStream pcm = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(new NAudio.Wave.Mp3FileReader(activate));
                    stream = new NAudio.Wave.BlockAlignReductionStream(pcm);

                }
                else
                {
                    try
                    {
                        NAudio.Wave.WaveStream pcm = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(activate));
                        stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
                        wave = new NAudio.Wave.WaveFileReader(activate);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Katalogowanie plików", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }

                output = new NAudio.Wave.DirectSoundOut();
                output.Init(stream);
                output.Play();

                output.PlaybackStopped += Output_PlaybackStopped;

                this.timer1.Start();
            }
        }

        private void Output_PlaybackStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            this.timer1.Stop();
        }

        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TextChanged -= Form1_TextChanged;
            if (output.PlaybackState == NAudio.Wave.PlaybackState.Paused
                || output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
            {
                this.Text = "Katalogowanie plików dźwiękowych [zatrzymałem [" + activate.Substring(activate.LastIndexOf("\\") + 1) + "]]";
                output.Stop();
                this.timer1.Stop();
                this.PlayPauseToolStripMenuItem.Text = "&Wznów/Wstrzymaj [00:00]";
                this.StopToolStripMenuItem.Enabled = false;
            }

            this.TextChanged += Form1_TextChanged;
        }

        private void PlayPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.TextChanged -= Form1_TextChanged;
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    this.Text = "Katalogowanie plików dźwiękowych [wstrzymałem [" + activate.Substring(activate.LastIndexOf("\\") + 1) + "]]";
                    output.Pause();
                    this.timer1.Stop();
                }
                else if (output.PlaybackState == NAudio.Wave.PlaybackState.Paused)
                {
                    this.Text = "Katalogowanie plików dźwiękowych [odtwarzam [" + activate.Substring(activate.LastIndexOf("\\") + 1) + "]]";
                    output.Play();
                    this.timer1.Start();
                    this.StopToolStripMenuItem.Enabled = true;
                }
            }

            this.TextChanged += Form1_TextChanged;
        }

        int second = 0, minute = 0, hour = 0;
        string time = String.Empty;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (second > 59)
            {
                second = 0;
                minute++;
            }

            if (minute > 59)
            {
                minute = 0;
                hour++;
            }
            else
            {
                time = ((hour > 0) ? hour.ToString() + ":" : String.Empty) + ((minute < 10) ? "0" + minute.ToString() : minute.ToString())
                + ":" + ((second < 10) ? "0" + second.ToString() : second.ToString());
            } 

            second++;

            this.PlayPauseToolStripMenuItem.Text = "&Wznów/Wstrzymaj" + " [" + time + "]";
        }
    }


    // Kod legacy, najwyrazniej carry-over z starej wersji programu
    /*
        private void chkMetadata_DoubleClick(object sender, EventArgs e)
        {

        }

        private void chkMetadata_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }
        
        private void button1_Click_1(object sender, EventArgs e)
        {
            var extractor = new TikaOnDotNet.TextExtraction.TextExtractor().Extract(path);
            string xmlNoSpaces = Regex.Replace(extractor.Text, @"\s+", string.Empty);

            Regex rx = new Regex("(<.*?>)", RegexOptions.IgnoreCase);
            Match mTxt = Regex.Match(extractor.Text, "(<.*?>)");
            try
            {
                System.IO.File.SetAttributes(pathTxt, FileAttributes.Normal);
            }
            catch (Exception) { }
            StreamReader reader = new StreamReader(pathTxt);
            
            string line = String.Empty;
            
            string subgroup = String.Empty;
            string _subgroup = String.Empty;
            var regex = @"([A-Za-z]+)(_[A-Za-z]+)(_[0-9]+)$";

            foreach (var group in this.GroupRegex(rx, mTxt))
                if (group.Length > 22)
                {
                    subgroup = group.Substring(group.IndexOf("=")+2);
                    _subgroup = this.Decrypt(Regex.Replace(subgroup, @">$", string.Empty).TrimEnd('"'), true).Substring(8);
                    break;
                }

            Regex rxMachineUser = new Regex(regex, RegexOptions.IgnoreCase);
            Match m = Regex.Match(_subgroup, regex);
            string r = this.GroupRegex(rxMachineUser, m)[0] + this.GroupRegex(rxMachineUser, m)[1] + this.GroupRegex(rxMachineUser, m)[2];
            
            var stream = System.IO.File.OpenRead(pathTxt);
            reader = new StreamReader(stream);
            
            line = Regex.Replace(reader.ReadLine(), @"\$(.+)", String.Empty);
            
            string x = this.Decrypt(Regex.Replace(subgroup, @">$", string.Empty).TrimEnd('"'), true).Substring(8);
            //MessageBox.Show(this.GetRealMachine(x));
            int lenMachine = this.GetRealMachine(x).Length;
            int lenUser = this.GetRealUser(x).Length;

            XDocument xDoc = XDocument.Load(path);

            var query = xDoc.Descendants("Metadata")
                            .Where(parent => parent.Elements("metadata")
                            .Any(child =>
                                ((bool)this.Decrypt(child.Attribute(line).Value, true).Contains(this.GetRealMachine(x))
                                && (bool)this.Decrypt(child.Attribute(line).Value, true).Contains(this.GetRealUser(x)))));
            bool realMachineUser = false;
            string[] assignmentKeyWords = null;
            foreach (var q in query)
            {
                assignmentKeyWords = q.Value.Split('$');
                foreach (var group in this.GroupRegex(rx, mTxt))
                    if (group.Length > 22)
                    {
                        subgroup = group.Substring(group.IndexOf("=") + 2);
                        _subgroup = this.Decrypt(Regex.Replace(subgroup, @">$", string.Empty).TrimEnd('"'), true).Substring(8);
                        break;
                    }
            }
            realMachineUser = Environment.MachineName.Contains(this.GroupRegex(rxMachineUser, m)[0]) && Environment.UserName.Equals(this.GroupRegex(rxMachineUser, m)[1].Replace("_", String.Empty));
            List<string> metadataKey = new List<string>();
            if (realMachineUser)
                foreach (var group in this.GroupRegex(rx, mTxt))
                    if (group.Length <= 32 && !group.Contains("/") && !group.Equals("<Metadata>"))
                        metadataKey.Add(Regex.Replace(group, "<|>", String.Empty));

            var metadataKeyDist = metadataKey.Distinct();
            Dictionary<string, string> assignments = new Dictionary<string, string>();
            
            foreach (var key in metadataKeyDist)
                foreach (var assignment in assignmentKeyWords)
                    if (assignment != String.Empty && !assignments.ContainsKey(key))
                        assignments.Add(key, assignment);
        }
        
        private string GetRealMachine(string input)
        {
            string machine = String.Empty;
            var extractor = new TikaOnDotNet.TextExtraction.TextExtractor().Extract(path);
            Regex rx = new Regex("(<.*?>)", RegexOptions.IgnoreCase);
            Match mTxt = Regex.Match(extractor.Text, "(<.*?>)");
            string subgroup = String.Empty;
            string _subgroup = String.Empty;
            string regex = @"([A-Za-z]+)(_[A-Za-z]+)(_[0-9]+)$";
            foreach (var group in this.GroupRegex(rx, mTxt))
                if (group.Length > 22)
                {
                    subgroup = group.Substring(group.IndexOf("=") + 2);
                    _subgroup = this.Decrypt(Regex.Replace(subgroup, @">$", string.Empty).TrimEnd('"'), true).Substring(8);
                    break;
                }

            Regex rxMachineUser = new Regex(regex, RegexOptions.IgnoreCase);
            Match m = Regex.Match(_subgroup, regex);
            machine = this.GroupRegex(rxMachineUser, m)[0];

            return machine;
        }
        
        private string GetRealUser(string input)
        {
            string user = String.Empty;
            var extractor = new TikaOnDotNet.TextExtraction.TextExtractor().Extract(path);
            Regex rx = new Regex("(<.*?>)", RegexOptions.IgnoreCase);
            Match mTxt = Regex.Match(extractor.Text, "(<.*?>)");
            string subgroup = String.Empty;
            string _subgroup = String.Empty;
            string regex = @"([A-Za-z]+)(_[A-Za-z]+)(_[0-9]+)$";
            foreach (var group in this.GroupRegex(rx, mTxt))
                if (group.Length > 22)
                {
                    subgroup = group.Substring(group.IndexOf("=") + 2);
                    _subgroup = this.Decrypt(Regex.Replace(subgroup, @">$", string.Empty).TrimEnd('"'), true).Substring(8);
                    break;
                }
            Regex rxMachineUser = new Regex(regex, RegexOptions.IgnoreCase);
            Match m = Regex.Match(_subgroup, regex);
            user = this.GroupRegex(rxMachineUser, m)[1].Replace("_", String.Empty);
            return user;
        }

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
        

        private void button3_Click(object sender, EventArgs e)
        {

        }
        private void chkExcludeMetadata_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void PostXML(string v, string requestData)
        {
            
             ///ToDo : umieścic plik XML na serwerze www
             
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
           
        }
        */
}
