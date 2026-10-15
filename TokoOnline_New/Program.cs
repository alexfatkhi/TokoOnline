
using System;
using Project_OOP.AProduct;
using TokoOnline_New.BCart_Checkout;

namespace TokoOnline_New
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Buyer[] registeredBuyers = new Buyer[]
            {
                new Buyer("B001", "Fatkhi", true),
                new Buyer("B002", "Ilham", true)
            };
            Product[] products = new Product[]
            {
                new Electronic("EL001", "Mouse", 100000, 10, false),
                new Electronic("EL002", "Headset", 120000, 10, false),
                new Electronic("EL003", "Keyboard", 300000, 5, true),
                new Electronic("EL004", "Keyboard", 300000, 5, true),


                new Clothing("CL001", "T-Shirt", 80000, 20, "M", true),
                new Clothing("CL002", "Jacket", 200000, 10, "L", false),
                new Clothing("CL003", "Hoodie", 200000, 10, "L", false),
                new Clothing("CL004", "Hoodie", 200000, 10, "L", false),


                new Food("FD001", "Bread", 15000, 8, 5),
                new Food("FD002", "Milk", 20000, 6, 10),
                new Food("FD003", "Nastar", 70000, 6, 7),
                new Food("FD004", "Chicken", 65000, 6, 3),
                new Food("FD005", "Dodol", 65000, 6, 0)

            };


            Console.WriteLine("=== TOKO ONLINE ===");
            Console.Write("Enter your name: ");

            string enteredName = (Console.ReadLine() ?? "").Trim();

            while (enteredName == "")
            {
                Console.Write("Name cannot be empty. Enter your name: ");
                enteredName = (Console.ReadLine() ?? "").Trim();
            }

            Buyer buyer = null;

            for (int i = 0; i < registeredBuyers.Length; i++)
            {
                if (registeredBuyers[i].Name.ToUpper() == enteredName.ToUpper())
                {
                    buyer = registeredBuyers[i];
                    break;
                }
            }

            if (buyer == null)
            {
                buyer = new Buyer("B003", enteredName, false);

                Console.WriteLine("Welcome, " + buyer.Name);
                Console.WriteLine("Status: Regular Buyer");
            }
            else
            {
                Console.WriteLine("Welcome back, " + buyer.Name);
                Console.WriteLine("Status: Member");
                Console.WriteLine("Points: " + buyer.Points);
            }

            Store store = new Store(products);
            Cart cart = new Cart(buyer);

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine();
                Console.WriteLine("=== MAIN MENU ===");
                Console.WriteLine("1. View Catalog");
                Console.WriteLine("2. Add Product to Cart");
                Console.WriteLine("3. View Cart");
                Console.WriteLine("4. Checkout");
                Console.WriteLine("5. Exit");
                Console.Write("Choose menu: ");

                string choice = Console.ReadLine() ?? "";

                if (choice == "1")
                {
                    store.PrintCatalog();
                }
                else if (choice == "2")
                {
                    store.PrintCatalog();

                    Console.Write("Enter product code: ");
                    string code = Console.ReadLine() ?? "";

                    Product selectedProduct = store.FindProduct(code);

                    if (selectedProduct == null)
                    {
                        Console.WriteLine("Product not found.");
                    }
                    else
                    {
                        Console.Write("Enter quantity: ");
                        int quantity;

                        if (int.TryParse(Console.ReadLine(), out quantity))
                        {
                            bool added = cart.Add(selectedProduct, quantity);

                            if (added)
                            {
                                Console.WriteLine("Product added to cart.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Quantity must be a number.");
                        }
                    }
                }
                else if (choice == "3")
                {
                    cart.PrintItems();
                }
                else if (choice == "4")
                {
                    int totalPayment = cart.Checkout();

                    if (totalPayment == 0)
                    {
                        Console.WriteLine(
                            "Checkout failed. Your cart may be empty or a product is unavailable.");
                    }
                    else
                    {
                        Console.WriteLine("Checkout successful.");
                        Console.WriteLine(
                            "Total payment: Rp" + totalPayment.ToString("N0"));
                        Console.WriteLine("Points: " + buyer.Points);
                    }
                }
                else if (choice == "5")
                {
                    isRunning = false;
                    Console.WriteLine("Thank you for shopping!");
                }
                else
                {
                    Console.WriteLine("Invalid menu choice.");
                }
            }
        }
    }
}
