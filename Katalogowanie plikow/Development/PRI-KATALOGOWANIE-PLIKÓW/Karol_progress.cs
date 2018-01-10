using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit.Util;
using PRI_KATALOGOWANIE_PLIKÓW.classes;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Karol_progress : Form
    {
        public class ComboBoxLanguage
        {
            public string display;
            public string value;

            public ComboBoxLanguage(string display, string value)
            {
                this.display = display;
                this.value = value;
            }

            public override string ToString()
            {
                return display;
            }

            public string getValue()
            {
                return value;
            }
        }

        public string program_path;
        VideoFile videoFile;
        MediaFile inputFile;
        MediaFile thumbnailFile;
        Engine engine;
        ExtractionOptions extractionOptions;
        public bool proceed;

        public Karol_progress(ref VideoFile videoFile, ref MediaFile inputFile, ref ExtractionOptions extractionOptions, ref string program_path, ref bool proceed)
        {
            this.videoFile = videoFile;
            this.inputFile = inputFile;
            this.program_path = program_path;
            this.engine = new Engine();
            this.extractionOptions = extractionOptions;
            InitializeComponent();

            this.languageComboBox.Items.AddRange(new object[]
            {
                new ComboBoxLanguage("Angielski", "eng"),
                new ComboBoxLanguage("Polski", "pol"),
                new ComboBoxLanguage("Niemiecki", "ger")
            });
            this.languageComboBox.SelectedIndex = 0;

            setThumbnail();
            getFramesToAnalyzeCount(this, new EventArgs());
        }


        public decimal getSamplingFrequency()
        {
            return this.samplingFrequencyInput.Value;
        }


        public string getLanguage()
        {
            string value = "";
            try
            {
                value = ((ComboBoxLanguage)this.languageComboBox.SelectedValue).getValue();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught at getLanguage(): " + e.ToString());
            }
            if (value == "") { value = "eng"; }
            return value;
        }


        public List<Tuple<TimeSpan, TimeSpan>> getTimeRange()
        {
            //if (this.analyzeWholeFileSwitch.Checked || )
            //{
            Tuple<TimeSpan, TimeSpan> t = new Tuple<TimeSpan, TimeSpan>(new TimeSpan(0), inputFile.Metadata.Duration);
            List<Tuple<TimeSpan, TimeSpan>> list = new List<Tuple<TimeSpan, TimeSpan>>();
            list.Add(t);
            return list;
            //}
        }


        private void setThumbnail()
        {
            string thumbnailPath = program_path + @"\temp\thumbnail.png";
            DirectoryInfo thumbnailDir = new DirectoryInfo(program_path + @"\temp");
            if (!thumbnailDir.Exists) thumbnailDir.Create();
            var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(inputFile.Metadata.Duration.Seconds / 10) };
            if (!File.Exists(thumbnailPath))
            {
                thumbnailFile = new MediaFile(thumbnailPath);
                engine.GetThumbnail(inputFile, thumbnailFile, options);
                this.videoThumbnail.ImageLocation = Path.GetFullPath(thumbnailPath);
                this.videoThumbnail.Load();
            }

            this.thumbnailLabel.Text = Path.GetFileName(inputFile.Filename);
        }


        private void getFramesToAnalyzeCount(object sender, EventArgs e)
        {
            decimal freq = getSamplingFrequency();
            long framesToAnalyze = (long)(videoFile.getFramesInTimeSpan() / freq);
            this.framesToAnalyzeDisplay.Text = framesToAnalyze.ToString();
        }


        private void okButton_Click(object sender, EventArgs e)
        {
            videoFile.setLanguage(getLanguage());
            extractionOptions.setSamplingFrequency(getSamplingFrequency());
            extractionOptions.setLanguage(getLanguage());
            getTimeRanges();
            proceed = true;

            this.Close();
            this.Owner.Show();
            this.Dispose();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            proceed = false;

            this.Close();
            this.Owner.Show();
            this.Dispose();
        }


        private void analyzeWholeFileSwitch_Click(object sender, EventArgs e)
        {
            timeRangeListview.Enabled = !analyzeWholeFileSwitch.Checked;
            addTimeRangeButton.Enabled = !analyzeWholeFileSwitch.Checked;
            removeTimeRangeButton.Enabled = !analyzeWholeFileSwitch.Checked;
            timeRangeInput1.Enabled = !analyzeWholeFileSwitch.Checked;
            timeRangeInput2.Enabled = !analyzeWholeFileSwitch.Checked;

            if (analyzeWholeFileSwitch.Checked == false && timeRangeListview.Items.Count == 0)
            {
                okButton.Enabled = false;
            }
            else if (analyzeWholeFileSwitch.Checked == true)
            {
                okButton.Enabled = true;
            }
        }

        private void addTimeRangeButton_Click(object sender, EventArgs e)
        {
            if (timeRangeInput1.MaskCompleted && timeRangeInput2.MaskCompleted)
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("pl-PL");
                string format = @"hh\:mm\:ss";
                TimeSpan start = TimeSpan.ParseExact(timeRangeInput1.Text, format, cultureInfo);
                TimeSpan finish = TimeSpan.ParseExact(timeRangeInput2.Text, format, cultureInfo);
                string durationWithLeadingZeros = "";
                durationWithLeadingZeros += inputFile.Metadata.Duration.Hours < 10 ? "0" + inputFile.Metadata.Duration.Hours : inputFile.Metadata.Duration.Hours.ToString();
                durationWithLeadingZeros += ":";
                durationWithLeadingZeros += inputFile.Metadata.Duration.Minutes < 10 ? "0" + inputFile.Metadata.Duration.Minutes : inputFile.Metadata.Duration.Minutes.ToString();
                durationWithLeadingZeros += ":";
                durationWithLeadingZeros += inputFile.Metadata.Duration.Seconds < 10 ? "0" + inputFile.Metadata.Duration.Seconds : inputFile.Metadata.Duration.Seconds.ToString();

                if (finish.CompareTo(start) < 0)
                {
                    TimeSpan temp = start;
                    start = finish;
                    finish = temp;
                }

                if (timeRangeInput1.Text != timeRangeInput2.Text)
                {
                    if (start.CompareTo(inputFile.Metadata.Duration) < 0)
                    {
                        if (finish.CompareTo(inputFile.Metadata.Duration) <= 0)
                        {
                            timeRangeListview.Items.Add(new ListViewItem(new[] { start.ToString(), finish.ToString() }));
                            if (timeRangeListview.Items.Count != 0 && okButton.Enabled == false) { okButton.Enabled = true; }
                            timeRangeInput1.Clear();
                            timeRangeInput2.Clear();
                        }
                        else
                        {
                            timeRangeListview.Items.Add(new ListViewItem(new[] { start.ToString(), durationWithLeadingZeros }));
                            if (timeRangeListview.Items.Count != 0 && okButton.Enabled == false) { okButton.Enabled = true; }
                            timeRangeInput1.Clear();
                            timeRangeInput2.Clear();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Wykroczono poza zakres trwania pliku video (" + durationWithLeadingZeros + ")");
                    }
                }
            }
            else { MessageBox.Show("Nieprawidłowy format wprowadonego czasu"); }
        }

        private void removeTimeRangeButton_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in timeRangeListview.SelectedItems)
            {
                timeRangeListview.Items.Remove(item);
            }
            if (timeRangeListview.Items.Count == 0 && okButton.Enabled == true) { okButton.Enabled = false; }
        }


        private void validateTime(object sender, CancelEventArgs e)
        {
            DateTime dateTime;
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("pl-PL");
            var maskedTextBox = sender as MaskedTextBox;
            if (!DateTime.TryParseExact(maskedTextBox.Text, "hh:mm:ss", cultureInfo, System.Globalization.DateTimeStyles.None, out dateTime))
            {
                e.Cancel = true;
            }
        }

        private void getTimeRanges()
        {
            if (analyzeWholeFileSwitch.Checked == true)
            {
                TimeSpan start = new TimeSpan(0, 0, 0);
                TimeSpan finish = inputFile.Metadata.Duration;
                //Console.WriteLine(start.ToString());
                extractionOptions.addTimeRange(new TimeRange(start, finish));
            }
            else
            {
                ListView.ListViewItemCollection listItems = timeRangeListview.Items;
                string format = @"hh\:mm\:ss";
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("pl-PL");
                foreach (ListViewItem tr in listItems)
                {
                    try
                    {
                        TimeSpan start = TimeSpan.ParseExact(tr.SubItems[0].Text, format, cultureInfo);
                        TimeSpan finish = TimeSpan.ParseExact(tr.SubItems[1].Text, format, cultureInfo);
                        //Console.WriteLine("new timeRange start: " + start.ToString());
                        if (start.CompareTo(inputFile.Metadata.Duration) < 0)
                        {
                            if (finish.CompareTo(inputFile.Metadata.Duration) <= 0)
                            {
                                extractionOptions.addTimeRange(new TimeRange(start, finish));
                            }
                        }
                    }
                    catch (Exception except)
                    {
                        Console.WriteLine("Exception in getTimeRanges(), " + except.Message);
                    }
                }
            }
        }
    }
}
