namespace TesseractTest
{
  partial class Form2
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
      this.samplingFrequencyInput = new System.Windows.Forms.NumericUpDown();
      this.samplingFrequencyInputLabel = new System.Windows.Forms.Label();
      this.languageComboBox = new System.Windows.Forms.ComboBox();
      this.languageComboBoxLabel = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.analyzeWholeFileSwitch = new System.Windows.Forms.CheckBox();
      this.videoThumbnail = new System.Windows.Forms.PictureBox();
      this.thumbnailLabel = new System.Windows.Forms.Label();
      this.okButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.samplingFrequencyInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.videoThumbnail)).BeginInit();
      this.SuspendLayout();
      // 
      // samplingFrequencyInput
      // 
      this.samplingFrequencyInput.DecimalPlaces = 1;
      this.samplingFrequencyInput.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.samplingFrequencyInput.Location = new System.Drawing.Point(632, 46);
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
      this.samplingFrequencyInput.TabIndex = 9;
      this.samplingFrequencyInput.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
      // 
      // samplingFrequencyInputLabel
      // 
      this.samplingFrequencyInputLabel.AutoSize = true;
      this.samplingFrequencyInputLabel.Location = new System.Drawing.Point(519, 49);
      this.samplingFrequencyInputLabel.Name = "samplingFrequencyInputLabel";
      this.samplingFrequencyInputLabel.Size = new System.Drawing.Size(107, 13);
      this.samplingFrequencyInputLabel.TabIndex = 6;
      this.samplingFrequencyInputLabel.Text = "Próbkuj co (1s - 0.1s)";
      // 
      // languageComboBox
      // 
      this.languageComboBox.FormattingEnabled = true;
      this.languageComboBox.Location = new System.Drawing.Point(573, 19);
      this.languageComboBox.Name = "languageComboBox";
      this.languageComboBox.Size = new System.Drawing.Size(121, 21);
      this.languageComboBox.TabIndex = 8;
      // 
      // languageComboBoxLabel
      // 
      this.languageComboBoxLabel.AutoSize = true;
      this.languageComboBoxLabel.Location = new System.Drawing.Point(498, 22);
      this.languageComboBoxLabel.Name = "languageComboBoxLabel";
      this.languageComboBoxLabel.Size = new System.Drawing.Size(69, 13);
      this.languageComboBoxLabel.TabIndex = 9;
      this.languageComboBoxLabel.Text = "Język analizy";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(270, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(99, 13);
      this.label2.TabIndex = 10;
      this.label2.Text = "Wybrane fragmenty";
      // 
      // analyzeWholeFileSwitch
      // 
      this.analyzeWholeFileSwitch.AutoSize = true;
      this.analyzeWholeFileSwitch.Location = new System.Drawing.Point(235, 21);
      this.analyzeWholeFileSwitch.Name = "analyzeWholeFileSwitch";
      this.analyzeWholeFileSwitch.Size = new System.Drawing.Size(105, 17);
      this.analyzeWholeFileSwitch.TabIndex = 1;
      this.analyzeWholeFileSwitch.Text = "Analizuj cały plik";
      this.analyzeWholeFileSwitch.UseVisualStyleBackColor = true;
      // 
      // videoThumbnail
      // 
      this.videoThumbnail.Location = new System.Drawing.Point(12, 49);
      this.videoThumbnail.Name = "videoThumbnail";
      this.videoThumbnail.Size = new System.Drawing.Size(191, 138);
      this.videoThumbnail.TabIndex = 12;
      this.videoThumbnail.TabStop = false;
      // 
      // thumbnailLabel
      // 
      this.thumbnailLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.thumbnailLabel.AutoSize = true;
      this.thumbnailLabel.Location = new System.Drawing.Point(91, 27);
      this.thumbnailLabel.Name = "thumbnailLabel";
      this.thumbnailLabel.Size = new System.Drawing.Size(32, 13);
      this.thumbnailLabel.TabIndex = 13;
      this.thumbnailLabel.Text = "Tytuł";
      this.thumbnailLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // okButton
      // 
      this.okButton.Location = new System.Drawing.Point(585, 163);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(109, 41);
      this.okButton.TabIndex = 100;
      this.okButton.Text = "OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // Form2
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(716, 230);
      this.Controls.Add(this.okButton);
      this.Controls.Add(this.thumbnailLabel);
      this.Controls.Add(this.videoThumbnail);
      this.Controls.Add(this.analyzeWholeFileSwitch);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.languageComboBoxLabel);
      this.Controls.Add(this.languageComboBox);
      this.Controls.Add(this.samplingFrequencyInput);
      this.Controls.Add(this.samplingFrequencyInputLabel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "Form2";
      this.Text = "Wybierz opcje ekstrakcji danych";
      ((System.ComponentModel.ISupportInitialize)(this.samplingFrequencyInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.videoThumbnail)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.NumericUpDown samplingFrequencyInput;
    private System.Windows.Forms.Label samplingFrequencyInputLabel;
    private System.Windows.Forms.ComboBox languageComboBox;
    private System.Windows.Forms.Label languageComboBoxLabel;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckBox analyzeWholeFileSwitch;
    private System.Windows.Forms.PictureBox videoThumbnail;
    private System.Windows.Forms.Label thumbnailLabel;
    private System.Windows.Forms.Button okButton;
  }
}