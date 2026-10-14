using Project_OOP;
using Project_OOP.AProduct;
using System;
using System.Collections.Generic;

namespace ProjectOOP_TokoOnline
{
    public class Customer
    {
        public string CustomerName { get; }
        public string Address { get; }

        public Customer(string customerName, string address)
        {
            CustomerName = customerName;
            Address = address;
        }

        // Process the cart: reduce stock, print the receipt, then empty the cart.
        // Member overrides this to also award points.
        public virtual void Checkout(Cart cart)
        {
            if (cart.IsEmpty())
            {
                Console.WriteLine("Your cart is empty.");
                return;
            }

            List<CartItem> lines = cart.GetItems();

            decimal subtotal = cart.GetTotal();
            decimal shipping = CalculateShipping(subtotal);
            decimal totalPayment = subtotal + shipping;

            // Reduce stock for every purchased line.
            foreach (CartItem line in lines)
            {
                line.Product.ReduceStock(line.Quantity);
            }

            Console.WriteLine("===== RECEIPT =====");
            Console.WriteLine($"Customer: {CustomerName}");
            Console.WriteLine("-------------------");

            foreach (CartItem line in lines)
            {
                Console.WriteLine(
                    $"{line.ProductName} x {line.Quantity} " +
                    $"= Rp{line.GetSubTotal():N0}"
                );
            }

            Console.WriteLine("-------------------");
            Console.WriteLine($"Subtotal: Rp{subtotal:N0}");
            Console.WriteLine(
                shipping == 0m
                    ? "Shipping: Rp0 (free)"
                    : $"Shipping: Rp{shipping:N0}"
            );
            Console.WriteLine($"Total payment: Rp{totalPayment:N0}");

            cart.Clear();
        }

        // Authorise a return: only returnable categories, within 7 days.
        // The caller restores stock (restoreStock) and refunds the money.
        public bool ReturnProduct(Product product, int daySincePurchase)
        {
            if (product == null)
            {
                return false;
            }

            if (!product.CanBeReturned())
            {
                return false;
            }

            return daySincePurchase >= 0 && daySincePurchase <= 7;
        }

        // Free shipping from Rp250.000, otherwise Rp15.000. Shared with Member.
        protected decimal CalculateShipping(decimal total)
        {
            if (total >= 250000m)
            {
                return 0m;
            }

            return 15000m;
        }
    }
}
