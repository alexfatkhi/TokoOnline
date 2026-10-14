using System;
using System.Collections.Generic;
using System.Text;

namespace Project_OOP.AProduct
{
    public class Outfit : Product
    {
        public bool IsPromo { get; }

        public Outfit(
            string code,
            string productName,
            decimal price,
            int stock,
            bool isPromo
        ) : base(code, productName, price, stock)
        {
            IsPromo = isPromo;
        }

        public override decimal FinalPrice()
        {
            if (IsPromo)
            {
                return Price * 0.8m;
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
