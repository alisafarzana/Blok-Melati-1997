using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class Ghost : Character //inherit from Character Class
    {

        private string currentDirection = "right";
        private int roamTimer = 0;
        private Random rnd = new Random();


        public enum GhostState { Waiting, Entering, Chasing, Roaming, Exiting }
        private GhostState state = GhostState.Waiting;
        public GhostState State
        {
            get { return state; }
            private set { state = value; }
        }
        
        private int stateTimer = 300; // waiting 5 sec
        private int roamDuration = 300;

        //ghost Sound
        private SoundPlayer ghostHidden;
        private SoundPlayer ghostSfx;


        //public GhostState GetState() => state;
 

        public Ghost(PictureBox box, int speed) : base(box, speed) //constructor
        {
           
            this.Speed = speed;
            CharacterBox.Visible = false; // start hidden
            ghostHidden = new SoundPlayer(Properties.Resources.ghost_Sound);
            ghostSfx = new SoundPlayer(Properties.Resources.ghost_Sound);


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

        public override void Move(string dir, List<PictureBox> obstacles, Size boundary) //override base class method
        {

            //ghost has different movement
            MoveWithCollision(dir, obstacles, boundary, ignoreBoundaries: false);
            Animate(dir);

        }

        //check the state of ghost and update accordingly
        public void Update(PictureBox player, List<PictureBox> obstacles, Size boundary, bool isPlayerHidden)
        {
            switch (state)
            {
                case GhostState.Waiting:
                    stateTimer--;
                    if (stateTimer <= 0)
                        StartEntering(boundary, obstacles);
                    break;

                case GhostState.Entering:
                    // move left ignoring boundaries so ghost walks in smoothly
                    // if ghost hits an obstacle, ignoreBoundaries will return false and reset to waiting
                    if (!MoveWithCollision("left", obstacles, boundary, ignoreBoundaries: true))
                    {
                        state = GhostState.Waiting;
                        CharacterBox.Visible = false;
                        stateTimer = 300;
                        return;
                    }
                    Animate("left");

                    if (CharacterBox.Right <= boundary.Width - 50)
                        state = GhostState.Chasing;
                    break;

                case GhostState.Chasing:
                    if (isPlayerHidden)
                    {
                        state = GhostState.Roaming;
                        roamDuration = 720;
                        return;
                    }
                    Chase(player, obstacles, boundary);
                    break;

                case GhostState.Roaming:
                    Roam(obstacles, boundary);

                    if (!isPlayerHidden)
                    {
                        state = GhostState.Chasing;
                        return;
                    }

                    roamDuration--;
                    if (roamDuration <= 0)
                        StartExiting();
                    break;

                case GhostState.Exiting:
                    // move right ignoring boundaries so ghost walks out
                    CharacterBox.Left += Speed;
                    Animate("right");

                    if (CharacterBox.Left > boundary.Width + CharacterBox.Width)
                    {
                        CharacterBox.Visible = false;
                        state = GhostState.Waiting;
                        stateTimer = 300;
                    }
                    break;
            }

            CharacterBox.BringToFront();
        }

        private void StartEntering(Size boundary, List<PictureBox> obstacles)// when the ghost enter
        {
            int spawnX = boundary.Width + 100; // spawn outside right
            int spawnY;

            bool validY = false;
            do
            {
                //PlayMusic("ghost_Sound.wav");
                spawnY = rnd.Next(50, boundary.Height - CharacterBox.Height);
                validY = true;
                foreach (var obs in obstacles)
                {
                    if (new Rectangle(spawnX, spawnY, CharacterBox.Width, CharacterBox.Height)
                        .IntersectsWith(obs.Bounds))
                    {
                        validY = false;
                        break;
                    }
                }
            } while (!validY);

            CharacterBox.Location = new Point(spawnX, spawnY);
            CharacterBox.Visible = true;
            state = GhostState.Entering;
            CharacterBox.BringToFront();

            
        }

        private void StartExiting()//when the ghost exit
        {
            State = GhostState.Exiting;
        }

        // Move in a direction while checking for collisions. If ignoreBoundaries is true, it won't stop at edges.
        private bool MoveWithCollision(string direction, List<PictureBox> obstacles, Size boundary, bool ignoreBoundaries = false)
        {
            Point newPos = CharacterBox.Location;
            switch (direction)
            {
                case "left": newPos.X -= Speed; break;
                case "right": newPos.X += Speed; break;
                case "up": newPos.Y -= Speed; break;
                case "down": newPos.Y += Speed; break;
            }

            Rectangle newBounds = new Rectangle(newPos, CharacterBox.Size);

            // Check collision with obstacles
            foreach (var obs in obstacles)
            {
                if (newBounds.IntersectsWith(obs.Bounds))
                    return false; // collision -> reset
            }

            if (!ignoreBoundaries)
            {
                if (newBounds.Left < 0) newBounds.X = 0;
                if (newBounds.Top < 0) newBounds.Y = 0;
                if (newBounds.Right > boundary.Width) newBounds.X = boundary.Width - CharacterBox.Width;
                if (newBounds.Bottom > boundary.Height) newBounds.Y = boundary.Height - CharacterBox.Height;
            }

            CharacterBox.Location = newBounds.Location;
            return true;
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

            //MoveWithCollision(currentDirection, obstacles, boundary);
            Move(currentDirection, obstacles, boundary);
            Animate(currentDirection);
        }

        public void PlayGhostMusic()
        {
            ghostSfx.PlayLooping(); // 🔁 repeat continuously
        }

        public void StopGhostMusic()
        {
            ghostSfx.Stop();
        }
        public void PlayHiddenSound()
        {
            ghostHidden.PlayLooping();
        }

        public void StopHiddenSound()
        {
            ghostHidden.Stop();
        }
    }
}
