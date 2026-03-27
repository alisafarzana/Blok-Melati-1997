using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Project
{
    public class Inventory
    {
        public List<Item> items { get; private set; } = new List<Item>();

        // Currently held item index (for hotbar)
        public int HeldIndex { get; private set; } = -1;

        public Item HeldItem => (HeldIndex >= 0 && HeldIndex < items.Count) ? items[HeldIndex] : null;


        public void AddItem(Item item)
        {
            items.Add(item);
            if (HeldIndex == -1)
                HeldIndex = 0; // auto select first item
        }

        public void RemoveItem(Item item)
        {
            int idx = items.IndexOf(item);
            if (idx == HeldIndex)
                HeldIndex = -1;

            items.Remove(item);

            if (HeldIndex >= items.Count)
                HeldIndex = items.Count - 1;

        }

        public bool hasItem(Item item)
        {
            if (items.Contains(item)) return true;
            else
            {
                return false;
            }
        }

        public List<Item> getItems()
        {
            return items;
        }

       

        public void SelectHotbar(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                if (HeldIndex == index)
                    HeldIndex = -1; // unhold if pressed again
                else
                    HeldIndex = index;
            }
        }

        public override string ToString()
        {
            var list = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                string name = items[i].Name;
                if (i == HeldIndex)
                    name = "[" + name + "]"; // highlight held item
                list.Add(name);
            }
            return string.Join(", ", list);
        }
    }
}
