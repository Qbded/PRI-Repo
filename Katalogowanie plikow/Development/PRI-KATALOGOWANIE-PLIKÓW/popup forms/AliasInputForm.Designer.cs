namespace PRI_KATALOGOWANIE_PLIKÓW.popup_forms
{
    partial class AliasInputForm
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
            this.TLP_window_layout = new System.Windows.Forms.TableLayoutPanel();
            this.LB_alias_prompt = new System.Windows.Forms.Label();
            this.TB_alias_input = new System.Windows.Forms.TextBox();
            this.BT_ok = new System.Windows.Forms.Button();
            this.TLP_window_layout.SuspendLayout();
            this.SuspendLayout();
            // 
            // TLP_window_layout
            // 
            this.TLP_window_layout.ColumnCount = 1;
            this.TLP_window_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_window_layout.Controls.Add(this.LB_alias_prompt, 0, 0);
            this.TLP_window_layout.Controls.Add(this.TB_alias_input, 0, 1);
            this.TLP_window_layout.Controls.Add(this.BT_ok, 0, 2);
            this.TLP_window_layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_window_layout.Location = new System.Drawing.Point(0, 0);
            this.TLP_window_layout.Name = "TLP_window_layout";
            this.TLP_window_layout.RowCount = 4;
            this.TLP_window_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_window_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_window_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_window_layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_window_layout.Size = new System.Drawing.Size(150, 100);
            this.TLP_window_layout.TabIndex = 0;
            // 
            // LB_alias_prompt
            // 
            this.LB_alias_prompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LB_alias_prompt.AutoSize = true;
            this.LB_alias_prompt.Location = new System.Drawing.Point(3, 9);
            this.LB_alias_prompt.Name = "LB_alias_prompt";
            this.LB_alias_prompt.Size = new System.Drawing.Size(144, 13);
            this.LB_alias_prompt.TabIndex = 0;
            this.LB_alias_prompt.Text = "Podaj nazwę użytkownika";
            this.LB_alias_prompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TB_alias_input
            // 
            this.TB_alias_input.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_alias_input.Location = new System.Drawing.Point(3, 38);
            this.TB_alias_input.Name = "TB_alias_input";
            this.TB_alias_input.Size = new System.Drawing.Size(144, 20);
            this.TB_alias_input.TabIndex = 1;
            this.TB_alias_input.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BT_ok
            // 
            this.BT_ok.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_ok.Location = new System.Drawing.Point(3, 67);
            this.BT_ok.Name = "BT_ok";
            this.BT_ok.Size = new System.Drawing.Size(144, 26);
            this.BT_ok.TabIndex = 2;
            this.BT_ok.Text = "OK";
            this.BT_ok.UseVisualStyleBackColor = true;
            this.BT_ok.Click += new System.EventHandler(this.BT_ok_Click);
            // 
            // AliasInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 100);
            this.Controls.Add(this.TLP_window_layout);
            this.MaximumSize = new System.Drawing.Size(166, 138);
            this.MinimumSize = new System.Drawing.Size(166, 138);
            this.Name = "AliasInputForm";
            this.TLP_window_layout.ResumeLayout(false);
            this.TLP_window_layout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_window_layout;
        private System.Windows.Forms.Label LB_alias_prompt;
        private System.Windows.Forms.TextBox TB_alias_input;
        private System.Windows.Forms.Button BT_ok;
    }
}