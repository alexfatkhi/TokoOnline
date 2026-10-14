using System;

namespace Project_OOP.AProduct
{
    public class Food : Product
    {
        // Days left until the product expires. 0 or less means expired.
        public int DaysUntilExpired { get; }

        public Food(
            string code,
            string name,
            decimal price,
            int stock,
            int daysUntilExpired
        ) : base(code, name, price, stock)
        {
            DaysUntilExpired = daysUntilExpired;
        }

        public override string GetCategory()
        {
            return "Food";
        }

        // Food has no promo or warranty; the final price is the normal price.
        public override decimal GetFinalPrice()
        {
            return Price;
        }

        // Food can never be returned.
        public override bool CanBeReturned()
        {
            return false;
        }

        public bool IsExpired()
        {
            return DaysUntilExpired <= 0;
        }
    }
}
