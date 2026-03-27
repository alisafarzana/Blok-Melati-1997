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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace OOP_Project
{
    public partial class Form1 : Form

    {

        private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();
        private System.Windows.Forms.Timer gameTimer;
        private List<PictureBox> Hide;
        private List<PictureBox> obstaclesList;

        private int pickupMsgTimer = 0;

        private bool isHiding = false;


        private Resizer resizer;

        private List<GameTaskBase> tasks = new List<GameTaskBase>();
        private int currentTaskIndex = 0;
        Player player;
        GameManager game;
        Ghost enemy;
        Health heart;

        //for FillBucketTask
        public Player Player => player;
        public GameManager Game => game;
        public HashSet<Keys> HeldKeys => heldKeys;
        public Label LblInventory => lblInventory;
        public Label LblPickup => lblPickup;


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

            //task
            if (currentTaskIndex < tasks.Count)
            {
                var currentTask = tasks[currentTaskIndex];
                currentTask.Update(this);
                if (!isGameOver) // <-- add this
                    lblStatus.Text = "Task: " + currentTask.GetTaskName();

                if (currentTask.IsCompleted)
                {
                    currentTaskIndex++;
                    if (currentTaskIndex < tasks.Count)
                    {
                        tasks[currentTaskIndex].Start(this);
                    }
                    else
                    {
                        if (!isGameOver)
                            lblStatus.Text = "ALL TASKS COMPLETED!";
                    }
                }

            }

            //Player Movement
            if (heldKeys.Contains(Keys.Left) || heldKeys.Contains(Keys.A)) { player.Move("left", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Right) || heldKeys.Contains(Keys.D)) { player.Move("right", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Up) || heldKeys.Contains(Keys.W)) { player.Move("up", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Down) || heldKeys.Contains(Keys.S)) { player.Move("down", game.GetObstacles(), this.ClientSize); moved = true; }

            characterBox.Refresh(); // Force redraw for smoother animation


            // UPDATE GHOST
            enemy.Update(characterBox, obstaclesList, this.ClientSize, isHiding);

            // SHOW "BE AWARE" when entering
            if (enemy.GetState() == Ghost.GhostState.Entering && warningTimer == 0)
            {
                lblWarning.Visible = true;
                warningTimer = 120; // 2 seconds
            }

            if (enemy.GetState() == Ghost.GhostState.Waiting)
            {
                enemy.Update(characterBox, game.GetObstacles(), this.ClientSize, isHiding);
            }
            // HANDLE WARNING TIMER
            if (warningTimer > 0)
            {
                warningTimer--;
                if (warningTimer == 0)
                    lblWarning.Visible = false;
            }


            // COLLISION
            if (!isHiding &&
                characterBox.Visible &&
                characterBox.Bounds.IntersectsWith(enemy.CharacterBox.Bounds) &&
                damageCooldown == 0)
            {
                isGameOver = heart.TakeDamage();
                damageCooldown = 300;

                if (isGameOver)
                {
                    gameTimer.Stop();
                    lblStatus.Text = "GAME OVER";
                    lblStatus.Visible = true;
                    lblStatus.BringToFront();
                }
            }

            if (damageCooldown > 0)
                damageCooldown--;

            // ITEM PICKUP
            if (moved)
            {
                string picked = game.CheckPickup(player);
                if (picked != null)
                {
                    lblInventory.Text = "Inventory: " + player.inventory.ToString();
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

            obstaclesList = new List<PictureBox>()
            {
                leftToiletBox,
                rightToiletBox,
                sinkBox,
                washBox,
            };

            Hide = new List<PictureBox>()
            {
                leftToiletBox,rightToiletBox
            };

            game = new GameManager(obstaclesList);


            game.AddItem(new Item("Shirt", shirtBox));
            game.AddItem(new Item("Sock", sockBox));
            game.AddItem(new Item("Sock", sock2Box));
            game.AddItem(new Item("Towel", towelBox));
            game.AddItem(new Item("Shirt", shirt2Box));
            game.AddItem(new Item("Bucket", bucketBox));



            // -- Game timer (runs every 16ms ~ 60fps) ---
            gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
            gameTimer.Tick += GameLoop;
            gameTimer.Start();


            // create Resizer
            resizer = new Resizer(this, player, enemy);


            //label be aware and gameover
            lblWarning = new Label();
            lblWarning.Text = "BE AWARE!";
            lblWarning.Font = new Font("Arial", 24, FontStyle.Bold);
            lblWarning.ForeColor = Color.Red;
            lblWarning.BackColor = Color.Transparent;
            lblWarning.AutoSize = true;
            lblWarning.Visible = false; // 👈 ADD THIS new

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


            lblInventory.Text = "Inventory: " + player.inventory.ToString();

            tasks.Add(new PickUpTask());
            tasks.Add(new FillBucketTask());
            tasks.Add(new WashTask());

            tasks[currentTaskIndex].Start(this);

        }



        protected override void OnKeyDown(KeyEventArgs e)
        {
            heldKeys.Add(e.KeyCode);

            // Hotbar selection
            if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D6)
            {
                int index = e.KeyCode - Keys.D1;
                player.inventory.SelectHotbar(index);

                // Update label immediately
                lblInventory.Text = "Inventory: " + player.inventory.ToString();
            }

            //other key logic
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
                    Rectangle toiletBounds = game.GetNearestToilet(player);
                    int newX = toiletBounds.Left + (toiletBounds.Width - player.CharacterBox.Width) / 2;
                    int newY = toiletBounds.Bottom + 5;
                    player.CharacterBox.Location = new Point(newX, newY);
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
