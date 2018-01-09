using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW.popup_forms
{
    public partial class AliasOrAddressInputForm : Form
    {
        public String alias_result;
        public System.Net.IPAddress address_result;

        public AliasOrAddressInputForm(ref System.Net.IPAddress ipAddr, ref String alias)
        {
            this.alias_result = alias;
            this.address_result = ipAddr;

            InitializeComponent();
        }

        private void RB_address_CheckedChanged(object sender, EventArgs e)
        {
            if(RB_address.Checked)
            {
                MTB_address_input.Text = "";
                MTB_address_input.Enabled = true;
            }
            else
            {
                MTB_address_input.Text = "";
                MTB_address_input.Enabled = false;
            }
        }

        private void RB_alias_CheckedChanged(object sender, EventArgs e)
        {
            if (RB_alias.Checked)
            {
                TB_alias_input.Text = "";
                TB_alias_input.Enabled = true;
            }
            else
            {
                TB_alias_input.Text = "";
                TB_alias_input.Enabled = false;
            }
        }

        private bool ValidateIPAddressInput()
        {
            String text = MTB_address_input.Text;
            text = Regex.Replace(text, @"\s+", "");
            text = Regex.Replace(text, ",", ".");

            System.Net.IPAddress ip = null;
            bool valid = System.Net.IPAddress.TryParse(text, out ip);
            if (valid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BT_ok_Click(object sender, EventArgs e)
        {
            if(RB_address.Checked)
            {
                bool inputValid = ValidateIPAddressInput();
                if (inputValid)
                {
                    String text = MTB_address_input.Text;
                    text = Regex.Replace(text, @"\s+", "");
                    text = Regex.Replace(text, ",", ".");
                    System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(text);
                    address_result = ipAddress;

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Podany adres IP nie może zostać uznany za poprawny.");
                    return;
                }
            }
            if(RB_alias.Checked)
            {
                List<char> illegal_characters = System.IO.Path.GetInvalidFileNameChars().ToList();
                illegal_characters.Add('_');
                bool validation_result = true;

                char[] enteredAlias = TB_alias_input.Text.ToArray();

                if (enteredAlias.Count() == 0)
                {
                    MessageBox.Show("Nie podano nazwy.");
                    return;
                }

                foreach (char illegal_character in illegal_characters)
                {
                    if (enteredAlias.Contains(illegal_character))
                    {
                        validation_result = false;
                        break;
                    }
                }

                if (validation_result == true)
                {
                    alias_result = TB_alias_input.Text;

                    this.Close();
                }
                else
                {
                    string illegal_characters_display = "";
                    foreach (char illegal_character in illegal_characters)
                    {
                        if (!Char.IsControl(illegal_character)) illegal_characters_display += illegal_character + ", ";
                    }
                    illegal_characters_display = illegal_characters_display.TrimEnd(new char[] { ',', ' ' });

                    MessageBox.Show("Podana nazwa zawiera niedopuszczalne znaki.\n" +
                                    "Nielegalne znaki to: " + illegal_characters_display + "\n" +
                                    "Jak i wszystkie znaki kontrolne.");
                }
            }
        }
    }
}
