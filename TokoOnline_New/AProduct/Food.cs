using System;

namespace Project_OOP.AProduct
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

        public override string GetNote()
        {
            return DaysUntilExpiry > 0
                ? $"Expires in {DaysUntilExpiry} days"
                : "EXPIRED";
        }
    }
}
