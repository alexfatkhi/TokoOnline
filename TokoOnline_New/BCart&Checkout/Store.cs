using System;
using System.Linq;
using Project_OOP.AProduct;

namespace TokoOnline_New.BCart_Checkout
{
    public class Store
    {
        // Katalog + pencarian kode ditangani Repository (List + Dictionary).
        private readonly Repository<Product> repository;

        public Store(Repository<Product> repository)
        {
            this.repository = repository;
        }

        // Pencarian produk lewat kode (via Dictionary di Repository).
        //---
        public Product FindProduct(string code)
        {
            return repository.GetByCode(code);
        }

        public void PrintCatalog()
        {
            Console.WriteLine();
            Console.WriteLine("=== PRODUCT CATALOG ===");

            // Judul kolom + garis pemisah, sejajar dengan format baris di Product.Info().
            Console.WriteLine(
                $"{"Code",-6} | {"Name",-13} | {"Category",-11} | " +
                $"{"Normal Price",-12} | {"Final Price",-12} | {"Stock",-6} | {"Note"}");
            Console.WriteLine(new string('-', 95));

            foreach (Product product in repository.GetAll())
            {
                product.Info();
            }
        }

        // FITUR 3: Produk dengan stok di bawah 5.
        public void PrintLowStock()
        {
            Console.WriteLine();
            Console.WriteLine("=== LOW STOCK (< 5) ===");

            // LINQ: ambil produk yang stoknya kurang dari 5, urut dari paling tipis.
            var lowStock = repository.GetAll()
                .Where(product => product.Stock < 5)
                .OrderBy(product => product.Stock)
                .ToList();

            if (lowStock.Count == 0)
            {
                Console.WriteLine("All products are sufficiently stocked.");
                return;
            }

            foreach (Product product in lowStock)
            {
                Console.WriteLine(
                    $"{product.Code,-6} {product.Name,-12} {product.GetType().Name,-11} Stock:{product.Stock}");
            }

            Console.WriteLine($"Total: {lowStock.Count} product(s) low on stock.");
        }

        public int Refund(Product item, int days)
        {
            if (item == null)
            {
                return 0;
            }

            if (item is IReturnable returnable)
            {
                if (returnable.CanReturn(days))
                {
                    item.AddStock(1);
                    return item.FinalPrice();
                }
            }

            return 0;
        }
    }
}
