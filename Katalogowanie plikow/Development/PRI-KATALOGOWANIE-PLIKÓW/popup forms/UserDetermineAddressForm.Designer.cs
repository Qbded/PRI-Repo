namespace PRI_KATALOGOWANIE_PLIKÓW.popup_forms
{
    partial class UserDetermineAddressForm
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
            this.TLP_main = new System.Windows.Forms.TableLayoutPanel();
            this.LV_detected_addresses = new System.Windows.Forms.ListView();
            this.adapter_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ip_address = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BT_manual_input = new System.Windows.Forms.Button();
            this.BT_continue = new System.Windows.Forms.Button();
            this.TLP_main.SuspendLayout();
            this.SuspendLayout();
            // 
            // TLP_main
            // 
            this.TLP_main.ColumnCount = 1;
            this.TLP_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_main.Controls.Add(this.LV_detected_addresses, 0, 0);
            this.TLP_main.Controls.Add(this.BT_manual_input, 0, 1);
            this.TLP_main.Controls.Add(this.BT_continue, 0, 2);
            this.TLP_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_main.Location = new System.Drawing.Point(0, 0);
            this.TLP_main.Name = "TLP_main";
            this.TLP_main.RowCount = 4;
            this.TLP_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.TLP_main.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TLP_main.Size = new System.Drawing.Size(284, 262);
            this.TLP_main.TabIndex = 0;
            // 
            // LV_detected_addresses
            // 
            this.LV_detected_addresses.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.LV_detected_addresses.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.adapter_name,
            this.ip_address});
            this.LV_detected_addresses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_detected_addresses.Location = new System.Drawing.Point(3, 3);
            this.LV_detected_addresses.MultiSelect = false;
            this.LV_detected_addresses.Name = "LV_detected_addresses";
            this.LV_detected_addresses.Size = new System.Drawing.Size(278, 192);
            this.LV_detected_addresses.TabIndex = 0;
            this.LV_detected_addresses.UseCompatibleStateImageBehavior = false;
            this.LV_detected_addresses.View = System.Windows.Forms.View.Details;
            this.LV_detected_addresses.ItemActivate += new System.EventHandler(this.LV_detected_addres_ItemActivate);
            // 
            // adapter_name
            // 
            this.adapter_name.Text = "Nazwa adaptera sieciowego";
            this.adapter_name.Width = 151;
            // 
            // ip_address
            // 
            this.ip_address.Text = "Adres IP";
            // 
            // BT_manual_input
            // 
            this.BT_manual_input.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_manual_input.Location = new System.Drawing.Point(3, 202);
            this.BT_manual_input.Name = "BT_manual_input";
            this.BT_manual_input.Size = new System.Drawing.Size(278, 23);
            this.BT_manual_input.TabIndex = 1;
            this.BT_manual_input.Text = "Ustaw adres IP ręcznie";
            this.BT_manual_input.UseVisualStyleBackColor = true;
            this.BT_manual_input.Click += new System.EventHandler(this.BT_manual_input_Click);
            // 
            // BT_continue
            // 
            this.BT_continue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_continue.Location = new System.Drawing.Point(3, 234);
            this.BT_continue.Name = "BT_continue";
            this.BT_continue.Size = new System.Drawing.Size(278, 23);
            this.BT_continue.TabIndex = 2;
            this.BT_continue.Text = "Zakończ wybieranie adresu";
            this.BT_continue.UseVisualStyleBackColor = true;
            this.BT_continue.Click += new System.EventHandler(this.BT_continue_Click);
            // 
            // UserDetermineAddressForm
            // 
            this.AcceptButton = this.BT_continue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.TLP_main);
            this.MaximumSize = new System.Drawing.Size(300, 300);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "UserDetermineAddressForm";
            this.Text = "Wybór adresu IP użytkownika";
            this.VisibleChanged += new System.EventHandler(this.UserDetermineAddressForm_VisibleChanged);
            this.TLP_main.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLP_main;
        private System.Windows.Forms.ListView LV_detected_addresses;
        private System.Windows.Forms.Button BT_manual_input;
        private System.Windows.Forms.Button BT_continue;
        private System.Windows.Forms.ColumnHeader adapter_name;
        private System.Windows.Forms.ColumnHeader ip_address;
    }
}