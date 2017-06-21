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
            this.TP_properties_advanced = new System.Windows.Forms.TabPage();
            this.TC_properties_container.SuspendLayout();
            this.SuspendLayout();
            // 
            // TC_properties_container
            // 
            this.TC_properties_container.Controls.Add(this.TP_properties_basic);
            this.TC_properties_container.Controls.Add(this.TP_properties_advanced);
            this.TC_properties_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_properties_container.Location = new System.Drawing.Point(0, 0);
            this.TC_properties_container.Name = "TC_properties_container";
            this.TC_properties_container.SelectedIndex = 0;
            this.TC_properties_container.Size = new System.Drawing.Size(284, 262);
            this.TC_properties_container.TabIndex = 0;
            // 
            // TP_properties_basic
            // 
            this.TP_properties_basic.Location = new System.Drawing.Point(4, 22);
            this.TP_properties_basic.Name = "TP_properties_basic";
            this.TP_properties_basic.Padding = new System.Windows.Forms.Padding(3);
            this.TP_properties_basic.Size = new System.Drawing.Size(276, 236);
            this.TP_properties_basic.TabIndex = 0;
            this.TP_properties_basic.Text = "Podstawowe";
            this.TP_properties_basic.UseVisualStyleBackColor = true;
            // 
            // TP_properties_advanced
            // 
            this.TP_properties_advanced.Location = new System.Drawing.Point(4, 22);
            this.TP_properties_advanced.Name = "TP_properties_advanced";
            this.TP_properties_advanced.Padding = new System.Windows.Forms.Padding(3);
            this.TP_properties_advanced.Size = new System.Drawing.Size(276, 236);
            this.TP_properties_advanced.TabIndex = 1;
            this.TP_properties_advanced.Text = "Zaawansowane";
            this.TP_properties_advanced.UseVisualStyleBackColor = true;
            // 
            // Properties_window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.TC_properties_container);
            this.Name = "Properties_window";
            this.Text = "Właściwości";
            this.TC_properties_container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TC_properties_container;
        private System.Windows.Forms.TabPage TP_properties_basic;
        private System.Windows.Forms.TabPage TP_properties_advanced;
    }
}