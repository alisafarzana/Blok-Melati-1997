using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class Item
    {
        public string Name { get; set; }
        public PictureBox ItemBox { get; private set; }
        public bool IsPickedUp { get; private set; }

        public Item(string name, PictureBox box)
        {
            Name = name;
            ItemBox = box;
            IsPickedUp = false;
        }

        public void PickUp()
        {
            IsPickedUp = true;
            ItemBox.Visible = false;
        }
    }
}
