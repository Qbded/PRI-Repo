namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Obrazy_main
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
            this.TL_layout_main = new System.Windows.Forms.TableLayoutPanel();
            this.LB_image_select_prompt = new System.Windows.Forms.Label();
            this.TL_secondary = new System.Windows.Forms.TableLayoutPanel();
            this.PB_image_preview = new System.Windows.Forms.PictureBox();
            this.LB_images = new System.Windows.Forms.ListBox();
            this.BT_execute = new System.Windows.Forms.Button();
            this.TL_layout_main.SuspendLayout();
            this.TL_secondary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_image_preview)).BeginInit();
            this.SuspendLayout();
            // 
            // TL_layout_main
            // 
            this.TL_layout_main.ColumnCount = 1;
            this.TL_layout_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_layout_main.Controls.Add(this.LB_image_select_prompt, 0, 0);
            this.TL_layout_main.Controls.Add(this.TL_secondary, 0, 1);
            this.TL_layout_main.Controls.Add(this.BT_execute, 0, 2);
            this.TL_layout_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TL_layout_main.Location = new System.Drawing.Point(0, 0);
            this.TL_layout_main.Name = "TL_layout_main";
            this.TL_layout_main.RowCount = 3;
            this.TL_layout_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.TL_layout_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_layout_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TL_layout_main.Size = new System.Drawing.Size(284, 262);
            this.TL_layout_main.TabIndex = 0;
            // 
            // LB_image_select_prompt
            // 
            this.LB_image_select_prompt.AutoSize = true;
            this.LB_image_select_prompt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_image_select_prompt.Location = new System.Drawing.Point(3, 0);
            this.LB_image_select_prompt.Name = "LB_image_select_prompt";
            this.LB_image_select_prompt.Size = new System.Drawing.Size(278, 22);
            this.LB_image_select_prompt.TabIndex = 0;
            this.LB_image_select_prompt.Text = "Prosimy wybrać obraz do porównania podobieństwa";
            // 
            // TL_secondary
            // 
            this.TL_secondary.ColumnCount = 2;
            this.TL_secondary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TL_secondary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TL_secondary.Controls.Add(this.PB_image_preview, 0, 0);
            this.TL_secondary.Controls.Add(this.LB_images, 1, 0);
            this.TL_secondary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TL_secondary.Location = new System.Drawing.Point(3, 25);
            this.TL_secondary.Name = "TL_secondary";
            this.TL_secondary.RowCount = 1;
            this.TL_secondary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_secondary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 202F));
            this.TL_secondary.Size = new System.Drawing.Size(278, 202);
            this.TL_secondary.TabIndex = 1;
            // 
            // PB_image_preview
            // 
            this.PB_image_preview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PB_image_preview.Location = new System.Drawing.Point(3, 3);
            this.PB_image_preview.Name = "PB_image_preview";
            this.PB_image_preview.Size = new System.Drawing.Size(133, 196);
            this.PB_image_preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PB_image_preview.TabIndex = 0;
            this.PB_image_preview.TabStop = false;
            this.PB_image_preview.SizeChanged += new System.EventHandler(this.PB_image_preview_SizeChanged);
            // 
            // LB_images
            // 
            this.LB_images.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_images.FormattingEnabled = true;
            this.LB_images.Location = new System.Drawing.Point(142, 3);
            this.LB_images.Name = "LB_images";
            this.LB_images.Size = new System.Drawing.Size(133, 196);
            this.LB_images.TabIndex = 1;
            this.LB_images.SelectedIndexChanged += new System.EventHandler(this.LB_images_SelectedIndexChanged);
            // 
            // BT_execute
            // 
            this.BT_execute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_execute.Location = new System.Drawing.Point(3, 233);
            this.BT_execute.Name = "BT_execute";
            this.BT_execute.Size = new System.Drawing.Size(278, 26);
            this.BT_execute.TabIndex = 2;
            this.BT_execute.Text = "Porównaj wybrany obraz do pozostałych";
            this.BT_execute.UseVisualStyleBackColor = true;
            this.BT_execute.Click += new System.EventHandler(this.BT_execute_Click);
            // 
            // Obrazy_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.TL_layout_main);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "Obrazy_main";
            this.Text = "Wyszukiwanie podobnych obrazów";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Obrazy_main_FormClosing);
            this.Load += new System.EventHandler(this.Obrazy_main_Load);
            this.TL_layout_main.ResumeLayout(false);
            this.TL_layout_main.PerformLayout();
            this.TL_secondary.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PB_image_preview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TL_layout_main;
        private System.Windows.Forms.Label LB_image_select_prompt;
        private System.Windows.Forms.TableLayoutPanel TL_secondary;
        private System.Windows.Forms.PictureBox PB_image_preview;
        private System.Windows.Forms.ListBox LB_images;
        private System.Windows.Forms.Button BT_execute;
    }
}
