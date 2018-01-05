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
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mtxtbox_ipInput
            // 
            this.mtxtbox_ipInput.Location = new System.Drawing.Point(36, 59);
            this.mtxtbox_ipInput.Mask = "###.###.###.###";
            this.mtxtbox_ipInput.ValidatingType = typeof(System.Net.IPAddress);
            this.mtxtbox_ipInput.Name = "mtxtbox_ipInput";
            this.mtxtbox_ipInput.Size = new System.Drawing.Size(88, 20);
            this.mtxtbox_ipInput.TabIndex = 0;
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(36, 85);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(88, 38);
            this.button_ok.TabIndex = 1;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(OkButtonClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please input valid IP address";
            // 
            // IPAddressInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(152, 144);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.mtxtbox_ipInput);
            this.Name = "IPAddressInputForm";
            this.Text = "IPAddressInputForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox mtxtbox_ipInput;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Label label1;
    }
}