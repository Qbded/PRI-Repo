using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using MediaToolkit.Model;
using MediaToolkit;
using MediaToolkit.Options;
using PRI_KATALOGOWANIE_PLIKÓW.classes;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Karol_main : Form
    {
        public event EventHandler OnDataAvalible;
        public List<string> names;
        public List<Tuple<int, string>> filepaths;
        public string program_path;
        BackgroundWorker bgwFileProcessor;

        private string engineLocation = ConfigManager.ReadString(ConfigManager.PROGRAM_LOCATION) + "tessdata";
        private event EventHandler OnAllDone;
        private event EventHandler OnPartDone;
        private int current_file = 0;


        public List<Tuple<int, string, string>> text_extraction_results;


        public Karol_main()
        {
            InitializeComponent();
            if (filepaths == null) filepaths = new List<Tuple<int, string>>();
            text_extraction_results = new List<Tuple<int, string, string>>();

            bgwFileProcessor = new BackgroundWorker();
            bgwFileProcessor.WorkerReportsProgress = true;
            bgwFileProcessor.WorkerSupportsCancellation = true;
            bgwFileProcessor.DoWork += new DoWorkEventHandler(bgwFileProcessor_DoWork);
            bgwFileProcessor.RunWorkerCompleted += new RunWorkerCompletedEventHandler(enableFormButtons);
            bgwFileProcessor.ProgressChanged += new ProgressChangedEventHandler(bgwFileProcessor_ReportProgress);
            this.OnAllDone += new EventHandler(Karol_main_all_done);
            this.OnPartDone += new EventHandler(Karol_main_part_done);
        }

        private void Karol_main_part_done(object sender, EventArgs e)
        {
            // Ekstrakcja idzie tak samo, ale dla current_file zwiększonego o jeden.
            text_extraction_results.Add(new Tuple<int, string, string>(filepaths[current_file].Item1 ,filepaths[current_file].Item2, string.Empty));
            var input = new MediaFile { Filename = filepaths[current_file].Item2 };
            using (var engine = new Engine())
            {
                engine.GetMetadata(input);
            }
            Console.WriteLine(input.Metadata.VideoData.Fps);
            try
            {
                double fps = input.Metadata.VideoData.Fps;
                TimeSpan time = input.Metadata.Duration;
                VideoFile file = new VideoFile(filepaths[current_file].Item2, fps, time);
                
                getExtractionOptions(ref file, ref input);

                HashSet<string> gottenTags = file.getTags();
                //if (gottenTags.Count() > 0)
                //{
                //  foreach (var tag in gottenTags)
                //  {
                //    outputTextBox.AppendText("[" + tag + "]");
                //  }
                //}
                //else
                //{
                //  outputTextBox.AppendText("No tags were gotten" + Environment.NewLine);
                //}
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        private void Karol_main_all_done(object sender, EventArgs e)
        {
            OnDataAvalible(this, EventArgs.Empty);
            this.Close();
            this.Dispose();
        }

        public void bgwFileProcessor_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker bgw = sender as BackgroundWorker;
            List<object> args = e.Argument as List<object>;
            VideoFile file = args.ElementAt(0) as VideoFile;
            MediaFile input = args.ElementAt(1) as MediaFile;
            ExtractionOptions extractionOptions = args.ElementAt(2) as ExtractionOptions;

            e.Result = extractTagsFromFile(ref file, ref input, ref extractionOptions, ref e);
        }



        void bgwFileProcessor_ReportProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 100) framesCountProgress.Value = 100;
            else framesCountProgress.Value = e.ProgressPercentage;
            framesCountProgress.Refresh();
        }

        void enableFormButtons(object sender, RunWorkerCompletedEventArgs e)
        {
            processFilesButton.Enabled = true;
            cancelFileProcessingButton.Enabled = false;

            if(e.Cancelled)
            {
                current_file++;

                processFilesButton.Enabled = false;
                openFileDialogButton.Enabled = false;
                cancelFileProcessingButton.Enabled = true;

                if (text_extraction_results.Count == filepaths.Count)
                {
                    if (!text_extraction_results.Last().Item2.Equals(string.Empty)) Karol_main_all_done(this, new EventArgs());
                }
                else Karol_main_part_done(this, new EventArgs());
            }
            else
            {
                if (!e.Result.Equals(string.Empty))
                {
                    Tuple<int, string, string> data_to_return = new Tuple<int, string, string>(text_extraction_results.Last().Item1,
                                                                                               text_extraction_results.Last().Item2,
                                                                                               (string)e.Result);
                    text_extraction_results[text_extraction_results.IndexOf(text_extraction_results.Last())] = data_to_return;
                }

                current_file++;

                processFilesButton.Enabled = false;
                openFileDialogButton.Enabled = false;
                cancelFileProcessingButton.Enabled = true;

                if (text_extraction_results.Count == filepaths.Count)
                {
                    if (!text_extraction_results.Last().Item2.Equals(string.Empty)) Karol_main_all_done(this, new EventArgs());
                }
                else Karol_main_part_done(this, new EventArgs());
            }
        }

        void cancelFileProcessing(object sender, EventArgs e)
        {
            if (bgwFileProcessor.WorkerSupportsCancellation == true)
            {
                bgwFileProcessor.CancelAsync();
            }
        }

        private HashSet<string> extractTagsFromFrame(string filePath, string language)
        {
            HashSet<string> tags = new HashSet<string>();
            try
            {
                using (var engine = new TesseractEngine(engineLocation, "pol", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(filePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            char[] delimiters = { ',', '.', '/', '<' , '>',
                                    '?', ';', '\'', '\\', ':',
                                    '"', '|', '[', ']', '{',
                                    '}', '-', '=', '_', '+',
                                    '!', '@', '#', '$', '%',
                                    '^', '&', '*', '(', ')',
                                    ' ', '`', '~', '’', '—',
                                    '“', '‘', '”', '„', '›',
                                    '0', '1', '2', '3', '4',
                                    '5', '6', '7', '8', '9',
                                    '\n', '\t', '\r' };
                            string[] unprocessedTags = text.Split(delimiters);
                            unprocessedTags = unprocessedTags.Select(s => s.ToLower()).ToArray();
                            tags = new HashSet<string>(unprocessedTags);
                            //System.IO.File.WriteAllText("./" + Path.GetFileNameWithoutExtension(filePath) + "_tags", tags);

                            //outputTextBox.AppendText("Mean confidence: " + page.GetMeanConfidence() + Environment.NewLine);

                            //foreach (var t in tags)
                            //{
                            //  outputTextBox.AppendText("[" + t + "] ");
                            //}
                            //outputTextBox.AppendText(Environment.NewLine);
                        }
                    }
                }
            }
            catch (Exception e2)
            {
                Trace.TraceError(e2.ToString());
                outputTextBox.AppendText("Unexpected Error: " + e2.Message + Environment.NewLine);
                outputTextBox.AppendText("Details: ");
                outputTextBox.AppendText(e2.ToString() + Environment.NewLine);
            }

            return tags;
        }


        private string extractTagsFromFile(ref VideoFile file, ref MediaFile input, ref ExtractionOptions extractionOptions, ref DoWorkEventArgs e)
        {
            string result = string.Empty;

            string frameDirectory = program_path + @"\temp";
            if (!Directory.Exists(frameDirectory))
            {
                Directory.CreateDirectory(frameDirectory);
            }
            string framePath = frameDirectory + @"\img.png";

            var outputFile = new MediaFile { Filename = framePath };
            using (var engine = new Engine())
            {
                TimeSpan totalDuration = new TimeSpan(0);
                foreach (TimeRange timeRange in extractionOptions.timeRanges)
                {
                    totalDuration = totalDuration.Add(timeRange.finish.Subtract(timeRange.start));
                }

                engine.GetMetadata(input);

                TimeSpan timeLapsed = new TimeSpan(0);
                foreach (TimeRange timeRange in extractionOptions.timeRanges)
                {
                    if (e.Cancel == false)
                    {
                        TimeSpan timePointer = timeRange.start;
                        TimeSpan timeStep = new TimeSpan((long)(10000000 * extractionOptions.getSamplingFrequency()));
                        // Saves the frame located on the x-th second of the video.
                        while (timePointer.CompareTo(timeRange.finish) < 0)
                        {
                            if (bgwFileProcessor.CancellationPending == true)
                            {
                                e.Cancel = true;
                                break;
                            }
                            var options = new ConversionOptions { Seek = TimeSpan.FromMilliseconds(timePointer.TotalMilliseconds) };
                            engine.GetThumbnail(input, outputFile, options);
                            file.addTags(extractTagsFromFrame(framePath, file.getLanguage()));
                            timePointer = timePointer.Add(timeStep);
                            timeLapsed = timeLapsed.Add(timeStep);

                            bgwFileProcessor.ReportProgress((int)(Math.Floor(100 * (timeLapsed.TotalSeconds / totalDuration.TotalSeconds))));
                            //Console.WriteLine("Progress at " + (int)(Math.Floor(100 * (timePointer.TotalSeconds / input.Metadata.Duration.TotalSeconds))) + "% (" + timePointer.TotalSeconds / input.Metadata.Duration.TotalSeconds + "%)");
                            //outputTextBox.AppendText("Time step: " + timeStep.Hours + ":" + timeStep.Minutes + ":" + timeStep.Seconds + "." + timeStep.Milliseconds + "/" + Environment.NewLine);
                            //outputTextBox.AppendText("" + timePointer.Hours + ":" + timePointer.Minutes + ":" + timePointer.Seconds + "." + timePointer.Milliseconds + "/" + Environment.NewLine);
                        }
                    } else
                    {
                        break;
                    }
                }
            }

            if (e.Cancel == false)
            {

                string resultDirectory = program_path + @"\result\";
                if (!Directory.Exists(resultDirectory))
                {
                    Directory.CreateDirectory(resultDirectory);
                }
                StreamWriter sw = new StreamWriter(resultDirectory + Path.GetFileNameWithoutExtension(file.getFilePath()) + @"_tags.txt");
                foreach (string tag in file.getTags())
                {
                    Console.WriteLine(tag);
                    sw.WriteLineAsync(tag);
                    sw.Flush();
                }
                sw.Close();

                result = program_path + @"\result\" + Path.GetFileNameWithoutExtension(file.getFilePath()) + @"_tags.txt";
            }

            return result;

            //outputTextBox.AppendText("Done!" + Environment.NewLine);
        }


        private void getExtractionOptions(ref VideoFile file, ref MediaFile input)
        {
            ExtractionOptions extractionOptions = new ExtractionOptions();
            bool proceed = false;

            Karol_progress form2 = new Karol_progress(ref file, ref input, ref extractionOptions, ref program_path, ref proceed);
            form2.Owner = this;
            form2.ShowDialog();
            proceed = form2.proceed;
            if(File.Exists(program_path + @"\temp\thumbnail.png")) System.IO.File.Delete(program_path + @"\temp\thumbnail.png");
            extractionOptions.DEBUG_displayTimeRanges();
            if (proceed)
            {
                Console.WriteLine("Proceeded.");
                List<object> bgwArgs = new List<object>();
                bgwArgs.Add(file);
                bgwArgs.Add(input);
                bgwArgs.Add(extractionOptions);
                bgwFileProcessor.RunWorkerAsync(bgwArgs);
            }
            if(!proceed)
            {
                if (text_extraction_results.Count < filepaths.Count)
                {
                    current_file++;
                    Karol_main_part_done(this, new EventArgs());
                }
                else
                {
                    Karol_main_all_done(this, new EventArgs());
                }
            }
        }


        private void processVideoFiles(object sender, System.EventArgs e)
        {
            if (!(filepaths.Count == 0))
            {
                processFilesButton.Enabled = false;
                openFileDialogButton.Enabled = false;
                cancelFileProcessingButton.Enabled = true;
                fileCountProgress.Maximum = filepaths.Count;

                fileCountProgress.Value = 0;
                framesCountProgress.Value = 0;

                current_file = filepaths.IndexOf(filepaths.First());
                Karol_main_part_done(this, new EventArgs());
            }
        }

        private void Karol_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((Special_function_window)this.Owner).Controls_set_lock(false);
        }


        /*
        private void openFileSelect(object sender, System.EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            if (string.IsNullOrWhiteSpace(outputTextBox.Text))
            {
                outputTextBox.AppendText("Wybrane pliki:" + Environment.NewLine);
            }

            string userFolderDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos);
            if (userFolderDirectory == "") { userFolderDirectory = @"C:\"; }
            openFileDialog1.InitialDirectory = userFolderDirectory;
            openFileDialog1.Filter = @"Pliki wideo (*.avi;*.wmv)|*.avi; *.wmv";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here.
                            foreach (string name in openFileDialog1.FileNames)
                            {
                                if (filepaths.Find(x => x.Item2.Equals(name)) == null)
                                {
                                    filepaths.Add(name);
                                    outputTextBox.AppendText(name + @";" + Environment.NewLine);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read image file from disk. Original error: " + ex.Message + Environment.NewLine);
                }
            }
        }
        */

        private void openFileSelect(object sender, System.EventArgs e)
        {
            //Nie używane w wersji ostatecznej, pliki do przeróbki zapewnia katalog.
        }

        public void display_refresh()
        {
            //foreach (Tuple<int,string> file in filepaths) outputTextBox.AppendText(file.Item2 + Environment.NewLine);
            foreach (string name in names) outputTextBox.AppendText(name + Environment.NewLine);
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}