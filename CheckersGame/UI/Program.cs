using System;
using System.Windows.Forms;

namespace CheckersGame.UI
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // כל מה שעושים פה: מפעילים את הטופס הראשי/מנהל המשחק
            Application.Run(new FormGameManager());
        }
    }
}