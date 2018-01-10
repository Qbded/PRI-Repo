namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Properties_window
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
            this.TC_properties_container = new System.Windows.Forms.TabControl();
            this.TP_properties_basic = new System.Windows.Forms.TabPage();
            this.LV_metadata_basic = new System.Windows.Forms.ListView();
            this.metadata_basic_field = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.metadata_basic_value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TP_properties_advanced = new System.Windows.Forms.TabPage();
            this.LV_metadata_advanced = new System.Windows.Forms.ListView();
            this.metadata_advanced_field = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.metadata_advanced_value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TP_extracted_text = new System.Windows.Forms.TabPage();
            this.LV_extracted_text_container = new System.Windows.Forms.ListView();
            this.TC_properties_container.SuspendLayout();
            this.TP_properties_basic.SuspendLayout();
            this.TP_properties_advanced.SuspendLayout();
            this.TP_extracted_text.SuspendLayout();
            this.SuspendLayout();
            // 
            // TC_properties_container
            // 
            this.TC_properties_container.Controls.Add(this.TP_properties_basic);
            this.TC_properties_container.Controls.Add(this.TP_properties_advanced);
            this.TC_properties_container.Controls.Add(this.TP_extracted_text);
            this.TC_properties_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_properties_container.Location = new System.Drawing.Point(0, 0);
            this.TC_properties_container.Name = "TC_properties_container";
            this.TC_properties_container.SelectedIndex = 0;
            this.TC_properties_container.Size = new System.Drawing.Size(284, 262);
            this.TC_properties_container.TabIndex = 0;
            this.TC_properties_container.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.TC_properties_container_Selecting);
            // 
            // TP_properties_basic
            // 
            this.TP_properties_basic.Controls.Add(this.LV_metadata_basic);
            this.TP_properties_basic.Location = new System.Drawing.Point(4, 22);
            this.TP_properties_basic.Name = "TP_properties_basic";
            this.TP_properties_basic.Padding = new System.Windows.Forms.Padding(3);
            this.TP_properties_basic.Size = new System.Drawing.Size(276, 236);
            this.TP_properties_basic.TabIndex = 0;
            this.TP_properties_basic.Text = "Podstawowe";
            this.TP_properties_basic.UseVisualStyleBackColor = true;
            // 
            // LV_metadata_basic
            // 
            this.LV_metadata_basic.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.metadata_basic_field,
            this.metadata_basic_value});
            this.LV_metadata_basic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_metadata_basic.Location = new System.Drawing.Point(3, 3);
            this.LV_metadata_basic.Name = "LV_metadata_basic";
            this.LV_metadata_basic.Size = new System.Drawing.Size(270, 230);
            this.LV_metadata_basic.TabIndex = 0;
            this.LV_metadata_basic.UseCompatibleStateImageBehavior = false;
            this.LV_metadata_basic.View = System.Windows.Forms.View.Details;
            // 
            // metadata_basic_field
            // 
            this.metadata_basic_field.Text = "Nazwa pola";
            // 
            // metadata_basic_value
            // 
            this.metadata_basic_value.Text = "Wartość pola";
            // 
            // TP_properties_advanced
            // 
            this.TP_properties_advanced.Controls.Add(this.LV_metadata_advanced);
            this.TP_properties_advanced.Location = new System.Drawing.Point(4, 22);
            this.TP_properties_advanced.Name = "TP_properties_advanced";
            this.TP_properties_advanced.Padding = new System.Windows.Forms.Padding(3);
            this.TP_properties_advanced.Size = new System.Drawing.Size(276, 236);
            this.TP_properties_advanced.TabIndex = 1;
            this.TP_properties_advanced.Text = "Zaawansowane";
            this.TP_properties_advanced.UseVisualStyleBackColor = true;
            // 
            // LV_metadata_advanced
            // 
            this.LV_metadata_advanced.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.metadata_advanced_field,
            this.metadata_advanced_value});
            this.LV_metadata_advanced.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_metadata_advanced.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.LV_metadata_advanced.Location = new System.Drawing.Point(3, 3);
            this.LV_metadata_advanced.MultiSelect = false;
            this.LV_metadata_advanced.Name = "LV_metadata_advanced";
            this.LV_metadata_advanced.Size = new System.Drawing.Size(270, 230);
            this.LV_metadata_advanced.TabIndex = 0;
            this.LV_metadata_advanced.UseCompatibleStateImageBehavior = false;
            this.LV_metadata_advanced.View = System.Windows.Forms.View.Details;
            // 
            // metadata_advanced_field
            // 
            this.metadata_advanced_field.Text = "Nazwa pola";
            // 
            // metadata_advanced_value
            // 
            this.metadata_advanced_value.Text = "Wartość pola";
            // 
            // TP_extracted_text
            // 
            this.TP_extracted_text.Controls.Add(this.LV_extracted_text_container);
            this.TP_extracted_text.Location = new System.Drawing.Point(4, 22);
            this.TP_extracted_text.Name = "TP_extracted_text";
            this.TP_extracted_text.Padding = new System.Windows.Forms.Padding(3);
            this.TP_extracted_text.Size = new System.Drawing.Size(276, 236);
            this.TP_extracted_text.TabIndex = 2;
            this.TP_extracted_text.Text = "Tekst z pliku";
            this.TP_extracted_text.UseVisualStyleBackColor = true;
            // 
            // LV_extracted_text_container
            // 
            this.LV_extracted_text_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_extracted_text_container.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.LV_extracted_text_container.Location = new System.Drawing.Point(3, 3);
            this.LV_extracted_text_container.MultiSelect = false;
            this.LV_extracted_text_container.Name = "LV_extracted_text_container";
            this.LV_extracted_text_container.Size = new System.Drawing.Size(270, 230);
            this.LV_extracted_text_container.TabIndex = 0;
            this.LV_extracted_text_container.UseCompatibleStateImageBehavior = false;
            this.LV_extracted_text_container.View = System.Windows.Forms.View.List;
            // 
            // Properties_window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.TC_properties_container);
            this.Name = "Properties_window";
            this.Text = "Właściwości";
            this.VisibleChanged += new System.EventHandler(this.Properties_window_VisibleChanged);
            this.TC_properties_container.ResumeLayout(false);
            this.TP_properties_basic.ResumeLayout(false);
            this.TP_properties_advanced.ResumeLayout(false);
            this.TP_extracted_text.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TC_properties_container;
        private System.Windows.Forms.TabPage TP_properties_basic;
        private System.Windows.Forms.TabPage TP_properties_advanced;
        private System.Windows.Forms.ListView LV_metadata_basic;
        private System.Windows.Forms.ListView LV_metadata_advanced;
        private System.Windows.Forms.ColumnHeader metadata_basic_field;
        private System.Windows.Forms.ColumnHeader metadata_basic_value;
        private System.Windows.Forms.ColumnHeader metadata_advanced_field;
        private System.Windows.Forms.ColumnHeader metadata_advanced_value;
        private System.Windows.Forms.ListView LV_extracted_text_container;
        private System.Windows.Forms.TabPage TP_extracted_text;
    }
}
