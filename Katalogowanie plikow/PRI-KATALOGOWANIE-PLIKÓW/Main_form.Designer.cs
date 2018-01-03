﻿namespace PRI_KATALOGOWANIE_PLIKÓW
{
    partial class Main_form
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
            this.components = new System.ComponentModel.Container();
            this.TP_main_menu = new System.Windows.Forms.TabPage();
            this.TL_main_menu = new System.Windows.Forms.TableLayoutPanel();
            this.BT_test_database = new System.Windows.Forms.Button();
            this.BT_extract_metadata = new System.Windows.Forms.Button();
            this.BT_config = new System.Windows.Forms.Button();
            this.TC_catalog_display = new System.Windows.Forms.TabControl();
            this.TP_catalog_display = new System.Windows.Forms.TabPage();
            this.Catalog_page_main_layout = new System.Windows.Forms.TableLayoutPanel();
            this.Catalog_page_top_table_layout = new System.Windows.Forms.TableLayoutPanel();
            this.BT_specials = new System.Windows.Forms.Button();
            this.TB_catalog_path_current = new System.Windows.Forms.TextBox();
            this.BT_previous = new System.Windows.Forms.Button();
            this.LV_catalog_display = new System.Windows.Forms.ListView();
            this.Nazwa = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Typ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Ostatnia_modyfikacja = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Ostatnie_katalogowanie = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Rozmiar = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Widoczność_w_sieci = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Kopiowalność_w_sieci = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Kopiowalność_bez_pytania = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TP_networking = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.LB_current_network = new System.Windows.Forms.Label();
            this.LB_role = new System.Windows.Forms.Label();
            this.BT_create_network = new System.Windows.Forms.Button();
            this.BT_connect_to_network = new System.Windows.Forms.Button();
            this.BT_display_members = new System.Windows.Forms.Button();
            this.BT_download_peer_list = new System.Windows.Forms.Button();
            this.BT_download_external_catalog = new System.Windows.Forms.Button();
            this.BT_disconnect_from_network = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TP_main_menu.SuspendLayout();
            this.TL_main_menu.SuspendLayout();
            this.TC_catalog_display.SuspendLayout();
            this.TP_catalog_display.SuspendLayout();
            this.Catalog_page_main_layout.SuspendLayout();
            this.Catalog_page_top_table_layout.SuspendLayout();
            this.TP_networking.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TP_main_menu
            // 
            this.TP_main_menu.Controls.Add(this.TL_main_menu);
            this.TP_main_menu.Location = new System.Drawing.Point(4, 29);
            this.TP_main_menu.Margin = new System.Windows.Forms.Padding(2);
            this.TP_main_menu.Name = "TP_main_menu";
            this.TP_main_menu.Padding = new System.Windows.Forms.Padding(2);
            this.TP_main_menu.Size = new System.Drawing.Size(447, 394);
            this.TP_main_menu.TabIndex = 0;
            this.TP_main_menu.Text = "Menu główne";
            this.TP_main_menu.UseVisualStyleBackColor = true;
            // 
            // TL_main_menu
            // 
            this.TL_main_menu.ColumnCount = 1;
            this.TL_main_menu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TL_main_menu.Controls.Add(this.BT_test_database, 0, 0);
            this.TL_main_menu.Controls.Add(this.BT_extract_metadata, 0, 1);
            this.TL_main_menu.Controls.Add(this.BT_config, 0, 2);
            this.TL_main_menu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TL_main_menu.Location = new System.Drawing.Point(2, 2);
            this.TL_main_menu.Name = "TL_main_menu";
            this.TL_main_menu.RowCount = 4;
            this.TL_main_menu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.TL_main_menu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.TL_main_menu.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.TL_main_menu.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TL_main_menu.Size = new System.Drawing.Size(443, 390);
            this.TL_main_menu.TabIndex = 7;
            // 
            // BT_test_database
            // 
            this.BT_test_database.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_test_database.Location = new System.Drawing.Point(3, 3);
            this.BT_test_database.Name = "BT_test_database";
            this.BT_test_database.Size = new System.Drawing.Size(437, 30);
            this.BT_test_database.TabIndex = 1;
            this.BT_test_database.Text = "Test bazy";
            this.BT_test_database.UseVisualStyleBackColor = true;
            this.BT_test_database.Click += new System.EventHandler(this.BT_test_database_click);
            // 
            // BT_extract_metadata
            // 
            this.BT_extract_metadata.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_extract_metadata.Enabled = false;
            this.BT_extract_metadata.Location = new System.Drawing.Point(3, 39);
            this.BT_extract_metadata.Name = "BT_extract_metadata";
            this.BT_extract_metadata.Size = new System.Drawing.Size(437, 30);
            this.BT_extract_metadata.TabIndex = 6;
            this.BT_extract_metadata.Text = "Kataloguj";
            this.BT_extract_metadata.UseVisualStyleBackColor = true;
            this.BT_extract_metadata.Click += new System.EventHandler(this.BT_extract_metadata_click);
            // 
            // BT_config
            // 
            this.BT_config.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_config.Location = new System.Drawing.Point(3, 75);
            this.BT_config.Name = "BT_config";
            this.BT_config.Size = new System.Drawing.Size(437, 30);
            this.BT_config.TabIndex = 7;
            this.BT_config.Text = "Opcje programu";
            this.BT_config.UseVisualStyleBackColor = true;
            // 
            // TC_catalog_display
            // 
            this.TC_catalog_display.Controls.Add(this.TP_main_menu);
            this.TC_catalog_display.Controls.Add(this.TP_catalog_display);
            this.TC_catalog_display.Controls.Add(this.TP_networking);
            this.TC_catalog_display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TC_catalog_display.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TC_catalog_display.Location = new System.Drawing.Point(0, 0);
            this.TC_catalog_display.Margin = new System.Windows.Forms.Padding(2);
            this.TC_catalog_display.Name = "TC_catalog_display";
            this.TC_catalog_display.SelectedIndex = 0;
            this.TC_catalog_display.Size = new System.Drawing.Size(455, 427);
            this.TC_catalog_display.TabIndex = 0;
            // 
            // TP_catalog_display
            // 
            this.TP_catalog_display.Controls.Add(this.Catalog_page_main_layout);
            this.TP_catalog_display.Location = new System.Drawing.Point(4, 29);
            this.TP_catalog_display.Name = "TP_catalog_display";
            this.TP_catalog_display.Padding = new System.Windows.Forms.Padding(3);
            this.TP_catalog_display.Size = new System.Drawing.Size(447, 394);
            this.TP_catalog_display.TabIndex = 2;
            this.TP_catalog_display.Text = "Wyświetlanie katalogów";
            this.TP_catalog_display.UseVisualStyleBackColor = true;
            // 
            // Catalog_page_main_layout
            // 
            this.Catalog_page_main_layout.AutoSize = true;
            this.Catalog_page_main_layout.ColumnCount = 1;
            this.Catalog_page_main_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Catalog_page_main_layout.Controls.Add(this.Catalog_page_top_table_layout, 0, 0);
            this.Catalog_page_main_layout.Controls.Add(this.LV_catalog_display, 0, 1);
            this.Catalog_page_main_layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Catalog_page_main_layout.Location = new System.Drawing.Point(3, 3);
            this.Catalog_page_main_layout.Name = "Catalog_page_main_layout";
            this.Catalog_page_main_layout.RowCount = 2;
            this.Catalog_page_main_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.Catalog_page_main_layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Catalog_page_main_layout.Size = new System.Drawing.Size(441, 388);
            this.Catalog_page_main_layout.TabIndex = 4;
            // 
            // Catalog_page_top_table_layout
            // 
            this.Catalog_page_top_table_layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Catalog_page_top_table_layout.ColumnCount = 3;
            this.Catalog_page_top_table_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.Catalog_page_top_table_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.Catalog_page_top_table_layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.Catalog_page_top_table_layout.Controls.Add(this.BT_specials, 2, 0);
            this.Catalog_page_top_table_layout.Controls.Add(this.TB_catalog_path_current, 1, 0);
            this.Catalog_page_top_table_layout.Controls.Add(this.BT_previous, 0, 0);
            this.Catalog_page_top_table_layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Catalog_page_top_table_layout.Location = new System.Drawing.Point(3, 3);
            this.Catalog_page_top_table_layout.Name = "Catalog_page_top_table_layout";
            this.Catalog_page_top_table_layout.RowCount = 1;
            this.Catalog_page_top_table_layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Catalog_page_top_table_layout.Size = new System.Drawing.Size(435, 36);
            this.Catalog_page_top_table_layout.TabIndex = 0;
            this.Catalog_page_top_table_layout.SizeChanged += new System.EventHandler(this.Catalog_page_top_table_layout_SizeChanged);
            // 
            // BT_specials
            // 
            this.BT_specials.Location = new System.Drawing.Point(371, 3);
            this.BT_specials.Name = "BT_specials";
            this.BT_specials.Size = new System.Drawing.Size(63, 29);
            this.BT_specials.TabIndex = 2;
            this.BT_specials.Text = "Inne";
            this.BT_specials.UseVisualStyleBackColor = true;
            this.BT_specials.Click += new System.EventHandler(this.BT_specials_Click);
            // 
            // TB_catalog_path_current
            // 
            this.TB_catalog_path_current.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TB_catalog_path_current.Location = new System.Drawing.Point(73, 3);
            this.TB_catalog_path_current.Name = "TB_catalog_path_current";
            this.TB_catalog_path_current.Size = new System.Drawing.Size(292, 26);
            this.TB_catalog_path_current.TabIndex = 3;
            // 
            // BT_previous
            // 
            this.BT_previous.Location = new System.Drawing.Point(3, 3);
            this.BT_previous.Name = "BT_previous";
            this.BT_previous.Size = new System.Drawing.Size(63, 29);
            this.BT_previous.TabIndex = 1;
            this.BT_previous.Text = "Cofnij";
            this.BT_previous.UseVisualStyleBackColor = true;
            this.BT_previous.Click += new System.EventHandler(this.BT_previous_click);
            // 
            // LV_catalog_display
            // 
            this.LV_catalog_display.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Nazwa,
            this.Typ,
            this.Ostatnia_modyfikacja,
            this.Ostatnie_katalogowanie,
            this.Rozmiar,
            this.Widoczność_w_sieci,
            this.Kopiowalność_w_sieci,
            this.Kopiowalność_bez_pytania});
            this.LV_catalog_display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV_catalog_display.Enabled = false;
            this.LV_catalog_display.LabelWrap = false;
            this.LV_catalog_display.Location = new System.Drawing.Point(3, 45);
            this.LV_catalog_display.Name = "LV_catalog_display";
            this.LV_catalog_display.Size = new System.Drawing.Size(435, 340);
            this.LV_catalog_display.TabIndex = 0;
            this.LV_catalog_display.TabStop = false;
            this.LV_catalog_display.UseCompatibleStateImageBehavior = false;
            this.LV_catalog_display.View = System.Windows.Forms.View.Details;
            this.LV_catalog_display.VirtualMode = true;
            this.LV_catalog_display.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.LV_catalog_display_after_label_edit);
            this.LV_catalog_display.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.LV_catalog_display_before_label_edit);
            this.LV_catalog_display.CacheVirtualItems += new System.Windows.Forms.CacheVirtualItemsEventHandler(this.LV_catalog_display_cache_items);
            this.LV_catalog_display.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LV_catalog_display_item_selected);
            this.LV_catalog_display.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.LV_catalog_display_retrieve_item);
            this.LV_catalog_display.VirtualItemsSelectionRangeChanged += new System.Windows.Forms.ListViewVirtualItemsSelectionRangeChangedEventHandler(this.LV_catalog_display_item_range_select);
            this.LV_catalog_display.VisibleChanged += new System.EventHandler(this.LV_catalog_display_visible_changed);
            this.LV_catalog_display.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LV_catalog_display_single_click);
            this.LV_catalog_display.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LV_catalog_display_double_click);
            this.LV_catalog_display.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LV_catalog_display_click_no_selection);
            // 
            // Nazwa
            // 
            this.Nazwa.Text = "Nazwa";
            this.Nazwa.Width = 100;
            // 
            // Typ
            // 
            this.Typ.Text = "Typ";
            this.Typ.Width = 100;
            // 
            // Ostatnia_modyfikacja
            // 
            this.Ostatnia_modyfikacja.Text = "Ostatnia modyfikacja";
            this.Ostatnia_modyfikacja.Width = 200;
            // 
            // Ostatnie_katalogowanie
            // 
            this.Ostatnie_katalogowanie.Text = "Ostatnie katalogowanie";
            this.Ostatnie_katalogowanie.Width = 200;
            // 
            // Rozmiar
            // 
            this.Rozmiar.Text = "Rozmiar";
            this.Rozmiar.Width = 100;
            // 
            // Widoczność_w_sieci
            // 
            this.Widoczność_w_sieci.Text = "Czy jest widoczny w sieci?";
            // 
            // Kopiowalność_w_sieci
            // 
            this.Kopiowalność_w_sieci.Text = "Czy użytkownicy sieci mogą prosić o jego skopiowanie?";
            // 
            // Kopiowalność_bez_pytania
            // 
            this.Kopiowalność_bez_pytania.Text = "Czy użytkownicy sieci mogą skopiować go bez pytania?";
            // 
            // TP_networking
            // 
            this.TP_networking.Controls.Add(this.tableLayoutPanel1);
            this.TP_networking.Location = new System.Drawing.Point(4, 29);
            this.TP_networking.Name = "TP_networking";
            this.TP_networking.Size = new System.Drawing.Size(447, 394);
            this.TP_networking.TabIndex = 3;
            this.TP_networking.Text = "Funkcjonalności sieciowe";
            this.TP_networking.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.LB_current_network, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LB_role, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BT_create_network, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.BT_connect_to_network, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.BT_display_members, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.BT_download_peer_list, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.BT_download_external_catalog, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.BT_disconnect_from_network, 0, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(447, 394);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // LB_current_network
            // 
            this.LB_current_network.AutoSize = true;
            this.LB_current_network.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_current_network.Location = new System.Drawing.Point(3, 0);
            this.LB_current_network.Name = "LB_current_network";
            this.LB_current_network.Size = new System.Drawing.Size(441, 32);
            this.LB_current_network.TabIndex = 0;
            this.LB_current_network.Text = "Połączono z siecią:";
            // 
            // LB_role
            // 
            this.LB_role.AutoSize = true;
            this.LB_role.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_role.Location = new System.Drawing.Point(3, 32);
            this.LB_role.Name = "LB_role";
            this.LB_role.Size = new System.Drawing.Size(441, 32);
            this.LB_role.TabIndex = 1;
            this.LB_role.Text = "Rola w sieci:";
            // 
            // BT_create_network
            // 
            this.BT_create_network.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_create_network.Location = new System.Drawing.Point(3, 67);
            this.BT_create_network.Name = "BT_create_network";
            this.BT_create_network.Size = new System.Drawing.Size(441, 26);
            this.BT_create_network.TabIndex = 2;
            this.BT_create_network.Text = "Stwórz sieć";
            this.BT_create_network.UseVisualStyleBackColor = true;
            // 
            // BT_connect_to_network
            // 
            this.BT_connect_to_network.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_connect_to_network.Location = new System.Drawing.Point(3, 99);
            this.BT_connect_to_network.Name = "BT_connect_to_network";
            this.BT_connect_to_network.Size = new System.Drawing.Size(441, 26);
            this.BT_connect_to_network.TabIndex = 3;
            this.BT_connect_to_network.Text = "Dołącz do sieci";
            this.BT_connect_to_network.UseVisualStyleBackColor = true;
            // 
            // BT_display_members
            // 
            this.BT_display_members.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_display_members.Location = new System.Drawing.Point(3, 131);
            this.BT_display_members.Name = "BT_display_members";
            this.BT_display_members.Size = new System.Drawing.Size(441, 26);
            this.BT_display_members.TabIndex = 4;
            this.BT_display_members.Text = "Wyświetl uczestników sieci";
            this.BT_display_members.UseVisualStyleBackColor = true;
            // 
            // BT_download_peer_list
            // 
            this.BT_download_peer_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_download_peer_list.Location = new System.Drawing.Point(3, 163);
            this.BT_download_peer_list.Name = "BT_download_peer_list";
            this.BT_download_peer_list.Size = new System.Drawing.Size(441, 26);
            this.BT_download_peer_list.TabIndex = 5;
            this.BT_download_peer_list.Text = "Pobierz listę uczestników sieci";
            this.BT_download_peer_list.UseVisualStyleBackColor = true;
            // 
            // BT_download_external_catalog
            // 
            this.BT_download_external_catalog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_download_external_catalog.Location = new System.Drawing.Point(3, 195);
            this.BT_download_external_catalog.Name = "BT_download_external_catalog";
            this.BT_download_external_catalog.Size = new System.Drawing.Size(441, 26);
            this.BT_download_external_catalog.TabIndex = 6;
            this.BT_download_external_catalog.Text = "Pobierz katalog obiegowy";
            this.BT_download_external_catalog.UseVisualStyleBackColor = true;
            // 
            // BT_disconnect_from_network
            // 
            this.BT_disconnect_from_network.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BT_disconnect_from_network.Location = new System.Drawing.Point(3, 227);
            this.BT_disconnect_from_network.Name = "BT_disconnect_from_network";
            this.BT_disconnect_from_network.Size = new System.Drawing.Size(441, 26);
            this.BT_disconnect_from_network.TabIndex = 7;
            this.BT_disconnect_from_network.Text = "Odłącz się od sieci";
            this.BT_disconnect_from_network.UseVisualStyleBackColor = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(385, 48);
            this.contextMenuStrip1.Text = "Uwzględnij w nazwie folderu datę i godzinę  katalogowania";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(384, 22);
            this.toolStripMenuItem1.Text = "Uwzględnij w nazwie folderu datę i godzinę  katalogowania";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(384, 22);
            this.toolStripMenuItem2.Text = "Zapisz raport katgalogowania";
            // 
            // Main_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(455, 427);
            this.Controls.Add(this.TC_catalog_display);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(471, 465);
            this.Name = "Main_form";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Katalogowanie plików";
            this.TP_main_menu.ResumeLayout(false);
            this.TL_main_menu.ResumeLayout(false);
            this.TC_catalog_display.ResumeLayout(false);
            this.TP_catalog_display.ResumeLayout(false);
            this.TP_catalog_display.PerformLayout();
            this.Catalog_page_main_layout.ResumeLayout(false);
            this.Catalog_page_top_table_layout.ResumeLayout(false);
            this.Catalog_page_top_table_layout.PerformLayout();
            this.TP_networking.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage TP_main_menu;
        private System.Windows.Forms.TabControl TC_catalog_display;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Button BT_test_database;
        private System.Windows.Forms.Button BT_extract_metadata;
        private System.Windows.Forms.TabPage TP_catalog_display;
        private System.Windows.Forms.ListView LV_catalog_display;
        private System.Windows.Forms.ColumnHeader Nazwa;
        private System.Windows.Forms.ColumnHeader Ostatnia_modyfikacja;
        private System.Windows.Forms.ColumnHeader Typ;
        private System.Windows.Forms.ColumnHeader Rozmiar;
        private System.Windows.Forms.TextBox TB_catalog_path_current;
        private System.Windows.Forms.Button BT_specials;
        private System.Windows.Forms.Button BT_previous;
        private System.Windows.Forms.ColumnHeader Ostatnie_katalogowanie;
        private System.Windows.Forms.TableLayoutPanel Catalog_page_main_layout;
        private System.Windows.Forms.TableLayoutPanel Catalog_page_top_table_layout;
        private System.Windows.Forms.ColumnHeader Widoczność_w_sieci;
        private System.Windows.Forms.ColumnHeader Kopiowalność_w_sieci;
        private System.Windows.Forms.ColumnHeader Kopiowalność_bez_pytania;
        private System.Windows.Forms.TableLayoutPanel TL_main_menu;
        private System.Windows.Forms.Button BT_config;
        private System.Windows.Forms.TabPage TP_networking;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label LB_current_network;
        private System.Windows.Forms.Label LB_role;
        private System.Windows.Forms.Button BT_create_network;
        private System.Windows.Forms.Button BT_connect_to_network;
        private System.Windows.Forms.Button BT_display_members;
        private System.Windows.Forms.Button BT_download_peer_list;
        private System.Windows.Forms.Button BT_download_external_catalog;
        private System.Windows.Forms.Button BT_disconnect_from_network;
    }
}

