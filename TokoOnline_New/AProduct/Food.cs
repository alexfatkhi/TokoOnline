using System;

namespace TokoOnline_New.AProduct
{
    // Makanan, turunan Product. Tidak bisa diretur.
    public class Food : Product
    {
        public int DaysUntilExpiry { get; }   // sisa hari sebelum kadaluarsa

        public Food(string code, string name, int price, int stock, int daysUntilExpiry)
            : base(code, name, price, stock)
        {
            DaysUntilExpiry = daysUntilExpiry;
        }

        // Makanan tidak ada diskon/garansi.
        public override int FinalPrice()
        {
            return Price;
        }

        // Bisa dibeli kalau stok ada dan belum kadaluarsa.
        public override bool CanBuy()
        {
            return base.CanBuy() && DaysUntilExpiry > 0;
        }

        public override void Info()
        {
            string note = DaysUntilExpiry > 0
                ? $"Expires in {DaysUntilExpiry} days"
                : "EXPIRED";

            Console.WriteLine(
                $"{Code,-6} {Name,-12} {GetType().Name,-11} " +
                $"Normal:Rp{Price,-8:N0} Final:Rp{FinalPrice(),-8:N0} Stock:{Stock}  {note}"
            );
        }
    }
}
