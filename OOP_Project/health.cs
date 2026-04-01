using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class Health
    {
        private int health= 3;
        private PictureBox heart1;
        private PictureBox heart2;
        private PictureBox heart3;

        public Health(PictureBox heart1, PictureBox heart2, PictureBox heart3)
        {
            this.heart1 = heart1;
            this.heart2 = heart2;
            this.heart3 = heart3;
        }

        public void UpdateHearts()
        {
            heart1.Visible = health >= 1;
            heart2.Visible = health >= 2;
            heart3.Visible = health >= 3;
        }

        // DAMAGE FUNCTION
        public bool TakeDamage()
        {
            if (health > 0)
            {
                health--;
                UpdateHearts();
               
            }
            if (health == 0)
            {

                return true;
                
            }
            return false;
        }
    }
}
