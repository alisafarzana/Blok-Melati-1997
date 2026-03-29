using OOP_Project;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class HangTask : GameTaskBase
{
    private List<PictureBox> hangBoxes;
    private Inventory playerInventory;
    private ProgressBar progressBar;
    private Form2 gameForm;
    private Player player;

    private Dictionary<string, Image> hangImages;
    private bool[] boxOccupied;

    private TimedTask hangTimer;

    private int currentBoxIndex = -1; // 🔥 track which box player is near

    public Player Player => player;
    public HangTask(Form2 form, Inventory inventory, List<PictureBox> boxes, ProgressBar progress)
    {
        gameForm = form;
        playerInventory = inventory;
        hangBoxes = boxes;
        progressBar = progress;
        player = form.Player;
        boxOccupied = new bool[boxes.Count];

        hangImages = new Dictionary<string, Image>()
        {
            { "Washed Sock", OOP_Project.Properties.Resources.socksHanged },
            { "Washed Shirt", OOP_Project.Properties.Resources.shirtHanged },
            { "Washed Towel", OOP_Project.Properties.Resources.towelHanged }
        };

        foreach (var box in hangBoxes)
        {
            box.BorderStyle = BorderStyle.None;
            box.Paint += HangBox_Paint;
        }

        progressBar.Minimum = 0;
        progressBar.Maximum = 100;
        progressBar.Value = 0;

        // 🔥 TIMER
        hangTimer = new TimedTask(3000, () =>
        {
            var heldItem = playerInventory.HeldItem;

            // 🔥 MUST have valid box
            if (currentBoxIndex != -1 &&
                heldItem != null &&
                hangImages.ContainsKey(heldItem.Name) &&
                !boxOccupied[currentBoxIndex])
            {
                hangBoxes[currentBoxIndex].Image = hangImages[heldItem.Name];
                boxOccupied[currentBoxIndex] = true;

                playerInventory.RemoveItem(heldItem);
                gameForm.UpdateInventoryLabel();

                hangBoxes[currentBoxIndex].Refresh();

                // optional completion check
                if (boxOccupied.All(b => b))
                {
                    IsCompleted = true;
                    progressBar.Visible = false;
                }
            }
        });
    }

    public override void Update(Form1 game)
    {
        var heldItem = playerInventory.HeldItem;

        //Rectangle playerArea = gameForm.Player.CharacterBox.Bounds;
        //playerArea.Inflate(30, 30);
        Rectangle playerArea = new Rectangle(
        player.CharacterBox.Left - 40,   // wider left
        player.CharacterBox.Top - 80,    // extend UP more
        player.CharacterBox.Width + 80,  // wider width
        player.CharacterBox.Height + 100  // extend height upward
        );

        


        // 🔥 CHECK WHICH BOX PLAYER IS NEAR
        currentBoxIndex = -1;

        for (int i = 0; i < hangBoxes.Count; i++)
        {
            if (playerArea.IntersectsWith(hangBoxes[i].Bounds))
            {
                currentBoxIndex = i;
                break;
            }
        }

        bool nearBox = currentBoxIndex != -1;

        if (gameForm.HeldKeysContains(Keys.F) &&
            heldItem != null &&
            hangImages.ContainsKey(heldItem.Name) &&
            nearBox &&
            !boxOccupied[currentBoxIndex])
        {
            if (!hangTimer.IsRunning)
                hangTimer.Start();

            // ✅ SHOW BAR
            progressBar.Visible = true;
            progressBar.Value = hangTimer.Progress;
        }
        else
        {
            hangTimer.Cancel();
            progressBar.Visible = false;
        }
    }

    public override string GetTaskName()
    {
        return "Hang Clothes";
    }

    private void HangBox_Paint(object sender, PaintEventArgs e)
    {
        PictureBox box = sender as PictureBox;
        int index = hangBoxes.IndexOf(box);

        if (!boxOccupied[index])
        {
            using (Pen pen = new Pen(Color.Yellow))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                e.Graphics.DrawRectangle(pen, 0, 0, box.Width - 1, box.Height - 1);
            }
        }
    }
}