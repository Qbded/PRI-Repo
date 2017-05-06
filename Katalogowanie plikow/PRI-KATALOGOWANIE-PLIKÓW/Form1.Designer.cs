namespace PRI_KATALOGOWANIE_PLIKÓW
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Plik", System.Windows.Forms.HorizontalAlignment.Left);
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkSelectAllListPositions = new System.Windows.Forms.CheckBox();
            this.chkExcludeMetadata = new System.Windows.Forms.CheckBox();
            this.chkMetadata = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BT_extract_metadata = new System.Windows.Forms.Button();
            this.BT_test_database = new System.Windows.Forms.Button();
            this.chkUseEquality = new System.Windows.Forms.CheckBox();
            this.chkUseCreteRule = new System.Windows.Forms.CheckBox();
            this.lbExampleCommand = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.bnCatalogue = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Plik = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Etykieta = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Metadane = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TB_path_displayer = new System.Windows.Forms.TextBox();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(447, 394);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Kryteria katalogowania";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkSelectAllListPositions);
            this.groupBox2.Controls.Add(this.chkExcludeMetadata);
            this.groupBox2.Controls.Add(this.chkMetadata);
            this.groupBox2.Location = new System.Drawing.Point(5, 178);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(434, 216);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Metadane";
            // 
            // chkSelectAllListPositions
            // 
            this.chkSelectAllListPositions.AutoSize = true;
            this.chkSelectAllListPositions.Location = new System.Drawing.Point(87, 0);
            this.chkSelectAllListPositions.Name = "chkSelectAllListPositions";
            this.chkSelectAllListPositions.Size = new System.Drawing.Size(215, 24);
            this.chkSelectAllListPositions.TabIndex = 4;
            this.chkSelectAllListPositions.Text = "&Zaznacz wszystko (Ctrl+A)";
            this.chkSelectAllListPositions.UseVisualStyleBackColor = true;
            this.chkSelectAllListPositions.CheckedChanged += new System.EventHandler(this.chkSelectAllListPositions_CheckedChanged);
            // 
            // chkExcludeMetadata
            // 
            this.chkExcludeMetadata.AutoSize = true;
            this.chkExcludeMetadata.Location = new System.Drawing.Point(6, 25);
            this.chkExcludeMetadata.Name = "chkExcludeMetadata";
            this.chkExcludeMetadata.Size = new System.Drawing.Size(282, 24);
            this.chkExcludeMetadata.TabIndex = 3;
            this.chkExcludeMetadata.Text = "&Wyklucz niektóre metadane z plików";
            this.chkExcludeMetadata.UseVisualStyleBackColor = true;
            this.chkExcludeMetadata.CheckedChanged += new System.EventHandler(this.chkExcludeMetadata_CheckedChanged);
            // 
            // chkMetadata
            // 
            this.chkMetadata.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chkMetadata.Enabled = false;
            this.chkMetadata.FormattingEnabled = true;
            this.chkMetadata.Location = new System.Drawing.Point(6, 48);
            this.chkMetadata.Name = "chkMetadata";
            this.chkMetadata.Size = new System.Drawing.Size(422, 147);
            this.chkMetadata.TabIndex = 2;
            this.chkMetadata.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkMetadata_ItemCheck);
            this.chkMetadata.SelectedIndexChanged += new System.EventHandler(this.chkMetadata_SelectedIndexChanged);
            this.chkMetadata.DoubleClick += new System.EventHandler(this.chkMetadata_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BT_extract_metadata);
            this.groupBox1.Controls.Add(this.BT_test_database);
            this.groupBox1.Controls.Add(this.chkUseEquality);
            this.groupBox1.Controls.Add(this.chkUseCreteRule);
            this.groupBox1.Controls.Add(this.lbExampleCommand);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtCommand);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(438, 171);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Użyj polecenia";
            // 
            // BT_extract_metadata
            // 
            this.BT_extract_metadata.Location = new System.Drawing.Point(299, 138);
            this.BT_extract_metadata.Name = "BT_extract_metadata";
            this.BT_extract_metadata.Size = new System.Drawing.Size(122, 28);
            this.BT_extract_metadata.TabIndex = 6;
            this.BT_extract_metadata.Text = "Kataloguj";
            this.BT_extract_metadata.UseVisualStyleBackColor = true;
            // 
            // BT_test_database
            // 
            this.BT_test_database.Location = new System.Drawing.Point(299, 108);
            this.BT_test_database.Name = "BT_test_database";
            this.BT_test_database.Size = new System.Drawing.Size(122, 24);
            this.BT_test_database.TabIndex = 1;
            this.BT_test_database.Text = "Test bazy";
            this.BT_test_database.UseVisualStyleBackColor = true;
            this.BT_test_database.Click += new System.EventHandler(this.BT_test_database_click);
            // 
            // chkUseEquality
            // 
            this.chkUseEquality.AutoSize = true;
            this.chkUseEquality.Enabled = false;
            this.chkUseEquality.Location = new System.Drawing.Point(21, 138);
            this.chkUseEquality.Name = "chkUseEquality";
            this.chkUseEquality.Size = new System.Drawing.Size(194, 24);
            this.chkUseEquality.TabIndex = 5;
            this.chkUseEquality.Text = "Użyj znaku &równości (=)";
            this.chkUseEquality.UseVisualStyleBackColor = true;
            this.chkUseEquality.CheckedChanged += new System.EventHandler(this.chkUseEquality_CheckedChanged);
            // 
            // chkUseCreteRule
            // 
            this.chkUseCreteRule.AutoSize = true;
            this.chkUseCreteRule.Location = new System.Drawing.Point(21, 108);
            this.chkUseCreteRule.Name = "chkUseCreteRule";
            this.chkUseCreteRule.Size = new System.Drawing.Size(212, 24);
            this.chkUseCreteRule.TabIndex = 3;
            this.chkUseCreteRule.Text = "&Użyj reguły typu \"create...\"";
            this.chkUseCreteRule.UseVisualStyleBackColor = true;
            this.chkUseCreteRule.CheckedChanged += new System.EventHandler(this.chkUseCreteRule_CheckedChanged);
            // 
            // lbExampleCommand
            // 
            this.lbExampleCommand.AutoSize = true;
            this.lbExampleCommand.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbExampleCommand.Location = new System.Drawing.Point(17, 72);
            this.lbExampleCommand.Name = "lbExampleCommand";
            this.lbExampleCommand.Size = new System.Drawing.Size(41, 20);
            this.lbExampleCommand.TabIndex = 4;
            this.lbExampleCommand.Text = "Np.: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Polecenie:";
            // 
            // txtCommand
            // 
            this.txtCommand.Location = new System.Drawing.Point(100, 32);
            this.txtCommand.Margin = new System.Windows.Forms.Padding(2);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(321, 26);
            this.txtCommand.TabIndex = 0;
            this.txtCommand.TextChanged += new System.EventHandler(this.txtCommand_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(455, 427);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.bnCatalogue);
            this.tabPage2.Controls.Add(this.listView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(447, 394);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Praca";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(334, 360);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 29);
            this.button2.TabIndex = 4;
            this.button2.Text = "W&yślij";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // bnCatalogue
            // 
            this.bnCatalogue.ContextMenuStrip = this.contextMenuStrip1;
            this.bnCatalogue.Location = new System.Drawing.Point(10, 277);
            this.bnCatalogue.Name = "bnCatalogue";
            this.bnCatalogue.Size = new System.Drawing.Size(429, 36);
            this.bnCatalogue.TabIndex = 3;
            this.bnCatalogue.Text = "&Kataloguj";
            this.bnCatalogue.UseVisualStyleBackColor = true;
            this.bnCatalogue.Click += new System.EventHandler(this.bnCatalogue_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(385, 48);
            this.contextMenuStrip1.Text = "Uwzględnij w nazwie folderu datę i godzinę  katalogowania";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(384, 22);
            this.toolStripMenuItem1.Text = "Uwzględnij w nazwie folderu datę i godzinę  katalogowania";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(384, 22);
            this.toolStripMenuItem2.Text = "Zapisz raport katgalogowania";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Plik,
            this.Etykieta,
            this.Metadane,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            listViewGroup1.Header = "Plik";
            listViewGroup1.Name = "listViewGroup1";
            this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1});
            this.listView1.Location = new System.Drawing.Point(10, 5);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(429, 266);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // Plik
            // 
            this.Plik.Text = "Plik";
            // 
            // Etykieta
            // 
            this.Etykieta.Text = "Etykieta";
            // 
            // Metadane
            // 
            this.Metadane.Text = "Metadane";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Strona web pobierania";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Ostatnia mofyfikacja";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Średnia arytmetyczna";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Mediana";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Moda";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Kwartyl 1.";
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Kwartyl 2.";
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Kwartyl 3.";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Odchylenie kwartylne";
            // 
            // TB_path_displayer
            // 
            this.TB_path_displayer.Location = new System.Drawing.Point(228, 0);
            this.TB_path_displayer.Name = "TB_path_displayer";
            this.TB_path_displayer.Size = new System.Drawing.Size(223, 20);
            this.TB_path_displayer.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 427);
            this.Controls.Add(this.TB_path_displayer);
            this.Controls.Add(this.tabControl1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Katalogowanie plików";
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lbExampleCommand;
        private System.Windows.Forms.CheckBox chkUseCreteRule;
        private System.Windows.Forms.CheckBox chkUseEquality;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox chkMetadata;
        private System.Windows.Forms.CheckBox chkExcludeMetadata;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button bnCatalogue;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader Plik;
        private System.Windows.Forms.ColumnHeader Etykieta;
        private System.Windows.Forms.ColumnHeader Metadane;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.CheckBox chkSelectAllListPositions;
        private System.Windows.Forms.Button BT_test_database;
        private System.Windows.Forms.TextBox TB_path_displayer;
        private System.Windows.Forms.Button BT_extract_metadata;
    }
}

