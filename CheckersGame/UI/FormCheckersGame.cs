using System;
using System.Drawing;
using System.Windows.Forms;
using CheckersGame.Logic;

namespace CheckersGame.UI
{
    public class FormCheckersGame : Form
    {
        private const int k_ButtonSize = 50;
        private Label m_LabelPlayer1;
        private Label m_LabelPlayer2;
        private Button[,] m_ButtonsBoard;

        private GameController m_GameController;

        // לחיצה ראשונה / שנייה
        private bool m_IsPieceSelected = false;
        private Button m_SelectedButton = null;

        // Timer לצעדי המחשב
        private Timer m_ComputerMoveTimer;
        private bool m_IsComputerMoveInProgress = false;

        public FormCheckersGame(GameController i_GameController)
        {
            m_GameController = i_GameController;
            initializeComponents();
            initializeBoard();
            updateScoresLabels();

            // אתחול ה-Timer
            m_ComputerMoveTimer = new Timer();
            m_ComputerMoveTimer.Interval = 1000;
            m_ComputerMoveTimer.Tick += m_ComputerMoveTimer_Tick;
        }

        private void initializeComponents()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Checkers Game";
            this.MaximizeBox = false; // ביטול כפתור ההגדלה
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // מניעת שינוי גודל הטופס

            // יצירת התוויות של השחקנים
            m_LabelPlayer1 = new Label();
            m_LabelPlayer1.AutoSize = true;
            m_LabelPlayer1.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);

