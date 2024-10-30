using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pschreibMinesweeper
{
    public partial class Cell : UserControl
    {
        public event EventHandler CellClicked;
        int x, y;
        Random random = new Random();
        
        Button cellButton;
        Label bombCountLabel;
        Panel backPanel;
        Color cellColor;

        public int Col { get => x; set => x = value; }
        public int Row { get => y; set => y = value; }
        public bool Clicked { get; private set; }
        public Image BombImage { get; set; }
        public Color CellColor { get => backPanel.BackColor; set { backPanel.BackColor = value; cellColor = value; } }
       
        public Cell()
        {
            InitializeComponent();
            SetUpGameBoard();
            BombImage = Properties.Resources.BombImage;
            Clicked = false;
        }

        /// <summary>
        /// raises cell clicked event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnCellClicked(object sender, EventArgs e)
        {
            CellClicked?.Invoke(this, e);
        }

        /// <summary>
        /// sets up the gameboard
        /// </summary>
        private void SetUpGameBoard()
        {
            SetUpButtons();
            SetUpLabels();
            SetUpPanels();

            cellColor = backPanel.BackColor;
        }

        /// <summary>
        /// sets up the buttons
        /// </summary>
        private void SetUpButtons()
        {
            cellButton = new Button();
            cellButton.Size = new Size(this.Width, this.Height);
            cellButton.Click += OnButtonClick;
            this.Controls.Add(cellButton);
        }

        /// <summary>
        /// sets up the labels
        /// </summary>
        private void SetUpLabels()
        {
            bombCountLabel = new Label();
            bombCountLabel.Size = new Size(this.Width, this.Height);
            bombCountLabel.Visible = false; // Initially hide
            bombCountLabel.BackColor = Color.LightGray;
            bombCountLabel.TextAlign = ContentAlignment.MiddleCenter;
            bombCountLabel.Font = new Font(bombCountLabel.Font.FontFamily, 16, FontStyle.Bold);
            bombCountLabel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(bombCountLabel);
        }

        /// <summary>
        /// sets up the panels
        /// </summary>
        private void SetUpPanels()
        {
            backPanel = new Panel();
            backPanel.BackColor = Color.LightGray; 
            backPanel.Size = new Size(this.Width, this.Height);
            this.Controls.Add(backPanel);
        }

        /// <summary>
        /// displays the count of bombs in the permieter of each cell in the gameboard 
        /// </summary>
        /// <param name="count"> number of bombs in panel perimeter </param>
        public void DisplayBombCount(int count)
        {
            bombCountLabel.Visible = true;

            // if there is a value display number of bombs
            if (count >= 1)
            {
                bombCountLabel.Text = count.ToString();
                DecideColorOfText(count);
            }

            // if a bomb (red panel) make background a bomb image
            if (cellColor == Color.Red)
            {
                // resize bomb image to 36x36 pixels
                int newSize = 36;
                Image resizedImage = new Bitmap(BombImage, newSize, newSize);

                // set bomb image to the background image of the bombCountLabel
                bombCountLabel.Image = resizedImage;

                // clear text for labels with bombs
                bombCountLabel.Text = "";

                // make bomb background a tasteful and calming shade of red 
                bombCountLabel.BackColor = Color.LightCoral;
            }
        }

        /// <summary>
        /// decides color of text that is displayed depending on how 
        /// many bombs (red panels) are located in the panels perimeter
        /// </summary>
        /// <param name="count"> number of bombs in panel perimeter </param>
        private void DecideColorOfText(int count)
        {
            switch (count)
            {
                case 1:
                    bombCountLabel.ForeColor = Color.Blue;
                    break;
                case 2:
                    bombCountLabel.ForeColor = Color.Green;
                    break;
                case 3:
                    bombCountLabel.ForeColor = Color.Red;
                    break;
                default:
                    bombCountLabel.ForeColor = Color.Purple;
                    break;
            }
        }

        /// <summary>
        /// click button cells 
        /// </summary>
        public void PerformClick()
        {
            cellButton.PerformClick();
        }

        /// <summary>
        /// disable clicking of cells
        /// </summary>
        public void DisableClick()
        {
            cellButton.Click -= OnButtonClick;
        }

        /// <summary>
        /// event handler button clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnButtonClick(object sender, EventArgs e)
        {
            Thread.Sleep(5);
            ((Button)sender).Visible = false;
            OnCellClicked(this, e);
        }
    }
}
