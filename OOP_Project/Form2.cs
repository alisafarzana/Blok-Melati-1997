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

        private bool isHiding = false;

        private Resizer resizer;

        //storyline
        private List<string> storyLine = new List<string>();
        private int currentLineIndex = 0;
        //Typewriter effect
        private Timer typewriterTimer = new Timer();
        private string currentText = "";
        private int charIndex = 0;
        private bool isTyping = false; // prevent skipping during typing


        //private List<GameTaskBase> tasks = new List<GameTaskBase>();
        //private int currentTaskIndex = 0;
        Player player;
        GameManager game;
        Ghost enemy;
        Health heart;
        PictureBox hangSock1;
        PictureBox hangSock2;
        PictureBox hangCloth1;
        PictureBox hangCloth2;
        PictureBox hangTowel;


        public Label LblInventory => lblInventory;
        //public Label LblPickup => lblPickup;
        public Label LblInstruction => lblInstruction;


        int damageCooldown = 0;
        int warningTimer = 150;
        Label lblWarning = new Label();
        Label lblStatus = new Label();
        Label lblInstruction = new Label();
        bool isGameOver = false;

        public Form2()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

           this.Resize += Form2_Resize;

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            lblInventory.Width = 533;
            lblInventory.Height = 35;
            lblInventory.AutoSize = false;

            player = new Player(charBox, 4);
            enemy = new Ghost(ghostBox, 2);
            heart = new Health(heart1, heart2, heart3);

            Level level1 = new Level(new List<PictureBox>()
             {
                ampaian1,ampaian2,ampaian3,ampaian4,ampaian5
             },
             new List<PictureBox>() // hide spots
             {
                bush1, bush2, bush3
             },
             new List<Item>()//items
             {
                new Item("Sock", hangSock1),
                new Item("Sock", hangSock2),
                new Item("Shirt", hangCloth1),
                new Item("Shirt", hangCloth2),
                new Item("Towel", hangTowel)
                
             }

            )
            {

            };
            game = new GameManager();
            game.LoadLevel(level1);



            lblInstruction = new Label();
            lblInstruction.Text = "CONTROLS: \n Move : WASD or Arrow Keys \n Hide/Unhide : E (when near a hiding spot) \n Perform Tasks : F \n Select Inventory Items : Number Keys";
            lblInstruction.BackColor = Color.FromArgb(200, 0, 0, 0); // semi-transparent black
            lblInstruction.ForeColor = Color.White;
            lblInstruction.Font = new Font("Arial", 12, FontStyle.Regular);
            lblInstruction.AutoSize = false; // allow manual size
            lblInstruction.Size = new Size(1000, 150); // adjust as needed
            lblInstruction.TextAlign = ContentAlignment.MiddleCenter;

            lblInstruction.Location = new Point((this.ClientSize.Width - lblInstruction.Width) / 2,
    (this.ClientSize.Height - lblInstruction.Height) / 2);
            lblInstruction.Visible = true;


            this.Controls.Add(lblInstruction);
            lblInstruction.BringToFront();

            //storyline
            storyLine = new List<string>()
            {
                "Yana quickly grabs the wet clothes, heart racing.",
                "Suddenly, a shadow moves... the ghost is here!",
                "She must hang the clothes and stay quiet, or she’ll be caught.",
                "Move fast with WASD/Arrow Keys, hide with E, and complete the task with F.",
                "Every creak echoes. The ghost could appear at any moment...",
                "and maybe...faster"
            };



            // Display the first line
            lblInstruction.Text = storyLine[currentLineIndex];

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


            //lblInventory.Text = "Inventory: " + player.inventory.ToString();
            UpdateInventoryLabel();

            




        }

        private void GameLoop(object sender, EventArgs e)
        {
            bool moved = false;

            //task


            //player movement
            if (heldKeys.Contains(Keys.Left) || heldKeys.Contains(Keys.A)) { player.Move("left", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Right) || heldKeys.Contains(Keys.D)) { player.Move("right", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Up) || heldKeys.Contains(Keys.W)) { player.Move("up", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Down) || heldKeys.Contains(Keys.S)) { player.Move("down", game.GetObstacles(), this.ClientSize); moved = true; }

            charBox.Refresh();

            //update ghost
            enemy.Update(charBox, game.GetObstacles(), this.ClientSize, isHiding);

            // SHOW "BE AWARE" when entering
            if (enemy.GetState() == Ghost.GhostState.Entering && warningTimer == 0)
            {
                lblWarning.Visible = true;
                warningTimer = 120; // 2 seconds
            }

            if (enemy.GetState() == Ghost.GhostState.Waiting)
            {
                enemy.Update(charBox, game.GetObstacles(), this.ClientSize, isHiding);
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
                charBox.Visible &&
                charBox.Bounds.IntersectsWith(enemy.CharacterBox.Bounds) &&
                damageCooldown == 0)
            {
                isGameOver = heart.TakeDamage();
                damageCooldown = 120;

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

            





        }



        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void Form2_KeyUp(object sender, KeyEventArgs e)
        {
            
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
                UpdateInventoryLabel();

            }

            //storyline intro
            if (e.KeyCode == Keys.Enter)
            {
                if (isTyping)
                {
                    // If typing is still ongoing, skip to full line
                    typewriterTimer.Stop();
                    lblInstruction.Text = currentText;
                    isTyping = false;
                }
                else
                {
                    // Move to next line
                    currentLineIndex++;
                    if (currentLineIndex < storyLine.Count)
                    {
                        ShowLine(storyLine[currentLineIndex]);
                    }
                    else
                    {
                        // End of storyline
                        lblInstruction.Visible = false;
                        //// -- Game timer (runs every 16ms ~ 60fps) ---
                        gameTimer = new System.Windows.Forms.Timer { Interval = 16 };
                        gameTimer.Tick += GameLoop;
                        gameTimer.Start();
                    }
                }



            }

            //other key logic
            if (e.KeyCode == Keys.E)
            {
                if (!isHiding && game.NearHide(player))
                {
                    isHiding = true;
                    player.Hide();
                }
                else if (isHiding)
                {
                    isHiding = false;
                    Rectangle hideBounds = game.GetNearestHideSpot(player);
                    int newX = hideBounds.Left + (hideBounds.Width - player.CharacterBox.Width) / 2;
                    int newY = hideBounds.Bottom + 5;
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

        private void UpdateInventoryLabel()

        {

            string text = "Inventory: " + player.inventory.ToString();
            lblInventory.Text = text;

            // Measure text width
            Size textSize = TextRenderer.MeasureText(text, lblInventory.Font);

            int fixedWidth = 533;
            int fixedHeight = 35;
            int maxWidth = 691;

            if (textSize.Width <= fixedWidth)
            {
                // Text fits, use fixed size
                lblInventory.AutoSize = false;
                lblInventory.Width = fixedWidth;
                lblInventory.Height = fixedHeight;
                lblInventory.Height = fixedHeight;
            }
            else if (textSize.Width <= maxWidth)
            {
                // Text exceeds fixed but under max width, expand width
                lblInventory.AutoSize = false;
                lblInventory.Width = textSize.Width + 20; // add some padding
                lblInventory.Height = fixedHeight;
            }
            else
            {
                // Text exceeds max width, allow wrapping
                lblInventory.AutoSize = true;
                lblInventory.MaximumSize = new Size(maxWidth, 0); // max width, height auto
            }
        }
        //to resize when enlarge
        private void Form2_Resize(object sender, EventArgs e)
        {
            if (resizer != null)

                resizer.Resize();


        }

        private void ShowLine(string line)
        {
            if (isTyping) return; // prevent overlapping
            isTyping = true;

            currentText = line;
            charIndex = 0;
            lblInstruction.Text = "";

            typewriterTimer.Interval = 30; // milliseconds per character
            typewriterTimer.Tick -= TypewriterTimer_Tick; // remove previous handlers
            typewriterTimer.Tick += TypewriterTimer_Tick;
            typewriterTimer.Start();
        }

        private void TypewriterTimer_Tick(object sender, EventArgs e)
        {
            if (charIndex < currentText.Length)
            {
                lblInstruction.Text += currentText[charIndex];
                charIndex++;
            }
            else
            {
                typewriterTimer.Stop();
                isTyping = false; // finished typing
            }
        }

        private void ghostBox_Click(object sender, EventArgs e)
        {

        }
    }
}


