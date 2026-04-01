using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class FillBucketTask : GameTaskBase 
    {
        private TimedTask fillTask;

        public override void Start(Form1 game)
        {
            fillTask = new TimedTask(500, () =>
            {
                var held = game.Player.Inventory.HeldItem;

                if (held != null && held.Name == "Bucket")
                {
                    held.Name = "Filled_Bucket";

                    game.LblInventory.Text = "Inventory: " + game.Player.Inventory.ToString();
                    game.LblPickup.Text = "Bucket filled!";
                    IsCompleted = true;
                }
            });
        }

        public override void Update(Form1 game)
        {
            var held = game.Player.Inventory.HeldItem;

            Rectangle playerArea = game.Player.CharacterBox.Bounds;
            playerArea.Inflate(10, 10);//expand area of the player

            //near sink
            bool nearSink = game.Game.GetWaterStations() //from game manager
            .Any(s => playerArea.IntersectsWith(s.Bounds)); //check if player area intersects with any water station (sink)

            if (game.HeldKeys.Contains(Keys.F) &&
                 held != null &&
                 held.Name == "Bucket" &&
                 nearSink)
            {
                if (!fillTask.IsRunning) //start task if not running yet
                    fillTask.Start();

                // SHOW PROGRESS BAR
                game.TaskBar.Visible = true; //
                game.TaskBar.Value = fillTask.Progress;

                game.LblPickup.Text = "Filling bucket...";
            }
            else
            {
                fillTask.Cancel();
                //HIDE PROGRESS BAR
                game.TaskBar.Visible = false;
            }
        }

        public override string GetTaskName()
        {
            return "Fill the bucket";
        }
    }
}
