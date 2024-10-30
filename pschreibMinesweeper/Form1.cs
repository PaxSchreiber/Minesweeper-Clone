using System;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

/*
 * Programmer: Paxton Schreiber
 * Description: This program is a minesweerp clone using
 * windows forms that allows the user to play minesweeper.
 * Features of the program include an in-game timer,
 * lifetime statistics, instructions, and a restart game
 * button.
 */

namespace pschreibMinesweeper
{
    public partial class Form1 : Form
    {
        Random random = new Random();
        Timer timer = new Timer();

        // cell formatting
        int verticalOffset = 35;
        int horizontalOffset = 20;

        // stat trackers
        int elapsedTimeInSeconds = 0;
        int totalGamesPlayed = 0;
        int totalWins = 0;
        int totalTiles = 0;
        int totalLosses = 0;
        List<int> gameTimes = new List<int>();

        // gameboard dimensions
        Cell[,] grid = new Cell[10, 10];       

        public Form1()
        {
            InitializeComponent();
            InitializeGrid();
            PlaceBombs();
            TimerBegin();

            Cell aCell = new Cell();
            aCell.Location = new Point(150, 150);
            this.Controls.Add(aCell);
        }

        /// <summary>
        /// initialize the grid of cells (gameboard)
        /// </summary>
        public void InitializeGrid()
        {
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    Cell cell = new Cell();
                    cell.CellClicked += OnCellClicked;
                    cell.Col = col;
                    cell.Row = row;
                    cell.Location = new Point(horizontalOffset + (col * cell.Width), verticalOffset + (row * cell.Height));
                    this.Controls.Add(cell);
                    grid[col, row] = cell;
                }
            }
        }

        /// <summary>
        /// randomly place bombs onto the gameboard 
        /// </summary>
        public void PlaceBombs()
        {
            int Bombs = 0;

            while (Bombs < 8)
            {
                // generate random x : y coords
                int x = random.Next(grid.GetLength(0));
                int y = random.Next(grid.GetLength(1));

                // check if random position is not a bomb already
                if (grid[x, y].CellColor == Color.LightGray)
                {
                    // turn the cell color red to represent a bomb
                    grid[x, y].CellColor = Color.Red;
                    Bombs++;
                }
            }
        }

        /// <summary>
        /// event handler for cell clicked event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnCellClicked(object sender, EventArgs e)
        {
            // start the timer
            timer.Start();

            Cell cell = (Cell)sender;

            int row = cell.Row;
            int col = cell.Col;
            CheckAdjacentCells(cell, row, col);

            // track how many bombs are in the perimeter
            int bombCount = CalculateBombCount(cell, row, col);
            cell.DisplayBombCount(bombCount);

            // increment total tiles 
            totalTiles++;

            // check if user has won or lost
            GameStatus(cell);
        }

        /// <summary>
        /// checks if user has won or lost 
        /// </summary>
        /// <param name="cell"></param>
        private void GameStatus(Cell cell)
        {
            // check if cell is red (bomb)
            if (cell.CellColor == Color.Red)
            {
                GameOver();
                timer.Stop(); 
            }

            // check if user has won
            if (totalTiles == 92)
            {
                MessageBox.Show("You win!");
                totalTiles = 0;
                totalWins++;
                timer.Stop();
            }
        }

        /// <summary>
        /// calculates the # of bombs in the perimeter of a cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns> count of bombs </returns>
        private int CalculateBombCount(Cell cell, int row, int col)
        {
            int bombCount = 0;

            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    // check range
                    if (i >= 0 && i < grid.GetLength(1) && j >= 0 && j < grid.GetLength(0))
                    {
                        // if bomb (red) increment counter for bombs
                        if (grid[j, i].CellColor == Color.Red)
                        {
                            bombCount++;
                        }
                    }
                }
            }
            return bombCount;
        }


        /// <summary>
        /// handle when user loses game of minesweeper
        /// </summary>
        private void GameOver()
        {
            // increment losses
            totalLosses++;

            MessageBox.Show("Game Over");

            // disable clicking cells
            foreach (Cell gridCell in grid)
            {
                gridCell.DisableClick();
            }
        }
        

        /// <summary>
        /// allows user to start a new game of minesweeper
        /// </summary>
        private void RestartGame()
        {
            // increment counters 
            totalGamesPlayed++;
            timer.Stop();

            // reset trackers
            elapsedTimeInSeconds = 0;
            totalTiles = 0;

            // reset status strip
            UpdateStatusStrip();

            // clear board
            this.Controls.Clear();

            // add back menustrip and status strip
            this.Controls.Add(menuStrip1);
            this.Controls.Add(ToolStripStatusLabel);

            // initialize a new grid
            InitializeGrid();

            // place new bombs on the grid
            PlaceBombs();
        }

        /// <summary>
        /// checks the perimeter of cells
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckAdjacentCells(Cell cell, int row, int col)
        {
            int bombCount = CalculateBombCount(cell, row, col);

            if (bombCount > 0)
            {
                cell.DisplayBombCount(bombCount);
                return; 
            }

            // check all of the permiter 
            CheckRight(cell, row, col);
            CheckLeft(cell, row, col);
            CheckDown(cell, row, col);
            CheckUp(cell, row, col);
            CheckCorners(cell, row, col);
        }

        /// <summary>
        /// checks left of the cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckLeft(Cell cell, int row, int col)
        {
            // check left
            if (col > 1)
            {
                if (cell.CellColor == grid[col - 1, row].CellColor)
                {
                    grid[col - 1, row].PerformClick();
                }
            }
        }

        /// <summary>
        /// checks right of the cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckRight(Cell cell, int row, int col)
        {
            if (col < grid.GetLength(0) - 1)
            {
                if (cell.CellColor == grid[col + 1, row].CellColor)
                {
                    grid[col + 1, row].PerformClick();
                }
            }
        }

        /// <summary>
        /// checks above the cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckUp(Cell cell, int row, int col)
        {
            // check up
            if (row > 1)
            {
                if (cell.CellColor == grid[col, row - 1].CellColor)
                {
                    grid[col, row - 1].PerformClick();
                }
            }
        }

        /// <summary>
        /// checks below the cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckDown(Cell cell, int row, int col)
        {
            // check down
            if (row < grid.GetLength(1) - 1)
            {
                if (cell.CellColor == grid[col, row + 1].CellColor)
                {
                    grid[col, row + 1].PerformClick();
                }
            }
        }

        /// <summary>
        /// checks diagonal 
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckCorners(Cell cell, int row, int col)
        {
            UpperDiagonals(cell, row, col);
            LowerDiagonals(cell, row, col);
        }

        /// <summary>
        /// checks upper diagonals
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void UpperDiagonals(Cell cell, int row, int col)
        {
            // check upper-left diagonal
            if (col > 0 && row > 0)
            {
                if (cell.CellColor == grid[col - 1, row - 1].CellColor)
                {
                    grid[col - 1, row - 1].PerformClick();
                }
            }

            // check upper-right diagonal
            if (col < grid.GetLength(0) - 1 && row > 0)
            {
                if (cell.CellColor == grid[col + 1, row - 1].CellColor)
                {
                    grid[col + 1, row - 1].PerformClick();
                }
            }
        }

        /// <summary>
        /// checks lower diagonals
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void LowerDiagonals(Cell cell, int row, int col)
        {
            // check lower-left diagonal
            if (col > 0 && row < grid.GetLength(1) - 1)
            {
                if (cell.CellColor == grid[col - 1, row + 1].CellColor)
                {
                    grid[col - 1, row + 1].PerformClick();
                }
            }

            // check lower-right diagonal
            if (col < grid.GetLength(0) - 1 && row < grid.GetLength(1) - 1)
            {
                if (cell.CellColor == grid[col + 1, row + 1].CellColor)
                {
                    grid[col + 1, row + 1].PerformClick();
                }
            }
        }

        /// <summary>
        /// intilaize the timer
        /// </summary>
        private void TimerBegin()
        {
            // update timer every 1 second
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// event handler for timer 
        /// </summary>
        private void Timer_Tick(object sender, EventArgs timer)
        {
            // increment time tracker 
            elapsedTimeInSeconds++;
            UpdateStatusStrip();

            // add time to a list
            gameTimes.Add(elapsedTimeInSeconds);
        }

        /// <summary>
        /// update status strip with current timer
        /// </summary>
        private void UpdateStatusStrip()
        {
            TimerStrip.Text = $"Timer:  {TimeSpan.FromSeconds(elapsedTimeInSeconds).ToString("mm\\:ss")}";
        }

        /// <summary>
        /// handler for quit menu strip option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string title = "Minesweeper";
            string message = "Do you want to exit Minesweeper?";

            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);

            if (result == DialogResult.Yes)
            {
                this.Close();
                Application.Exit();
            }
        }

        /// <summary>
        /// handler for instructions menu strip option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = ("Instructions");
            string message = ("1.Identify the location of the mines in the field by strategically selecting blocks to uncover what each block contains." +
                "\n\n2.Choose a block that does not contain a mine and the number of mines adjacent to the selected block will appear in the selected location. " +
                "This includes any block above, below, to the side, or diagonal from the selected block." +
                "\n\n3.Use the given numbers to strategically select all of the blocks in the field that do not contain a mine." +
                "\n\n4.Select all of the blocks that do not contain mines and you win!");

            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, title, buttons, MessageBoxIcon.Information);
        }

        /// <summary>
        /// handler for about menu stip option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string aboutTitle = ("Aboot");
            string message = ("\nProgrammer: Paxton Schreiber\nClass: CS 3020");
            MessageBoxButtons buttons = MessageBoxButtons.OK;

            MessageBox.Show(message, aboutTitle, buttons);
        }

        /// <summary>
        /// handler for new game menu strip option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string title = "Minesweeper";
            string message = "Do you want start a new game?";

            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);

            if (result == DialogResult.Yes)
            {
                RestartGame();
            }
        }

        /// <summary>
        /// handler for stats menu strip option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (totalGamesPlayed > 0)
            {
                string winRatio = $"{totalWins}/{totalLosses}";
                double averageTime = gameTimes.Count > 0 ? gameTimes.Average() : 0;

                string message = $"Total Games Played: {totalGamesPlayed}\nW/L: {winRatio}\n";
                message += $"Average Time: {TimeSpan.FromSeconds(averageTime).ToString("mm\\:ss")}";

                MessageBox.Show(message, "Stats", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Finish a game of Minesweeper first!", "Stats", MessageBoxButtons.OK);
            }
        }
    }
}
