using System;
using System.Collections.Generic;

namespace lab3
{
    public partial class Item
    {
        public Item(double cost, bool availability, string name)
        {
            Cost = cost;
            Availability = availability;
            Name = name;
        }
        public Item()
        {
            
        }

        public int ItemId { get; set; }
        public double Cost { get; set; }
        public bool Availability { get; set; }
        public string Name { get; set; }
    }
}
