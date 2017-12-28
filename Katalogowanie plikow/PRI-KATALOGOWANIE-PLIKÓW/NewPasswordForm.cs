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
    public partial class NewPasswordForm : Form
    {
        public NewPasswordForm()
        {
            this.DialogResult = DialogResult.Cancel;
            InitializeComponent();
        }


        private void OnLoginButtonClick(object sender, EventArgs e)
        {
            if(newPasswordInput.Text != null 
                && !newPasswordInput.Text.Equals("")
                && repeatPasswordInput.Text != null
                && !repeatPasswordInput.Text.Equals("")
                && newPasswordInput.Text.Equals(repeatPasswordInput.Text))
            {
                AppInstanceLoginManager loginManager = new AppInstanceLoginManager();
                loginManager.SetDigest(newPasswordInput.Text);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
