using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ScrollBar;

namespace OOP_Project
{
    public partial class Form2 : Form
    {
        private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();
        private Timer gameTimer;
        private bool isHiding = false;
        private Resizer resizer;

        private List<string> storyLine = new List<string>();
        private int currentLineIndex = 0;
        private Timer typewriterTimer = new Timer();
        private string currentText = "";
        private int charIndex = 0;
        private bool isTyping = false;
        private PictureBox jumpScareBox;

        // 🔥 Remove old 'player' instance, use only Player property
        // Player player;  <-- REMOVED
        GameManager game;
        Ghost enemy;
        Health heart;

        private Inventory passedInventory;
        private HangTask hangTask;
        private ProgressBar hangProgress;

       
        public Player Player { get; private set; } // 🔥 SINGLE PLAYER INSTANCE

        Label lblWarning = new Label();
        Label lblStatus = new Label();
        Label lblInstruction = new Label();
        Label lblCompleted = new Label();
        bool isGameOver = false;
        int damageCooldown = 0;
        int warningTimer = 150;

        private SoundPlayer bgSound = new SoundPlayer(Properties.Resources.bg_Sound);
        private SoundPlayer ghostLaugh = new SoundPlayer(Properties.Resources.ghostLaugh);
        private SoundPlayer ghost_Sound = new SoundPlayer(Properties.Resources.ghost_Sound);
        private SoundPlayer ghostEnd = new SoundPlayer(Properties.Resources.ghostEnd);
        private bool isGhostPlaying = false;
        private bool isHidingSoundPlaying = false;

        public bool HeldKeysContains(Keys key) => heldKeys.Contains(key);

        public Form2(Inventory inventory)
        {
            InitializeComponent();

            // 🔥 Only create one Player here
            Player = new Player(charBox, 4);  // use PictureBox from designer
            Player.inventory = inventory;
            Controls.Add(Player.CharacterBox);

            this.DoubleBuffered = true;
            this.Resize += Form2_Resize;
            passedInventory = inventory;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            bgSound.PlayLooping();

            // Setup inventory label
            lblInventory.Width = 533;
            lblInventory.Height = 35;
            lblInventory.AutoSize = false;

            // 🔥 Remove duplicate player creation
            // player = new Player(charBox, 4);  <-- REMOVED

            enemy = new Ghost(ghostBox, 2);
            heart = new Health(heart1, heart2, heart3);

            // Level setup
            Level level1 = new Level(
                new List<PictureBox>() { ampaian1, ampaian2, ampaian3, ampaian4, ampaian5 },
                new List<PictureBox>() { bush1, bush2, bush3 },
                new List<Item>() { }
            );
            game = new GameManager();
            game.LoadLevel(level1);

            // Instruction Label
            lblInstruction = new Label();
            lblInstruction.Text = "CONTROLS: \n Move : WASD or Arrow Keys \n Hide/Unhide : E (when near a hiding spot) \n Perform Tasks : F \n Select Inventory Items : Number Keys";
            lblInstruction.BackColor = Color.FromArgb(200, 0, 0, 0);
            lblInstruction.ForeColor = Color.White;
            lblInstruction.Font = new Font("Arial", 12, FontStyle.Regular);
            lblInstruction.AutoSize = false;
            lblInstruction.Size = new Size(1000, 150);
            lblInstruction.TextAlign = ContentAlignment.MiddleCenter;
            lblInstruction.Location = new Point((this.ClientSize.Width - lblInstruction.Width) / 2,
                                                (this.ClientSize.Height - lblInstruction.Height) / 2);
            lblInstruction.Visible = true;
            this.Controls.Add(lblInstruction);
            lblInstruction.BringToFront();

            // Storyline
            storyLine = new List<string>()
            {
                "Yana quickly grabs the wet clothes, heart racing.",
                "Suddenly, a shadow moves... the ghost is here!",
                "She must hang the clothes and stay quiet, or she’ll be caught.",
                "Move fast with WASD/Arrow Keys, hide with E, and complete the task with F.",
                "Every creak echoes. The ghost could appear at any moment...",
                "and maybe...faster"
            };
            lblInstruction.Text = storyLine[currentLineIndex];

            // Resizer
            resizer = new Resizer(this, Player, enemy); // 🔥 Use Player property

            // Warning Label
            lblWarning = new Label();
            lblWarning.Text = "BE AWARE!";
            lblWarning.Font = new Font("Arial", 24, FontStyle.Bold);
            lblWarning.ForeColor = Color.Red;
            lblWarning.BackColor = Color.Transparent;
            lblWarning.AutoSize = true;
            lblWarning.Visible = false;
            this.Controls.Add(lblWarning);
            lblWarning.Left = (this.ClientSize.Width / 2) - (lblWarning.Width / 2);
            lblWarning.Top = (this.ClientSize.Height / 2) - (lblWarning.Height / 2);
            lblWarning.BringToFront();

            // Game Over Label
            lblStatus.Text = "GAME OVER";
            lblStatus.Font = new Font("Arial", 28, FontStyle.Bold);
            lblStatus.ForeColor = Color.DarkRed;
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Location = new Point((this.ClientSize.Width / 2) - 100, (this.ClientSize.Height / 2) - 50);
            lblStatus.AutoSize = true;
            lblStatus.Visible = false;
            this.Controls.Add(lblStatus);

            // Game Completed label
            lblCompleted.Text = "GAME COMPLETED";
            lblCompleted.Font = new Font("Arial", 28, FontStyle.Bold);
            lblCompleted.ForeColor = Color.Green;
            lblCompleted.BackColor = Color.Transparent;
            lblCompleted.AutoSize = true;
            lblCompleted.Visible = false; // hide initially
            this.Controls.Add(lblCompleted);
            // center it
            lblCompleted.Left = (this.ClientSize.Width - lblCompleted.Width) / 2;
            lblCompleted.Top = (this.ClientSize.Height - lblCompleted.Height) / 2;
            lblCompleted.BringToFront();

            // Hang ProgressBar
            hangProgress = new ProgressBar
            {
                Size = new Size(100, 23),
                Visible = false,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };
            this.Controls.Add(hangProgress);
            hangProgress.BringToFront();

            // 🔥 Updated HangTask constructor (only pass Form2)
            List<PictureBox> hangBoxes = new List<PictureBox>() { hangBox1, hangBox2, hangBox3, hangBox4, hangBox5 };
            hangTask = new HangTask(this, Player.inventory, hangBoxes, hangProgress);  // ✅ correct

            UpdateInventoryLabel();

            // Game Timer
            gameTimer = new Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameLoop;
            //gameTimer.Start();

            // 👻 Jumpscare image (FULL SCREEN)
            jumpScareBox = new PictureBox();
            jumpScareBox.Dock = DockStyle.Fill;
            jumpScareBox.SizeMode = PictureBoxSizeMode.StretchImage;

            // ✅ Use resource image (you already added it)
            jumpScareBox.Image = Properties.Resources.jumpScareBox;

            jumpScareBox.Visible = false;

            this.Controls.Add(jumpScareBox);
            jumpScareBox.BringToFront();   // above everything initially
            lblStatus.BringToFront();      // BUT keep GAME OVER above it
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isGameOver) return;

