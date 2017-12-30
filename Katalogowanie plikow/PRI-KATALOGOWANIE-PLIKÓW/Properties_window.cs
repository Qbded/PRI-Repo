using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    public partial class Properties_window : Form
    {
        #region Deklaracja zmiennych

        public List<string> names_passed;
        public DataTable data_passed;

        #endregion

        #region Konstruktor

        public Properties_window()
        {
            if (names_passed == null) names_passed = new List<string>();
            if (data_passed == null) data_passed = new DataTable();

            InitializeComponent();
        }

        #endregion

        #region Logika okna

        private void LV_metadata_basic_fill()
        {
            for(int i = 0; i < 11; i++)
            {
                ListViewItem row = new ListViewItem();
                row.Text = names_passed[i];
                string temp = (data_passed.Rows[0].ItemArray[i]).ToString();
                row.SubItems.Add(temp);

                LV_metadata_basic.Items.Add(row);
            }
        }
        private void LV_metadata_advanced_fill()
        {
            for (int i = 11; i < data_passed.Rows[0].ItemArray.Count(); i++)
            {
                ListViewItem row = new ListViewItem();
                row.Text = names_passed[i];
                string temp = (data_passed.Rows[0].ItemArray[i]).ToString();
                if (names_passed[i] == "EXTRACTED_TEXT")
                {
                    temp = temp.Replace('\r', ' ');
                }
                row.SubItems.Add(temp);

                LV_metadata_advanced.Items.Add(row);
            }
        }

        private void Properties_window_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible == true)
            {
                LV_metadata_basic_fill();
                LV_metadata_advanced_fill();

                LV_metadata_basic.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                LV_metadata_advanced.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        #endregion
    }
}
