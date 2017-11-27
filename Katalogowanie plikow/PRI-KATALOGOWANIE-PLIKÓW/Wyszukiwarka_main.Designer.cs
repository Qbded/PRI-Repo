namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Wyszukiwarka_main
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
            this.TL_main = new System.Windows.Forms.TableLayoutPanel();
            this.TL_secondary = new System.Windows.Forms.TableLayoutPanel();
            this.LB_locations = new System.Windows.Forms.Label();
            this.LB_search_parameters = new System.Windows.Forms.Label();
            this.LB_parameter_options = new System.Windows.Forms.Label();
            this.TL_datasource = new System.Windows.Forms.TableLayoutPanel();
            this.TL_parameters = new System.Windows.Forms.TableLayoutPanel();
            this.TL_values = new System.Windows.Forms.TableLayoutPanel();
            this.BT_execute = new System.Windows.Forms.Button();
            this.TL_main.SuspendLayout();
            this.TL_secondary.SuspendLayout();
            this.SuspendLayout();
            // 
            // TL_main
            // 
            this.TL_main.ColumnCount = 1;
            this.TL_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TL_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TL_main.Controls.Add(this.TL_secondary, 0, 0);
            this.TL_main.Controls.Add(this.BT_execute, 0, 1);
            this.TL_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TL_main.Location = new System.Drawing.Point(0, 0);
            this.TL_main.Name = "TL_main";
            this.TL_main.RowCount = 2;
            this.TL_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TL_main.Size = new System.Drawing.Size(534, 346);
            this.TL_main.TabIndex = 0;
            // 
            // TL_secondary
            // 
            this.TL_secondary.ColumnCount = 3;
            this.TL_secondary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.TL_secondary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.TL_secondary.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.TL_secondary.Controls.Add(this.LB_locations, 0, 0);
            this.TL_secondary.Controls.Add(this.LB_search_parameters, 1, 0);
            this.TL_secondary.Controls.Add(this.LB_parameter_options, 2, 0);
            this.TL_secondary.Controls.Add(this.TL_datasource, 0, 1);
            this.TL_secondary.Controls.Add(this.TL_parameters, 1, 1);
            this.TL_secondary.Controls.Add(this.TL_values, 2, 1);
            this.TL_secondary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TL_secondary.Location = new System.Drawing.Point(3, 3);
            this.TL_secondary.Name = "TL_secondary";
            this.TL_secondary.RowCount = 2;
            this.TL_secondary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.TL_secondary.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_secondary.Size = new System.Drawing.Size(528, 308);
            this.TL_secondary.TabIndex = 0;
            // 
            // LB_locations
            // 
            this.LB_locations.AutoSize = true;
            this.LB_locations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_locations.Location = new System.Drawing.Point(3, 0);
            this.LB_locations.Name = "LB_locations";
            this.LB_locations.Size = new System.Drawing.Size(170, 22);
            this.LB_locations.TabIndex = 3;
            this.LB_locations.Text = "Gdzie mam szukać?";
            // 
            // LB_search_parameters
            // 
            this.LB_search_parameters.AutoSize = true;
            this.LB_search_parameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_search_parameters.Location = new System.Drawing.Point(179, 0);
            this.LB_search_parameters.Name = "LB_search_parameters";
            this.LB_search_parameters.Size = new System.Drawing.Size(169, 22);
            this.LB_search_parameters.TabIndex = 4;
            this.LB_search_parameters.Text = "Po czym mam szukać?";
            // 
            // LB_parameter_options
            // 
            this.LB_parameter_options.AutoSize = true;
            this.LB_parameter_options.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_parameter_options.Location = new System.Drawing.Point(354, 0);
            this.LB_parameter_options.Name = "LB_parameter_options";
            this.LB_parameter_options.Size = new System.Drawing.Size(171, 22);
            this.LB_parameter_options.TabIndex = 5;
            this.LB_parameter_options.Text = "Szczegóły parametru:";
            // 
            // TL_datasource
            // 
            this.TL_datasource.AutoScroll = true;
            this.TL_datasource.ColumnCount = 1;
            this.TL_datasource.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_datasource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TL_datasource.Location = new System.Drawing.Point(3, 25);
            this.TL_datasource.Name = "TL_datasource";
            this.TL_datasource.RowCount = 7;
            this.TL_datasource.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TL_datasource.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TL_datasource.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TL_datasource.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TL_datasource.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TL_datasource.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TL_datasource.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TL_datasource.Size = new System.Drawing.Size(170, 280);
            this.TL_datasource.TabIndex = 6;
            // 
            // TL_parameters
            // 
            this.TL_parameters.AutoScroll = true;
            this.TL_parameters.ColumnCount = 1;
            this.TL_parameters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_parameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TL_parameters.Location = new System.Drawing.Point(179, 25);
            this.TL_parameters.Name = "TL_parameters";
            this.TL_parameters.Size = new System.Drawing.Size(169, 280);
            this.TL_parameters.TabIndex = 7;
            // 
            // TL_values
            // 
            this.TL_values.AutoScroll = true;
            this.TL_values.ColumnCount = 1;
            this.TL_values.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TL_values.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TL_values.Location = new System.Drawing.Point(354, 25);
            this.TL_values.Name = "TL_values";
            this.TL_values.Size = new System.Drawing.Size(171, 280);
            this.TL_values.TabIndex = 8;
            // 
            // BT_execute
            // 
            this.BT_execute.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_execute.Enabled = false;
            this.BT_execute.Location = new System.Drawing.Point(3, 317);
            this.BT_execute.Name = "BT_execute";
            this.BT_execute.Size = new System.Drawing.Size(528, 26);
            this.BT_execute.TabIndex = 1;
            this.BT_execute.Text = "Przeszukaj";
            this.BT_execute.UseVisualStyleBackColor = true;
            this.BT_execute.Click += new System.EventHandler(this.BT_execute_Click);
            // 
            // Wyszukiwarka_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 346);
            this.Controls.Add(this.TL_main);
            this.MinimumSize = new System.Drawing.Size(550, 375);
            this.Name = "Wyszukiwarka_main";
            this.Text = "Przeszukiwarka katalogu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Wyszukiwarka_main_FormClosing);
            this.TL_main.ResumeLayout(false);
            this.TL_secondary.ResumeLayout(false);
            this.TL_secondary.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TL_main;
        private System.Windows.Forms.TableLayoutPanel TL_secondary;
        private System.Windows.Forms.Button BT_execute;
        private System.Windows.Forms.Label LB_locations;
        private System.Windows.Forms.Label LB_search_parameters;
        private System.Windows.Forms.Label LB_parameter_options;
        private System.Windows.Forms.TableLayoutPanel TL_datasource;
        private System.Windows.Forms.TableLayoutPanel TL_parameters;
        private System.Windows.Forms.TableLayoutPanel TL_values;
    }
}