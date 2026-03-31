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
        private Inventory inventory;
        public static bool Level1Completed = false;
        public static Inventory SavedInventory;

        public GameMenu(int level, Inventory inventory = null)
        {
            this.Level = level;
            this.inventory = inventory; // 🔥 store it
        }

        public void startGame(Form currentForm)
        {
            if (Level == 1)
            {
                
                Form1 form = new Form1();
                form.Show();
                currentForm.Hide();
            }

            else if (Level == 2)
            {
                if (!Level1Completed)
                {
                    MessageBox.Show("You must complete Level 1 first!");
                    return;
                }

                Form2 form2 = new Form2();
                form2.Show();
                currentForm.Hide();
            }


        }
        //public bool checkWin()
       
}
}
