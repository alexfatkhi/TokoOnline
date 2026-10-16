
using TokoOnline_New.AProduct;
using System;
using TokoOnline_New.BCart_Checkout;
using TokoOnline_New.Exceptions;
using TokoOnline_New.TokoOnline_New.BCart_Checkout;

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

            //simpan produk kereposit
            Repository<Product> repository = new Repository<Product>();

            for (int i = 0; i < products.Length; i++)
            {
                repository.Add(products[i]);
            }


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

            Store store = new Store(repository);
            Cart cart = new Cart(buyer);

            string[] availableVouchers =
            {
                "HEMAT10",
                "BELANJA10",
                "ILHAM10"
            };

            HashSet<string> usedVouchers = new HashSet<string>();

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

            Queue<Order> warehouseQueue = new Queue<Order>();
            int orderNumber = 1;

            while (isRunning)
            {
                Console.WriteLine();
                Console.WriteLine("=== MAIN MENU ===");
                Console.WriteLine("1. View Catalog");
                Console.WriteLine("2. Add Product to Cart");
                Console.WriteLine("3. View Cart");
                Console.WriteLine("4. Checkout");
                Console.WriteLine("5. Return");
                Console.WriteLine("6. Process Warehouse Order");
                Console.WriteLine("7. Exit");
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
                            try
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
                            catch (OutOfStockException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (ExpiredProductException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (CartFullException ex)
                            {
                                Console.WriteLine(ex.Message);
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
                        Console.Write("Enter voucher code (press Enter to skip): ");
                        string voucherCode = (Console.ReadLine() ?? "").Trim().ToUpper();

                        bool voucherValid = false;
                        bool voucherUsed = false;

                        if (voucherCode != "")
                        {
                            for (int i = 0; i < availableVouchers.Length; i++)
                            {
                                if (availableVouchers[i] == voucherCode)
                                {
                                    voucherValid = true;
                                    break;
                                }
                            }

                            if (usedVouchers.Contains(voucherCode))
                            {
                                voucherUsed = true;
                            }

                            if (!voucherValid)
                            {
                                Console.WriteLine("Invalid voucher code.");
                            }
                            else if (voucherUsed)
                            {
                                Console.WriteLine("Voucher has already been used.");
                            }
                        }

                        int subtotal = cart.CalculateTotal();
                        int shippingCost = cart.CalculateShipping();

                        int totalPayment = cart.Checkout();

                        if (totalPayment == 0)
                        {
                            Console.WriteLine("Checkout failed or cart is empty.");
                        }
                        else
                        {
                            int discount = 0;

                            if (voucherValid && !voucherUsed)
                            {
                                discount = subtotal * 10 / 100;
                                totalPayment = totalPayment - discount;

                                usedVouchers.Add(voucherCode);

                                Console.WriteLine("Voucher applied.");
                                Console.WriteLine( "Discount: Rp" + discount.ToString("N0"));
                            }

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

                            Order order = new Order(
                                orderNumber.ToString("D3"),
                                buyer.Name,
                                totalPayment
                            );

                            warehouseQueue.Enqueue(order);
                            orderNumber++;

                            Console.WriteLine("Order added to warehouse queue.");

                            Console.WriteLine();
                            Console.WriteLine("=== CHECKOUT SUCCESSFUL ===");
                            Console.WriteLine(
                                "Order Total   : Rp" + subtotal.ToString("N0"));
                            Console.WriteLine(
                                "Shipping Cost : Rp" + shippingCost.ToString("N0"));
                            Console.WriteLine(
                                "Discount      : Rp" + discount.ToString("N0"));
                            Console.WriteLine(
                                "Final Payment : Rp" + totalPayment.ToString("N0"));
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
                    if (warehouseQueue.Count == 0)
                    {
                        Console.WriteLine("no orders queue");
                    }
                    else
                    {
                        Order order = warehouseQueue.Dequeue();

                        Console.WriteLine("Processed Order :");
                    }
                }
                else if (choice == "7")
                {
                    isRunning = false;
                    Console.WriteLine("Thank you for shopping!");
                }

            }
        }
    }
}
