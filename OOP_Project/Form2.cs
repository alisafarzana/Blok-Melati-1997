using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;

namespace OOP_Project
{
    public partial class Form2 : Form
    {
        private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();
        private System.Windows.Forms.Timer gameTimer;
        private List<PictureBox> Hide;
        private int pickupMsgTimer = 0;

        private bool isHiding = false;


        private Resizer resizer;


        Player player;
    
        GameManager game;
        Ghost enemy;
        Health heart;

        int damageCooldown = 0;
        int warningTimer = 150;
        Label lblWarning = new Label();
        Label lblStatus = new Label();
        bool isGameOver = false;

        public Form2()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // this.Resize += Form2_Resize;

        }

        private void Form2_Load(object sender, EventArgs e)
        {

            this.KeyPreview = true;


            player = new Player(charBox, 5);
            enemy = new Ghost(ghostBox, 2);
            heart = new Health(heart1, heart2, heart3);

            List<PictureBox> obstaclesList = new List<PictureBox>()
            {
                ampaian1, ampaian2,ampaian3, ampaian4, ampaian5
            };

            Hide = new List<PictureBox>()
            {
                bush1,bush2,bush3
            };

            game = new GameManager(obstaclesList);

            // -- Game timer (runs every 16ms ~ 60fps) ---
            gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();


            // create Resizer
            resizer = new Resizer(this, player, enemy);


           


            lblStatus.Text = "GAME OVER";
            lblStatus.Font = new Font("Arial", 28, FontStyle.Bold);
            lblStatus.ForeColor = Color.DarkRed;
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Location = new Point((this.ClientSize.Width / 2) - 100, (this.ClientSize.Height / 2) - 50);
            lblStatus.AutoSize = true;
            lblStatus.Visible = false;

            this.Controls.Add(lblStatus);



        }

        private void GameLoop(object sender, EventArgs e)
        {
            bool moved = false;

            if (heldKeys.Contains(Keys.Left) || heldKeys.Contains(Keys.A)) { player.Move("left", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Right) || heldKeys.Contains(Keys.D)) { player.Move("right", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Up) || heldKeys.Contains(Keys.W)) { player.Move("up", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Down) || heldKeys.Contains(Keys.S)) { player.Move("down", game.GetObstacles(), this.ClientSize); moved = true; }

            charBox.Refresh(); // Force redraw for smoother animation

            if (warningTimer > 0)
            {
                warningTimer--;
                if (warningTimer % 10 == 0) lblWarning.Visible = !lblWarning.Visible;
            }
            else
            {
                lblWarning.Visible = false;
            }


            //check damage

            if (damageCooldown > 0)
            {
                damageCooldown--;
            }

            if (!isHiding)
            {
                enemy.Chase(charBox, game.GetObstacles(), this.ClientSize);

                if (charBox.Visible && charBox.Bounds.IntersectsWith(ghostBox.Bounds) && damageCooldown == 0)
                {

                    isGameOver = heart.TakeDamage();
                    heart.UpdateHearts();

                    damageCooldown = 60;

                    if (isGameOver == true)
                    {
                        gameTimer.Stop();
                        lblStatus.Visible = true;
                        lblStatus.BringToFront();
                    }

                }
            }
            else
            {
                // Player hiding -- ghost roams randomly
                enemy.Roam(game.GetObstacles(), this.ClientSize);
            }


            //if (moved)
            //{
            //    string picked = game.CheckPickup(player);
            //    if (picked != null)
            //    {
            //        lblInventory.Text = "Inventory: " + string.Join(", ", player.inventory.ToString());
            //        lblPickup.Text = $"Picked up: {picked}!";
            //        pickupMsgTimer = 120;
            //    }
            //}

            //if (pickupMsgTimer > 0)
            //{
            //    pickupMsgTimer--;
            //    if (pickupMsgTimer == 0) lblPickup.Text = "";
            //}



        }



        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            heldKeys.Add(e.KeyCode);
        }

        private void Form2_KeyUp(object sender, KeyEventArgs e)
        {
            heldKeys.Remove(e.KeyCode);
        }

       
        protected override void OnKeyDown(KeyEventArgs e)
        {
            heldKeys.Add(e.KeyCode);
            if (e.KeyCode == Keys.E)
            {
                if (!isHiding && game.NearHide(player,Hide))
                {
                    isHiding = true;
                    player.Hide();
                }
                else if (isHiding)
                {
                    isHiding = false;
                    player.Show();
                }
            }
            base.OnKeyDown(e);
        }
     

        protected override void OnKeyUp(KeyEventArgs e)
        {
            heldKeys.Remove(e.KeyCode);
            base.OnKeyUp(e);
        }
    }
}


