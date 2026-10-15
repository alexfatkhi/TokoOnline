using System;

namespace Project_OOP.AProduct
{
    // Elektronik, turunan Product dan bisa diretur.
    public class Electronic : Product, IReturnable
    {
        public bool WithWarranty { get; }   // true = pakai garansi
        public int ReturnDayLimit { get; }

        public Electronic(string code, string name, int price, int stock, bool withWarranty)
            : base(code, name, price, stock)
        {
            WithWarranty = withWarranty;
            ReturnDayLimit = 7;
        }

        // Pakai garansi tambah 5%, selain itu harga normal.
        public override int FinalPrice()
        {
            if (WithWarranty)
            {
                return Price + CalculatePercent(5);
            }

            return Price;
        }

        public bool CanReturn(int days)
        {
            return days >= 0 && days <= ReturnDayLimit;
        }

        public override void Info()
        {
            string note = WithWarranty ? "Warranty (+5%)" : "No warranty";

            Console.WriteLine(
                $"{Code,-6} {Name,-12} {GetType().Name,-11} " +
                $"Normal:Rp{Price,-8:N0} Final:Rp{FinalPrice(),-8:N0} Stock:{Stock}  " +
                $"{note}"
            );
        }
    }
}
