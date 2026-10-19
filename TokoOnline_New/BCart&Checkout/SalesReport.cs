using System;
using System.Collections.Generic;
using System.Linq;
using Project_OOP.AProduct;

namespace TokoOnline_New.BCart_Checkout
{
    // Laporan penjualan dg pakai LINQ.

    public class SalesReport
    {
        // Satu baris penjualan produk apa, berapa unit, berapa rupiah.
        private sealed class SalesLine
        {
            public Product Product { get; }
            public int Quantity { get; }
            public int Revenue { get; }

            public SalesLine(Product product, int quantity, int revenue)
            {
                Product = product;
                Quantity = quantity;
                Revenue = revenue;
            }
        }

        private readonly List<SalesLine> lines = new List<SalesLine>();

        // Catat seluruh item dari sebuah Order ke dalam log penjualan.
        public void Record(Order order)
        {
            if (order == null)
            {
                return;
            }

            for (int i = 0; i < order.Products.Count; i++)
            {
                Product product = order.Products[i];
                int quantity = order.Quantities[i];

                lines.Add(new SalesLine(product, quantity, product.FinalPrice() * quantity));
            }
        }

        // FITUR 1: Produk terlaris (unit terjual paling banyak).
        public void PrintBestSeller()
        {
            Console.WriteLine();
            Console.WriteLine("=== BEST SELLER ===");

            if (lines.Count == 0)
            {
                Console.WriteLine("No sales recorded yet.");
                return;
            }

            // LINQ: kelompokkan per kode produk, jumlahkan unit & omzetnya,
            // lalu urutkan dari yang paling laku.
            var ranking = lines
                .GroupBy(line => line.Product.Code)
                .Select(group => new
                {
                    Product = group.First().Product,
                    UnitsSold = group.Sum(line => line.Quantity),
                    Revenue = group.Sum(line => line.Revenue)
                })
                .OrderByDescending(item => item.UnitsSold)
                .ThenByDescending(item => item.Revenue)
                .ToList();

            int rank = 1;
            foreach (var item in ranking)
            {
                Console.WriteLine(
                    $"{rank,2}. {item.Product.Code,-6} {item.Product.Name,-12} " +
                    $"Sold:{item.UnitsSold,-4} Revenue:Rp{item.Revenue:N0}");
                rank++;
            }

            var topSeller = ranking.First();
            Console.WriteLine(
                $"Terlaris: {topSeller.Product.Name} ({topSeller.UnitsSold} unit terjual).");
        }

        // FITUR 2: Total penjualan per jenis produk (Electronic/Clothing/Food).
        public void PrintSalesByType()
        {
            Console.WriteLine();
            Console.WriteLine("=== SALES PER PRODUCT TYPE ===");

            if (lines.Count == 0)
            {
                Console.WriteLine("No sales recorded yet.");
                return;
            }

            // LINQ: kelompokkan per nama jenis (nama class), jumlahkan unit & omzet.
            var byType = lines
                .GroupBy(line => line.Product.GetType().Name)
                .Select(group => new
                {
                    Type = group.Key,
                    UnitsSold = group.Sum(line => line.Quantity),
                    Revenue = group.Sum(line => line.Revenue)
                })
                .OrderByDescending(item => item.Revenue)
                .ToList();

            foreach (var item in byType)
            {
                Console.WriteLine(
                    $"{item.Type,-11} Units:{item.UnitsSold,-4} Total:Rp{item.Revenue:N0}");
            }

            Console.WriteLine($"Grand Total: Rp{lines.Sum(line => line.Revenue):N0}");
        }
    }
}
