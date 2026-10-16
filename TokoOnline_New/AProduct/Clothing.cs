using System;

namespace   TokoOnline_New.AProduct
{
    // Pakaian, turunan Product dan bisa diretur.
    public class Clothing : Product, IReturnable
    {
        public string Size { get; }   // ukuran: S/M/L
        public bool OnPromo { get; }   // true = sedang promo
        public int ReturnDayLimit { get; }

        public Clothing(string code, string name, int price, int stock, string size, bool onPromo)
            : base(code, name, price, stock)
        {
            Size = size;
            OnPromo = onPromo;
            ReturnDayLimit = 7;
        }

        // Promo potong 20%, selain itu harga normal.
        public override int FinalPrice()
        {
            if (OnPromo)
            {
                return Price - CalculatePercent(20);
            }

            return Price;
        }

        public bool CanReturn(int days)
        {
            return days >= 0 && days <= ReturnDayLimit;
        }

        public override void Info()
        {
            string note = OnPromo ? "PROMO 20%" : "-";

            Console.WriteLine(
                $"{Code,-6} {Name,-12} {GetType().Name,-11} " +
                $"Normal:Rp{Price,-8:N0} Final:Rp{FinalPrice(),-8:N0} Stock:{Stock}  " +
                $"Size:{Size}  {note}"
            );
        }
    }
}