            bool moved = false;

            // Player movement 🔥 Use Player property
            if (heldKeys.Contains(Keys.Left) || heldKeys.Contains(Keys.A)) { Player.Move("left", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Right) || heldKeys.Contains(Keys.D)) { Player.Move("right", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Up) || heldKeys.Contains(Keys.W)) { Player.Move("up", game.GetObstacles(), this.ClientSize); moved = true; }
            else if (heldKeys.Contains(Keys.Down) || heldKeys.Contains(Keys.S)) { Player.Move("down", game.GetObstacles(), this.ClientSize); moved = true; }

            charBox.Refresh();

            // Update Hang Task
            hangTask.Update(null);

            // Hang progress follows player
            if (hangProgress.Visible)
            {
                hangProgress.Location = new Point(
                    Player.CharacterBox.Left + (Player.CharacterBox.Width - hangProgress.Width) / 2,
                    Player.CharacterBox.Top - hangProgress.Height - 5
                );
                hangProgress.BringToFront();
            }

            // Update ghost
            enemy.Update(charBox, game.GetObstacles(), this.ClientSize, isHiding);

            var state = enemy.GetState();

            // 👻 Ghost active (NOT hiding)
            if ((state == Ghost.GhostState.Entering ||
                 state == Ghost.GhostState.Chasing ||
                 state == Ghost.GhostState.Roaming) && !isHiding)
            {
                if (!isGhostPlaying)
                {
                    bgSound.Stop();
                    ghost_Sound.Stop(); // stop hiding sound if somehow active

                    ghostLaugh.PlayLooping(); // 👻 chasing music
                    isGhostPlaying = true;
                    isHidingSoundPlaying = false;
                }
            }
            // 🙈 Player hiding
            else if (isHiding)
            {
                if (!isHidingSoundPlaying)
                {
                    ghostLaugh.Stop(); // stop chasing sound
                    bgSound.Stop();

                    ghost_Sound.PlayLooping(); // 🙈 hiding sound
                    isHidingSoundPlaying = true;
                    isGhostPlaying = false;
                }
            }
            // 🌙 No ghost
            else
            {
                if (isGhostPlaying || isHidingSoundPlaying)
                {
                    ghostLaugh.Stop();
                    ghost_Sound.Stop();

                    ghostEnd.Play(); // optional ending sound
                    bgSound.PlayLooping();

                    isGhostPlaying = false;
                    isHidingSoundPlaying = false;
                }
            }


