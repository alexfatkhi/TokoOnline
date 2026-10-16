
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

            Product[] pendingProducts = new Product[10];
            int[] pendingQuantities = new int[10];
            int pendingCount = 0;

            // Riwayat pembelian
            Product[] historyProducts = new Product[100];
            int[] historyQuantities = new int[100];
            int[] historyReturned = new int[100];
            int[] historyPrices = new int[100];
            DateTime[] historyDates = new DateTime[100];

            int historyCount = 0;

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine();
                Console.WriteLine("=== MAIN MENU ===");
                Console.WriteLine("1. View Catalog");
                Console.WriteLine("2. Add Product to Cart");
                Console.WriteLine("3. View Cart");
                Console.WriteLine("4. Checkout");
                Console.WriteLine("5. Return");
                Console.WriteLine("6. Exit");
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
                                bool alreadyExists = false;

                                for (int i = 0; i < pendingCount; i++)
                                {
                                    if (pendingProducts[i].Code == selectedProduct.Code)
                                    {
                                        pendingQuantities[i] =
                                            pendingQuantities[i] + quantity;

                                        alreadyExists = true;
                                        break;
                                    }
                                }

                                if (!alreadyExists)
                                {
                                    pendingProducts[pendingCount] = selectedProduct;
                                    pendingQuantities[pendingCount] = quantity;
                                    pendingCount++;
                                }

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
                    if (historyCount + pendingCount > historyProducts.Length)
                    {
                        Console.WriteLine("Purchase history is full.");
                    }
                    else
                    {
                        int subtotal = cart.CalculateTotal();
                        int shippingCost = cart.CalculateShipping();

                        int totalPayment = cart.Checkout();

                        if (totalPayment == 0)
                        {
                            Console.WriteLine(
                                "Checkout failed or cart is empty.");
                        }
                        else
                        {
                            // Simpan riwayat pembelian
                            for (int i = 0; i < pendingCount; i++)
                            {
                                historyProducts[historyCount] =
                                    pendingProducts[i];

                                historyQuantities[historyCount] =
                                    pendingQuantities[i];

                                historyReturned[historyCount] = 0;

                                historyPrices[historyCount] =
                                    pendingProducts[i].FinalPrice();

                                historyDates[historyCount] = DateTime.Today;

                                historyCount++;
                            }

                            pendingProducts = new Product[10];
                            pendingQuantities = new int[10];
                            pendingCount = 0;

                            Console.WriteLine();
                            Console.WriteLine("=== CHECKOUT SUCCESSFUL ===");
                            Console.WriteLine(
                                "Order Total : Rp" + subtotal.ToString("N0"));
                            Console.WriteLine(
                                "Shipping Cost  : Rp" + shippingCost.ToString("N0"));
                            Console.WriteLine(
                                "Subtotal   : Rp" + totalPayment.ToString("N0"));
                            Console.WriteLine("Buyer points  : " + buyer.Points);
                            Console.WriteLine("Stock has been updated.");
                        }
                    }
                }

                else if (choice == "5")
                {
                    Console.WriteLine();
                    Console.WriteLine("=== PURCHASE HISTORY ===");

                    if (historyCount == 0)
                    {
                        Console.WriteLine("No purchase history.");
                    }
                    else
                    {
                        for (int i = 0; i < historyCount; i++)
                        {
                            Console.WriteLine($"{historyProducts[i].Code} {historyProducts[i].Name} | Purchased: {historyQuantities[i]} | Returned: {historyReturned[i]} | Date: {historyDates[i]:dd/MM/yyyy}");
                        }

                        Console.Write("Enter product code to return: ");
                        string code = (Console.ReadLine() ?? "").Trim();

                        bool foundPurchase = false;
                        bool hasUnreturned = false;
                        bool foodCannotReturn = false;
                        bool periodExpired = false;
                        bool refundDone = false;

                        for (int i = 0; i < historyCount; i++)
                        {
                            if (historyProducts[i].Code != code)
                            {
                                continue;
                            }

                            foundPurchase = true;

                            if (historyReturned[i] >= historyQuantities[i])
                            {
                                continue;
                            }

                            hasUnreturned = true;

                            if (!(historyProducts[i] is IReturnable))
                            {
                                foodCannotReturn = true;
                                continue;
                            }

                            int days =
                                (DateTime.Today - historyDates[i]).Days;

                            IReturnable returnable =
                                (IReturnable)historyProducts[i];

                            if (!returnable.CanReturn(days))
                            {
                                periodExpired = true;
                                continue;
                            }

                            int refundAmount =
                                store.Refund(historyProducts[i], days);

                            if (refundAmount > 0)
                            {
                                historyReturned[i]++;

                                Console.WriteLine("Return successful.");
                                Console.WriteLine(
                                    "Refund amount: Rp" +
                                    historyPrices[i].ToString("N0"));
                                Console.WriteLine("Product stock has been restored.");

                                refundDone = true;
                                break;
                            }
                        }

                        if (!refundDone)
                        {
                            if (!foundPurchase)
                            {
                                Console.WriteLine("Product was never purchased.");
                            }
                            else if (!hasUnreturned)
                            {
                                Console.WriteLine("All purchased units were already returned.");
                            }
                            else if (foodCannotReturn)
                            {
                                Console.WriteLine("Food cannot be returned.");
                            }
                            else if (periodExpired)
                            {
                                Console.WriteLine("Return period has expired.");
                            }
                            else
                            {
                                Console.WriteLine("Refund rejected.");
                            }
                        }
                    }
                }
                else if (choice == "6")
                {
                    isRunning = false;
                    Console.WriteLine("Thank you for shopping!");
                }

            }
        }
    }
}
