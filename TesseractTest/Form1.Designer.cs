namespace TesseractTest
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.button1 = new System.Windows.Forms.Button();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.colorDialog1 = new System.Windows.Forms.ColorDialog();
      this.outputTextBox = new System.Windows.Forms.TextBox();
      this.outputTextBoxLabel = new System.Windows.Forms.Label();
      this.processFilesButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(21, 12);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(219, 33);
      this.button1.TabIndex = 0;
      this.button1.Text = "Wybierz plik(i)";
      this.button1.Click += new System.EventHandler(this.openFileSelect);
      // 
      // openFileDialog1
      // 
      this.openFileDialog1.FileName = "openFileDialog1";
      this.openFileDialog1.Multiselect = true;
      this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
      // 
      // outputTextBox
      // 
      this.outputTextBox.Location = new System.Drawing.Point(21, 93);
      this.outputTextBox.Multiline = true;
      this.outputTextBox.Name = "outputTextBox";
      this.outputTextBox.ReadOnly = true;
      this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.outputTextBox.Size = new System.Drawing.Size(219, 256);
      this.outputTextBox.TabIndex = 1;
      this.outputTextBox.TabStop = false;
      // 
      // outputTextBoxLabel
      // 
      this.outputTextBoxLabel.AutoSize = true;
      this.outputTextBoxLabel.Location = new System.Drawing.Point(94, 77);
      this.outputTextBoxLabel.Name = "outputTextBoxLabel";
      this.outputTextBoxLabel.Size = new System.Drawing.Size(71, 13);
      this.outputTextBoxLabel.TabIndex = 2;
      this.outputTextBoxLabel.Text = "Wybrane pliki";
      // 
      // processFilesButton
      // 
      this.processFilesButton.Location = new System.Drawing.Point(281, 12);
      this.processFilesButton.Name = "processFilesButton";
      this.processFilesButton.Size = new System.Drawing.Size(210, 337);
      this.processFilesButton.TabIndex = 3;
      this.processFilesButton.Text = "GO!";
      this.processFilesButton.UseVisualStyleBackColor = true;
      this.processFilesButton.Click += new System.EventHandler(this.processVideoFiles);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.ClientSize = new System.Drawing.Size(516, 361);
      this.Controls.Add(this.processFilesButton);
      this.Controls.Add(this.outputTextBoxLabel);
      this.Controls.Add(this.outputTextBox);
      this.Controls.Add(this.button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "Form1";
      this.Text = "Tesseract test";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.ColorDialog colorDialog1;
    private System.Windows.Forms.TextBox outputTextBox;
    private System.Windows.Forms.Label outputTextBoxLabel;
    private System.Windows.Forms.Button processFilesButton;
  }
}

