using System;
using System.Collections.Generic;
using Project_OOP.AProduct;
using TokoOnline_New.CException;

namespace TokoOnline_New.BCart_Checkout
{
    public class Cart
    {
        // Batas jenis produk dalam keranjang.
        public const int MaxItemTypes = 10;

        public Buyer Buyer { get; private set; }

        private readonly List<Product> products = new List<Product>();
        private readonly List<int> quantities = new List<int>();

        public Cart(Buyer buyer)
        {
            Buyer = buyer;
        }

        public int ItemCount => products.Count;
        public bool IsEmpty => products.Count == 0;

        public void Add(Product product)
        {
            Add(product, 1);
        }

        // Menambah produk ke keranjang eranjang.
        // Menthrow:
        // - ArgumentException        
        // - ExpiredProductException  
        // - OutOfStockException      
        // - CartFullException        
        public void Add(Product product, int qty)
        {
            if (product == null)
            {
                throw new ArgumentException("Product is invalid.");
            }

            if (qty <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0.");
            }

            // Makanan kedaluwarsa tidak boleh dibeli.
            if (product is Food food && food.DaysUntilExpiry <= 0)
            {
                throw new ExpiredProductException(
                    $"'{product.Name}' is expired and cannot be purchased.");
            }

            // Produk habis stok.
            if (product.Stock <= 0)
            {
                throw new OutOfStockException(
                    $"'{product.Name}' is out of stock.");
            }

            // Kalau produk sudah ada di keranjang, cukup tambah qty-nya.
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Code == product.Code)
                {
                    int newQuantity = quantities[i] + qty;

                    if (newQuantity > product.Stock)
                    {
                        throw new OutOfStockException(
                            $"Not enough stock for '{product.Name}'. Available: {product.Stock - quantities[i]}.");
                    }

                    quantities[i] = newQuantity;
                    return;
                }
            }

            // Produk baru butuh slot baru -> cek keranjang penuh dulu.
            if (products.Count >= MaxItemTypes)
            {
                throw new CartFullException(
                    $"Cart is full. Maximum {MaxItemTypes} product types allowed.");
            }

            if (qty > product.Stock)
            {
                throw new OutOfStockException(
                    $"Not enough stock for '{product.Name}'. Available: {product.Stock}.");
            }

            products.Add(product);
            quantities.Add(qty);
        }

        public int CalculateTotal()
        {
            int total = 0;

            for (int i = 0; i < products.Count; i++)
            {
                total += products[i].FinalPrice() * quantities[i];
            }

            return total;
        }

        public int CalculateShipping()
        {
            int subtotal = CalculateTotal();

            if (subtotal == 0 || subtotal >= 250000)
            {
                return 0;
            }

            return 15000;
        }

        // Checkout -> hasilkan Order (buat diantrikan ke gudang), lalu kosongkan keranjang.
        // null kalau keranjang kosong.
        public Order Checkout(string orderId, int discount)
        {
            if (products.Count == 0)
            {
                return null;
            }

            // Pastikan semua item masih bisa dibeli saat checkout.
            for (int i = 0; i < products.Count; i++)
            {
                if (!products[i].CanBuy())
                {
                    throw new OutOfStockException(
                        $"'{products[i].Name}' is out of stock.");
                }

                if (quantities[i] > products[i].Stock)
                {
                    throw new OutOfStockException(
                        $"Not enough stock for '{products[i].Name}'. Available: {products[i].Stock}.");
                }
            }

            int subtotal = CalculateTotal();
            int shipping = CalculateShipping();

            if (discount < 0)
            {
                discount = 0;
            }

            if (discount > subtotal)
            {
                discount = subtotal;
            }

            // Snapshot item untuk disimpan di Order sebelum keranjang dikosongkan.
            List<Product> orderedProducts = new List<Product>(products);
            List<int> orderedQuantities = new List<int>(quantities);

            for (int i = 0; i < products.Count; i++)
            {
                products[i].ReduceStock(quantities[i]);
            }

            Buyer.AddPoints(subtotal);

            Order order = new Order(
                orderId,
                Buyer.Name,
                orderedProducts,
                orderedQuantities,
                subtotal,
                shipping,
                discount);

            products.Clear();
            quantities.Clear();

            return order;
        }

        public void PrintItems()
        {
            Console.WriteLine();
            Console.WriteLine("=== CART ITEMS ===");

            if (products.Count == 0)
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            for (int i = 0; i < products.Count; i++)
            {
                int subtotal = products[i].FinalPrice() * quantities[i];

                Console.WriteLine(
                    products[i].Name
                    + " | Quantity: " + quantities[i]
                    + " | Price: Rp" + products[i].FinalPrice().ToString("N0")
                    + " | Subtotal: Rp" + subtotal.ToString("N0"));
            }

            Console.WriteLine("Total: Rp" + CalculateTotal().ToString("N0"));
            Console.WriteLine("Shipping: Rp" + CalculateShipping().ToString("N0"));
        }
    }
}
