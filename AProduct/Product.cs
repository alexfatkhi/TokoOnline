using System;
using System.Collections.Generic;
using System.Text;

namespace Project_OOP.AProduct
{
    public abstract class Product
    {
        public string Code { get; }
        public string ProductName { get; }
        public decimal Price { get; }
        public int Stock { get; private set; }

        protected Product( string code, string productName, decimal price, int stock)
        {
            Code = code;
            ProductName = productName;
            Price = price;
            Stock = stock;
        }

        public abstract decimal FinalPrice();

        public abstract bool CanBeReturned(int daysSincePurchase);

        public bool ReduceStock(int quantity)
        {
            if (quantity <= 0 || quantity > Stock)
            {
                return false;
            }

            Stock -= quantity;
            return true;
        }

        public bool RestoreStock(int quantity)
        {
            if (quantity <= 0)
            {
                return false;
            }

            Stock += quantity;
            return true;
        }
    }
}

