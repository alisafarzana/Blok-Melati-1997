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
        private Level currentLevel;
        // Obstacles like walls or static things
        private List<PictureBox> obstacles;

    

        private List<Item> worldItems = new List<Item>();

        public GameManager()
        {

        }
        public List<PictureBox> GetWashStations()
        {
            return currentLevel.WashStations;
        }

        public List<PictureBox> GetWaterStations()
        {
            return currentLevel.WaterStation;
        }

        public void LoadLevel(Level level)
        {
            currentLevel = level;
            obstacles = level.Obstacles;

            worldItems.Clear();
            worldItems.AddRange(level.Items);
        }

        

        public List<PictureBox> GetObstacles()
        {
            return currentLevel.Obstacles;
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

        public bool NearHide(Player player)
        {
            Rectangle playerBounds = player.Bounds;
            playerBounds.Inflate(5, 5);

            foreach (PictureBox hide in currentLevel.HideSpots)
            {
                if (playerBounds.IntersectsWith(hide.Bounds))
                    return true;
            }
          
            return false;
        }

        public Rectangle GetNearestHideSpot(Player player)
        {
            //for unhide
            // Compare distance to left and right toilet

            if(currentLevel.HideSpots == null|| currentLevel.HideSpots.Count  == 0)
            {
                return Rectangle.Empty; // Not enough hide spots
            }
            PictureBox nearest = null;
            int minDistance = int.MaxValue;

            foreach(var hide in currentLevel.HideSpots)
            {
                int distance = Math.Abs(player.CharacterBox.Left - hide.Left);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = hide;
                }


            }

            return nearest.Bounds;
            //int distLeft = Math.Abs(player.CharacterBox.Left - leftToiletBox.Left);
            //int distRight = Math.Abs(player.CharacterBox.Left - rightToiletBox.Left);

            //return distLeft < distRight ? leftToiletBox.Bounds : rightToiletBox.Bounds;
        }

        public bool AllItemsPicked()
        {
            return worldItems.All(item => item.IsPickedUp);
        }

        
    }
}

