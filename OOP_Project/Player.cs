using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class Player : Character
    {
    

 

        public Player(PictureBox box, int speed) : base(box, speed)
        {
            CharacterBox = box;
            this.speed = speed;
            animations = new Dictionary<string, List<Image>>()
            {
                { "right", new List<Image>()
                {
                    Properties.Resources.R1,
                    Properties.Resources.R2,
                    Properties.Resources.R3
                }

                },
                { "left", new List<Image>()
                {
                    Properties.Resources.L1,
                    Properties.Resources.L2,
                    Properties.Resources.L3
                }
                },
                { "up", new List<Image>()
                {
                    Properties.Resources.T1,
                    Properties.Resources.T2,
                    Properties.Resources.T3
                }
                },
                { "down", new List<Image>()
                {
                    Properties.Resources.D1,
                    Properties.Resources.D2,
                    Properties.Resources.D3
                }
                }
            };
        }

        public Inventory inventory { get; private set; } = new Inventory();

        /// Call after every move. Returns item name if picked up, else null.

    
        public Rectangle Bounds => CharacterBox.Bounds;
        public void SetVisible(bool visible)
        {
            CharacterBox.Visible = visible;
        }

        public void Hide()
        {
            CharacterBox.Visible = false;
        }

        public void Show()
        {
            CharacterBox.Visible = true;
        }
    }
}
