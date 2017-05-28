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

namespace TesseractTest
{
  public partial class Form2 : Form
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

    MediaFile inputFile;
    MediaFile thumbnailFile;
    Engine engine;
    

    public Form2(MediaFile inputFile)
    {
      this.inputFile = inputFile;
      this.engine = new Engine();
      InitializeComponent();
    }


    public decimal getSamplingFrequency()
    {
      return this.samplingFrequencyInput.Value;
    }


    public string getLanguage()
    {
      return ((ComboBoxLanguage)this.languageComboBox.SelectedValue).getValue();
    }


    private void setThumbnail()
    {
      string thumbnailPath = Directory.GetCurrentDirectory() + @"\temp\thumbnail.png";
      var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(inputFile.Metadata.Duration.Seconds / 10) };
      thumbnailFile = new MediaFile(thumbnailPath);
      engine.GetThumbnail(inputFile, thumbnailFile, options);
      this.videoThumbnail.ImageLocation = thumbnailPath;
    }


    private void okButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
