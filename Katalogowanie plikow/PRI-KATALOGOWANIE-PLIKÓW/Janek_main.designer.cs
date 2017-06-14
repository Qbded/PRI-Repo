namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Janek_main
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
            this.File = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Label = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Metadate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DownloadedFrom = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LastModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ArithmMean = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Median = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Mode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.QDeviation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.State = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Durance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.P = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.L = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q1Min = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q1Max = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q2Min = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q2Max = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q3Min = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Q3Max = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.QDevMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.QDevMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Average = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Mod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Total = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.PlayPauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bnChooseFolder = new System.Windows.Forms.Button();
            this.bnPreview = new System.Windows.Forms.Button();
            this.bnShare = new System.Windows.Forms.Button();
            this.bnCatalogue = new System.Windows.Forms.Button();
            this.chkOmitDollarSign = new System.Windows.Forms.CheckBox();
            this.chkUseEquality = new System.Windows.Forms.CheckBox();
            this.chkUseCreteRule = new System.Windows.Forms.CheckBox();
            this.lbExampleCommand = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // File
            // 
            this.File.DisplayIndex = 0;
            this.File.Text = "Plik";
            // 
            // Label
            // 
            this.Label.DisplayIndex = 1;
            this.Label.Text = "Etykieta";
            // 
            // Metadate
            // 
            this.Metadate.DisplayIndex = 2;
            this.Metadate.Text = "Metadane";
            // 
            // DownloadedFrom
            // 
            this.DownloadedFrom.DisplayIndex = 3;
            this.DownloadedFrom.Text = "Strona web pobierania";
            // 
            // LastModified
            // 
            this.LastModified.DisplayIndex = 4;
            this.LastModified.Text = "Ostatnia mofyfikacja";
            // 
            // ArithmMean
            // 
            this.ArithmMean.DisplayIndex = 5;
            this.ArithmMean.Text = "Średnia arytmetyczna";
            // 
            // Median
            // 
            this.Median.DisplayIndex = 6;
            this.Median.Text = "Mediana";
            // 
            // Mode
            // 
            this.Mode.DisplayIndex = 7;
            this.Mode.Text = "Moda";
            // 
            // Q1
            // 
            this.Q1.DisplayIndex = 8;
            this.Q1.Text = "Kwartyl 1.";
            // 
            // Q2
            // 
            this.Q2.DisplayIndex = 9;
            this.Q2.Text = "Kwartyl 2.";
            // 
            // Q3
            // 
            this.Q3.DisplayIndex = 10;
            this.Q3.Text = "Kwartyl 3.";
            // 
            // QDeviation
            // 
            this.QDeviation.DisplayIndex = 11;
            this.QDeviation.Text = "Odchylenie kwartylne";
            // 
            // State
            // 
            this.State.DisplayIndex = 12;
            this.State.Text = "Stan";
            // 
            // Durance
            // 
            this.Durance.DisplayIndex = 13;
            this.Durance.Text = "Czas trwania";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Controls.Add(this.bnChooseFolder);
            this.groupBox1.Controls.Add(this.bnPreview);
            this.groupBox1.Controls.Add(this.bnShare);
            this.groupBox1.Controls.Add(this.bnCatalogue);
            this.groupBox1.Controls.Add(this.chkOmitDollarSign);
            this.groupBox1.Controls.Add(this.chkUseEquality);
            this.groupBox1.Controls.Add(this.chkUseCreteRule);
            this.groupBox1.Controls.Add(this.lbExampleCommand);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtCommand);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.groupBox1.Location = new System.Drawing.Point(11, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(700, 641);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Użyj polecenia";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.P,
            this.L,
            this.Q1Min,
            this.Q1Max,
            this.Q2Min,
            this.Q2Max,
            this.Q3Min,
            this.Q3Max,
            this.QDevMin,
            this.QDevMax,
            this.Average,
            this.Mod,
            this.Total});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Location = new System.Drawing.Point(5, 229);
            this.listView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(688, 354);
            this.listView1.TabIndex = 14;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
            // 
            // P
            // 
            this.P.Text = "Plik";
            this.P.Width = 120;
            // 
            // L
            // 
            this.L.Text = "Etykieta";
            this.L.Width = 120;
            // 
            // Q1Min
            // 
            this.Q1Min.Text = "Kwartyl 1.(minimum)";
            this.Q1Min.Width = 150;
            // 
            // Q1Max
            // 
            this.Q1Max.Text = "Kwartyl 1.(maksimum)";
            this.Q1Max.Width = 150;
            // 
            // Q2Min
            // 
            this.Q2Min.Text = "Kwatyl 2.(minimum)";
            this.Q2Min.Width = 150;
            // 
            // Q2Max
            // 
            this.Q2Max.Text = "Kwartyl 2.(maksimum)";
            this.Q2Max.Width = 150;
            // 
            // Q3Min
            // 
            this.Q3Min.Text = "Kwatyl 3.(minimum)";
            this.Q3Min.Width = 150;
            // 
            // Q3Max
            // 
            this.Q3Max.Text = "Kwartyl 3.(maksimum)";
            this.Q3Max.Width = 150;
            // 
            // QDevMin
            // 
            this.QDevMin.Text = "Odchylenie kwartylne(minimum)";
            this.QDevMin.Width = 200;
            // 
            // QDevMax
            // 
            this.QDevMax.Text = "Odchylenie kwartylne(maksimum)";
            this.QDevMax.Width = 200;
            // 
            // Average
            // 
            this.Average.Text = "Średnia arytmetyczna";
            this.Average.Width = 120;
            // 
            // Mod
            // 
            this.Mod.Text = "Moda";
            this.Mod.Width = 120;
            // 
            // Total
            // 
            this.Total.Text = "Czas trwania [ms]";
            this.Total.Width = 120;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PlayPauseToolStripMenuItem,
            this.StopToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(209, 56);
            // 
            // PlayPauseToolStripMenuItem
            // 
            this.PlayPauseToolStripMenuItem.Name = "PlayPauseToolStripMenuItem";
            this.PlayPauseToolStripMenuItem.Size = new System.Drawing.Size(208, 26);
            this.PlayPauseToolStripMenuItem.Text = "&Wznów/Wstrzymaj";
            this.PlayPauseToolStripMenuItem.Click += new System.EventHandler(this.PlayPauseToolStripMenuItem_Click);
            // 
            // StopToolStripMenuItem
            // 
            this.StopToolStripMenuItem.Name = "StopToolStripMenuItem";
            this.StopToolStripMenuItem.Size = new System.Drawing.Size(208, 26);
            this.StopToolStripMenuItem.Text = "&Zatrzymaj";
            this.StopToolStripMenuItem.Click += new System.EventHandler(this.StopToolStripMenuItem_Click);
            // 
            // bnChooseFolder
            // 
            this.bnChooseFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnChooseFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bnChooseFolder.Location = new System.Drawing.Point(643, 39);
            this.bnChooseFolder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.bnChooseFolder.Name = "bnChooseFolder";
            this.bnChooseFolder.Size = new System.Drawing.Size(52, 30);
            this.bnChooseFolder.TabIndex = 12;
            this.bnChooseFolder.Text = "...";
            this.bnChooseFolder.UseVisualStyleBackColor = true;
            this.bnChooseFolder.Click += new System.EventHandler(this.bnChooseFolder_Click);
            // 
            // bnPreview
            // 
            this.bnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnPreview.Enabled = false;
            this.bnPreview.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bnPreview.Location = new System.Drawing.Point(400, 590);
            this.bnPreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bnPreview.Name = "bnPreview";
            this.bnPreview.Size = new System.Drawing.Size(143, 39);
            this.bnPreview.TabIndex = 11;
            this.bnPreview.Text = "&Podgląd";
            this.bnPreview.UseVisualStyleBackColor = true;
            this.bnPreview.Click += new System.EventHandler(this.bnPreview_Click);
            // 
            // bnShare
            // 
            this.bnShare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnShare.Enabled = false;
            this.bnShare.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bnShare.Location = new System.Drawing.Point(552, 591);
            this.bnShare.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bnShare.Name = "bnShare";
            this.bnShare.Size = new System.Drawing.Size(143, 39);
            this.bnShare.TabIndex = 10;
            this.bnShare.Text = "U&dostępnij";
            this.bnShare.UseVisualStyleBackColor = true;
            // 
            // bnCatalogue
            // 
            this.bnCatalogue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCatalogue.Enabled = false;
            this.bnCatalogue.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bnCatalogue.Location = new System.Drawing.Point(0, 590);
            this.bnCatalogue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bnCatalogue.Name = "bnCatalogue";
            this.bnCatalogue.Size = new System.Drawing.Size(388, 39);
            this.bnCatalogue.TabIndex = 9;
            this.bnCatalogue.Text = "&Kataloguj";
            this.bnCatalogue.UseVisualStyleBackColor = true;
            this.bnCatalogue.Click += new System.EventHandler(this.bnCatalogue_Click);
            // 
            // chkOmitDollarSign
            // 
            this.chkOmitDollarSign.AutoSize = true;
            this.chkOmitDollarSign.Location = new System.Drawing.Point(28, 193);
            this.chkOmitDollarSign.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkOmitDollarSign.Name = "chkOmitDollarSign";
            this.chkOmitDollarSign.Size = new System.Drawing.Size(687, 29);
            this.chkOmitDollarSign.TabIndex = 7;
            this.chkOmitDollarSign.Text = "&Pomiń numer (znak dolara ($) w regule) w nazwie folderu/ów katalogowania";
            this.chkOmitDollarSign.UseVisualStyleBackColor = true;
            // 
            // chkUseEquality
            // 
            this.chkUseEquality.AutoSize = true;
            this.chkUseEquality.Enabled = false;
            this.chkUseEquality.Location = new System.Drawing.Point(28, 155);
            this.chkUseEquality.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkUseEquality.Name = "chkUseEquality";
            this.chkUseEquality.Size = new System.Drawing.Size(243, 29);
            this.chkUseEquality.TabIndex = 5;
            this.chkUseEquality.Text = "Użyj znaku &równości (=)";
            this.chkUseEquality.UseVisualStyleBackColor = true;
            // 
            // chkUseCreteRule
            // 
            this.chkUseCreteRule.AutoSize = true;
            this.chkUseCreteRule.Location = new System.Drawing.Point(28, 118);
            this.chkUseCreteRule.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkUseCreteRule.Name = "chkUseCreteRule";
            this.chkUseCreteRule.Size = new System.Drawing.Size(260, 29);
            this.chkUseCreteRule.TabIndex = 3;
            this.chkUseCreteRule.Text = "&Użyj reguły typu \"create...\"";
            this.chkUseCreteRule.UseVisualStyleBackColor = true;
            // 
            // lbExampleCommand
            // 
            this.lbExampleCommand.AutoSize = true;
            this.lbExampleCommand.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbExampleCommand.Location = new System.Drawing.Point(23, 89);
            this.lbExampleCommand.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbExampleCommand.Name = "lbExampleCommand";
            this.lbExampleCommand.Size = new System.Drawing.Size(53, 25);
            this.lbExampleCommand.TabIndex = 4;
            this.lbExampleCommand.Text = "Np.: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Polecenie:";
            // 
            // txtCommand
            // 
            this.txtCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCommand.Location = new System.Drawing.Point(133, 39);
            this.txtCommand.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(503, 30);
            this.txtCommand.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(713, 663);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Katalogowanie plików dźwiękowych [bezczynny]";
            this.TextChanged += new System.EventHandler(this.Form1_TextChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColumnHeader File;
        private System.Windows.Forms.ColumnHeader Label;
        private System.Windows.Forms.ColumnHeader Metadate;
        private System.Windows.Forms.ColumnHeader DownloadedFrom;
        private System.Windows.Forms.ColumnHeader LastModified;
        private System.Windows.Forms.ColumnHeader ArithmMean;
        private System.Windows.Forms.ColumnHeader Median;
        private System.Windows.Forms.ColumnHeader Mode;
        private System.Windows.Forms.ColumnHeader Q1;
        private System.Windows.Forms.ColumnHeader Q2;
        private System.Windows.Forms.ColumnHeader Q3;
        private System.Windows.Forms.ColumnHeader QDeviation;
        private System.Windows.Forms.ColumnHeader State;
        private System.Windows.Forms.ColumnHeader Durance;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkOmitDollarSign;
        private System.Windows.Forms.CheckBox chkUseEquality;
        private System.Windows.Forms.CheckBox chkUseCreteRule;
        private System.Windows.Forms.Label lbExampleCommand;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Button bnPreview;
        private System.Windows.Forms.Button bnShare;
        private System.Windows.Forms.Button bnCatalogue;
        private System.Windows.Forms.Button bnChooseFolder;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader P;
        private System.Windows.Forms.ColumnHeader L;
        private System.Windows.Forms.ColumnHeader Q1Min;
        private System.Windows.Forms.ColumnHeader Q1Max;
        private System.Windows.Forms.ColumnHeader Q2Min;
        private System.Windows.Forms.ColumnHeader Q2Max;
        private System.Windows.Forms.ColumnHeader Q3Min;
        private System.Windows.Forms.ColumnHeader Q3Max;
        private System.Windows.Forms.ColumnHeader QDevMin;
        private System.Windows.Forms.ColumnHeader QDevMax;
        private System.Windows.Forms.ColumnHeader Average;
        private System.Windows.Forms.ColumnHeader Mod;
        private System.Windows.Forms.ColumnHeader Total;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem PlayPauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StopToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
    }
}

