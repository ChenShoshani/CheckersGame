using System;
using System.Windows.Forms;
using CheckersGame.Logic;

namespace CheckersGame.UI
{
    public class FormGameManager : Form
    {
        public FormGameManager()
        {
            // אפשר לא לקרוא ל-InitializeComponent אם אין לנו Designer
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Game Manager";
            this.Load += FormGameManager_Load;
        }

        private void FormGameManager_Load(object sender, EventArgs e)
        {
            FormGameSettings settingsForm = new FormGameSettings();
            DialogResult result = settingsForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                GameController gameController = new GameController(
                    settingsForm.BoardSize,
                    settingsForm.Player1Name,
                    settingsForm.Player2Name,
                    settingsForm.IsAgainstComputer);

                FormCheckersGame gameForm = new FormCheckersGame(gameController);

                this.Hide();
                gameForm.ShowDialog();
                this.Close();
            }
            else
            {
                this.Close();
            }
        }
    }
}