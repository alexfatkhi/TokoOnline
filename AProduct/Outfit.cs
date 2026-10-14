using System;

namespace Project_OOP.AProduct
{
    public class Outfit : Product
    {
        public decimal DiscountRate { get; } // e.g. 0.20 = 20% off
        public bool IsOnPromo { get; }

        public Outfit(
            string code,
            string name,
            decimal price,
            int stock,
            bool isOnPromo,
            decimal discountRate = 0.20m
        ) : base(code, name, price, stock)
        {
            if (discountRate < 0 || discountRate > 1)
            {
                throw new ArgumentException(
                    $"Validation: discount rate of '{name}' must be between 0 and 1."
                );
            }

            IsOnPromo = isOnPromo;
            DiscountRate = discountRate;
        }

        public override string GetCategory()
        {
            return "Outfit";
        }

        // Apply the discount only while the promo is active.
        public override decimal GetFinalPrice()
        {
            if (IsOnPromo)
            {
                return Price * (1 - DiscountRate);
            }

            return Price;
        }

        // Outfits can be returned (the 7-day window is checked by Customer).
        public override bool CanBeReturned()
        {
            return true;
        }
    }
}
