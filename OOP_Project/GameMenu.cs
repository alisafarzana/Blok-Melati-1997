using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class GameMenu
    {
        
        private int Level;
        private string storyText;

        public GameMenu(int level)
        {
            this.Level = level;
        }

        public void startGame(Form currentForm)
        {
            if (Level == 1)
            {
                storyText = "School Rule #7:\r\nComplete every task given by a senior.\r\n\r\nSchool Rule #8:\r\nIf something follows you after midnight…\r\n\r\nDon’t let it catch you.";
                Form1 form = new Form1();
                form.Show();
                currentForm.Hide();
            }

            else if (Level == 2)
            {
                storyText = "School Rule #9:\r\nDon’t let the ghost catch you.\r\n\r\nSchool Rule #10:\r\nIf you see a ghost, run.";
                Form2 form = new Form2();
                form.Show();
                currentForm.Hide();
            }


        }
        //public bool checkWin()
       
}
}
