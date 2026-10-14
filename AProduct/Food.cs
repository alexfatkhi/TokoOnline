using System;
using System.Collections.Generic;
using System.Text;

namespace Project_OOP.AProduct
{
    public class Food : Product
    {
        public DateTime ExpiryDate { get; }

        public Food(
            string code,
            string productName,
            decimal price,
            int stock,
            DateTime expiryDate
        ) : base(code, productName, price, stock)
        {
            ExpiryDate = expiryDate;
        }

        public override decimal FinalPrice()
        {
            return Price;
        }

        public bool CanBePurchased()
        {
            return DateTime.Today <= ExpiryDate;
        }

        public override bool CanBeReturned(int daysSincePurchase)
        {
            return false;
        }
    }
}
