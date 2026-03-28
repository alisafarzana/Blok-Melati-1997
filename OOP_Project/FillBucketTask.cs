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
            fillTask = new TimedTask(4000, () =>
            {
                var held = game.Player.inventory.HeldItem;

                if (held != null && held.Name == "Bucket")
                {
                    held.Name = "Filled_Bucket";

                    game.LblInventory.Text = "Inventory: " + game.Player.inventory.ToString();
                    game.LblPickup.Text = "Bucket filled!";
                    IsCompleted = true;
                }
            });
        }

        public override void Update(Form1 game)
        {
            var held = game.Player.inventory.HeldItem;

            Rectangle playerArea = game.Player.CharacterBox.Bounds;
            playerArea.Inflate(10, 10);

            //near sink
            bool nearSink = game.Game.GetWaterStations()
            .Any(s => playerArea.IntersectsWith(s.Bounds));

            if (game.HeldKeys.Contains(Keys.F) &&
                 held != null &&
                 held.Name == "Bucket" &&
                 nearSink)
            {
                if (!fillTask.IsRunning)
                    fillTask.Start();

                game.LblPickup.Text = "Filling bucket...";
            }
            else
            {
                fillTask.Cancel();
            }
        }

        public override string GetTaskName()
        {
            return "Fill the bucket";
        }
    }
}
