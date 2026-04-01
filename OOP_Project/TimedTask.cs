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
        //TimedTask class is timer for each task
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
                return Math.Min(100, (int)((elapsed / (float)duration) * 100)); //calculate percentage
            }
        }

        public TimedTask(int durationMs, Action onComplete)
        {
            duration = durationMs;
            elapsed = 0;  //current progress time
            this.onComplete = onComplete;

            timer = new Timer();
            timer.Interval = 16; // ~60 FPS 

            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;

                if (elapsed >= duration) // check whether the time already reach the duration for the task
                {
                    timer.Stop(); //timer stop when task complete
                    IsRunning = false;
                    onComplete?.Invoke();//call the onComplete action when task complete
                }
            };
        }

        public void Start()
        {
            if (IsRunning) return; //if isRunning == true, then return nothing

            //start task
            elapsed = 0;
            IsRunning = true;
            timer.Start();
        }

        public void Cancel()
        {
            //cancel task
            timer.Stop();
            IsRunning = false;
            elapsed = 0;
        }
    }
}
