using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class GameManager
    {
        private List<PictureBox> obstacles = new List<PictureBox>();
        private List<Item> worldItems = new List<Item>();
        private PictureBox leftToiletBox;
        private PictureBox rightToiletBox;
        private PictureBox sinkBox;
        private PictureBox washBox;

    

        public GameManager(PictureBox leftToilet, PictureBox rightToilet, PictureBox sink, PictureBox wash)
        {
            leftToiletBox = leftToilet;
            rightToiletBox = rightToilet;
            sinkBox = sink;
            washBox = wash;
            worldItems = new List<Item>();

            //Toilet and sink collision
            obstacles.Add(leftToiletBox);
            obstacles.Add(rightToiletBox);
            obstacles.Add(sinkBox);
            obstacles.Add(washBox);

        }

        public void AddItem(Item item)
        {
            worldItems.Add(item);
        }

        public List<PictureBox> GetObstacles()
        {
            return obstacles;
        }

        public string CheckPickup(Player player)
        {
            foreach (var item in worldItems)
            {
                if (item.IsPickedUp) continue;

                if (player.Bounds.IntersectsWith(item.ItemBox.Bounds))
                {
                    item.PickUp();
                    player.inventory.AddItem(item.Name);
                    return item.Name;
                }
            }
            return null;
        }

        public bool NearToilet (Player player)
        {
            Rectangle playerBounds = player.Bounds;
            playerBounds.Inflate(5, 5);

            if (playerBounds.IntersectsWith(leftToiletBox.Bounds))
                return true;

            if (playerBounds.IntersectsWith(rightToiletBox.Bounds))
                return true;

            return false;
        }


    }
}
