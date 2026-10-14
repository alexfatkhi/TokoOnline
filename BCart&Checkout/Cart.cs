using Project_OOP.AProduct;
using System;
using System.Collections.Generic;

namespace Project_OOP
{
    public class Cart
    {
        public const int MAX_ITEMS = 10;

        private readonly CartItem?[] items = new CartItem?[MAX_ITEMS];
        private int itemCount = 0;

        // Add a product to the cart, or return false with the reason handled by the caller:
        // null product, bad quantity, quantity over stock, expired food, or cart full.
        public bool AddItem(Product product, int quantity)
        {
            if (product == null ||
                quantity <= 0 ||
                quantity > product.Stock)
            {
                return false;
            }

            if (product is Food food && food.IsExpired())
            {
                return false;
            }

            // If the product is already in the cart, merge the quantities.
            for (int i = 0; i < itemCount; i++)
            {
                CartItem? currentItem = items[i];

                if (currentItem != null &&
                    currentItem.Product.Code == product.Code)
                {
                    int newQuantity = currentItem.Quantity + quantity;

                    if (newQuantity > product.Stock)
                    {
                        return false;
                    }

                    return currentItem.UpdateQuantity(newQuantity);
                }
            }

            if (itemCount >= items.Length)
            {
                return false;
            }

            items[itemCount] = new CartItem(product, quantity);
            itemCount++;

            return true;
        }

        public decimal GetTotal()
        {
            decimal total = 0m;

            for (int i = 0; i < itemCount; i++)
            {
                CartItem? item = items[i];

                if (item != null)
                {
                    total += item.GetSubTotal();
                }
            }

            return total;
        }

        public bool IsEmpty()
        {
            return itemCount == 0;
        }

        // Snapshot of the active cart lines, used by Customer.Checkout.
        public List<CartItem> GetItems()
        {
            List<CartItem> result = new List<CartItem>();

            for (int i = 0; i < itemCount; i++)
            {
                CartItem? item = items[i];

                if (item != null)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public void PrintContents()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Your cart is empty.");
                return;
            }

            Console.WriteLine("=== Shopping Cart ===");

            for (int i = 0; i < itemCount; i++)
            {
                CartItem? item = items[i];

                if (item != null)
                {
                    Console.WriteLine(
                        $"{i + 1}. {item.ProductName} " +
                        $"x {item.Quantity} = Rp{item.GetSubTotal():N0}"
                    );
                }
            }

            Console.WriteLine($"Running total: Rp{GetTotal():N0}");
        }

        // Empty the cart after a successful checkout.
        public void Clear()
        {
            Array.Clear(items, 0, items.Length);
            itemCount = 0;
        }
    }
}
