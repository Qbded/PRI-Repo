using PRI_KATALOGOWANIE_PLIKÓW.classes;
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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }


        private void OnLoginButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            String password = passwordInput.Text;
            VerifyPassword(password);
            this.Close();
        }


        private void VerifyPassword(String password)
        {
            LoginManager loginManager = new LoginManager();
            bool verified = loginManager.VerifyPassword(password);
            if (verified)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
