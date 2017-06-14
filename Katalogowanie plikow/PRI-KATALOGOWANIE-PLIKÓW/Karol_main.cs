﻿using System;
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


namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Karol_main : Form
    {
        public event EventHandler OnDataAvalible;
        public List<string> filenames;
        public string program_path;
        BackgroundWorker bgwFileProcessor;

        public List<Tuple<string, string>> text_extraction_results;


        public Karol_main()
        {
            InitializeComponent();
            if(filenames == null) filenames = new List<string>();

            openFileDialogButton.Enabled = false;
            bgwFileProcessor = new BackgroundWorker();
            bgwFileProcessor.WorkerReportsProgress = true;
            bgwFileProcessor.WorkerSupportsCancellation = true;
            bgwFileProcessor.DoWork += new DoWorkEventHandler(bgwFileProcessor_DoWork);
            bgwFileProcessor.RunWorkerCompleted += new RunWorkerCompletedEventHandler(enableFormButtons);
            bgwFileProcessor.ProgressChanged += new ProgressChangedEventHandler(bgwFileProcessor_ReportProgress);
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
            if(e.ProgressPercentage < 100) framesCountProgress.Value = e.ProgressPercentage;
            framesCountProgress.Refresh();
        }


        void enableFormButtons(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Ekstrakcja zakonczona!");
            processFilesButton.Enabled = true;
            //openFileDialogButton.Enabled = true;
            cancelFileProcessingButton.Enabled = false;

            text_extraction_results = new List<Tuple<string, string>>();
            text_extraction_results.Add((Tuple<string, string>)e.Result);
            OnDataAvalible(this, EventArgs.Empty);

            this.Close();
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
                using (var engine = new TesseractEngine(program_path + @"/tessdata", "pol", EngineMode.Default))
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


        private Tuple<string,string> extractTagsFromFile(ref VideoFile file, ref MediaFile input, ref ExtractionOptions extractionOptions, ref DoWorkEventArgs e)
        {
            Tuple<string, string> result = new Tuple<string, string>(string.Empty,string.Empty);
            string frameDirectory = program_path + @"\temp";
            if (!Directory.Exists(frameDirectory))
            {
                Directory.CreateDirectory(frameDirectory);
            }
            string framePath = frameDirectory + @"\img.png";

            TimeSpan timePointer = new TimeSpan(0);
            TimeSpan timeStep = new TimeSpan((long)(10000000 * extractionOptions.getSamplingFrequency()));

            var outputFile = new MediaFile { Filename = framePath };
            using (var engine = new Engine())
            {
                engine.GetMetadata(input);

                // Saves the frame located on the x-th second of the video.
                while (timePointer.CompareTo(input.Metadata.Duration) < 0)
                {
                    if (bgwFileProcessor.CancellationPending == true)
                    {
                        e.Cancel = true;
                    }
                    var options = new ConversionOptions { Seek = TimeSpan.FromMilliseconds(timePointer.TotalMilliseconds) };
                    engine.GetThumbnail(input, outputFile, options);
                    file.addTags(extractTagsFromFrame(framePath, file.getLanguage()));
                    timePointer = timePointer.Add(timeStep);

                    bgwFileProcessor.ReportProgress((int)(Math.Floor(100 * (timePointer.TotalSeconds / input.Metadata.Duration.TotalSeconds))));
                    //Console.WriteLine("Progress at " + (int)(Math.Floor(100 * (timePointer.TotalSeconds / input.Metadata.Duration.TotalSeconds))) + "% (" + timePointer.TotalSeconds / input.Metadata.Duration.TotalSeconds + "%)");
                    //outputTextBox.AppendText("Time step: " + timeStep.Hours + ":" + timeStep.Minutes + ":" + timeStep.Seconds + "." + timeStep.Milliseconds + "/" + Environment.NewLine);
                    //outputTextBox.AppendText("" + timePointer.Hours + ":" + timePointer.Minutes + ":" + timePointer.Seconds + "." + timePointer.Milliseconds + "/" + Environment.NewLine);
                }
            }

            /* Stara logika tworzenia folderu do którego wrzucamy plik z zapisanymi tagami
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
            */
            string tag_container = string.Empty;
            foreach (string tag in file.getTags())
            {
                tag_container += tag + Environment.NewLine;
            }

            Tuple<string, string> file_extract = new Tuple<string, string>(
                file.getFilePath(),
                tag_container);

            return file_extract;
            //outputTextBox.AppendText("Done!" + Environment.NewLine);
        }


        private void GetExtractionOptions(ref VideoFile file, ref MediaFile input)
        {
            ExtractionOptions extractionOptions = new ExtractionOptions();
            Karol_progress form2 = new Karol_progress(ref file, ref input, ref extractionOptions, ref program_path);
            form2.ShowDialog();
            if(File.Exists(program_path + @"\temp\thumbnail.png")) File.Delete(program_path + @"\temp\thumbnail.png");

            List<object> bgwArgs = new List<object>();
            bgwArgs.Add(file);
            bgwArgs.Add(input);
            bgwArgs.Add(extractionOptions);
            bgwFileProcessor.RunWorkerAsync(bgwArgs);
            //extractTagsFromFile(ref file, ref input, ref extractionOptions);
            //System.IO.File.Delete(Directory.GetCurrentDirectory() + @"\temp\img.png");
        }


        private void processVideoFiles(object sender, System.EventArgs e)
        {
            if (!(filenames.Count == 0))
            {
                processFilesButton.Enabled = false;
                openFileDialogButton.Enabled = false;
                cancelFileProcessingButton.Enabled = true;
                fileCountProgress.Maximum = filenames.Count;

                fileCountProgress.Value = 0;
                framesCountProgress.Value = 0;

                if (filenames.Count > 0)
                {
                    foreach (string f in filenames)
                    {
                        var input = new MediaFile { Filename = f };
                        using (var engine = new Engine())
                        {
                            engine.GetMetadata(input);
                        }
                        Console.WriteLine(input.Metadata.VideoData.Fps);
                        try
                        {
                            double fps = input.Metadata.VideoData.Fps;
                            TimeSpan time = input.Metadata.Duration;
                            VideoFile file = new VideoFile(f, fps, time);
                            GetExtractionOptions(ref file, ref input);

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
                    filenames.Clear();
                }
            }
        }

        public void refresh_display()
        {
            if(filenames.Count != 0)
            {
                outputTextBox.Text = string.Empty;
                foreach(string name in filenames) outputTextBox.AppendText(name + @";" + Environment.NewLine);
            }
        }

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
                                if (!filenames.Contains(name))
                                {
                                    filenames.Add(name);
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


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

    }
}
