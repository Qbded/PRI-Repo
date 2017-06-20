namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Karol_main
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
            this.openFileDialogButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.outputTextBoxLabel = new System.Windows.Forms.Label();
            this.processFilesButton = new System.Windows.Forms.Button();
            this.fileCountProgress = new System.Windows.Forms.ProgressBar();
            this.framesCountProgress = new System.Windows.Forms.ProgressBar();
            this.cancelFileProcessingButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialogButton
            // 
            this.openFileDialogButton.Enabled = false;
            this.openFileDialogButton.Location = new System.Drawing.Point(21, 12);
            this.openFileDialogButton.Name = "openFileDialogButton";
            this.openFileDialogButton.Size = new System.Drawing.Size(219, 45);
            this.openFileDialogButton.TabIndex = 0;
            this.openFileDialogButton.Text = "Wybierz plik(i)";
            this.openFileDialogButton.Click += new System.EventHandler(this.openFileSelect);
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
            this.processFilesButton.Location = new System.Drawing.Point(281, 93);
            this.processFilesButton.Name = "processFilesButton";
            this.processFilesButton.Size = new System.Drawing.Size(210, 191);
            this.processFilesButton.TabIndex = 3;
            this.processFilesButton.Text = "GO!";
            this.processFilesButton.UseVisualStyleBackColor = true;
            this.processFilesButton.Click += new System.EventHandler(this.processVideoFiles);
            // 
            // fileCountProgress
            // 
            this.fileCountProgress.Location = new System.Drawing.Point(281, 12);
            this.fileCountProgress.Name = "fileCountProgress";
            this.fileCountProgress.Size = new System.Drawing.Size(210, 13);
            this.fileCountProgress.Step = 1;
            this.fileCountProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.fileCountProgress.TabIndex = 4;
            // 
            // framesCountProgress
            // 
            this.framesCountProgress.Location = new System.Drawing.Point(281, 22);
            this.framesCountProgress.Name = "framesCountProgress";
            this.framesCountProgress.Size = new System.Drawing.Size(210, 35);
            this.framesCountProgress.Step = 1;
            this.framesCountProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.framesCountProgress.TabIndex = 5;
            // 
            // cancelFileProcessingButton
            // 
            this.cancelFileProcessingButton.Enabled = false;
            this.cancelFileProcessingButton.Location = new System.Drawing.Point(281, 303);
            this.cancelFileProcessingButton.Name = "cancelFileProcessingButton";
            this.cancelFileProcessingButton.Size = new System.Drawing.Size(210, 46);
            this.cancelFileProcessingButton.TabIndex = 6;
            this.cancelFileProcessingButton.Text = "Anuluj";
            this.cancelFileProcessingButton.UseVisualStyleBackColor = true;
            this.cancelFileProcessingButton.Click += new System.EventHandler(this.cancelFileProcessing);
            // 
            // Karol_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(516, 361);
            this.Controls.Add(this.cancelFileProcessingButton);
            this.Controls.Add(this.framesCountProgress);
            this.Controls.Add(this.fileCountProgress);
            this.Controls.Add(this.processFilesButton);
            this.Controls.Add(this.outputTextBoxLabel);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.openFileDialogButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Karol_main";
            this.Text = "Ekstracja tekstu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Karol_main_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openFileDialogButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Label outputTextBoxLabel;
        private System.Windows.Forms.Button processFilesButton;
        private System.Windows.Forms.ProgressBar fileCountProgress;
        private System.Windows.Forms.ProgressBar framesCountProgress;
        private System.Windows.Forms.Button cancelFileProcessingButton;
    }
}