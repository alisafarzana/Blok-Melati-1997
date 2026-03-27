using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class WashTask :GameTaskBase
    {
        public override void Update(Form1 game)
        {
            var held = game.Player.inventory.HeldItem;

            Rectangle playerArea = game.Player.CharacterBox.Bounds;
            playerArea.Inflate(10, 10);

            if (game.HeldKeys.Contains(Keys.F) &&
                held != null &&
                held.Name == "Filled_Bucket" &&
                playerArea.IntersectsWith(game.Game.WashBox.Bounds))
            {
                bool hasClothes = game.Player.inventory.items
                    .Any(i => i.Name == "Shirt" || i.Name == "Sock" || i.Name == "Towel");

                if (hasClothes)
                {
                    IsCompleted = true;
                    game.LblPickup.Text = "Clothes washed!";
                }
            }
        }

        public override string GetTaskName()
        {
            return "Wash clothes";
        }
    }
}
