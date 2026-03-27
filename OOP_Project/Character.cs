


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public abstract class Character
    {
        public PictureBox CharacterBox;
        protected int speed;
        private int frameIndex = 0;
        private int animationCounter = 0;

        protected string direction;

        protected Dictionary<string, List<Image>> animations;



        public Character(PictureBox box, int speed)
        {
            this.CharacterBox = box;
            this.speed = speed;

        }

        public virtual void Move(string dir, List<PictureBox> obstacles, Size boundary)
        {
            direction = dir;
            switch (dir)
            {
                case "right":
                    CharacterBox.Left += speed;
                    if (CharacterBox.Right > boundary.Width)
                        CharacterBox.Left = boundary.Width - CharacterBox.Width;

                    foreach (var obj in obstacles)
                    {
                        if (CharacterBox.Bounds.IntersectsWith(obj.Bounds))
                        {
                            CharacterBox.Left -= speed;
                            return;
                        }
                    }
                    break;
                case "left":
                    CharacterBox.Left -= speed;
                    if (CharacterBox.Left < 0)
                        CharacterBox.Left = 0;

                    foreach (var obj in obstacles)
                    {
                        if (CharacterBox.Bounds.IntersectsWith(obj.Bounds))
                        {
                            CharacterBox.Left += speed; // cancel move
                            return;
                        }
                    }
                    break;
                case "up":
                    CharacterBox.Top -= speed;
                    if (CharacterBox.Top < 0)
                        CharacterBox.Top = 0;

                    foreach (var obj in obstacles)
                    {
                        if (CharacterBox.Bounds.IntersectsWith(obj.Bounds))
                        {
                            CharacterBox.Top += speed;
                            return;
                        }
                    }
                    break;
                case "down":
                    CharacterBox.Top += speed;
                    if (CharacterBox.Bottom > boundary.Height)
                        CharacterBox.Top = boundary.Height - CharacterBox.Height;

                    foreach (var obj in obstacles)
                    {
                        if (CharacterBox.Bounds.IntersectsWith(obj.Bounds))
                        {
                            CharacterBox.Top -= speed;
                            return;
                        }
                    }
                    break;

            }
            Animate();
        }

        protected void Animate(string dir = null)
        {
            if (dir != null) direction = dir; // override direction if given
            animationCounter++;
            if (animationCounter >= speed)
            {
                animationCounter = 0;
                frameIndex++;
                if (animations.ContainsKey(direction) && animations[direction].Count > 0)
                {
                    if (frameIndex >= animations[direction].Count) frameIndex = 0;
                    CharacterBox.Image = animations[direction][frameIndex];
                }
            }
        }


    }
}
