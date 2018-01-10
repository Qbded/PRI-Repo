namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Metadata_extractor
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
            this.PB_extraction_progress = new System.Windows.Forms.ProgressBar();
            this.BT_interrupt = new System.Windows.Forms.Button();
            this.LB_file_count = new System.Windows.Forms.Label();
            this.LB_file_supported_count = new System.Windows.Forms.Label();
            this.LB_metadata_count = new System.Windows.Forms.Label();
            this.LB_metadata_count_container = new System.Windows.Forms.Label();
            this.LB_file_supported_count_container = new System.Windows.Forms.Label();
            this.LB_file_count_container = new System.Windows.Forms.Label();
            this.BGW_metadata_extractor = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // PB_extraction_progress
            // 
            this.PB_extraction_progress.Location = new System.Drawing.Point(13, 12);
            this.PB_extraction_progress.Name = "PB_extraction_progress";
            this.PB_extraction_progress.Size = new System.Drawing.Size(363, 23);
            this.PB_extraction_progress.TabIndex = 0;
            // 
            // BT_interrupt
            // 
            this.BT_interrupt.Location = new System.Drawing.Point(13, 82);
            this.BT_interrupt.Name = "BT_interrupt";
            this.BT_interrupt.Size = new System.Drawing.Size(363, 23);
            this.BT_interrupt.TabIndex = 1;
            this.BT_interrupt.Text = "Przerwij";
            this.BT_interrupt.UseVisualStyleBackColor = true;
            this.BT_interrupt.Click += new System.EventHandler(this.BT_interrupt_click);
            // 
            // LB_file_count
            // 
            this.LB_file_count.AutoSize = true;
            this.LB_file_count.Location = new System.Drawing.Point(10, 39);
            this.LB_file_count.Name = "LB_file_count";
            this.LB_file_count.Size = new System.Drawing.Size(177, 13);
            this.LB_file_count.TabIndex = 2;
            this.LB_file_count.Text = "Ilość plików w wskazanym folderze: ";
            // 
            // LB_file_supported_count
            // 
            this.LB_file_supported_count.AutoSize = true;
            this.LB_file_supported_count.Location = new System.Drawing.Point(10, 52);
            this.LB_file_supported_count.Name = "LB_file_supported_count";
            this.LB_file_supported_count.Size = new System.Drawing.Size(250, 13);
            this.LB_file_supported_count.TabIndex = 3;
            this.LB_file_supported_count.Text = "Ilość plików o obsługiwanych formatach w folderze:";
            // 
            // LB_metadata_count
            // 
            this.LB_metadata_count.AutoSize = true;
            this.LB_metadata_count.Location = new System.Drawing.Point(10, 65);
            this.LB_metadata_count.Name = "LB_metadata_count";
            this.LB_metadata_count.Size = new System.Drawing.Size(196, 13);
            this.LB_metadata_count.TabIndex = 4;
            this.LB_metadata_count.Text = "Ilość wyekstrachowanych metadanych: ";
            // 
            // LB_metadata_count_container
            // 
            this.LB_metadata_count_container.AutoSize = true;
            this.LB_metadata_count_container.Location = new System.Drawing.Point(212, 65);
            this.LB_metadata_count_container.Name = "LB_metadata_count_container";
            this.LB_metadata_count_container.Size = new System.Drawing.Size(13, 13);
            this.LB_metadata_count_container.TabIndex = 5;
            this.LB_metadata_count_container.Text = "0";
            // 
            // LB_file_supported_count_container
            // 
            this.LB_file_supported_count_container.AutoSize = true;
            this.LB_file_supported_count_container.Location = new System.Drawing.Point(266, 52);
            this.LB_file_supported_count_container.Name = "LB_file_supported_count_container";
            this.LB_file_supported_count_container.Size = new System.Drawing.Size(13, 13);
            this.LB_file_supported_count_container.TabIndex = 6;
            this.LB_file_supported_count_container.Text = "0";
            // 
            // LB_file_count_container
            // 
            this.LB_file_count_container.AutoSize = true;
            this.LB_file_count_container.Location = new System.Drawing.Point(193, 39);
            this.LB_file_count_container.Name = "LB_file_count_container";
            this.LB_file_count_container.Size = new System.Drawing.Size(13, 13);
            this.LB_file_count_container.TabIndex = 7;
            this.LB_file_count_container.Text = "0";
            // 
            // BGW_metadata_extractor
            // 
            this.BGW_metadata_extractor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BGW_extract_metadata);
            this.BGW_metadata_extractor.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BGW_progress_updated);
            this.BGW_metadata_extractor.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BGW_done);
            // 
            // Metadata_extractor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 117);
            this.Controls.Add(this.LB_file_count_container);
            this.Controls.Add(this.LB_file_supported_count_container);
            this.Controls.Add(this.LB_metadata_count_container);
            this.Controls.Add(this.LB_metadata_count);
            this.Controls.Add(this.LB_file_supported_count);
            this.Controls.Add(this.LB_file_count);
            this.Controls.Add(this.BT_interrupt);
            this.Controls.Add(this.PB_extraction_progress);
            this.Name = "Metadata_extractor";
            this.Text = "Ekstakcja metadanych";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar PB_extraction_progress;
        private System.Windows.Forms.Button BT_interrupt;
        private System.Windows.Forms.Label LB_file_count;
        private System.Windows.Forms.Label LB_file_supported_count;
        private System.Windows.Forms.Label LB_metadata_count;
        private System.Windows.Forms.Label LB_metadata_count_container;
        private System.Windows.Forms.Label LB_file_supported_count_container;
        private System.Windows.Forms.Label LB_file_count_container;
        private System.ComponentModel.BackgroundWorker BGW_metadata_extractor;
    }
}