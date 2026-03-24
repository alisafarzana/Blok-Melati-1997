using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class Ghost : Character
    {

        private string currentDirection = "right";
        private int roamTimer = 0;
        private Random rnd = new Random();

        public Ghost(PictureBox box, int speed) : base(box, speed)
        {
            CharacterBox = box;
            this.speed = speed;

            animations = new Dictionary<string, List<Image>>()
            {
                { "right", new List<Image>()
                {
                    Properties.Resources.ghostRight
                }

                },
                { "left", new List<Image>()
                {
                    Properties.Resources.ghostLeft

                }
                },
                { "down", new List<Image>()
                {
                    Properties.Resources.ghostFront //for down
                 
                }
                },
                { "up", new List<Image>()
                {
                    Properties.Resources.ghostBack //for top
                 
                }
                }
            };
        }

        public override void Move(string dir, List<PictureBox> obstacles, Size boundary)
        {
            base.Move(dir, obstacles, boundary);
        }


        public void Chase(PictureBox target, List<PictureBox> obstacles, Size boundary)
        {

            if (target.Left < CharacterBox.Left)
                Move("left", obstacles, boundary);
            else if (target.Left > CharacterBox.Left)
                Move("right", obstacles, boundary);

            if (target.Top < CharacterBox.Top)
                Move("up", obstacles, boundary);
            else if (target.Top > CharacterBox.Top)
                Move("down", obstacles, boundary);

            //if (target.Left < this.GetBox().Left) Move("left",  obstacles, this.boundary);

        }
        public void Roam(List<PictureBox> obstacles, Size boundary)
        {
            roamTimer--;

            if (roamTimer <= 0)
            {
                string[] directions = { "left", "right", "up", "down" };
                currentDirection = directions[rnd.Next(directions.Length)];
                roamTimer = 60; // change direction every ~1 second
            }

            Move(currentDirection, obstacles, boundary);
        }

    }
}
