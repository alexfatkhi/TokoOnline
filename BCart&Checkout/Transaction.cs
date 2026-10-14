using Project_OOP.AProduct;
using ProjectOOP_TokoOnline;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_OOP
{
    public class Transaction
    {
        private Product?[] productsBought = new Product?[10];
        private int[] quantities = new int[10];
        private decimal[] purchasePrices = new decimal[10];
        private bool[] returnedItems = new bool[10];

        public Customer Customer { get; }
        public DateTime PurchaseDate { get; }
        public int ItemCount { get; }

        public decimal Subtotal { get; }
        public decimal ShippingCost { get; }
        public decimal TotalPayment { get; }
        public int PointsEarned { get; }

        public static int TotalTransactions { get; private set; } = 0;

        public Transaction( Customer customer, CartItem?[] cartItems, int itemCount, decimal shippingCost, int pointsEarned)
        {
            Customer = customer;
            PurchaseDate = DateTime.Today;
            ItemCount = itemCount;
            ShippingCost = shippingCost;
            PointsEarned = pointsEarned;

            decimal subtotal = 0m;

            for (int i = 0; i < itemCount; i++)
            {
                CartItem? item = cartItems[i];

                if (item != null)
                {
                    productsBought[i] = item.Product;
                    quantities[i] = item.Quantity;
                    purchasePrices[i] = item.Subtotal() / item.Quantity;

                    subtotal += item.Subtotal();
                }
            }

            Subtotal = subtotal;
            TotalPayment = Subtotal + ShippingCost;

            TotalTransactions++;
        }

        public void PrintReceipt()
        {
            Console.WriteLine("===== RECEIPT =====");
            Console.WriteLine("Customer: " + Customer.CustomerName);
            Console.WriteLine("Date: " + PurchaseDate.ToShortDateString());
            Console.WriteLine("-------------------");

            for (int i = 0; i < ItemCount; i++)
            {
                Product? product = productsBought[i];

                if (product != null)
                {
                    string status = returnedItems[i] ? " (Returned)" : "";

                    Console.WriteLine(
                        $"{product.ProductName} x {quantities[i]} " +
                        $"= Rp{purchasePrices[i] * quantities[i]:N0}{status}"
                    );
                }
            }

            Console.WriteLine("-------------------");
            Console.WriteLine($"Subtotal: Rp{Subtotal:N0}");
            Console.WriteLine($"Shipping: Rp{ShippingCost:N0}");
            Console.WriteLine($"Total payment: Rp{TotalPayment:N0}");
            Console.WriteLine($"Points earned: {PointsEarned}");
        }

        public bool ProcessReturn(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= ItemCount)
            {
                return false;
            }

            Product? product = productsBought[itemIndex];

            if (product == null || returnedItems[itemIndex])
            {
                return false;
            }

            int daysSincePurchase =
                (DateTime.Today - PurchaseDate).Days;

            if (!product.CanBeReturned(daysSincePurchase))
            {
                return false;
            }

            bool stockRestored = product.RestoreStock(quantities[itemIndex]);

            if (!stockRestored)
            {
                return false;
            }

            returnedItems[itemIndex] = true;

            return true;
        }

        public decimal GetRefundAmount(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= ItemCount)
            {
                return 0m;
            }

            if (!returnedItems[itemIndex])
            {
                return 0m;
            }

            return purchasePrices[itemIndex] * quantities[itemIndex];
        }
    }
}
