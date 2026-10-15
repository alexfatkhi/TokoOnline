
using System;
using Project_OOP.AProduct;
using ProjectOOP_TokoOnline;

namespace Project_OOP
{
    public class Cart
    {
        private CartItem[] items = new CartItem[10];
        private int itemCount = 0;

        public bool AddItem(Product product, int quantity)
        {
            if (product == null || quantity <= 0 || quantity > product.Stock)
            {
                return false;
            }

            if (product is Food)
            {
                Food food = (Food)product;

                if (food.IsExpired())
                {
                    return false;
                }
            }

            for (int i = 0; i < itemCount; i++)
            {
                CartItem currentItem = items[i];

                if (currentItem != null)
                {
                    Product currentProduct = currentItem.Product;

                    if (currentProduct.Code == product.Code)
                    {
                        int newQuantity = currentItem.Quantity + quantity;

                        if (newQuantity > product.Stock)
                        {
                            return false;
                        }

                        return currentItem.UpdateQuantity(newQuantity);
                    }
                }
            }

            if (itemCount >= items.Length)
            {
                return false;
            }

            CartItem newItem = new CartItem(product, quantity);
            items[itemCount] = newItem;

            itemCount++;

            return true;
        }

        public decimal CalculateTotal()
        {
            decimal total = 0;

            for (int i = 0; i < itemCount; i++)
            {
                CartItem item = items[i];

                if (item != null)
                {
                    total = total + item.Subtotal();
                }
            }

            return total;
        }

        public decimal CalculateShipping(decimal subtotal)
        {
            if (subtotal >= 250000)
            {
                return 0;
            }

            return 15000;
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
                    Product product = item.Product;
                    string productName = product.Name;
                    decimal itemSubtotal = item.Subtotal();

                    Console.WriteLine(
                        $"{i + 1}. {productName} x {item.Quantity} " +
                        $"= Rp{itemSubtotal:N0}"
                    );
                }
            }

            Console.WriteLine($"Subtotal: Rp{CalculateTotal():N0}");
        }

        public Transaction Checkout(Customer customer)
        {
            if (itemCount == 0)
            {
                Console.WriteLine("Your cart is empty.");
                return null;
            }

            // Check all items before reducing stock.
            for (int i = 0; i < itemCount; i++)
            {
                CartItem item = items[i];

                if (item == null)
                {
                    Console.WriteLine("Invalid cart item.");
                    return null;
                }

                Product product = item.Product;

                if (item.Quantity <= 0 || item.Quantity > product.Stock)
                {
                    Console.WriteLine("Invalid quantity or insufficient stock.");
                    return null;
                }

                if (product is Food)
                {
                    Food food = (Food)product;

                    if (food.IsExpired())
                    {
                        Console.WriteLine("A food product has expired.");
                        return null;
                    }
                }
            }

            decimal subtotal = CalculateTotal();
            decimal shippingCost = CalculateShipping(subtotal);
            int pointsEarned = 0;

            if (customer is Member)
            {
                Member member = (Member)customer;
                pointsEarned = member.CountPoint(subtotal);
            }

            for (int i = 0; i < itemCount; i++)
            {
                CartItem? item = items[i];

                if (item != null)
                {
                    Product product = item.Product;
                    product.ReduceStock(item.Quantity);
                }
            }

            Transaction newTransaction = new Transaction(
                customer,
                items,
                itemCount,
                shippingCost,
                pointsEarned
            );

            if (customer is Member)
            {
                Member member = (Member)customer;
                member.AddPoint(subtotal);
            }
            //kosongin cart abis checkout

            items = new CartItem[10];
            itemCount = 0;

            //dilemparr buat ke menu
            return newTransaction;
        }
    }
}
