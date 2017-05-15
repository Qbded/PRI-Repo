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
      this.samplingFrequencyInputLabel = new System.Windows.Forms.Label();
      this.samplingFrequencyInput = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this.samplingFrequencyInput)).BeginInit();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(21, 12);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(219, 33);
      this.button1.TabIndex = 0;
      this.button1.Text = "Wybierz plik(i)";
      this.button1.Click += new System.EventHandler(this.button1_Click);
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
      this.outputTextBox.Size = new System.Drawing.Size(219, 204);
      this.outputTextBox.TabIndex = 1;
      this.outputTextBox.TabStop = false;
      this.outputTextBox.TextChanged += new System.EventHandler(this.OutputTextBox_TextChanged);
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
      // samplingFrequencyInputLabel
      // 
      this.samplingFrequencyInputLabel.AutoSize = true;
      this.samplingFrequencyInputLabel.Location = new System.Drawing.Point(296, 22);
      this.samplingFrequencyInputLabel.Name = "samplingFrequencyInputLabel";
      this.samplingFrequencyInputLabel.Size = new System.Drawing.Size(107, 13);
      this.samplingFrequencyInputLabel.TabIndex = 4;
      this.samplingFrequencyInputLabel.Text = "Próbkuj co (1s - 0.1s)";
      // 
      // samplingFrequencyInput
      // 
      this.samplingFrequencyInput.DecimalPlaces = 1;
      this.samplingFrequencyInput.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.samplingFrequencyInput.Location = new System.Drawing.Point(409, 19);
      this.samplingFrequencyInput.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.samplingFrequencyInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.samplingFrequencyInput.Name = "samplingFrequencyInput";
      this.samplingFrequencyInput.Size = new System.Drawing.Size(62, 20);
      this.samplingFrequencyInput.TabIndex = 5;
      this.samplingFrequencyInput.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.ClientSize = new System.Drawing.Size(516, 361);
      this.Controls.Add(this.samplingFrequencyInput);
      this.Controls.Add(this.samplingFrequencyInputLabel);
      this.Controls.Add(this.outputTextBoxLabel);
      this.Controls.Add(this.outputTextBox);
      this.Controls.Add(this.button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "Form1";
      this.Text = "Tesseract test";
      this.Load += new System.EventHandler(this.Form1_Load);
      ((System.ComponentModel.ISupportInitialize)(this.samplingFrequencyInput)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.ColorDialog colorDialog1;
    private System.Windows.Forms.TextBox outputTextBox;
    private System.Windows.Forms.Label outputTextBoxLabel;
    private System.Windows.Forms.Label samplingFrequencyInputLabel;
    private System.Windows.Forms.NumericUpDown samplingFrequencyInput;
  }
}

