using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Project
{
    public class Inventory
    {
        private List<Item> item = new List<Item>();


        public List<Item> Items { get; private set; } = new List<Item>();

        // Currently held item index (for hotbar (when click number keys, can hold item))
        public int HeldIndex { get; private set; } = -1;

        public Item HeldItem => (HeldIndex >= 0 && HeldIndex < Items.Count) ? Items[HeldIndex] : null;


        public void AddItem(Item item) 
        {
            Items.Add(item);
            if (HeldIndex == -1)
                HeldIndex = 0; // auto select first item
        }

        public void RemoveItem(Item item)
        {

            int idx = Items.IndexOf(item);
            if (idx == HeldIndex)
                HeldIndex = -1;

            Items.Remove(item);

            if (HeldIndex >= Items.Count)
                HeldIndex = Items.Count - 1;

        }

        public bool hasItem(Item item)
        {
            if (Items.Contains(item)) return true;
            else
            {
                return false;
            }
        }

        public List<Item> getItems()
        {
            return Items;
        }

       

        public void SelectHotbar(int index) //select number keys
        {
            if (index >= 0 && index < Items.Count)
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
            for (int i = 0; i < Items.Count; i++)
            {
                string name = Items[i].Name;
                if (i == HeldIndex)
                    name = "[" + name + "]"; // highlight held item
                list.Add(name);
            }
            return string.Join(", ", list);
        }

        public Inventory Clone() //save inventory from level 1 and reset everytime the player die or game complete
        {
            Inventory newInv = new Inventory();

            foreach (var item in Items)
            {
                newInv.AddItem(item);
            }

            return newInv;
        }
    }
}
