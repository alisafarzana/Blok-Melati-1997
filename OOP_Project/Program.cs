using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            //Background Sound
            SoundPlayer bgMusic = new SoundPlayer(Properties.Resources.bg_Sound);
            bgMusic.PlayLooping();
            Application.Run(new Start());
        }


    }
}
