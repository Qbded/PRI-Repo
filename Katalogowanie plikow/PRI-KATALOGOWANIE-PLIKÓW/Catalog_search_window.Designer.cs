namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Catalog_search_window
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
            this.TL_window_main = new System.Windows.Forms.TableLayoutPanel();
            this.TL_parameters_main = new System.Windows.Forms.TableLayoutPanel();
            this.TL_parameters_manipulation = new System.Windows.Forms.TableLayoutPanel();
            this.BT_add_parameter = new System.Windows.Forms.Button();
            this.BT_remove_parameter = new System.Windows.Forms.Button();
            this.LV_parameters = new System.Windows.Forms.ListView();
            this.TL_window_main.SuspendLayout();
            this.TL_parameters_main.SuspendLayout();
            this.TL_parameters_manipulation.SuspendLayout();
            this.SuspendLayout();
            // 
            // TL_window_main
            // 
            this.TL_window_main.ColumnCount = 1;
            this.TL_window_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TL_window_main.Controls.Add(this.TL_parameters_main, 0, 1);
            this.TL_window_main.Location = new System.Drawing.Point(12, 12);
            this.TL_window_main.Name = "TL_window_main";
            this.TL_window_main.RowCount = 2;
            this.TL_window_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TL_window_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TL_window_main.Size = new System.Drawing.Size(260, 238);
            this.TL_window_main.TabIndex = 0;
            // 
            // TL_parameters_main
            // 
            this.TL_parameters_main.ColumnCount = 2;
            this.TL_parameters_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78.34646F));
            this.TL_parameters_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.65354F));
            this.TL_parameters_main.Controls.Add(this.TL_parameters_manipulation, 1, 0);
            this.TL_parameters_main.Controls.Add(this.LV_parameters, 0, 0);
            this.TL_parameters_main.Location = new System.Drawing.Point(3, 122);
            this.TL_parameters_main.Name = "TL_parameters_main";
            this.TL_parameters_main.RowCount = 1;
            this.TL_parameters_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_parameters_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TL_parameters_main.Size = new System.Drawing.Size(254, 113);
            this.TL_parameters_main.TabIndex = 0;
            // 
            // TL_parameters_manipulation
            // 
            this.TL_parameters_manipulation.ColumnCount = 1;
            this.TL_parameters_manipulation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TL_parameters_manipulation.Controls.Add(this.BT_add_parameter, 0, 0);
            this.TL_parameters_manipulation.Controls.Add(this.BT_remove_parameter, 0, 1);
            this.TL_parameters_manipulation.Location = new System.Drawing.Point(202, 3);
            this.TL_parameters_manipulation.Name = "TL_parameters_manipulation";
            this.TL_parameters_manipulation.RowCount = 2;
            this.TL_parameters_manipulation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TL_parameters_manipulation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TL_parameters_manipulation.Size = new System.Drawing.Size(49, 107);
            this.TL_parameters_manipulation.TabIndex = 0;
            // 
            // BT_add_parameter
            // 
            this.BT_add_parameter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_add_parameter.Location = new System.Drawing.Point(3, 3);
            this.BT_add_parameter.Name = "BT_add_parameter";
            this.BT_add_parameter.Size = new System.Drawing.Size(43, 47);
            this.BT_add_parameter.TabIndex = 0;
            this.BT_add_parameter.Text = "+";
            this.BT_add_parameter.UseVisualStyleBackColor = true;
            // 
            // BT_remove_parameter
            // 
            this.BT_remove_parameter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_remove_parameter.Location = new System.Drawing.Point(3, 56);
            this.BT_remove_parameter.Name = "BT_remove_parameter";
            this.BT_remove_parameter.Size = new System.Drawing.Size(43, 48);
            this.BT_remove_parameter.TabIndex = 1;
            this.BT_remove_parameter.Text = "-";
            this.BT_remove_parameter.UseVisualStyleBackColor = true;
            // 
            // LV_parameters
            // 
            this.LV_parameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_parameters.Location = new System.Drawing.Point(3, 3);
            this.LV_parameters.Name = "LV_parameters";
            this.LV_parameters.Size = new System.Drawing.Size(193, 107);
            this.LV_parameters.TabIndex = 1;
            this.LV_parameters.UseCompatibleStateImageBehavior = false;
            // 
            // Catalog_search_window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.TL_window_main);
            this.Name = "Catalog_search_window";
            this.Text = "Przeszukiwanie katalogu";
            this.Load += new System.EventHandler(this.Catalog_search_window_Load);
            this.TL_window_main.ResumeLayout(false);
            this.TL_parameters_main.ResumeLayout(false);
            this.TL_parameters_manipulation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TL_window_main;
        private System.Windows.Forms.TableLayoutPanel TL_parameters_main;
        private System.Windows.Forms.TableLayoutPanel TL_parameters_manipulation;
        private System.Windows.Forms.Button BT_add_parameter;
        private System.Windows.Forms.Button BT_remove_parameter;
        private System.Windows.Forms.ListView LV_parameters;
    }
}