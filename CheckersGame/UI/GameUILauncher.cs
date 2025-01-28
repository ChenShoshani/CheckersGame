using System.Windows.Forms;

namespace CheckersGame.UI
{
    public class GameUILauncher
    {
        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormGameSettings settingsForm = new FormGameSettings();
            settingsForm.ShowDialog();
        }
    }
}