using System;

namespace TokoOnline_New.AProduct
{
    // Induk abstract semua produk = Food, Electronic, Clothing.
    public abstract class Product
    {
        public static int TotalProducts { get; private set; } = 0;   // total produk dibuat

        public string Code { get; private set; }
        public string Name { get; }
        public int Price { get; }
        public int Stock { get; private set; }

        // protected hanya dipanggil class child lewat base.
        protected Product(string code, string name, int price, int stock)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Validation: product code cannot be empty.");
            }

            if (price <= 0)
            {
                throw new ArgumentException( $"Validation: price of '{name}' must be greater than 0 (got {price})." );
            }

            if (stock < 0)
            {
                throw new ArgumentException(
                    $"Validation: stock of '{name}' cannot be negative (got {stock})."
                );
            }

            Code = code;
            Name = name;
            Price = price;
            Stock = stock;

            TotalProducts++;
        }

        // Wajib dioverride tiap child dengan rumusnya sendiri.
        public abstract int FinalPrice();

        // Hitung sekian persen dari harga (buat diskon/garansi).
        protected int CalculatePercent(int percent)
        {
            return Price * percent / 100;
        }

        // Kurangi stok saat dibeli. true kalau berhasil.
        public bool ReduceStock(int qty)
        {
            if (qty <= 0 || qty > Stock)
            {
                return false;
            }

            Stock -= qty;
            return true;
        }

        // Tambah stok (misal saat retur berhasil).
        public void AddStock(int qty)
        {
            if (qty <= 0)
            {
                return;
            }

            Stock += qty;
        }

        // virtual, child boleh override kalau butuh perilaku beda.
        public virtual bool CanBuy()
        {
            return Stock > 0;
        }

        public virtual void Info()
        {
            Console.WriteLine(
                $"{Code,-6} {Name,-12} {GetType().Name,-11} " +
                $"Normal:Rp{Price,-8:N0} Final:Rp{FinalPrice(),-8:N0} Stock:{Stock}"
            );
        }
    }
}
