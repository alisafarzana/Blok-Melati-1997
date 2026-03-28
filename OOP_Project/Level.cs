using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP_Project
{
    public class Level
    {
      

        public List<PictureBox> Obstacles { get; set; }
        public List<PictureBox> HideSpots { get; set; }

        public List<Item> Items { get; private set; }
        public List<PictureBox> WashStations { get; private set; }
        public List<PictureBox> WaterStation { get; set; }

        public Level(List<PictureBox> obstacles,List<PictureBox> hideSpots,List<Item> items = null , List<PictureBox> washStations = null, List<PictureBox> waterStation = null)
        {
            Obstacles = obstacles;
            HideSpots = hideSpots;
            Items = items;
            WashStations = washStations;
            WaterStation = waterStation;
        }
    }
}
