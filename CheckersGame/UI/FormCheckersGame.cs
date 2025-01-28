using System;
using System.Drawing;
using System.Windows.Forms;
using CheckersGame.Logic;
using CheckersGame.Properties;

namespace CheckersGame.UI
{
    public class FormCheckersGame : Form
    {
        private const int k_ButtonSize = 50;
        private Label m_LabelPlayer1;
        private Label m_LabelPlayer2;
        private Button[,] m_ButtonsBoard;
        private GameController m_GameController;
        private bool m_IsPieceSelected = false;
        private Button m_SelectedButton = null;
        private Timer m_ComputerMoveTimer;
        private bool m_IsComputerMoveInProgress = false;

        public FormCheckersGame(GameController i_GameController)
        {
            m_GameController = i_GameController;
            initializeComponents();
            initializeBoard();
            updateScoresLabels();
            m_ComputerMoveTimer = new Timer();
            m_ComputerMoveTimer.Interval = 1000;
            m_ComputerMoveTimer.Tick += m_ComputerMoveTimer_Tick;
        }

        private void initializeComponents()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Checkers Game";
            this.MaximizeBox = false; 
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            m_LabelPlayer1 = new Label();
            m_LabelPlayer1.AutoSize = true;
            m_LabelPlayer1.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            m_LabelPlayer2 = new Label();
            m_LabelPlayer2.AutoSize = true;
            m_LabelPlayer2.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
            this.Controls.Add(m_LabelPlayer1);
            this.Controls.Add(m_LabelPlayer2);
            updatePlayerLabelsPosition();
        }

        private void updatePlayerLabelsPosition()
        {
            int boardSize = m_GameController.Board.Size; 
            int boardWidthInPixels = boardSize * k_ButtonSize; 
            int formWidth = boardWidthInPixels + 40;

            this.ClientSize = new Size(formWidth, this.ClientSize.Height);
            m_LabelPlayer1.Text = $"{m_GameController.FirstPlayer.Name}: {m_GameController.FirstPlayer.Score}";
            m_LabelPlayer2.Text = $"{m_GameController.SecondPlayer.Name}: {m_GameController.SecondPlayer.Score}";
            int player1LabelX = (formWidth / 4) - (m_LabelPlayer1.Width / 2); 
            int player2LabelX = (3 * formWidth / 4) - (m_LabelPlayer2.Width / 2); 

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

                    button.TabStop = false;
                    button.CausesValidation = false;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.Size = new Size(k_ButtonSize, k_ButtonSize);
                    button.Location = new Point(leftOffset + col * k_ButtonSize, topOffset + row * k_ButtonSize);
                    button.Tag = new Point(row, col);
                    button.Click += button_Click;

                    if ((row + col) % 2 == 0)
                    {
                        button.BackColor = Color.FromArgb(220, 220, 220);
                        button.Enabled = false;
                    }
                    else
                    {
                        button.BackColor = Color.FromArgb(50, 50, 50);
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
            handleButtonClick(sender);
        }

        private void handleButtonClick(object sender)
        {
            bool isCanContinue = true;
            Button clickedButton = sender as Button;

            if (clickedButton == null)
            {
                isCanContinue = false;
            }

            if (isCanContinue && m_GameController.IsComputerPlayer(m_GameController.CurrentPlayer))
            {
                isCanContinue = false;
            }

            if (isCanContinue)
            {
                handleValidButtonClick(clickedButton);
            }
        }

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
        }

        private void handleFirstClick(Button i_ClickedButton, eCellState i_CellState)
        {
            if (isCellBelongToCurrentPlayer(i_CellState))
            {
                m_IsPieceSelected = true;
                m_SelectedButton = i_ClickedButton;
                i_ClickedButton.BackColor = Color.LightBlue;
                this.ActiveControl = null;
            }
        }

        private void handleSecondClick(int i_Row, int i_Col)
        {
            Point fromPos = (Point)m_SelectedButton.Tag;

            if (fromPos.X == i_Row && fromPos.Y == i_Col)
            {
                resetSelectedButtonColor(fromPos);
                m_IsPieceSelected = false;
                m_SelectedButton = null;
            }
            else
            {
                string fromCellStr = convertRowColToCellString(fromPos.X, fromPos.Y);
                string toCellStr = convertRowColToCellString(i_Row, i_Col);
                eMoveResult moveResult = m_GameController.MakeMove(fromCellStr, toCellStr);

                handleMoveResult(moveResult);
                resetSelectedButtonColor(fromPos);
                m_IsPieceSelected = false;
                m_SelectedButton = null;
            }
        }

