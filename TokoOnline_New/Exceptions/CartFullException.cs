using System;
using System.Collections.Generic;
using System.Text;

namespace TokoOnline_New.Exceptions
{
    public class CartFullException : Exception
    {
        public CartFullException()
            : base("Cart is full. Maximum 10 product types.")
        {

        }
    }
}
