﻿namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Karol_progress
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
            this.components = new System.ComponentModel.Container();
            this.samplingFrequencyInput = new System.Windows.Forms.NumericUpDown();
            this.samplingFrequencyInputLabel = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageComboBoxLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.analyzeWholeFileSwitch = new System.Windows.Forms.CheckBox();
            this.videoThumbnail = new System.Windows.Forms.PictureBox();
            this.okButton = new System.Windows.Forms.Button();
            this.thumbnailLabel = new System.Windows.Forms.TextBox();
            this.framesToAnalyzeDisplay = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addTimeRangeButton = new System.Windows.Forms.Button();
            this.timeRangeInput1 = new System.Windows.Forms.MaskedTextBox();
            this.timeRangeInput2 = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.removeTimeRangeButton = new System.Windows.Forms.Button();
            this.timeRangeListview = new System.Windows.Forms.ListView();
            this.Od = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Do = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.wrongTimeFromatTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.videoDurationLabel = new System.Windows.Forms.Label();
            this.videoDurationDisplay = new System.Windows.Forms.TextBox();
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
            this.samplingFrequencyInput.Location = new System.Drawing.Point(616, 46);
            this.samplingFrequencyInput.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.samplingFrequencyInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.samplingFrequencyInput.Name = "samplingFrequencyInput";
            this.samplingFrequencyInput.Size = new System.Drawing.Size(78, 20);
            this.samplingFrequencyInput.TabIndex = 9;
            this.samplingFrequencyInput.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.samplingFrequencyInput.ValueChanged += new System.EventHandler(this.getFramesToAnalyzeCount);
            // 
            // samplingFrequencyInputLabel
            // 
            this.samplingFrequencyInputLabel.AutoSize = true;
            this.samplingFrequencyInputLabel.Location = new System.Drawing.Point(503, 49);
            this.samplingFrequencyInputLabel.Name = "samplingFrequencyInputLabel";
            this.samplingFrequencyInputLabel.Size = new System.Drawing.Size(107, 13);
            this.samplingFrequencyInputLabel.TabIndex = 6;
            this.samplingFrequencyInputLabel.Text = "Próbkuj co (2s - 0.1s)";
            // 
            // languageComboBox
            // 
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Location = new System.Drawing.Point(557, 19);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(137, 21);
            this.languageComboBox.TabIndex = 8;
            // 
            // languageComboBoxLabel
            // 
            this.languageComboBoxLabel.AutoSize = true;
            this.languageComboBoxLabel.Location = new System.Drawing.Point(482, 22);
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
            this.analyzeWholeFileSwitch.Checked = true;
            this.analyzeWholeFileSwitch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.analyzeWholeFileSwitch.Location = new System.Drawing.Point(235, 21);
            this.analyzeWholeFileSwitch.Name = "analyzeWholeFileSwitch";
            this.analyzeWholeFileSwitch.Size = new System.Drawing.Size(105, 17);
            this.analyzeWholeFileSwitch.TabIndex = 1;
            this.analyzeWholeFileSwitch.Text = "Analizuj cały plik";
            this.analyzeWholeFileSwitch.UseVisualStyleBackColor = true;
            this.analyzeWholeFileSwitch.Click += new System.EventHandler(this.analyzeWholeFileSwitch_Click);
            // 
            // videoThumbnail
            // 
            this.videoThumbnail.Location = new System.Drawing.Point(12, 49);
            this.videoThumbnail.Name = "videoThumbnail";
            this.videoThumbnail.Size = new System.Drawing.Size(191, 138);
            this.videoThumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.videoThumbnail.TabIndex = 12;
            this.videoThumbnail.TabStop = false;
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
            // thumbnailLabel
            // 
            this.thumbnailLabel.Location = new System.Drawing.Point(12, 23);
            this.thumbnailLabel.Name = "thumbnailLabel";
            this.thumbnailLabel.ReadOnly = true;
            this.thumbnailLabel.Size = new System.Drawing.Size(191, 20);
            this.thumbnailLabel.TabIndex = 101;
            this.thumbnailLabel.TabStop = false;
            this.thumbnailLabel.WordWrap = false;
            // 
            // framesToAnalyzeDisplay
            // 
            this.framesToAnalyzeDisplay.Location = new System.Drawing.Point(485, 90);
            this.framesToAnalyzeDisplay.Name = "framesToAnalyzeDisplay";
            this.framesToAnalyzeDisplay.ReadOnly = true;
            this.framesToAnalyzeDisplay.Size = new System.Drawing.Size(209, 20);
            this.framesToAnalyzeDisplay.TabIndex = 102;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(482, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 13);
            this.label1.TabIndex = 103;
            this.label1.Text = "Szacowana ilość klatek do przeanalizowania";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(475, 163);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(104, 41);
            this.cancelButton.TabIndex = 105;
            this.cancelButton.Text = "Anuluj";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // addTimeRangeButton
            // 
            this.addTimeRangeButton.Enabled = false;
            this.addTimeRangeButton.Location = new System.Drawing.Point(346, 188);
            this.addTimeRangeButton.Name = "addTimeRangeButton";
            this.addTimeRangeButton.Size = new System.Drawing.Size(31, 30);
            this.addTimeRangeButton.TabIndex = 106;
            this.addTimeRangeButton.Text = "+";
            this.addTimeRangeButton.UseVisualStyleBackColor = true;
            this.addTimeRangeButton.Click += new System.EventHandler(this.addTimeRangeButton_Click);
            // 
            // timeRangeInput1
            // 
            this.timeRangeInput1.Enabled = false;
            this.timeRangeInput1.Location = new System.Drawing.Point(220, 194);
            this.timeRangeInput1.Mask = "00:00:00";
            this.timeRangeInput1.Name = "timeRangeInput1";
            this.timeRangeInput1.Size = new System.Drawing.Size(51, 20);
            this.timeRangeInput1.TabIndex = 108;
            this.timeRangeInput1.Validating += new System.ComponentModel.CancelEventHandler(this.validateTime);
            // 
            // timeRangeInput2
            // 
            this.timeRangeInput2.Enabled = false;
            this.timeRangeInput2.Location = new System.Drawing.Point(289, 194);
            this.timeRangeInput2.Mask = "00:00:00";
            this.timeRangeInput2.Name = "timeRangeInput2";
            this.timeRangeInput2.Size = new System.Drawing.Size(51, 20);
            this.timeRangeInput2.TabIndex = 109;
            this.timeRangeInput2.Validating += new System.ComponentModel.CancelEventHandler(this.validateTime);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(277, 197);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 110;
            this.label3.Text = "-";
            // 
            // removeTimeRangeButton
            // 
            this.removeTimeRangeButton.Enabled = false;
            this.removeTimeRangeButton.Location = new System.Drawing.Point(383, 188);
            this.removeTimeRangeButton.Name = "removeTimeRangeButton";
            this.removeTimeRangeButton.Size = new System.Drawing.Size(29, 30);
            this.removeTimeRangeButton.TabIndex = 111;
            this.removeTimeRangeButton.Text = "-";
            this.removeTimeRangeButton.UseVisualStyleBackColor = true;
            this.removeTimeRangeButton.Click += new System.EventHandler(this.removeTimeRangeButton_Click);
            // 
            // timeRangeListview
            // 
            this.timeRangeListview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Od,
            this.Do});
            this.timeRangeListview.Enabled = false;
            this.timeRangeListview.FullRowSelect = true;
            this.timeRangeListview.GridLines = true;
            this.timeRangeListview.Location = new System.Drawing.Point(220, 74);
            this.timeRangeListview.MultiSelect = false;
            this.timeRangeListview.Name = "timeRangeListview";
            this.timeRangeListview.Size = new System.Drawing.Size(192, 94);
            this.timeRangeListview.TabIndex = 112;
            this.timeRangeListview.UseCompatibleStateImageBehavior = false;
            this.timeRangeListview.View = System.Windows.Forms.View.Details;
            // 
            // Od
            // 
            this.Od.Text = "Od";
            this.Od.Width = 96;
            // 
            // Do
            // 
            this.Do.Text = "Do";
            this.Do.Width = 96;
            // 
            // videoDurationLabel
            // 
            this.videoDurationLabel.AutoSize = true;
            this.videoDurationLabel.Location = new System.Drawing.Point(482, 113);
            this.videoDurationLabel.Name = "videoDurationLabel";
            this.videoDurationLabel.Size = new System.Drawing.Size(94, 13);
            this.videoDurationLabel.TabIndex = 113;
            this.videoDurationLabel.Text = "Czas trwania filmu:";
            // 
            // videoDurationDisplay
            // 
            this.videoDurationDisplay.Location = new System.Drawing.Point(485, 129);
            this.videoDurationDisplay.Name = "videoDurationDisplay";
            this.videoDurationDisplay.ReadOnly = true;
            this.videoDurationDisplay.Size = new System.Drawing.Size(209, 20);
            this.videoDurationDisplay.TabIndex = 114;
            // 
            // Karol_progress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 230);
            this.Controls.Add(this.videoDurationDisplay);
            this.Controls.Add(this.videoDurationLabel);
            this.Controls.Add(this.timeRangeListview);
            this.Controls.Add(this.removeTimeRangeButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.timeRangeInput2);
            this.Controls.Add(this.timeRangeInput1);
            this.Controls.Add(this.addTimeRangeButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.framesToAnalyzeDisplay);
            this.Controls.Add(this.thumbnailLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.videoThumbnail);
            this.Controls.Add(this.analyzeWholeFileSwitch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.languageComboBoxLabel);
            this.Controls.Add(this.languageComboBox);
            this.Controls.Add(this.samplingFrequencyInput);
            this.Controls.Add(this.samplingFrequencyInputLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Karol_progress";
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
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox thumbnailLabel;
        private System.Windows.Forms.TextBox framesToAnalyzeDisplay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button addTimeRangeButton;
        private System.Windows.Forms.MaskedTextBox timeRangeInput1;
        private System.Windows.Forms.MaskedTextBox timeRangeInput2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button removeTimeRangeButton;
        private System.Windows.Forms.ListView timeRangeListview;
        private System.Windows.Forms.ColumnHeader Od;
        private System.Windows.Forms.ColumnHeader Do;
        private System.Windows.Forms.ToolTip wrongTimeFromatTooltip;
        private System.Windows.Forms.Label videoDurationLabel;
        private System.Windows.Forms.TextBox videoDurationDisplay;
    }
}