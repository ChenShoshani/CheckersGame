using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckersGame.UI
{
    public class FormGameSettings : Form
    {
        private Label m_LabelBoardSize;
        private RadioButton m_RadioButtonSize6;
        private RadioButton m_RadioButtonSize8;
        private RadioButton m_RadioButtonSize10;

        private Label m_LabelPlayers;
        private Label m_LabelPlayer1;
        private TextBox m_TextBoxPlayer1;

        private CheckBox m_CheckBoxPlayer2;
        private Label m_LabelPlayer2;
        private TextBox m_TextBoxPlayer2;

        private Button m_ButtonDone;

        public int BoardSize { get; private set; }

        public string Player1Name { get; private set; }

        public string Player2Name { get; private set; }

        public bool IsAgainstComputer { get; private set; }

        public FormGameSettings()
        {
            initializeComponents();
        }

        private void initializeComponents()
        {
            // הגדרות חלון
            this.Text = "Game Settings";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(260, 200); // טופס קטן יותר

            // Label ל-Board Size
            m_LabelBoardSize = new Label();
            m_LabelBoardSize.Text = "Board Size:";
            m_LabelBoardSize.Location = new Point(10, 10);
            m_LabelBoardSize.AutoSize = true;

            // RadioButtons ו-Labels לגודל הלוח
            m_RadioButtonSize6 = new RadioButton();
            m_RadioButtonSize6.Location = new Point(20, 35);
            m_RadioButtonSize6.Size = new Size(15, 15);
            m_RadioButtonSize6.Checked = true;

            Label labelSize6 = new Label();
            labelSize6.Text = "6 x 6";
            labelSize6.Location = new Point(40, 32);
            labelSize6.AutoSize = true;

            m_RadioButtonSize8 = new RadioButton();
            m_RadioButtonSize8.Location = new Point(80, 35);
            m_RadioButtonSize8.Size = new Size(15, 15);

            Label labelSize8 = new Label();
            labelSize8.Text = "8 x 8";
            labelSize8.Location = new Point(100, 32);
            labelSize8.AutoSize = true;

            m_RadioButtonSize10 = new RadioButton();
            m_RadioButtonSize10.Location = new Point(150, 35);
            m_RadioButtonSize10.Size = new Size(15, 15);

            Label labelSize10 = new Label();
            labelSize10.Text = "10 x 10";
            labelSize10.Location = new Point(170, 32);
            labelSize10.AutoSize = true;

            // Label ל-Players
            m_LabelPlayers = new Label();
            m_LabelPlayers.Text = "Players:";
            m_LabelPlayers.Location = new Point(10, 60);
            m_LabelPlayers.AutoSize = true;

            // Label ו-TextBox לשחקן 1
            m_LabelPlayer1 = new Label();
            m_LabelPlayer1.Text = "Player 1:";
            m_LabelPlayer1.Location = new Point(20, 85);
            m_LabelPlayer1.AutoSize = true;

            m_TextBoxPlayer1 = new TextBox();
            m_TextBoxPlayer1.Location = new Point(100, 82);
            m_TextBoxPlayer1.Width = 140;

            // CheckBox, Label ו-TextBox לשחקן 2
            m_CheckBoxPlayer2 = new CheckBox();
            m_CheckBoxPlayer2.Text = "";
            m_CheckBoxPlayer2.Location = new Point(20, 115);
            m_CheckBoxPlayer2.AutoSize = false;
            m_CheckBoxPlayer2.Size = new Size(15, 15);
            m_CheckBoxPlayer2.CheckedChanged += m_CheckBoxPlayer2_CheckedChanged;

            m_LabelPlayer2 = new Label();
            m_LabelPlayer2.Text = "Player 2:";
            m_LabelPlayer2.Location = new Point(40, 112);
            m_LabelPlayer2.AutoSize = true;

            m_TextBoxPlayer2 = new TextBox();
            m_TextBoxPlayer2.Location = new Point(100, 110);
            m_TextBoxPlayer2.Width = 140;
            m_TextBoxPlayer2.Text = "Computer";
            m_TextBoxPlayer2.Enabled = false;

            // כפתור Done
            m_ButtonDone = new Button();
            m_ButtonDone.Text = "Done";
            m_ButtonDone.Location = new Point(150, 150); // מרווח מהמסגרת הימנית
            m_ButtonDone.Size = new Size(80, 30);
            m_ButtonDone.Click += buttonDone_Click;


            // הוספת הפקדים לטופס
            this.Controls.Add(m_LabelBoardSize);
            this.Controls.Add(m_RadioButtonSize6);
            this.Controls.Add(labelSize6);
            this.Controls.Add(m_RadioButtonSize8);
            this.Controls.Add(labelSize8);
            this.Controls.Add(m_RadioButtonSize10);
            this.Controls.Add(labelSize10);
            this.Controls.Add(m_LabelPlayers);
            this.Controls.Add(m_LabelPlayer1);
            this.Controls.Add(m_TextBoxPlayer1);
            this.Controls.Add(m_CheckBoxPlayer2);
            this.Controls.Add(m_LabelPlayer2);
            this.Controls.Add(m_TextBoxPlayer2);
            this.Controls.Add(m_ButtonDone);
        }

        private void m_CheckBoxPlayer2_CheckedChanged(object sender, EventArgs e)
        {
            m_TextBoxPlayer2.Enabled = m_CheckBoxPlayer2.Checked;
            m_TextBoxPlayer2.Text = m_CheckBoxPlayer2.Checked ? "" : "Computer";
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            // קביעת גודל הלוח
            determineBoardSize();

            // איסוף פרטי השחקנים
            collectPlayerDetails();

            // בדיקת תקינות שמות השחקנים
            if (!validatePlayerNames())
            {
                return;
            }

            // אם הכול תקין - סגירת הטופס
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // קביעת גודל הלוח
        private void determineBoardSize()
        {
            if (m_RadioButtonSize6.Checked)
            {
                BoardSize = 6;
            }
            else if (m_RadioButtonSize8.Checked)
            {
                BoardSize = 8;
            }
            else
            {
                BoardSize = 10;
            }
        }

        // איסוף פרטי השחקנים
        private void collectPlayerDetails()
        {
            Player1Name = m_TextBoxPlayer1.Text.Trim();
            Player2Name = m_TextBoxPlayer2.Text.Trim();
            IsAgainstComputer = !m_CheckBoxPlayer2.Checked;
        }

        // בדיקת תקינות שמות השחקנים
        private bool validatePlayerNames()
        {
            bool isValid = true;

            // בדיקה אם שני השמות ריקים
            if (string.IsNullOrWhiteSpace(Player1Name) && (!IsAgainstComputer && string.IsNullOrWhiteSpace(Player2Name)))
            {
                MessageBox.Show("Please enter valid names for Player 1 and Player 2.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isValid = false;
            }
            else
            {
                // בדיקת שם שחקן 1
                if (string.IsNullOrWhiteSpace(Player1Name))
                {
                    MessageBox.Show("Please enter a valid name for Player 1.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isValid = false;
                }

                // בדיקת שם שחקן 2 אם זה לא נגד מחשב
                if (!IsAgainstComputer && string.IsNullOrWhiteSpace(Player2Name))
                {
                    MessageBox.Show("Please enter a valid name for Player 2.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}