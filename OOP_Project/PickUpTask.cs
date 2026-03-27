using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Project
{
    public class PickUpTask : GameTaskBase
    {
        public override void Update(Form1 game)
        {
            if (game.Game.AllItemsPicked())
            {
                IsCompleted = true;
                game.LblPickup.Text = "All items collected!";
            }
        }

        public override string GetTaskName()
        {
            return "Pick up all items";
        }
    }
}
