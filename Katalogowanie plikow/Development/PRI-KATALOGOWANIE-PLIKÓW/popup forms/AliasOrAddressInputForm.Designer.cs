namespace PRI_KATALOGOWANIE_PLIKÓW.popup_forms
{
    partial class AliasOrAddressInputForm
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
            this.TLP_main_layout = new System.Windows.Forms.TableLayoutPanel();
            this.TB_alias_input = new System.Windows.Forms.TextBox();
            this.RB_alias = new System.Windows.Forms.RadioButton();
            this.MTB_address_input = new System.Windows.Forms.MaskedTextBox();
            this.RB_address = new System.Windows.Forms.RadioButton();
            this.LB_text = new System.Windows.Forms.Label();
            this.BT_ok = new System.Windows.Forms.Button();
            this.TLP_main_layout.SuspendLayout();
            this.SuspendLayout();
            // 
            // TLP_main_layout
            // 
            this.TLP_main_layout.ColumnCount = 1;
            this.TLP_main_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_main_layout.Controls.Add(this.TB_alias_input, 0, 4);
            this.TLP_main_layout.Controls.Add(this.RB_alias, 0, 3);
            this.TLP_main_layout.Controls.Add(this.MTB_address_input, 0, 2);
            this.TLP_main_layout.Controls.Add(this.RB_address, 0, 1);
            this.TLP_main_layout.Controls.Add(this.LB_text, 0, 0);
            this.TLP_main_layout.Controls.Add(this.BT_ok, 0, 5);
            this.TLP_main_layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_main_layout.Location = new System.Drawing.Point(0, 0);
            this.TLP_main_layout.Name = "TLP_main_layout";
            this.TLP_main_layout.RowCount = 7;
            this.TLP_main_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_main_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_main_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_main_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_main_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_main_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_main_layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_main_layout.Size = new System.Drawing.Size(284, 199);
            this.TLP_main_layout.TabIndex = 0;
            // 
            // TB_alias_input
            // 
            this.TB_alias_input.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_alias_input.Enabled = false;
            this.TB_alias_input.Location = new System.Drawing.Point(3, 134);
            this.TB_alias_input.Name = "TB_alias_input";
            this.TB_alias_input.Size = new System.Drawing.Size(278, 20);
            this.TB_alias_input.TabIndex = 3;
            this.TB_alias_input.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // RB_alias
            // 
            this.RB_alias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.RB_alias.AutoSize = true;
            this.RB_alias.Location = new System.Drawing.Point(3, 103);
            this.RB_alias.Name = "RB_alias";
            this.RB_alias.Size = new System.Drawing.Size(278, 17);
            this.RB_alias.TabIndex = 1;
            this.RB_alias.TabStop = true;
            this.RB_alias.Text = "Alias użytkownika";
            this.RB_alias.UseVisualStyleBackColor = true;
            this.RB_alias.CheckedChanged += new System.EventHandler(this.RB_alias_CheckedChanged);
            // 
            // MTB_address_input
            // 
            this.MTB_address_input.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.MTB_address_input.Location = new System.Drawing.Point(3, 70);
            this.MTB_address_input.Mask = "###.###.###.###";
            this.MTB_address_input.Name = "MTB_address_input";
            this.MTB_address_input.Size = new System.Drawing.Size(278, 20);
            this.MTB_address_input.TabIndex = 2;
            this.MTB_address_input.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // RB_address
            // 
            this.RB_address.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.RB_address.AutoSize = true;
            this.RB_address.Location = new System.Drawing.Point(3, 39);
            this.RB_address.Name = "RB_address";
            this.RB_address.Size = new System.Drawing.Size(278, 17);
            this.RB_address.TabIndex = 0;
            this.RB_address.TabStop = true;
            this.RB_address.Text = "Adres IP";
            this.RB_address.UseVisualStyleBackColor = true;
            this.RB_address.CheckedChanged += new System.EventHandler(this.RB_address_CheckedChanged);
            // 
            // LB_text
            // 
            this.LB_text.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_text.AutoSize = true;
            this.LB_text.Location = new System.Drawing.Point(3, 9);
            this.LB_text.Name = "LB_text";
            this.LB_text.Size = new System.Drawing.Size(278, 13);
            this.LB_text.TabIndex = 4;
            this.LB_text.Text = "Podaj adres IP albo alias użytkownika";
            this.LB_text.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BT_ok
            // 
            this.BT_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_ok.Location = new System.Drawing.Point(3, 164);
            this.BT_ok.Name = "BT_ok";
            this.BT_ok.Size = new System.Drawing.Size(278, 23);
            this.BT_ok.TabIndex = 5;
            this.BT_ok.Text = "OK";
            this.BT_ok.UseVisualStyleBackColor = true;
            this.BT_ok.Click += new System.EventHandler(this.BT_ok_Click);
            // 
            // AliasOrAddressInputForm
            // 
            this.AcceptButton = this.BT_ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 199);
            this.Controls.Add(this.TLP_main_layout);
            this.MaximumSize = new System.Drawing.Size(300, 237);
            this.MinimumSize = new System.Drawing.Size(300, 237);
            this.Name = "AliasOrAddressInputForm";
            this.TLP_main_layout.ResumeLayout(false);
            this.TLP_main_layout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_main_layout;
        private System.Windows.Forms.TextBox TB_alias_input;
        private System.Windows.Forms.RadioButton RB_alias;
        private System.Windows.Forms.MaskedTextBox MTB_address_input;
        private System.Windows.Forms.RadioButton RB_address;
        private System.Windows.Forms.Label LB_text;
        private System.Windows.Forms.Button BT_ok;
    }
}
