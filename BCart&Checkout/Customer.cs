using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectOOP_TokoOnline
{
    public class Customer
    {
        public string CustomerName { get; }
        public string Address { get; }

        public Customer( string customerName, string address)
        {
            CustomerName = customerName;
            Address = address;
        }
    }
}
