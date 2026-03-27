using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class TimedTask
    {
        private int durationTicks;
        private int progress = 0;
        private Timer timer;
        private Action onComplete;

        public bool IsRunning { get; private set; } = false;

        // duration in milliseconds
        public TimedTask(int durationMs, Action onComplete)
        {
            durationTicks = durationMs / 16; // ~60fps
            this.onComplete = onComplete;

            timer = new Timer();
            timer.Interval = 16;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            progress++;

            if (progress >= durationTicks)
            {
                Complete();
            }
        }

        public void Start()
        {
            if (!IsRunning)
            {
                progress = 0;
                IsRunning = true;
                timer.Start();
            }
        }

        public void Cancel()
        {
            timer.Stop();
            progress = 0;
            IsRunning = false;
        }

        private void Complete()
        {
            timer.Stop();
            IsRunning = false;
            onComplete?.Invoke();
        }
    }
}