            m_LabelPlayer2 = new Label();
            m_LabelPlayer2.AutoSize = true;
            m_LabelPlayer2.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);

            this.Controls.Add(m_LabelPlayer1);
            this.Controls.Add(m_LabelPlayer2);

            // קריאה לעדכון מיקום התוויות
            updatePlayerLabelsPosition();
        }

        private void updatePlayerLabelsPosition()
        {
            int boardSize = m_GameController.Board.Size; // גודל הלוח
            int boardWidthInPixels = boardSize * k_ButtonSize; // רוחב הלוח בפיקסלים
            int formWidth = boardWidthInPixels + 40; // רוחב הטופס הכולל
            this.ClientSize = new Size(formWidth, this.ClientSize.Height);

            // חישוב המיקום האמצעי של התוויות
            m_LabelPlayer1.Text = $"{m_GameController.FirstPlayer.Name}: {m_GameController.FirstPlayer.Score}";
            m_LabelPlayer2.Text = $"{m_GameController.SecondPlayer.Name}: {m_GameController.SecondPlayer.Score}";

            int player1LabelX = (formWidth / 4) - (m_LabelPlayer1.Width / 2); // רבע מהטופס
            int player2LabelX = (3 * formWidth / 4) - (m_LabelPlayer2.Width / 2); // שלושה רבעים מהטופס

            m_LabelPlayer1.Location = new Point(player1LabelX, 10);
            m_LabelPlayer2.Location = new Point(player2LabelX, 10);
        }

        private void initializeBoard()
        {
            int boardSize = m_GameController.Board.Size;
            m_ButtonsBoard = new Button[boardSize, boardSize];

            int topOffset = 40;
            int leftOffset = 10;

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    Button button = new Button();
                    button.Size = new Size(k_ButtonSize, k_ButtonSize);
                    button.Location = new Point(leftOffset + col * k_ButtonSize, topOffset + row * k_ButtonSize);
                    button.Tag = new Point(row, col);
                    button.Click += button_Click;

                    // אפור אם משבצת (row+col) זוגי
                    if ((row + col) % 2 == 0)
                    {
                        button.BackColor = Color.LightGray;
                        button.Enabled = false;
                    }
                    else
                    {
                        button.BackColor = Color.White;
                    }

                    this.Controls.Add(button);
                    m_ButtonsBoard[row, col] = button;
                }
            }

            int formWidth = boardSize * k_ButtonSize + 40;
            int formHeight = boardSize * k_ButtonSize + 80;
            this.ClientSize = new Size(formWidth, formHeight);

            updateBoardUI();
        }

        private void button_Click(object sender, EventArgs e)
        {
            // מתודה ראשית קטנה וקלה לקריאה
            handleButtonClick(sender);
        }

        /// <summary>
        /// זוהי מתודה מטפלת ב-Click של כפתור, אחרי שבדקנו שהוא לא null
        /// ושלא מדובר בתור המחשב
        /// </summary>
        private void handleButtonClick(object sender)
        {
            bool isCanContinue = true;

            Button clickedButton = sender as Button;
            if (clickedButton == null)
            {
                // אם הכפתור לחוץ לא תקין
                isCanContinue = false;
            }

            // אם זה תור המחשב – בוחרים לא לאפשר לחיצה
            if (isCanContinue && m_GameController.IsComputerPlayer(m_GameController.CurrentPlayer))
            {
                isCanContinue = false;
            }

            // רק אם canContinue עדיין true => ממשיכים
            if (isCanContinue)
            {
                handleValidButtonClick(clickedButton);
            }

            // שימו לב: אין כאן return נוסף -> המתודה מסתיימת כאן
        }

        /// <summary>
        /// הגיע כפתור תקין והתור הוא של המשתמש: נטפל בלחיצה כראוי
        /// </summary>
        private void handleValidButtonClick(Button i_ClickedButton)
        {
            Point position = (Point)i_ClickedButton.Tag;
            int row = position.X;
            int col = position.Y;

            eCellState cellState = m_GameController.Board.GetCellState(row, col);

            if (!m_IsPieceSelected)
            {
                handleFirstClick(i_ClickedButton, cellState);
            }
            else
            {
                handleSecondClick(row, col);
            }
            // המתודה מסתיימת כאן, Return יחיד משתמע בסופה
        }

        /// <summary>
        /// לחיצה ראשונה: מנסים לבחור חייל ששייך לשחקן הנוכחי
        /// </summary>
        private void handleFirstClick(Button i_ClickedButton, eCellState i_CellState)
        {
            if (isCellBelongToCurrentPlayer(i_CellState))
            {
                m_IsPieceSelected = true;
                m_SelectedButton = i_ClickedButton;
                i_ClickedButton.BackColor = Color.LightBlue;
            }
            // אין else ריק – אם זה לא חייל של השחקן, פשוט לא עושים כלום
            // Return אחד בסוף המתודה באופן טבעי
        }

        /// <summary>
        /// לחיצה שנייה: מנסים להזיז מהחייל שנבחר אל מקום פנוי
        /// </summary>
        private void handleSecondClick(int i_Row, int i_Col)
        {
            Point fromPos = (Point)m_SelectedButton.Tag;
            string fromCellStr = convertRowColToCellString(fromPos.X, fromPos.Y);
            string toCellStr = convertRowColToCellString(i_Row, i_Col);

            eMoveResult moveResult = m_GameController.MakeMove(fromCellStr, toCellStr);
            handleMoveResult(moveResult);

            // משחררים את הבחירה
            m_IsPieceSelected = false;
            if (m_SelectedButton != null)
            {
                m_SelectedButton.BackColor = Color.White;
                m_SelectedButton = null;
            }
            // Return אחד בסוף המתודה באופן טבעי
        }

        private void handleMoveResult(eMoveResult i_Result)
        {
            switch (i_Result)
            {
                case eMoveResult.Success:
                    updateBoardUI();
                    updateScoresLabels();
                    checkGameOver();

                    // אחרי מהלך השחקן, אם עכשיו תור המחשב:
                    if (!m_GameController.IsGameOver(out Player ignoreWinner) && m_GameController.IsComputerPlayer(m_GameController.CurrentPlayer))
                    {
                        m_IsComputerMoveInProgress = true;
                        m_ComputerMoveTimer.Start();
                    }
                    break;

                case eMoveResult.AdditionalCaptureRequired:
                    // עדיין השחקן חייב להמשיך לאכול
                    // לא מציגים פה "You must capture again!" אוטומטית
                    // רק מעדכנים לוח
                    updateBoardUI();
                    updateScoresLabels();
                    break;

                case eMoveResult.MustCapture:
                    // פה אפשר לרשום את ההודעה שהייתה קודם "You must capture again!"
                    // כי השחקן ניסה מהלך שלא אכילה כאשר הוא מחויב להמשיך ללכוד
                    MessageBox.Show("You must capture!");
                    break;

                case eMoveResult.InvalidMove:
                    MessageBox.Show("Invalid move!");
                    break;

                case eMoveResult.InvalidFormat:
                    MessageBox.Show("Invalid cell format!");
                    break;
            }
        }

        private bool isCellBelongToCurrentPlayer(eCellState i_CellState)
        {
            Player current = m_GameController.CurrentPlayer;
            return (i_CellState == current.Symbol ||
                   (i_CellState == eCellState.PlayerXKing && current.Symbol == eCellState.PlayerX) ||
                   (i_CellState == eCellState.PlayerOKing && current.Symbol == eCellState.PlayerO));
        }

        private void updateBoardUI()
        {
            int boardSize = m_GameController.Board.Size;
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    eCellState cellState = m_GameController.Board.GetCellState(row, col);
                    // שימוש במטודה GetCharForCell מהלוגיקה
                    char cellChar = m_GameController.Board.GetCharForCell(cellState);
                    m_ButtonsBoard[row, col].Text = cellChar.ToString();
                }
            }
        }

        private void updateScoresLabels()
        {
            m_LabelPlayer1.Text = $"{m_GameController.FirstPlayer.Name}: {m_GameController.FirstPlayer.Score}";
            m_LabelPlayer2.Text = $"{m_GameController.SecondPlayer.Name}: {m_GameController.SecondPlayer.Score}";
        }

        private string convertRowColToCellString(int i_Row, int i_Col)
        {
            char rowChar = (char)('A' + i_Row);
            char colChar = (char)('a' + i_Col);
            return $"{rowChar}{colChar}";
        }

        private void checkGameOver()
        {
            if (m_GameController.IsGameOver(out Player winner))
            {
                if (winner == null)
                {
                    DialogResult dr = MessageBox.Show("Tie! Another round?", "Game Over", MessageBoxButtons.YesNo);
                    handleEndGameDialogResult(dr);
                }
                else
                {
                    DialogResult dr = MessageBox.Show($"{winner.Name} Won! Another round?", "Game Over", MessageBoxButtons.YesNo);
                    handleEndGameDialogResult(dr);
                }
            }
        }

        private void handleEndGameDialogResult(DialogResult i_Result)
        {
            if (i_Result == DialogResult.Yes)
            {
                m_GameController.ResetGame(m_GameController.Board.Size);
                updateBoardUI();
                updateScoresLabels();
            }
            else
            {
                this.Close();
            }
        }

        // ---------------------
        // ******* Timer *******
        // ---------------------
        private void m_ComputerMoveTimer_Tick(object sender, EventArgs e)
        {
            // בכל Tick, אם עדיין צריך לבצע מהלך של המחשב (צעד אחד):
            if (!m_IsComputerMoveInProgress)
            {
                m_ComputerMoveTimer.Stop();
                return;
            }

            // מבצעים צעד אחד
            eMoveResult moveResult = m_GameController.MakeComputerMove();
            updateBoardUI();
            updateScoresLabels();

            if (moveResult == eMoveResult.AdditionalCaptureRequired)
            {
                // יש עוד לכידה לאותו שחקן מחשב
                // נשארים בתור מחשב, אז מחכים לטיק הבא
                return;
            }
            else
            {
                // או Success, או InvalidMove (אין מהלך), או MustCapture...
                // בודקים אם המשחק נגמר
                if (m_GameController.IsGameOver(out Player winner))
                {
                    m_ComputerMoveTimer.Stop();
                    m_IsComputerMoveInProgress = false;
                    checkGameOver();
                }
                else
                {
                    // אם לא נגמר, ייתכן והתור עבר לשחקן השני
                    if (!m_GameController.IsComputerPlayer(m_GameController.CurrentPlayer))
                    {
                        // תור השחקן השני (אנושי)
                        m_ComputerMoveTimer.Stop();
                        m_IsComputerMoveInProgress = false;
                    }
                    // אחרת, עדיין מחשב => ממשיכים בטיימר עד טיק הבא
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // אין Forfeit – פשוט סוגרים
        }
    }
}
