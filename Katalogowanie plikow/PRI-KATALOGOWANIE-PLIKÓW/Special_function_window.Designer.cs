namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Special_function_window
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BT_extract_from_images = new System.Windows.Forms.Button();
            this.BT_compare_audio_files = new System.Windows.Forms.Button();
            this.BT_search_catalog = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.BT_extract_from_images, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.BT_compare_audio_files, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BT_search_catalog, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(260, 238);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // BT_extract_from_images
            // 
            this.BT_extract_from_images.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_extract_from_images.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.BT_extract_from_images.Location = new System.Drawing.Point(3, 3);
            this.BT_extract_from_images.Name = "BT_extract_from_images";
            this.BT_extract_from_images.Size = new System.Drawing.Size(254, 72);
            this.BT_extract_from_images.TabIndex = 0;
            this.BT_extract_from_images.Text = "Wyodrębnij tekst z plików multimedialnych";
            this.BT_extract_from_images.UseVisualStyleBackColor = true;
            this.BT_extract_from_images.Click += new System.EventHandler(this.BT_extract_from_images_Click);
            // 
            // BT_compare_audio_files
            // 
            this.BT_compare_audio_files.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_compare_audio_files.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_compare_audio_files.Location = new System.Drawing.Point(3, 81);
            this.BT_compare_audio_files.Name = "BT_compare_audio_files";
            this.BT_compare_audio_files.Size = new System.Drawing.Size(254, 72);
            this.BT_compare_audio_files.TabIndex = 1;
            this.BT_compare_audio_files.Text = "Porównaj pliki dźwiękowe pod kątem podobieństwa";
            this.BT_compare_audio_files.UseVisualStyleBackColor = true;
            this.BT_compare_audio_files.Click += new System.EventHandler(this.BT_compare_audio_files_Click);
            // 
            // BT_search_catalog
            // 
            this.BT_search_catalog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_search_catalog.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_search_catalog.Location = new System.Drawing.Point(3, 159);
            this.BT_search_catalog.Name = "BT_search_catalog";
            this.BT_search_catalog.Size = new System.Drawing.Size(254, 76);
            this.BT_search_catalog.TabIndex = 2;
            this.BT_search_catalog.Text = "Przeszukaj katalog";
            this.BT_search_catalog.UseVisualStyleBackColor = true;
            this.BT_search_catalog.Click += new System.EventHandler(this.BT_search_catalog_Click);
            // 
            // Special_function_window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Special_function_window";
            this.Text = "Opcje specjalne";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Special_function_window_FormClosing);
            this.Load += new System.EventHandler(this.Special_function_window_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button BT_extract_from_images;
        private System.Windows.Forms.Button BT_compare_audio_files;
        private System.Windows.Forms.Button BT_search_catalog;
    }
}