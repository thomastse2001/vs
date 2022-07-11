using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AspNetCore6WebApp.WinForm.Forms
{
    public partial class FormTicTacToe : Form
    {
        /// Tic Tac Toe
        /// https://en.wikipedia.org/wiki/Tic-tac-toe
        /// https://www.codeproject.com/Tips/842418/Designing-the-Layout-of-Windows-Forms-using-a
        /// 1 2 3
        /// 4 5 6
        /// 7 8 9

        public string VersionString { get; set; } = string.Empty;

        private enum GameMode
        {
            [Description("Two Players")]
            TwoPlayers = 0,
            [Description("VS Computer")]
            VsComputer = 1
        }

        private enum Player
        {
            [Description("Player One")]
            Player1 = 1,
            [Description("Player Two")]
            Player2 = 2
        }

        private static class Mark
        {
            public static char Empty = ' ';
            public static char Cross = 'X';
            public static char Nough = 'O';
        }

        private enum GameResult
        {
            GameContinue = 0,
            Player1Win = 1,
            Player2Win = 2,
            Draw = 3
        }

        /// Parameters.
        private Player CurrentPlayer = Player.Player1;
        ///// https://stackoverflow.com/questions/1014005/how-to-populate-instantiate-a-c-sharp-array-with-a-single-value
        //private char[] MyArray = Enumerable.Repeat(Mark.Empty, 10).ToArray();
        ///// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/multidimensional-arrays
        //private char[,] MyArray2d = new char[3, 3];
        private readonly List<int> MarkList = new();

        public FormTicTacToe()
        {
            InitializeComponent();
        }

        private static int GetArrayIndexByPosition(int column, int row) { return row * 3 + column + 1; }

        private void ClearLogOnUI()
        {
            Invoke(new MethodInvoker(delegate
            {
                TxtLog.Clear();
            }));
        }

        private void AppendLogToUI(string format, params object?[] args)
        {
            Invoke(new MethodInvoker(delegate
            {
                int iMax = 10000;
                TxtLog.AppendText((TxtLog.TextLength > 0 ? Environment.NewLine : null) + ((args == null || args.Length < 1) ? format : string.Format(format, args)));
                if (TxtLog.TextLength > iMax) TxtLog.Text = TxtLog.Text[^iMax..];
                TxtLog.SelectionStart = TxtLog.TextLength;
                TxtLog.ScrollToCaret();
            }));
        }

        private void FormTicTacToe_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VersionString)) LblVersion.Text = string.Empty;
            else
            {
                LblVersion.Text = VersionString;
                LblVersion.Top = 9;
                LblVersion.Left = this.Width - LblVersion.Width - 24;
            }
            TxtLog.Width = this.Width - 40;
            TxtLog.Text = string.Join(Environment.NewLine,
                "Welcome !!!",
                "Select the mode and start to play !!!"
                );
            //TxtLog.Text = string.Join(Environment.NewLine,
            //    "* It is a two player game in a 3x3 grid.",
            //    "* Only one player can play at a time.",
            //    "* One uses 'X', another uses 'O'.",
            //    "* If any of the players have filled a square then the other player and the same player cannot override that square.",
            //    "* There are only two conditions that may be match will be draw or may be win.",
            //    "* The player that succeeds in placing three respective mark (X or O) in a horizontal, vertical or diagonal row wins the game."
            //    );
            /// https://www.codeproject.com/Questions/204702/how-to-bind-combobox-with-Enum
            /// https://www.c-sharpcorner.com/code/323/how-do-i-get-description-of-enum.aspx
            Dictionary<GameMode, string> cbxDist = new();
            foreach (GameMode i in Enum.GetValues(typeof(GameMode)))// cbxDist.Add(i, Enum.GetName(typeof(PlayingMode), i));
                cbxDist.Add(i, TT.EnumExtensionMethods.GetEnumDescription(i));
            CbxMode.DisplayMember = "Value";
            CbxMode.ValueMember = "Key";
            CbxMode.DataSource = new BindingSource(cbxDist, null);
            CbxMode.Focus();
        }

        private void TlpGrid_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            try
            {
                /// https://www.codeproject.com/Questions/5293989/How-to-insert-a-horizontal-line-inbetween-the-tabl
                //AppendLogToUI("Row = {0}, Column = {1}: X = {6}, Y = {7}, Left = {2}, Top = {3}, Width = {4}, Height = {5}", e.Row, e.Column, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width, e.CellBounds.Height, e.CellBounds.X, e.CellBounds.Y);
                //AppendLogToUI("Row = {0}, Column = {1}: X = {6}, Y = {7}, Left = {2}, Top = {3}, Width = {4}, Height = {5}", e.Row, e.Column, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Width, e.ClipRectangle.Height, e.ClipRectangle.X, e.ClipRectangle.Y);
                if (e.Row > 0) e.Graphics.DrawLine(Pens.Black, e.CellBounds.Location, new Point(e.CellBounds.Right, e.CellBounds.Top));
                if (e.Column > 0) e.Graphics.DrawLine(Pens.Black, e.CellBounds.Location, new Point(e.CellBounds.Left, e.CellBounds.Bottom));
            }
            catch
            {
            }
        }

        private void InitializeGame()
        {
            try
            {
                int column;
                CurrentPlayer = Player.Player1;
                MarkList.Clear();
                TlpGrid.Controls.Clear();
                for (column = 0; column < 3; column++) for (int row = 0; row < 3; row++)
                    {
                        int i = GetArrayIndexByPosition(column, row);
                        Label label = new()
                        {
                            Name = "Lbl" + i.ToString(),
                            Text = i.ToString(),
                            Font = new Font("Arial", 10),
                            Dock = DockStyle.None,
                            TextAlign = ContentAlignment.TopLeft,
                            AutoSize = true
                        };
                        label.MouseClick += new MouseEventHandler(NumberLabel_MouseClick);
                        TlpGrid.Controls.Add(label, column, row);
                    }
                ClearLogOnUI();
                string s = "Start New Game. ";
                if ((GameMode)CbxMode.SelectedValue == GameMode.TwoPlayers) s += "Two Players Mode is selected.";
                else s += "VS Computer Mode is selected. Play Two is computer.";
                AppendLogToUI(s);
                AppendLogToUI("Play One first.");
            }
            catch { }
        }

        private void NumberLabel_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                //Label label = sender as Label;
                //AppendLogToUI("Label = {0}", label.Text);
                if (sender == null) return;
                Label label = (Label)sender;
                if (!int.TryParse(label.Text, out int index))
                {
                    return;
                }
                int indexm1 = index - 1;
                ProceedRound(indexm1 % 3, indexm1 / 3);
            }
            catch { }
        }

        private void BtnRestartGame_Click(object sender, EventArgs e)
        {
            InitializeGame();
        }

        private void CbxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitializeGame();
        }

        /// https://stackoverflow.com/questions/15449504/how-do-i-determine-the-cell-being-clicked-on-in-a-tablelayoutpanel
        /// https://www.codeproject.com/Questions/126882/How-can-I-know-the-clicked-cell-in-the-TableLayout
        private static Point? GetRowColIndex(TableLayoutPanel tlp, int x, int y)
        {
            if (x > tlp.Width || y > tlp.Height || x < 0 || y < 0) return null;
            int[] widths = tlp.GetColumnWidths();
            int i = widths.Length - 1;
            int w = tlp.Width - widths[i];
            while (i >= 0 && x < w) w -= widths[--i];
            int[] heights = tlp.GetRowHeights();
            int j = heights.Length - 1;
            int h = tlp.Height - heights[j];
            while (j >= 0 && y < h) h -= heights[--j];
            return new Point(i, j);
        }

        private static bool IsWinFromArray(int[] array)
        {
            return
                /// 1st horizontal line and 1st vertical line
                (array.Contains(1) &&
                ((array.Contains(2) && array.Contains(3))
                || (array.Contains(4) && array.Contains(7))))
                /// 2nd horizaontal line, 2nd vertical line, 1st diagonal line and 2nd diagonal line
                || (array.Contains(5) &&
                ((array.Contains(4) && array.Contains(6))
                || (array.Contains(2) && array.Contains(8))
                || (array.Contains(1) && array.Contains(9))
                || (array.Contains(7) && array.Contains(3))))
                /// 3rd horizontal line, 3rd vertical line
                || (array.Contains(9) &&
                ((array.Contains(7) && array.Contains(8))
                || (array.Contains(3) && array.Contains(6))));
        }

        private static GameResult AnalyzeResult(List<int> markList)
        {
            //char c = array[1];
            ///// 1st horizontal line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array[2]) && c.Equals(array[3]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 1st vertical line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array[4]) && c.Equals(array[7]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 2nd horizontal line
            //c = array[5];
            //if (Mark.Empty.Equals(c) == false && c.Equals(array[4]) && c.Equals(array[6]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 2nd vertical line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array[2]) && c.Equals(array[8]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 1st diagonal line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array[1]) && c.Equals(array[9]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 2nd diagonal line.
            //if (Mark.Empty.Equals(c) == false && c.Equals(array[3]) && c.Equals(array[7]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 3rd horizontal line
            //c = array[9];
            //if (Mark.Empty.Equals(c) == false && c.Equals(array[7]) && c.Equals(array[8]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 3rd vertical line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array[3]) && c.Equals(array[6]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            //return array.Contains(Mark.Empty) ? GameResult.GameContinue : GameResult.Draw;

            ///// 1st vertical line
            //char c = array2d[0, 0];
            //if (Mark.Empty.Equals(c) == false && c.Equals(array2d[0, 1]) && c.Equals(array2d[0, 2]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 1st horizontal line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array2d[1, 0]) && c.Equals(array2d[2, 0]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 2nd vertical line
            //c = array2d[1, 1];
            //if (Mark.Empty.Equals(c) == false && c.Equals(array2d[1, 0]) && c.Equals(array2d[1, 2]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 2nd horizontal line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array2d[0, 1]) && c.Equals(array2d[2, 1]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 1st diagonal line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array2d[0, 0]) && c.Equals(array2d[2, 2]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 2nd diagonal line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array2d[2, 0]) && c.Equals(array2d[0, 2]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 3rd vertical line
            //c = array2d[2, 2];
            //if (Mark.Empty.Equals(c) == false && c.Equals(array2d[2, 0]) && c.Equals(array2d[2, 1]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            ///// 3rd horizontal line
            //if (Mark.Empty.Equals(c) == false && c.Equals(array2d[0, 2]) && c.Equals(array2d[1, 2]))
            //    return Mark.Cross.Equals(c) ? GameResult.Player1Win : GameResult.Player2Win;
            //bool b = true;
            //int column = 0;
            //while (b && column < 3)
            //{
            //    int row = 0;
            //    while (b && row < 3)
            //    {
            //        if (Mark.Empty.Equals(array2d[column, row])) b = false;
            //        row++;
            //    }
            //    column++;
            //}
            //return b ? GameResult.Draw : GameResult.GameContinue;

            //int p1Count = player1List.Count;
            //int p2Count = player2List.Count;
            //if (p1Count < 3 && p2Count < 3) return GameResult.GameContinue;
            //if (p1Count > p2Count && IsWinFromList(player1List)) return GameResult.Player1Win;
            //if (IsWinFromList(player2List)) return GameResult.Player2Win;
            //if (p1Count >= 5) return GameResult.Draw;
            //return GameResult.GameContinue;

            int iCount = markList.Count;
            if (iCount < 5) return GameResult.GameContinue;
            /// Current round is player one if count is odd
            if ((iCount % 2) == 1)
            {
                /// https://stackoverflow.com/questions/37382990/how-to-split-an-array-to-2-arrays-with-odd-and-even-indices-respectively
                int[] array = markList.Where((x, i) => i % 2 == 0).ToArray();
                if (IsWinFromArray(array)) return GameResult.Player1Win;
            }
            int[] array2 = markList.Where((x, i) => i % 2 == 1).ToArray();
            if (IsWinFromArray(array2)) return GameResult.Player2Win;
            return iCount >= 9 ? GameResult.Draw : GameResult.GameContinue;
        }

        private void MarkPosition(int column, int row)
        {
            try
            {
                char targetChar;
                Color targetColor;
                if (CurrentPlayer == Player.Player1)
                {
                    targetChar = Mark.Cross;
                    targetColor = Color.Red;
                }
                else
                {
                    targetChar = Mark.Nough;
                    targetColor = Color.Blue;
                }
                TlpGrid.Controls.Remove(TlpGrid.GetControlFromPosition(column, row));
                TlpGrid.Controls.Add(new Label()
                {
                    ForeColor = targetColor,
                    Font = new Font("Arial", 60, FontStyle.Bold),
                    Dock = DockStyle.None,
                    Anchor = AnchorStyles.None,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = true,
                    Text = targetChar.ToString()
                }, column, row);
            }
            catch { }
        }

        private void ProceedRound(int column, int row)
        {
            try
            {
                string s;
                /// Validation
                if (column < 0 || column > 2 || row < 0 || row > 2)
                {
                    s = "Out of range. Please select again.";
                    MessageBox.Show(s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AppendLogToUI(s);
                    return;
                }
                int index = GetArrayIndexByPosition(column, row);
                if (MarkList.Contains(index))
                {
                    s = "This position is already filled. Please select the other.";
                    MessageBox.Show(s, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                /// Mark the position
                MarkList.Add(index);
                MarkPosition(column, row);
                //char targetChar;
                //Color targetColor;
                //if (CurrentPlayer == Player.Player1)
                //{
                //    targetChar = Mark.Cross;
                //    targetColor = Color.Red;
                //}
                //else
                //{
                //    targetChar = Mark.Nough;
                //    targetColor = Color.Blue;
                //}
                //TlpGrid.Controls.Remove(TlpGrid.GetControlFromPosition(column, row));
                //TlpGrid.Controls.Add(new Label()
                //{
                //    ForeColor = targetColor,
                //    Font = new Font("Arial", 60, FontStyle.Bold),
                //    Dock = DockStyle.None,
                //    Anchor = AnchorStyles.None,
                //    TextAlign = ContentAlignment.MiddleCenter,
                //    AutoSize = true,
                //    Text = targetChar.ToString()
                //}, column, row);
                /// Analyze
                GameResult gameResult = AnalyzeResult(MarkList);
                if (gameResult != GameResult.GameContinue)
                {
                    s = "";
                    switch (gameResult)
                    {
                        case GameResult.Player1Win:
                            s = "Player One Win !!!";
                            break;
                        case GameResult.Player2Win:
                            s = (GameMode)CbxMode.SelectedValue == GameMode.TwoPlayers ? "Player Two Win !!!" : "Computer Win !!!";
                            break;
                        case GameResult.Draw:
                            s = "Draw";
                            break;
                    }
                    AppendLogToUI(s);
                    MessageBox.Show(s, "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    InitializeGame();
                    return;
                }
                /// Round Result
                s = "";
                if (CurrentPlayer == Player.Player1)
                {
                    CurrentPlayer = Player.Player2;
                    s = (GameMode)CbxMode.SelectedValue == GameMode.TwoPlayers ? TT.EnumExtensionMethods.GetEnumDescription(CurrentPlayer) : "Computer turn.";
                }
                else
                {
                    CurrentPlayer = Player.Player1;
                    s = TT.EnumExtensionMethods.GetEnumDescription(CurrentPlayer);
                }
                AppendLogToUI("{0} turn. Please mark a postion.", s);
                /// Computer decision
                if (CurrentPlayer == Player.Player2 && (GameMode)CbxMode.SelectedValue == GameMode.VsComputer)
                {
                    int nextIndex = DecideByComputer(MarkList);
                    if (nextIndex < 1 && nextIndex > 9)
                    {
                        AppendLogToUI("Computer cannot decide.");
                        return;
                    }
                    int nextIndexm1 = nextIndex - 1;
                    ProceedRound(nextIndexm1 % 3, nextIndexm1 / 3);
                }
            }
            catch { }
        }

        private void TlpGrid_MouseClick(object sender, MouseEventArgs e)
        {
            Point? p = GetRowColIndex(TlpGrid, e.X, e.Y);
            if (p == null) return;
            //AppendLogToUI("e.X, e.Y = {0}, {1}, p.X, p.Y = {2}, {3}", e.X, e.Y, p?.X, p?.Y);
            ProceedRound(p.Value.X, p.Value.Y);
        }

        private void KeyboardAction(Keys keyCode)
        {
            int index;
            if (keyCode >= Keys.NumPad1 && keyCode <= Keys.NumPad9) index = (int)keyCode - 96;
            else
            {
                if (keyCode >= Keys.D1 && keyCode <= Keys.D9) index = (int)keyCode - 48;
                else return;
            }
            //AppendLogToUI("Inside {0}", index);
            int indexm1 = index - 1;
            ProceedRound(indexm1 % 3, indexm1 / 3);
        }

        private void TxtLog_KeyUp(object sender, KeyEventArgs e)
        {
            KeyboardAction(e.KeyCode);
        }

        private void CbxMode_KeyUp(object sender, KeyEventArgs e)
        {
            KeyboardAction(e.KeyCode);
        }

        private void BtnRestartGame_KeyUp(object sender, KeyEventArgs e)
        {
            KeyboardAction(e.KeyCode);
        }

        private static int GetCurrentChance(List<int> list, int[] playerArray)
        {
            if (list.Contains(1) == false && (
                (playerArray.Contains(2) && playerArray.Contains(3))
                || (playerArray.Contains(4) && playerArray.Contains(7))
                || (playerArray.Contains(5) && playerArray.Contains(9))
                )) return 1;
            if (list.Contains(2) == false && (
                (playerArray.Contains(1) && playerArray.Contains(3))
                || (playerArray.Contains(5) && playerArray.Contains(8))
                )) return 2;
            if (list.Contains(3) == false && (
                (playerArray.Contains(1) && playerArray.Contains(2))
                || (playerArray.Contains(6) && playerArray.Contains(9))
                || (playerArray.Contains(7) && playerArray.Contains(5))
                )) return 3;
            if (list.Contains(4) == false && (
                (playerArray.Contains(5) && playerArray.Contains(6))
                || (playerArray.Contains(1) && playerArray.Contains(7))
                )) return 4;
            if (list.Contains(5) == false && (
                (playerArray.Contains(1) && playerArray.Contains(9))
                || (playerArray.Contains(3) && playerArray.Contains(7))
                || (playerArray.Contains(4) && playerArray.Contains(6))
                || (playerArray.Contains(2) && playerArray.Contains(8))
                )) return 5;
            if (list.Contains(6) == false && (
                (playerArray.Contains(4) && playerArray.Contains(5))
                || (playerArray.Contains(3) && playerArray.Contains(9))
                )) return 6;
            if (list.Contains(7) == false && (
                (playerArray.Contains(1) && playerArray.Contains(4))
                || (playerArray.Contains(8) && playerArray.Contains(9))
                || (playerArray.Contains(3) && playerArray.Contains(5))
                )) return 7;
            if (list.Contains(8) == false && (
                (playerArray.Contains(2) && playerArray.Contains(5))
                || (playerArray.Contains(7) && playerArray.Contains(9))
                )) return 8;
            if (list.Contains(9) == false && (
                (playerArray.Contains(3) && playerArray.Contains(6))
                || (playerArray.Contains(7) && playerArray.Contains(8))
                || (playerArray.Contains(1) && playerArray.Contains(5))
                )) return 9;
            return 0;
        }

        private static int DecideByComputer(List<int> list)
        {
            if (list.Count == 1) return list[0] == 5 ? 1 : 5;
            int[] p1Array = list.Where((x, i) => i % 2 == 0).ToArray();
            int[] p2Array = list.Where((x, i) => i % 2 == 1).ToArray();
            /// Block the chance of player one
            int chance = GetCurrentChance(list, p1Array);
            if (chance >= 1 && chance <= 9) return chance;
            /// Get the chance of player two
            chance = GetCurrentChance(list, p2Array);
            if (chance >= 1 && chance <= 9) return chance;
            if (!list.Contains(5)) return 5;
            if (!list.Contains(1)) return 1;
            if (!list.Contains(3)) return 3;
            if (!list.Contains(7)) return 7;
            if (!list.Contains(9)) return 9;
            if (!list.Contains(2)) return 2;
            if (!list.Contains(4)) return 4;
            if (!list.Contains(6)) return 6;
            if (!list.Contains(8)) return 8;
            return 0;
        }
    }
}
