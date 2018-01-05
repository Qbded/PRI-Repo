using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW.popup_forms
{
    public partial class IPAddressInputForm : Form
    {
        public IPAddress resultRef;
        public IPAddressInputForm(ref IPAddress result)
        {
            this.resultRef = result;

            InitializeComponent();
        }


        private void OkButtonClick(object sender, EventArgs e)
        {
            bool inputValid = ValidateIPAddressInput();
            if (!inputValid) return;

            String text = mtxtbox_ipInput.Text;
            text = Regex.Replace(text, @"\s+", "");
            text = Regex.Replace(text, ",", ".");
            IPAddress ipAddress = IPAddress.Parse(text);
            resultRef = ipAddress;

            this.Close();
        }


        private bool ValidateIPAddressInput()
        {
            String text = mtxtbox_ipInput.Text;
            text = Regex.Replace(text, @"\s+", "");
            text = Regex.Replace(text, ",", ".");

            IPAddress ip = null;
            bool valid = IPAddress.TryParse(text, out ip);
            if (valid)
            {
                return true;
            }
            else
            {
                return false;
            }
            //String pattern = "$[0-2]?[0-9]?[0-9].[0-2]?[0-9]?[0-9].[0-2]?[0-9]?[0-9].[0-2]?[0-9]?[0-9]^";
            //Regex regex = new Regex(pattern);
            //if (regex.Match(text).Success)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
    }
}
