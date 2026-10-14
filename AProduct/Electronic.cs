using System;
using System.Collections.Generic;
using System.Text;

namespace Project_OOP.AProduct
{
    public class Electronic : Product
    {
        public bool WarrantyAdded { get; }

        public Electronic(
            string code,
            string productName,
            decimal price,
            int stock,
            bool warrantyAdded
        ) : base(code, productName, price, stock)
        {
            WarrantyAdded = warrantyAdded;
        }

        public override decimal FinalPrice()
        {
            if (WarrantyAdded)
            {
                return Price * 1.05m;
            }

            return Price;
        }

        public override bool CanBeReturned(int daysSincePurchase)
        {
            return daysSincePurchase >= 0
                && daysSincePurchase <= 7;
        }
    }
}
