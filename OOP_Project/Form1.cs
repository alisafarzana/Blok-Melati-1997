using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;

namespace OOP_Project
{
    public partial class Form1 : Form

    {
        
        private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();
        private System.Windows.Forms.Timer gameTimer;

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


        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Resize += Form1_Resize;


        }


        private void keyIsDown(object sender, KeyEventArgs e)
        {

            heldKeys.Add(e.KeyCode);

        }

        private void GameLoop(object sender, EventArgs e)
        {
            bool moved = false;

            if (heldKeys.Contains(Keys.Left) || heldKeys.Contains(Keys.A)) { player.Move("left", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Right) || heldKeys.Contains(Keys.D)) { player.Move("right", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Up) || heldKeys.Contains(Keys.W)) { player.Move("up", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Down) || heldKeys.Contains(Keys.S)) { player.Move("down", game.GetObstacles(), this.ClientSize); moved = true; }

            characterBox.Refresh(); // Force redraw for smoother animation

            
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

            if(damageCooldown > 0)
            {
                damageCooldown--;
            }

            if (!isHiding)
            {
                enemy.Chase(characterBox, game.GetObstacles(), basebox.ClientSize);

                if (characterBox.Visible && characterBox.Bounds.IntersectsWith(ghostPic.Bounds) && damageCooldown == 0)
                {

                    isGameOver = heart.TakeDamage();
                    heart.UpdateHearts();

                    damageCooldown = 300;

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
                enemy.Roam(game.GetObstacles(), basebox.ClientSize);
            }


            if (moved)
            {
                string picked = game.CheckPickup(player);
                if (picked != null)
                {
                    lblInventory.Text = "Inventory: " + string.Join(", ", player.inventory.ToString());
                    lblPickup.Text = $"Picked up: {picked}!";
                    pickupMsgTimer = 120;
                }
            }

            if (pickupMsgTimer > 0)
            {
                pickupMsgTimer--;
                if (pickupMsgTimer == 0) lblPickup.Text = "";
            }
     

        }

        private void keyIsUp(object sender, KeyEventArgs e)
        {
            heldKeys.Remove(e.KeyCode);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player = new Player(characterBox, 4);
            enemy = new Ghost(ghostPic, 1);
            heart = new Health(heart1, heart2, heart3);

            game = new GameManager(leftToiletBox, rightToiletBox, sinkBox, washBox);

            
            game.AddItem(new Item("Shirt", shirtBox));
            game.AddItem(new Item("Sock", sockBox));
            game.AddItem(new Item("Towel", towelBox));
            game.AddItem(new Item("Shirt", shirt2Box));
            game.AddItem(new Item("Bucket", bucketBox));

            //basebox.Parent = this;
            //leftToiletBox.Parent = basebox;
            //rightToiletBox.Parent = basebox;
            //sinkBox.Parent = basebox;
            //washBox.Parent = basebox;
            //characterBox.Parent = basebox;

            // -- Game timer (runs every 16ms ~ 60fps) ---
            gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

     
            // create Resizer
            resizer = new Resizer(this, player,enemy);


            //label be aware and gameover
            lblWarning = new Label();
            lblWarning.Text = "BE AWARE!";
            lblWarning.Font = new Font("Arial", 24, FontStyle.Bold);
            lblWarning.ForeColor = Color.Red;
            lblWarning.BackColor = Color.Transparent;
            lblWarning.AutoSize = true;

            this.Controls.Add(lblWarning);

            lblWarning.Left = (this.ClientSize.Width / 2) - (lblWarning.Width / 2);

            lblWarning.Top = (this.ClientSize.Height / 2) - (lblWarning.Height / 2);

            lblWarning.BringToFront();

            lblStatus.Text = "GAME OVER";
            lblStatus.Font = new Font("Arial", 28, FontStyle.Bold);
            lblStatus.ForeColor = Color.DarkRed;
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Location = new Point((this.ClientSize.Width / 2) - 100, (this.ClientSize.Height / 2) - 50);
            lblStatus.AutoSize = true;
            lblStatus.Visible = false;

            this.Controls.Add(lblStatus);


        }
        


        protected override void OnKeyDown(KeyEventArgs e)
        {
            heldKeys.Add(e.KeyCode);
            if (e.KeyCode == Keys.E)
            {
                if (!isHiding && game.NearToilet(player))
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

        //to resize when enlarge
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
                
                resizer.Resize();


        }

       
    }
}
