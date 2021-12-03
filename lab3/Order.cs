using System;
using System.Collections.Generic;

namespace lab3
{
    public partial class Order
    {
        public int OrdId { get; set; }
        public double Cost { get; set; }
        public int UsId { get; set; }
        public virtual User Us { get; set; }
    }
}
