﻿using System;
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
    

    public Karol_progress(ref VideoFile videoFile, ref MediaFile inputFile, ref ExtractionOptions extractionOptions, ref string program_path)
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

      setThumbnail();
      getFramesToAnalyze(this, new EventArgs());
    }


    public decimal getSamplingFrequency()
    {
      return this.samplingFrequencyInput.Value;
    }


    public string getLanguage()
    {
      string value = "";
      try {
        value = ((ComboBoxLanguage)this.languageComboBox.SelectedValue).getValue();
      }
      catch(Exception e)
      {
        Console.WriteLine("Exception caught at getLanguage(): " + e.ToString());
      }
      if(value == "") { value = "eng"; }
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
      var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(inputFile.Metadata.Duration.Seconds / 10) };
      thumbnailFile = new MediaFile(thumbnailPath);
      engine.GetThumbnail(inputFile, thumbnailFile, options);
      this.videoThumbnail.ImageLocation = Path.GetFullPath(thumbnailPath);
      this.videoThumbnail.Load();

      this.thumbnailLabel.Text = Path.GetFileName(inputFile.Filename);
    }


    private void getFramesToAnalyze(object sender, EventArgs e)
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

      this.Close();
      this.Dispose();
    }


    private void analyzeWholeFileSwitch_Click(object sender, EventArgs e)
    {
      timeRangeListbox.Enabled = !analyzeWholeFileSwitch.Checked;
      addTimeRangeButton.Enabled = !analyzeWholeFileSwitch.Checked;
    }
  }
}
