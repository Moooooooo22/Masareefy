using System;
using System.Windows.Forms;
using Masareefy.Forms;

namespace Masareefy
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
            }

            Application.Run(new MainForm());
        }
    }
}
