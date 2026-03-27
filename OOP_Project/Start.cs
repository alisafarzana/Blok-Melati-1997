using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public partial class Start : Form
    {

        private Point level1 = new Point(400, 249);
        private Point level2 = new Point(400, 285);
        private Point Exit = new Point(400, 320);
        private int selectedLevel;
        public Start()
        {
            InitializeComponent();
        }

        private void Start_Load(object sender, EventArgs e)
        {
          
            selectedLevel = 1; //default to level 1
            PointArrowToLevel(selectedLevel);
           
        }

        private void PointArrowToLevel(int level)
        {
            int offsetX = -20;
            int offsetY = 0;

            //move arrow to the correct level

            if (level == 1)
            {
                arrow.Location = new Point(level1.X + offsetX, level1.Y + offsetY);
            }
            else if (level == 2)
            {
                arrow.Location = new Point(level2.X + offsetX, level2.Y + offsetY);
            }
            else if (level == 3) //exit
            {
                arrow.Location = new Point(Exit.X + offsetX, Exit.Y + offsetY);
            }

            arrow.Refresh();
        }

        private void Start_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && selectedLevel <= 3)
            {
                selectedLevel++;
                PointArrowToLevel(selectedLevel);
                


            }
            else if (e.KeyCode == Keys.Up && selectedLevel >= 1 )
            {
                selectedLevel--;
                PointArrowToLevel(selectedLevel);
                
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (selectedLevel == 1 || selectedLevel == 2)
                {
                    GameMenu menu = new GameMenu(selectedLevel);
                    menu.startGame(this);
                }
                else if (selectedLevel == 3) //exit
                {
                    Application.Exit();
                }
            }
           
        }
    }
}
