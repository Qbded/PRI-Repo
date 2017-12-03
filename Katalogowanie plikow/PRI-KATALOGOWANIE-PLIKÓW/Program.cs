using PRI_KATALOGOWANIE_PLIKÓW.classes;
using System;
using System.Windows.Forms;

namespace PRI_KATALOGOWANIE_PLIKÓW
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.ProcessExit +=
                new EventHandler(OnProcessExit);
            //new DatabaseEncryptor().DecryptDatabaseFile();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main_form());
        }


        static void OnProcessExit(object sender, EventArgs e)
        {
            //new DatabaseEncryptor().EncryptDatabaseFile();
        }
    }
}
