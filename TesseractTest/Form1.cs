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

namespace TesseractTest
{
  public partial class Form1 : Form
  {
    List<string> filenames;

    class VideoFile
    {
      private string language;
      private string filePath;
      private HashSet<string> tags;
      
      private double fps;
      private TimeSpan time;

      // Getters
      public double getFps()
      {
        return fps;
      }

      public string getLanguage()
      {
        return language;
      }

      public string getFilePath()
      {
        return filePath;
      }

      public HashSet<string> getTags()
      {
        return tags;
      }

      // Constructors
      public VideoFile(string filePath, double fps, TimeSpan time, string language = "eng")
      {
        this.filePath = filePath;
        this.fps = fps;
        this.time = time;
        this.language = language;
        tags = new HashSet<string>();
      }


      public void addTags(HashSet<string> newTags)
      {
        tags.Concat(newTags);
      }


      public double getFramesInTimeSpan()
      {
        return fps * time.Seconds;
      }
      public double getFramesInTimeSpan(TimeSpan timeSpan)
      {
        return fps * timeSpan.Seconds;
      }
    }
    

    public Form1()
    {
      InitializeComponent();
      filenames = new List<string>();
    }


    private HashSet<string> extractTagsFromFrame(string filePath, string language)
    {
      HashSet<string> tags = new HashSet<string>();
      try
      {
        using (var engine = new TesseractEngine(@"../../tessdata", "pol", EngineMode.Default))
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

              foreach (var t in tags)
              {
                //outputTextBox.AppendText(t + Environment.NewLine);
                outputTextBox.AppendText("[" + t + "] ");
              }
              outputTextBox.AppendText(Environment.NewLine);
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


    private void extractTagsFromFile(VideoFile file)
    {
      String frameDirectory = Directory.GetCurrentDirectory() + @"\temp";
      if (!Directory.Exists(frameDirectory))
      {
        Directory.CreateDirectory(frameDirectory);
      }
      String framePath = frameDirectory + @"\img.png";
      
      var inputFile = new MediaFile { Filename = file.getFilePath() };
      var outputFile = new MediaFile { Filename = framePath };

      using (var engine = new Engine())
      {
        engine.GetMetadata(inputFile);

        // Saves the frame located on the x-th second of the video.
        var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(7) };
        engine.GetThumbnail(inputFile, outputFile, options);
      }
      file.addTags(extractTagsFromFrame(framePath, file.getLanguage()));

      outputTextBox.AppendText("Done!" + Environment.NewLine);
    }


    private void ShowExtractionOptions(VideoFile file, MediaFile input)
    {
      Form2 form2 = new Form2(input);
      form2.ShowDialog();
    }


    private void Form1_Load(object sender, EventArgs e)
    {

    }


    private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
    {

    }


    private void processVideoFiles(object sender, System.EventArgs e)
    {
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
          try {
            double fps = input.Metadata.VideoData.Fps;
            TimeSpan time = input.Metadata.Duration;
            VideoFile file = new VideoFile(f, fps, time);
            ShowExtractionOptions(file, input);

            extractTagsFromFile(file);
            HashSet<string> gottenTags = file.getTags();
            foreach(var tag in gottenTags)
            {
              outputTextBox.AppendText("[" + tag + "]");
            }
          }
          catch(Exception exc)
          {
            Console.WriteLine(exc.Message);
          }
        }
      }
    }


    private void openFileSelect(object sender, System.EventArgs e)
    {
      Stream myStream = null;
      OpenFileDialog openFileDialog1 = new OpenFileDialog();

      if (string.IsNullOrWhiteSpace(outputTextBox.Text))
      {
        outputTextBox.AppendText("Opened files:" + Environment.NewLine);
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
              foreach(string name in openFileDialog1.FileNames)
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

  }
}
