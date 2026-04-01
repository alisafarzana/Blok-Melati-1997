using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class StoryManager
    {


        private List<string> storyLines;
        private int currentLineIndex = 0;
        private Timer typewriterTimer = new Timer();
        private Label displayLabel;
        private string currentText = "";
        private int charIndex = 0;
        private bool isTyping = false;

        public bool IsTyping() => isTyping; //check if typing currently
        public event Action StoryFinished;
        public StoryManager(List<string> lines, Label label)
        {
            storyLines = lines;
            displayLabel = label;

            typewriterTimer.Interval = 30; // ms per character
            typewriterTimer.Tick += TypewriterTimer_Tick;
        }

        // Start showing the current line (or next line)
        public void ShowNextLine()
        {
            if (isTyping) return;

            if (currentLineIndex < storyLines.Count)
            {
                StartTypingLine(storyLines[currentLineIndex]);
                currentLineIndex++;
            }
            else
            {
                displayLabel.Visible = false;
                StoryFinished?.Invoke(); // notify Form1 that story is done
            }
        }

        //Skip the current typing and show full line immediately
        public void SkipCurrentLine()
        {
            if (!isTyping) return;

            typewriterTimer.Stop();
            displayLabel.Text = currentText;
            isTyping = false;
        }

        //Internal helper to start typewriter effect
        private void StartTypingLine(string line)
        {
            currentText = line;
            charIndex = 0;
            displayLabel.Text = "";
            isTyping = true;
            displayLabel.Visible = true;
            typewriterTimer.Start();
        }

        //Typewriter timer tick
        private void TypewriterTimer_Tick(object sender, EventArgs e)
        {
            if (charIndex < currentText.Length)
            {
                displayLabel.Text += currentText[charIndex];
                charIndex++;
            }
            else
            {
                typewriterTimer.Stop();
                isTyping = false;
            }
        }

        //Optional: reset to beginning
        public void ResetStory()
        {
            currentLineIndex = 0;
            isTyping = false;
            typewriterTimer.Stop();
            displayLabel.Text = "";
        }

    }
}
