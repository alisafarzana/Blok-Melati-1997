using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class Resizer
    {

        private Form form;
        private Size originalFormSize;
        private Player player;
        private Ghost ghost;

        private Dictionary<Control, Rectangle> originalControlBounds = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, float> originalFontSizes = new Dictionary<Control, float>();
        private Point originalCharPosition;

        public Resizer(Form form, Player player, Ghost ghost)
        {
            this.form = form;
            this.player = player;
            this.ghost = ghost;
            originalFormSize = form.ClientSize;

            // Capture all controls recursively
            StoreControlBoundsAndFonts(form);

            // Store player.CharacterBox explicitly in case it's nested
            if (!originalControlBounds.ContainsKey(player.CharacterBox))
                originalControlBounds[player.CharacterBox] = player.CharacterBox.Bounds;

            if (!originalFontSizes.ContainsKey(player.CharacterBox))
                originalFontSizes[player.CharacterBox] = player.CharacterBox.Font.Size;

            if (!originalControlBounds.ContainsKey(ghost.CharacterBox))
                originalControlBounds[ghost.CharacterBox] = ghost.CharacterBox.Bounds;

            originalCharPosition = player.CharacterBox.Location;
        }

        private void StoreControlBoundsAndFonts(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                originalControlBounds[ctrl] = ctrl.Bounds;
                originalFontSizes[ctrl] = ctrl.Font.Size;

                if (ctrl.HasChildren)
                    StoreControlBoundsAndFonts(ctrl); // recursive
            }
        }

        public void Resize()
        {
            float xRatio = (float)form.ClientSize.Width / originalFormSize.Width;
            float yRatio = (float)form.ClientSize.Height / originalFormSize.Height;
            float ratio = Math.Min(xRatio, yRatio);

            // Resize all controls recursively
            foreach (var kvp in originalControlBounds)
            {
                Control ctrl = kvp.Key;
                Rectangle original = kvp.Value;

             

                ctrl.Bounds = new Rectangle(
                    (int)(original.X * xRatio),
                    (int)(original.Y * yRatio),
                    (int)(original.Width * xRatio),
                    (int)(original.Height * yRatio)
                );

                // Resize font for labels
                if (ctrl is Label)
                {
                    ctrl.Font = new Font(ctrl.Font.FontFamily, originalFontSizes[ctrl] * ratio, ctrl.Font.Style);
                }
            }

            //resize player
            Rectangle playerOriginal = originalControlBounds[player.CharacterBox];
            player.CharacterBox.Bounds = new Rectangle(
                (int)(playerOriginal.X * xRatio),
                (int)(playerOriginal.Y * yRatio),
                (int)(playerOriginal.Width * xRatio),
                (int)(playerOriginal.Height * yRatio)

            );

            //resize ghost
            Rectangle ghostOriginal = originalControlBounds[ghost.CharacterBox];
            ghost.CharacterBox.Bounds = new Rectangle(
                (int)(ghostOriginal.X * xRatio),
                (int)(ghostOriginal.Y * yRatio),
                (int)(ghostOriginal.Width * xRatio),
                (int)(ghostOriginal.Height * yRatio)

            );
        }

      
    }
    }

