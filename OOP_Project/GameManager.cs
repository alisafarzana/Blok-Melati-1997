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
        private List<PictureBox> obstacles;
        private List<Item> worldItems = new List<Item>();
        private PictureBox leftToiletBox, rightToiletBox,sinkBox,washBox, bush1, bush2, bush3;

        public PictureBox SinkBox => sinkBox;
        public PictureBox WashBox => washBox;

        public GameManager(List<PictureBox> obstacle)
        {
            obstacles = obstacle;
            leftToiletBox = obstacle[0];
            rightToiletBox = obstacle[1];
            sinkBox = obstacle[2];
            washBox = obstacle[3];



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
                    player.inventory.AddItem(item);
                    return item.Name;
                }
            }
            return null;
        }

        public bool NearHide(Player player, List<PictureBox> Hide)
        {
            Rectangle playerBounds = player.Bounds;
            playerBounds.Inflate(5, 5);

            foreach (PictureBox hide in Hide)
            {
                if (playerBounds.IntersectsWith(hide.Bounds))
                    return true;
            }
          
            return false;
        }

        public Rectangle GetNearestToilet(Player player)
        {
            // Compare distance to left and right toilet
            int distLeft = Math.Abs(player.CharacterBox.Left - leftToiletBox.Left);
            int distRight = Math.Abs(player.CharacterBox.Left - rightToiletBox.Left);

            return distLeft < distRight ? leftToiletBox.Bounds : rightToiletBox.Bounds;
        }

        public bool AllItemsPicked()
        {
            return worldItems.All(item => item.IsPickedUp);
        }

        
    }
}

