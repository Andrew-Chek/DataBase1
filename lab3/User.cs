using System;
using System.Collections.Generic;

namespace lab3
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public int UsId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
