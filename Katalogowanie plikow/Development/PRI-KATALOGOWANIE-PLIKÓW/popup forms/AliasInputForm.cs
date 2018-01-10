using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW.popup_forms
{
    public partial class AliasInputForm : Form
    {
        public String resultRef;

        private char[] enteredAlias;
        private List<char> illegal_characters = System.IO.Path.GetInvalidFileNameChars().ToList();
        private bool validation_result = false;

        public AliasInputForm(ref String result)
        {
            this.resultRef = result;

            InitializeComponent();

            illegal_characters.Add('_');
        }

        private void BT_ok_Click(object sender, EventArgs e)
        {
            validation_result = true;
            enteredAlias = TB_alias_input.Text.ToArray();

            if (enteredAlias.Count() == 0)
            {
                MessageBox.Show("Nie podano nazwy.");
            }

            foreach (char illegal_character in illegal_characters)
            {
                if (enteredAlias.Contains(illegal_character)) 
                {
                    validation_result = false;
                    break;
                }
            }

            if(validation_result == true)
            {
                resultRef = TB_alias_input.Text.ToUpper();

                this.Close();
            }
            else
            {
                string illegal_characters_display = "";
                foreach (char illegal_character in illegal_characters)
                {
                    if(!Char.IsControl(illegal_character)) illegal_characters_display += illegal_character + ", ";
                }
                illegal_characters_display = illegal_characters_display.TrimEnd(new char[] { ',', ' ' });

                MessageBox.Show("Podana nazwa zawiera niedopuszczalne znaki.\n" +
                                "Nielegalne znaki to: " + illegal_characters_display +"\n" +
                                "Jak i wszystkie znaki kontrolne.");
            }
        }
    }
}
