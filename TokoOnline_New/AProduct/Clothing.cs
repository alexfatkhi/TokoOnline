using System;

namespace Project_OOP.AProduct
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

        public override string GetNote()
        {
            string promo = OnPromo ? "PROMO 20%" : "-";
            return $"Size:{Size}  {promo}";
        }
    }
}


//kasih suffix exception