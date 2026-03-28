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
        private int duration;
        private int elapsed;
        private Timer timer;
        private Action onComplete;

        public bool IsRunning { get; private set; } = false;

        public int Progress
        {
            get
            {
                if (duration == 0) return 0;
                return Math.Min(100, (int)((elapsed / (float)duration) * 100));
            }
        }

        public TimedTask(int durationMs, Action onComplete)
        {
            duration = durationMs;
            elapsed = 0;
            this.onComplete = onComplete;

            timer = new Timer();
            timer.Interval = 16; // ~60 FPS

            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;

                if (elapsed >= duration)
                {
                    timer.Stop();
                    IsRunning = false;
                    onComplete?.Invoke();
                }
            };
        }

        public void Start()
        {
            if (IsRunning) return;

            elapsed = 0;
            IsRunning = true;
            timer.Start();
        }

        public void Cancel()
        {
            timer.Stop();
            IsRunning = false;
            elapsed = 0;
        }
    }
}
