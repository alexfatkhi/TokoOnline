using System;
using System.Collections.Generic;
using System.Text;

namespace TokoOnline_New.BCart_Checkout
{
    public class Buyer
    {
        public string BuyerId { get; private set; }
        public string Name { get;  set; }
        public bool Member { get; set; }
        public int Points { get; private set; }

        public void AddPoints(int totalSpent)
        {

        }
    }
}
