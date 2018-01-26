namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class DownloadProgressWindow
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
            this.PB_download_progress = new System.Windows.Forms.ProgressBar();
            this.LB_file_name = new System.Windows.Forms.Label();
            this.LB_download_numbers = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.PB_download_progress, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.LB_file_name, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LB_download_numbers, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 98);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // PB_download_progress
            // 
            this.PB_download_progress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_download_progress.Location = new System.Drawing.Point(3, 36);
            this.PB_download_progress.Name = "PB_download_progress";
            this.PB_download_progress.Size = new System.Drawing.Size(278, 23);
            this.PB_download_progress.TabIndex = 0;
            // 
            // LB_file_name
            // 
            this.LB_file_name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_file_name.AutoSize = true;
            this.LB_file_name.Location = new System.Drawing.Point(3, 9);
            this.LB_file_name.Name = "LB_file_name";
            this.LB_file_name.Size = new System.Drawing.Size(278, 13);
            this.LB_file_name.TabIndex = 1;
            this.LB_file_name.Text = "label1";
            // 
            // LB_download_numbers
            // 
            this.LB_download_numbers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_download_numbers.AutoSize = true;
            this.LB_download_numbers.Location = new System.Drawing.Point(3, 73);
            this.LB_download_numbers.Name = "LB_download_numbers";
            this.LB_download_numbers.Size = new System.Drawing.Size(278, 13);
            this.LB_download_numbers.TabIndex = 2;
            this.LB_download_numbers.Text = "label1";
            // 
            // DownloadProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 98);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "DownloadProgressWindow";
            this.Text = "Ściąganie pliku";
            this.Load += new System.EventHandler(this.DownloadProgressWindow_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ProgressBar PB_download_progress;
        private System.Windows.Forms.Label LB_file_name;
        private System.Windows.Forms.Label LB_download_numbers;
    }
}