using System;
using System.Collections.Generic;
using System.Text;

namespace TokoOnline_New.Exceptions
{
    public class ExpiredProductException : Exception
    {
        public ExpiredProductException()
            : base("Product cannot be added because is expired.")
        {

        }
    }
}