            // Warning
            if (!hangTask.IsCompleted && enemy.GetState() == Ghost.GhostState.Entering && warningTimer == 0)
            {
                lblWarning.Visible = true;
                warningTimer = 120;
                
            }
            if (warningTimer > 0) { warningTimer--; if (warningTimer == 0) lblWarning.Visible = false; }

            // Collision
            if (!isHiding && charBox.Visible && charBox.Bounds.IntersectsWith(enemy.CharacterBox.Bounds) && damageCooldown == 0)
            {
                isGameOver = heart.TakeDamage();
                damageCooldown = 60;
                if (isGameOver)
                {
                    gameTimer.Stop();
                    // 👻 Show jumpscare
                    jumpScareBox.Visible = true;
                    jumpScareBox.BringToFront();
                    lblStatus.Text = "GAME OVER";
                    lblStatus.Visible = true;
                  
                    lblStatus.Left = (this.ClientSize.Width - lblStatus.Width) / 2;
                    lblStatus.Top = (this.ClientSize.Height / 2) - 200; // 👈 higher (adjust this value)
                    lblStatus.BringToFront();

                    btnMenuJC.Visible = true;
                    btnMenuJC.Enabled = true;

                    jumpScareBox.Controls.Add(btnMenuJC);
                    btnMenuJC.BringToFront();
                    
                }
            }
            if (damageCooldown > 0) damageCooldown--;

            if (hangTask.IsCompleted && !lblCompleted.Visible)
            {
                lblCompleted.Visible = true;
                // optional: stop player movement
                isGameOver = true; // stop game loop if you want
                btnMenu2.Visible = true;
                btnMenu2.Enabled = true;
            }
            
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            heldKeys.Add(e.KeyCode);

            // Hotbar selection 🔥 Use Player.Inventory
            if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D6)
            {
                int index = e.KeyCode - Keys.D1;
                Player.inventory.SelectHotbar(index);
                UpdateInventoryLabel();
            }

            // Task key
            if (e.KeyCode == Keys.F)
            {
                // HangTask handles progress and item hanging
            }

            // Hide/unhide 🔥 Use Player property
            if (e.KeyCode == Keys.E)
            {
                if (!isHiding && game.NearHide(Player))
                {
                    isHiding = true;
                    Player.Hide();
                }
                else if (isHiding)
                {
                    isHiding = false;
                    Rectangle hideBounds = game.GetNearestHideSpot(Player);
                    int newX = hideBounds.Left + (hideBounds.Width - Player.CharacterBox.Width) / 2;
                    int newY = hideBounds.Bottom + 5;
                    Player.CharacterBox.Location = new Point(newX, newY);
                    Player.Show();
                }
            }

            if (e.KeyCode == Keys.Enter)
            {
                if (currentLineIndex < storyLine.Count - 1)
                {
                    currentLineIndex++;
                    lblInstruction.Text = storyLine[currentLineIndex];
                }
                else
                {
                    lblInstruction.Visible = false;
                    gameTimer.Start();
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            heldKeys.Remove(e.KeyCode);
            base.OnKeyUp(e);
        }

        public void UpdateInventoryLabel()
        {
            if (Player.inventory == null)
            {
                lblInventory.Text = "Inventory: (empty)";
                return;
            }

            // 🔥 Use Player.Inventory
            string text = "Inventory: " + Player.inventory.ToString();
            lblInventory.Text = text;
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            resizer?.Resize();
            jumpScareBox?.BringToFront();
            lblStatus?.BringToFront(); // keep text above jumpscare
        }

        private void btnMenu2_Click(object sender, EventArgs e)
        {
            Start start = new Start();
            start.Show();
            this.Close();
        }

        private void btnMenuJC_Click(object sender, EventArgs e)
        {
            ghostEnd.Stop();
            bgSound.Play();
            Start start = new Start();
            start.Show();
            this.Close();
        }
    }
}