using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using TokoOnline_New.AProduct;
using TokoOnline_New.Exceptions;

namespace TokoOnline_New.BCart_Checkout
{
    public class Cart
    {
        public Buyer Buyer { get; private set; }
        private Product[] products = new Product[10];
        private int[] quantities = new int[10];
        private int itemCount = 0;

        public Cart(Buyer buyer)
        {
            Buyer = buyer;
        }

        public bool Add(Product product)
        {
            return Add(product, 1);
        }

        public bool Add(Product product, int qty)
        {
            if (product == null)
            {
                Console.WriteLine("Product is invalid.");
                return false;
            }

            if (qty <= 0)
            {
                Console.WriteLine("Quantity must be greater than 0.");
                return false;
            }

            if (product is Food)
            {
                Food food = (Food)product;

                if (food.DaysUntilExpiry <= 0)
                {
                    throw new ExpiredProductException();
                }
            }

            if (product.Stock <= 0)
            {
                Console.WriteLine("Product cannot be added because is out of stock.");
                return false;
            }

            if (!product.CanBuy())
            {
                Console.WriteLine("Product cannot be purchased.");
                return false;
            }

            for (int i = 0; i < itemCount; i++)
            {
                if (products[i].Code == product.Code)
                {
                    int newQuantity = quantities[i] + qty;

                    if (newQuantity > product.Stock)
                    {
                        throw new OutOfStockException(product.Stock);
                    }

                    quantities[i] = newQuantity;
                    return true;
                }
            }

            if (itemCount >= products.Length)
            {
                throw new CartFullException();
            }

            products[itemCount] = product;
            quantities[itemCount] = qty;
            itemCount++;

            return true;
        }


        public int CalculateTotal()
        {
            int total = 0;
            
            for (int i=0; i < itemCount; i++)
            {
                int subtotal = products[i].FinalPrice() * quantities[i];
                total += subtotal;
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

        public async Task<int> Checkout()
        {
            if (itemCount == 0)
            {
                return 0;
            }

            for (int i = 0; i < itemCount; i++)
            {
                if (!products[i].CanBuy())
                {
                    return 0;
                }

                if (quantities[i] > products[i].Stock)
                {
                    return 0;
                }
            }

            int subtotal = CalculateTotal();
            int shipping = CalculateShipping();
            int totalPayment = subtotal + shipping;

            for (int i = 0; i < itemCount; i++)
            {
                products[i].ReduceStock(quantities[i]);
            }
            bool paymentSuccess = await ProcessPayment();

            if (paymentSuccess == false)
            {
                // balikin stock
                for (int i = 0; i < itemCount; i++)
                {
                    products[i].AddStock(quantities[i]);
                }

                Console.WriteLine("Checkout cancelled. Stock restored.");
                return 0;
            }

            Buyer.AddPoints(subtotal);

            products = new Product[10];
            quantities = new int[10];
            itemCount = 0;

            return totalPayment;
        }

        public async Task<bool> ProcessPayment ()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            cts.CancelAfter(5000); //batas bayar 5 detik

            try
            {
                Console.WriteLine("Processing Payment...");

                await Task.Delay(7000, cts.Token); //simulasi aja

                Console.WriteLine("Payment Successfull!");
                return true;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Payment Timeout!");
                return false;
            }
            finally
            {
                cts.Dispose();
            }

        }

        public void PrintItems()
        {
            Console.WriteLine();
            Console.WriteLine("=== CART ITEMS ===");

            if (itemCount == 0)
            {
                Console.WriteLine("Cart is empty.");
                return;
            }

            for (int i = 0; i < itemCount; i++)
            {
                int subtotal =
                    products[i].FinalPrice() * quantities[i];

                Console.WriteLine(
                    products[i].Name
                    + " | Quantity: " + quantities[i]
                    + " | Price: Rp"
                    + products[i].FinalPrice().ToString("N0")
                    + " | Subtotal: Rp"
                    + subtotal.ToString("N0"));
            }

            Console.WriteLine(
                "Total: Rp" + CalculateTotal().ToString("N0"));

            Console.WriteLine(
                "Shipping: Rp" +
                CalculateShipping().ToString("N0"));
        }
    }
}