using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Media;
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



        private int pickupMsgTimer = 0;

        private bool isHiding = false;


        private Resizer resizer;

        private List<string> storyLine = new List<string>();
        private int currentLineIndex = 0;
        //Typewriter effect
        private Timer typewriterTimer = new Timer();
        private string currentText = "";
        private int charIndex = 0;
        private bool isTyping = false; // prevent skipping during typing
        private SoundPlayer bgSound = new SoundPlayer("bg_Sound.wav");
        private SoundPlayer ghostLaugh = new SoundPlayer("ghostLaugh.wav");
        private SoundPlayer ghostEnd = new SoundPlayer("ghostEnd.wav");
        private bool isGhostLaughPlaying = false;
        private bool isHiddenSoundPlaying = false;
        private PictureBox jumpScareBox;

        private List<GameTaskBase> tasks = new List<GameTaskBase>();
        private int currentTaskIndex = 0;
        private bool tasksComplete = false;
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
        public Label LblInstruction => lblInstruction;

        public ProgressBar TaskBar => taskBar;


        int damageCooldown = 0;
        int warningTimer = 150;
        Label lblWarning = new Label();
        Label lblStatus = new Label();
        Label lblInstruction = new Label();
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

            
            if (tasksComplete || isGameOver)
            {
                // stop all updates
                return;
            }

            if (isGameOver) return;
            bool moved = false;


            // ✅ Make taskBar follow player
            taskBar.Left = player.CharacterBox.Left;
            taskBar.Top = player.CharacterBox.Top - 20; // above player

            //task

            if (currentTaskIndex < tasks.Count)
            {
                var currentTask = tasks[currentTaskIndex];
                currentTask.Update(this);
                if (!isGameOver) 
                    lblStatus.Text = "Task: " + currentTask.GetTaskName();

                if (currentTask.IsCompleted)
                {
                    currentTaskIndex++;
                    if (currentTaskIndex < tasks.Count)
                    {
                        tasks[currentTaskIndex].Start(this);
                        tasksComplete = false;
                    }
                    else
                    {
                        if (!isGameOver)
                        {
                            lblStatus.Text = "LEVEL 1 COMPLETED!";
                            tasksComplete = true;
                            lblStatus.Visible = true;
                            lblStatus.ForeColor = Color.Green;
                            lblStatus.Left = (this.ClientSize.Width - lblStatus.Width) / 2;
                            lblStatus.Top = (this.ClientSize.Height - lblStatus.Height) / 2;
                            lblStatus.BringToFront();

                        }

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
            
            enemy.Update(characterBox, game.GetObstacles(), this.ClientSize, isHiding);

            //if (enemy.GetState() == Ghost.GhostState.Entering ||
            //    enemy.GetState() == Ghost.GhostState.Chasing ||
            //    enemy.GetState() == Ghost.GhostState.Roaming)
            //{
            //    if (!isGhostPlaying)
            //    {
            //        bgSound.Stop();              // stop background
            //        enemy.PlayGhostMusic();      // play ghost sound
            //        isGhostPlaying = true;
            //    }
            //}
            //else
            //{
            //    if (isGhostPlaying)
            //    {
            //        enemy.StopGhostMusic();      // stop ghost
            //        bgSound.PlayLooping();       // resume bg
            //        isGhostPlaying = false;
            //    }
            //}

            var state = enemy.GetState();

            // 🎯 1. ENTERING or CHASING → ghostLaugh
            if (state == Ghost.GhostState.Entering || state == Ghost.GhostState.Chasing)
            {
                if (!isGhostLaughPlaying)
                {
                    bgSound.Stop();
                    enemy.StopHiddenSound();   // stop hiding sound
                    ghostLaugh.PlayLooping();

                    isGhostLaughPlaying = true;
                    isHiddenSoundPlaying = false;
                }
            }

            // 🎯 2. ROAMING + HIDING → ghost_Sound
            else if (state == Ghost.GhostState.Roaming && isHiding)
            {
                if (!isHiddenSoundPlaying)
                {
                    bgSound.Stop();
                    ghostLaugh.Stop();         // stop laugh
                    enemy.PlayHiddenSound();

                    isHiddenSoundPlaying = true;
                    isGhostLaughPlaying = false;
                }
            }

            // 🎯 3. OTHERWISE → background music
            else
            {
                if (isGhostLaughPlaying || isHiddenSoundPlaying)
                {
                    ghostLaugh.Stop();
                    enemy.StopHiddenSound();
                    bgSound.PlayLooping();

                    isGhostLaughPlaying = false;
                    isHiddenSoundPlaying = false;
                }
            }

            // SHOW "BE AWARE" when entering
            if (!tasksComplete && enemy.GetState() == Ghost.GhostState.Entering && warningTimer == 0)
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
                    heldKeys.Clear();

                    // 👻 SHOW JUMPSCARE
                    jumpScareBox.Visible = true;
                    jumpScareBox.BringToFront();

                    lblStatus.Text = "GAME OVER";
                    lblStatus.Visible = true;

                    lblStatus.Left = (this.ClientSize.Width - lblStatus.Width) / 2;
                    lblStatus.Top = (this.ClientSize.Height / 2) - 120; // 👈 higher (adjust this value)
                    lblStatus.BringToFront();

                    btnHome.Visible = true;
                    btnHome.Enabled = true;

                    // 🎵 Play ghostEnd.wav
                    bgSound.Stop();       // stop background if still playing
                    ghostLaugh.Stop();    // stop ghost laugh if still playing
                    ghostEnd.Play();      // play ghostEnd once

                    return;
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
                    //lblInventory.Text = "Inventory: " + player.inventory.ToString();
                    UpdateInventoryLabel();
                    lblPickup.Text = $"Picked up: {picked}!";
                    pickupMsgTimer = 120;
                }
            }

            if (pickupMsgTimer > 0)
            {
                pickupMsgTimer--;
                if (pickupMsgTimer == 0) lblPickup.Text = "";
            }

            FixLayers();
            //next level
            if (tasksComplete == true)
            {
                GameMenu.Level1Completed = true;
                GameMenu.SavedInventory = player.inventory;
                btnNext.Visible = true;
                btnHome.Visible = true;
                btnNext.Enabled = true;
                btnHome.Enabled = true;
;

            }
        }


        private void keyIsUp(object sender, KeyEventArgs e)
        {
            heldKeys.Remove(e.KeyCode);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bgSound.PlayLooping();
            lblInventory.Width = 533;
            lblInventory.Height = 35;
            lblInventory.AutoSize = false;

            player = new Player(characterBox, 4);
            enemy = new Ghost(ghostPic, 1);
            heart = new Health(heart1, heart2, heart3);

            Level level1 = new Level(new List<PictureBox>()
             {
                leftToiletBox,
                rightToiletBox,
                sinkBox,
                washBox
             },
             new List<PictureBox>() // hide spots
             {
                leftToiletBox,
                rightToiletBox
             },
             new List<Item>()//items
             {
                new Item("Shirt", shirtBox),
                new Item("Sock", sockBox),
                new Item("Sock", sock2Box),
                new Item("Towel", towelBox),
                new Item("Shirt", shirt2Box),
                new Item("Bucket", bucketBox)
             },
             new List<PictureBox>() //washstations
             {
                washBox
             },
             new List<PictureBox>() //water stations
             {
                sinkBox
             }
            );

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
                "Meet Yana, a quiet student at a girls' school...",
                "One night, past midnight, a senior appeared in the empty corridor.",
                "\"Yana... I need you to wash my clothes,\" the senior whispered.",
                "Yana’s heart raced. She knew disobeying meant punishment...",
                "But obeying meant she might be trapped in the dark school alone.",
                "She must complete the tasks quietly and carefully.",
                "Remember: the shadows are watching.",
                "If something approaches, hide immediately! Don't let it catch you...",
                "Use WASD or Arrow Keys to move Yana around the rooms.",
                "Press E to hide when near hiding spots to avoid detection.",
                "Press F to perform tasks like washing or filling the bucket.",
                "Select items from the inventory using the number keys.",
                "Every sound could give you away... Good luck!"
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

            tasks.Add(new PickUpTask());
            tasks.Add(new FillBucketTask());
            tasks.Add(new WashTask());

            tasks[currentTaskIndex].Start(this);


            //task bar setup
            taskBar.Visible = false;
            taskBar.Minimum = 0;
            taskBar.Maximum = 100;
            taskBar.Value = 0;
            taskBar.BringToFront();

            jumpScareBox = new PictureBox();
            jumpScareBox.Image = Properties.Resources.jumpScareBox; // from resources
            jumpScareBox.SizeMode = PictureBoxSizeMode.StretchImage;
            jumpScareBox.Dock = DockStyle.Fill; // full screen
            jumpScareBox.Visible = false;

            this.Controls.Add(jumpScareBox);

            // make sure it's ABOVE everything first
            jumpScareBox.BringToFront();

            // BUT keep GAME OVER label above it
            lblStatus.BringToFront();
        }



        protected override void OnKeyDown(KeyEventArgs e)
        {
            heldKeys.Add(e.KeyCode);

            if(isGameOver)
            {
                return;
            }

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
        private void Form1_Resize(object sender, EventArgs e)
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

        private void FixLayers()
        {
            
            rightToiletBox.BringToFront(); 
            leftToiletBox.BringToFront();
            sinkBox.SendToBack();  // sink below ghost


            taskBar.BringToFront();  // UI always on top
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(player.inventory);
            form2.Show();
            this.Close();
        }

        private void gamePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            Start start = new Start();
            start.Show();
            this.Close(); 
        }
    }
}
       

        