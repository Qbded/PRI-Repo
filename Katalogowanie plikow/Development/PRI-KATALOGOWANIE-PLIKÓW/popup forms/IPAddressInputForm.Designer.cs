namespace PRI_KATALOGOWANIE_PLIKÓW.popup_forms
{
    partial class IPAddressInputForm
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
            this.mtxtbox_ipInput = new System.Windows.Forms.MaskedTextBox();
            this.button_ok = new System.Windows.Forms.Button();
            this.TLP_window_layout = new System.Windows.Forms.TableLayoutPanel();
            this.LB_address_prompt = new System.Windows.Forms.Label();
            this.TLP_window_layout.SuspendLayout();
            this.SuspendLayout();
            // 
            // mtxtbox_ipInput
            // 
            this.mtxtbox_ipInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.mtxtbox_ipInput.Location = new System.Drawing.Point(3, 38);
            this.mtxtbox_ipInput.Mask = "###.###.###.###";
            this.mtxtbox_ipInput.Name = "mtxtbox_ipInput";
            this.mtxtbox_ipInput.Size = new System.Drawing.Size(144, 20);
            this.mtxtbox_ipInput.TabIndex = 1;
            this.mtxtbox_ipInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mtxtbox_ipInput.ValidatingType = typeof(System.Net.IPAddress);
            // 
            // button_ok
            // 
            this.button_ok.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_ok.Location = new System.Drawing.Point(3, 67);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(144, 26);
            this.button_ok.TabIndex = 2;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // TLP_window_layout
            // 
            this.TLP_window_layout.ColumnCount = 1;
            this.TLP_window_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_window_layout.Controls.Add(this.LB_address_prompt, 0, 0);
            this.TLP_window_layout.Controls.Add(this.button_ok, 0, 2);
            this.TLP_window_layout.Controls.Add(this.mtxtbox_ipInput, 0, 1);
            this.TLP_window_layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_window_layout.Location = new System.Drawing.Point(0, 0);
            this.TLP_window_layout.Name = "TLP_window_layout";
            this.TLP_window_layout.RowCount = 4;
            this.TLP_window_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_window_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_window_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_window_layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_window_layout.Size = new System.Drawing.Size(150, 100);
            this.TLP_window_layout.TabIndex = 3;
            // 
            // LB_address_prompt
            // 
            this.LB_address_prompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_address_prompt.Location = new System.Drawing.Point(3, 9);
            this.LB_address_prompt.Name = "LB_address_prompt";
            this.LB_address_prompt.Size = new System.Drawing.Size(144, 13);
            this.LB_address_prompt.TabIndex = 3;
            this.LB_address_prompt.Text = "Podaj poprawny adres IPv4";
            this.LB_address_prompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // IPAddressInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 100);
            this.Controls.Add(this.TLP_window_layout);
            this.MaximumSize = new System.Drawing.Size(166, 138);
            this.MinimumSize = new System.Drawing.Size(166, 138);
            this.Name = "IPAddressInputForm";
            this.TLP_window_layout.ResumeLayout(false);
            this.TLP_window_layout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox mtxtbox_ipInput;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.TableLayoutPanel TLP_window_layout;
        private System.Windows.Forms.Label LB_address_prompt;
    }
}
