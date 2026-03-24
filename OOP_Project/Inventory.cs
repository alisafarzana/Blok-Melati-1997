using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Project
{
    public class Inventory
    {
        public List<string> items { get; private set; } = new List<string>();

        public void AddItem(string item)
        {
            items.Add(item);
        }

        public void removeItem(string item)
        {
            items.Remove(item);

        }

        public bool hasItem(string item)
        {
            if (items.Contains(item)) return true;
            else
            {
                return false;
            }
        }

        public List<string> getItems()
        {
            return items;
        }

        public override string ToString()
        {
            return string.Join(", ", items);
        }
    }
}
