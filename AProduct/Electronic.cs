using System;

namespace Project_OOP.AProduct
{
    public class Electronic : Product
    {
        // Warranty rate added to the price, e.g. 0.05 = +5%. 0 means no warranty.
        public decimal Guarantee { get; }

        public Electronic(
            string code,
            string name,
            decimal price,
            int stock,
            decimal guarantee
        ) : base(code, name, price, stock)
        {
            if (guarantee < 0)
            {
                throw new ArgumentException(
                    $"Validation: guarantee of '{name}' cannot be negative."
                );
            }

            Guarantee = guarantee;
        }

        public override string GetCategory()
        {
            return "Electronic";
        }

        // Price plus warranty surcharge (e.g. +5%).
        public override decimal GetFinalPrice()
        {
            return Price * (1 + Guarantee);
        }

        // Electronics can be returned (the 7-day window is checked by Customer).
        public override bool CanBeReturned()
        {
            return true;
        }
    }
}
