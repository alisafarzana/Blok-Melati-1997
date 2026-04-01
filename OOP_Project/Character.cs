


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
        //declare private fields
        private PictureBox characterBox;
        private int speed;
        private int frameIndex = 0;
        private int animationCounter = 0;

        //declare protected fields to allow access in derived classes
        protected string direction;
        protected Dictionary<string, List<Image>> animations;

        //public properties for encapsulation
        public PictureBox CharacterBox { get { return characterBox ; } set { characterBox = value; } }
        public int Speed { get { return speed; } set { speed = value; } }
        public Point Position => CharacterBox.Location;
        public Rectangle Bounds => CharacterBox.Bounds;

        //Constructor to initialize character with a PictureBox and speed
        public Character(PictureBox box, int speed)
        {
            this.CharacterBox = box;
            this.Speed = speed;

        }

        //Virtual Move method that can be overridden by derived classes for specific movement behavior
        public virtual void Move(string dir, List<PictureBox> obstacles, Size boundary)
        {
            direction = dir; // direction right, left, up, down
            switch (dir)
            {
                case "right":
                    CharacterBox.Left += speed;
                    if (CharacterBox.Right > boundary.Width) //avoid moving out of bounds
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


        //animation method to cycle through frames based on direction and speed. Can be called after every move to update the character's image.
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
