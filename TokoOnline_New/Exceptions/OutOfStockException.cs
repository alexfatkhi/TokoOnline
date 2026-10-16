using System;
using System.Collections.Generic;
using System.Text;

namespace TokoOnline_New.Exceptions
{
    public class OutOfStockException : Exception
    {
        public OutOfStockException(int stock)
            : base("Not enough stock. Available stock :" + stock)
        {

        }
    }
}
