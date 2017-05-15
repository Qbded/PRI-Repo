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

namespace TesseractTest
{
  public partial class Form1 : Form
  {
    List<string> extractedText;

    public Form1()
    {
      InitializeComponent();

      extractedText = new List<string>();
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }

    private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
    {

    }

    private void button1_Click(object sender, System.EventArgs e)
    {
      Stream myStream = null;
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      //VideoFileReader vidReader = new VideoFileReader();

      if (string.IsNullOrWhiteSpace(outputTextBox.Text))
      {
        outputTextBox.AppendText("Opened files:" + Environment.NewLine);
      }

      string userFolderDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos);
      if (userFolderDirectory == "") { userFolderDirectory = @"C:\"; }
      openFileDialog1.InitialDirectory = userFolderDirectory;
      openFileDialog1.Filter = @"Pliki wideo (*.avi;*.wmv)|*.avi; *.wmv|Wszystie pliki (*.*)|*.*";
      openFileDialog1.FilterIndex = 2;
      openFileDialog1.RestoreDirectory = true;
      openFileDialog1.Multiselect = true;

      HashSet<string> tags;

      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        try
        {
          if ((myStream = openFileDialog1.OpenFile()) != null)
          {
            using (myStream)
            {
              // Insert code to read the stream here.
              string[] filenames = openFileDialog1.FileNames;
              int totalFrames = 0;
              //outputTextBox.AppendText(Directory.GetCurrentDirectory());
              foreach (string f in filenames)
              {
                outputTextBox.AppendText(f + @";" + Environment.NewLine);

                //var image = new Bitmap(f);
                try
                {
                  using (var engine = new TesseractEngine(@"../../tessdata", "eng", EngineMode.Default))
                  {
                    using (var img = Pix.LoadFromFile(f))
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
                                              '“', '‘', '”', '„', '›' };
                        string[] unprocessedTags = text.Split(delimiters);
                        tags = new HashSet<string>(unprocessedTags);

                        outputTextBox.AppendText("Mean confidence: " + page.GetMeanConfidence() + Environment.NewLine);

                        foreach(var t in tags)
                        {
                          outputTextBox.AppendText(t + Environment.NewLine);
                        }
                      }
                    }
                  }
                }
                catch (Exception e2)
                {
                  Trace.TraceError(e.ToString());
                  outputTextBox.AppendText("Unexpected Error: " + e2.Message + Environment.NewLine);
                  outputTextBox.AppendText("Details: ");
                  outputTextBox.AppendText(e.ToString() + Environment.NewLine);
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message + Environment.NewLine);
        }
      }
    }

    private void OutputTextBox_TextChanged(object sender, EventArgs e)
    {

    }

  }
}