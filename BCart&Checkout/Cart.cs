using Project_OOP.AProduct;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project_OOP
{
    public class Cart
    {
        private CartItem?[] items = new CartItem?[10];
        private int itemCount = 0;

        public bool AddItem(Product product, int quantity)
        {
            if (product == null || quantity <= 0 || quantity > product.Stock)
            {
                return false;
            }

            if (product is Food food && !food.CanBePurchased())
            {
                return false;
            }

            for (int i = 0; i < itemCount; i++)
            {
                CartItem? currentItem = items[i];

                if (currentItem != null && currentItem.Product.Code == product.Code)
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

        public decimal CalculateTotal()
        {
            decimal total = 0m;

            for (int i = 0; i < itemCount; i++)
            {
                CartItem? item = items[i];

                if (item != null)
                {
                    total += item.Subtotal();
                }
            }

            return total;
        }

        public decimal CalculateShipping(decimal subtotal)
        {
            if (subtotal >= 250000m)
            {
                return 0m;
            }
            else
            {
                return 15000m;
            }
        }

        public void ShowItems()
        {
            if (itemCount == 0)
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
                        $"{i + 1}. {item.Product.ProductName} " +
                        $"x {item.Quantity} = Rp{item.Subtotal():N0}"
                    );
                }
            }

            Console.WriteLine($"Subtotal: Rp{CalculateTotal():N0}");
        }

        public Transaction? Checkout(Customer customer)
        {
            if (itemCount == 0)
            {
                Console.WriteLine("Your cart is empty.");
                return null;
            }

            // Validate all items before changing stock
            for (int i = 0; i < itemCount; i++)
            {
                CartItem? item = items[i];

                if (item == null ||
                    item.Quantity <= 0 ||
                    item.Quantity > item.Product.Stock)
                {
                    Console.WriteLine("Invalid item or insufficient stock.");
                    return null;
                }

                if (item.Product is Food food &&
                    !food.CanBePurchased())
                {
                    Console.WriteLine("A food product has expired.");
                    return null;
                }
            }

            decimal subtotal = CalculateTotal();
            decimal shippingCost = CalculateShipping(subtotal);
            int pointsEarned = 0;

            if (customer is Member memberForPoints)
            {
                pointsEarned = memberForPoints.CountPoint(subtotal);
            }

            // Reduce stock after all validations succeed
            for (int i = 0; i < itemCount; i++)
            {
                CartItem? item = items[i];

                if (item != null)

                if (item != null)
                {
                    item.Product.ReduceStock(item.Quantity);
                }
            }

            // Create a record of the completed purchase
            Transaction transaction = new Transaction(
                customer,
                items,
                itemCount,
                shippingCost,
                pointsEarned
            );

            // Award points only after successful checkout
            if (customer is Member memberToReward)
            {
                memberToReward.AddPoint(subtotal);
            }

            // Empty the cart after checkout
            Array.Clear(items, 0, items.Length);
            itemCount = 0;

            return transaction;
        }
    }
}