        private void resetSelectedButtonColor(Point i_ButtonPosition)
        {
            int row = i_ButtonPosition.X;
            int col = i_ButtonPosition.Y;

            if (m_SelectedButton != null)
            {
                m_SelectedButton.BackColor = getOriginalCellColor(row, col);
            }
        }

        private Color getOriginalCellColor(int i_Row, int i_Col)
        {
            Color originalColor;

            if ((i_Row + i_Col) % 2 == 0)
            {
                originalColor = Color.FromArgb(220, 220, 220);
            }
            else
            {
                originalColor = Color.FromArgb(50, 50, 50);
            }

            return originalColor;
        }

        private void handleMoveResult(eMoveResult i_Result)
        {
            switch (i_Result)
            {
                case eMoveResult.InvalidFormat:
                    MessageBox.Show("Invalid cell format!");
                    break;
                case eMoveResult.InvalidMove:
                    MessageBox.Show("Invalid move!");
                    break;
                case eMoveResult.MustCapture:
                    MessageBox.Show("You must capture!");
                    break;
                case eMoveResult.MustCaptureAgain:
                    MessageBox.Show("You have to continue your previous capture!");
                    break;
                case eMoveResult.AdditionalCaptureRequired:
                    updateBoardUI();
                    break;
                case eMoveResult.Success:
                    updateBoardUI();
                    checkGameOver();
                    if (!m_GameController.IsGameOver(out Player ignoreWinner)
                        && m_GameController.IsComputerPlayer(m_GameController.CurrentPlayer))
                    {
                        m_IsComputerMoveInProgress = true;
                        m_ComputerMoveTimer.Start();
                    }
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

        private Image pieceToImage(eCellState i_State)
        {
            Image result = null;

            switch (i_State)
            {
                case eCellState.PlayerX:
                    result = Resources.blackpiece;
                    break;

                case eCellState.PlayerXKing:
                    result = Resources.blackpieceking;
                    break;

                case eCellState.PlayerO:
                    result = Resources.redpiece;
                    break;

                case eCellState.PlayerOKing:
                    result = Resources.redpieceking;
                    break;

                default:
                    result = null;
                    break;
            }

            return result;
        }

        private void updateBoardUI()
        {
            int boardSize = m_GameController.Board.Size;

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    eCellState cellState = m_GameController.Board.GetCellState(row, col);
                    Image pieceImage = pieceToImage(cellState);

                    m_ButtonsBoard[row, col].BackgroundImageLayout = ImageLayout.Stretch;
                    m_ButtonsBoard[row, col].BackgroundImage = pieceImage;
                    m_ButtonsBoard[row, col].Text = string.Empty;

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
                string message;

                if (winner == null)
                {
                    message = $"Tie!{Environment.NewLine}Another round?";
                }
                else
                {
                    message = $"{winner.Name} Won!{Environment.NewLine}Another round?";
                }

                DialogResult userChoice = MessageBox.Show(message, "Game Over", MessageBoxButtons.YesNo);

                handleEndGameDialogResult(userChoice);
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

        private void m_ComputerMoveTimer_Tick(object sender, EventArgs e)
        {
            bool isStopTimer = false;

            if (!m_IsComputerMoveInProgress)
            {
                isStopTimer = true;
            }
            else
            {
                eMoveResult moveResult = m_GameController.MakeComputerMove();
                updateBoardUI();

                if (moveResult == eMoveResult.AdditionalCaptureRequired)
                {
                    isStopTimer = false;
                }
                else if (m_GameController.IsGameOver(out Player winner))
                {
                    isStopTimer = true;
                    m_IsComputerMoveInProgress = false;
                    checkGameOver();
                }
                else if (!m_GameController.IsComputerPlayer(m_GameController.CurrentPlayer))
                {
                    isStopTimer = true;
                    m_IsComputerMoveInProgress = false;
                }
            }

            if (isStopTimer)
            {
                m_ComputerMoveTimer.Stop();
            }
        }
    }
}
