using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Project
{
    public abstract class GameTaskBase
    {
        public bool IsCompleted { get; protected set; } = false;
        public abstract void Update(Form1 game);

        public virtual void Start(Form1 game) { }

        public abstract string GetTaskName();
    }
}
