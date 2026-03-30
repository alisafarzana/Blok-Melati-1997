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
        private TimedTask washTask;
        public override void Start(Form1 game)
        {
            washTask = new TimedTask(500, () =>
            {
                var held = game.Player.inventory.HeldItem;

                // ✅ Must be holding a valid unwashed clothing item
                if (held != null && IsWashable(held.Name))
                {
                    // ✅ Rename item after washing
                    held.Name = GetWashedName(held.Name);

                    game.LblInventory.Text = "Inventory: " + game.Player.inventory.ToString();
                    game.LblPickup.Text = "Item washed!";

                    // ✅ Check if ALL clothes are washed → complete task
                    bool allWashed = game.Player.inventory.items
                        .Where(i => IsClothing(i.Name))
                        .All(i => i.Name.StartsWith("Washed"));

                    if (allWashed)
                        IsCompleted = true;
                }
            });
        }

        public override void Update(Form1 game)
        {
            var held = game.Player.inventory.HeldItem;

            Rectangle playerArea = game.Player.CharacterBox.Bounds;
            playerArea.Inflate(10, 10);

            bool nearWash = game.Game.GetWashStations()
                .Any(w => playerArea.IntersectsWith(w.Bounds));


            if (game.HeldKeys.Contains(Keys.F) &&
                held != null &&
                IsWashable(held.Name) && // ✅ must be unwashed clothing
                nearWash)
            {
                if (!washTask.IsRunning)
                    washTask.Start();

                // ✅ SHOW BAR
                game.TaskBar.Visible = true;
                game.TaskBar.Value = washTask.Progress;

                game.LblPickup.Text = "Washing " + held.Name + "...";
            }
            else
            {
                washTask.Cancel();

                // ✅ HIDE BAR
                game.TaskBar.Visible = false;
            }
        }

        public override string GetTaskName()
        {
            return "Wash clothes";
        }

        private bool IsWashable(string name)
        {
            return name == "Shirt" || name == "Sock" || name == "Towel";
        }

        // ✅ Identify all clothing (washed or not)
        private bool IsClothing(string name)
        {
            return name.Contains("Shirt") || name.Contains("Sock") || name.Contains("Towel");
        }

        // ✅ Convert to washed version
        private string GetWashedName(string name)
        {
            switch (name)
            {
                case "Shirt": return "Washed Shirt";
                case "Sock": return "Washed Sock";
                case "Towel": return "Washed Towel";
                default: return name;
            }
        }
    }
}
