
using System;
using System.Threading.Tasks;
using Project_OOP.AProduct;
using TokoOnline_New.BCart_Checkout;
using TokoOnline_New.CException;

namespace TokoOnline_New
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Buyer[] registeredBuyers = new Buyer[]
            {
                new Buyer("B001", "Fatkhi", true),
                new Buyer("B002", "Ilham", true)
            };

            // Produk dimasukkan ke Repository<Product> (List + Dictionary).
            Repository<Product> repository = new Repository<Product>();

            Product[] seedProducts = new Product[]
            {
                new Electronic("EL001", "Mouse", 100000, 10, false),
                new Electronic("EL002", "Headset", 120000, 10, false),
                new Electronic("EL003", "Keyboard", 300000, 4, true),
                new Electronic("EL004", "Keyboard", 300000, 5, true),

                new Clothing("CL001", "T-Shirt", 80000, 20, "M", true),
                new Clothing("CL002", "Jacket", 200000,2, "L", false),
                new Clothing("CL003", "Hoodie", 200000, 10, "L", false),
                new Clothing("CL004", "Hoodie", 200000, 10, "L", false),

                new Food("FD001", "Bread", 15000, 8, 5),
                new Food("FD002", "Milk", 20000, 6, 10),
                new Food("FD003", "Nastar", 70000, 6, 7),
                new Food("FD004", "Chicken", 65000, 2, 3),
                new Food("FD005", "Dodol", 65000, 6, 0)
            };

            foreach (Product product in seedProducts)
            {
                repository.Add(product);
            }

            // Daftar kode voucher diskon 10%
            VoucherService voucherService = new VoucherService(
                new string[] { "HEMAT10", "DISKON10", "GUDANG10" });

            // Gudang menampung pesanan dalam Queue (FIFO).
            Warehouse warehouse = new Warehouse();

            // Laporan penjualan (produk terlaris & total penjualan per jenis, pakai LINQ).
            SalesReport salesReport = new SalesReport();

            Console.WriteLine("====== TOKO ONLINE =======");
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

            int orderSequence = 1;

            // Riwayat pembelian (untuk fitur retur).
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
                Console.WriteLine("6. Process Warehouse Queue");
                Console.WriteLine("7. Product terLaris");
                Console.WriteLine("8. Total penjualan pertipe product");
                Console.WriteLine("9. Products dg Stock < 5");
                Console.WriteLine("10. Exit");
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
                            // Tangkap exception yang dilempar keranjang.
                            try
                            {
                                cart.Add(selectedProduct, quantity);
                                Console.WriteLine("Product added to cart.");
                            }
                            // makanan kedaluwarsa (custom)
                            catch (ExpiredProductException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            // stok habis / qty melebihi stok (custom)
                            catch (OutOfStockException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            // keranjang penuh (custom)
                            catch (CartFullException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            // produk null / qty <= 0
                            catch (ArgumentException ex)
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
                    if (cart.IsEmpty)
                    {
                        Console.WriteLine("Cart is empty.");
                    }
                    else if (historyCount + cart.ItemCount > historyProducts.Length)
                    {
                        Console.WriteLine("Purchase history is full.");
                    }
                    else
                    {
                        int subtotal = cart.CalculateTotal();

                        // Voucher diskon 10% (opsional, sekali pakai per kode).
                        Console.Write("Enter voucher code (biarkan kosong jika mau diskip): ");
                        string voucherCode = (Console.ReadLine() ?? "").Trim();

                        int discount = 0;
                        if (voucherService.TryRedeem(
                                voucherCode, subtotal, out int appliedDiscount, out string voucherMessage))
                        {
                            discount = appliedDiscount;
                        }
                        Console.WriteLine(voucherMessage);

                        string orderId = $"ORD-{orderSequence:000}";

                        Order order = null;

                        try
                        {
                            order = cart.Checkout(orderId, discount);
                        }
                        catch (OutOfStockException ex)
                        {
                            Console.WriteLine("Checkout failed: " + ex.Message);
                        }

                        if (order == null)
                        {
                            Console.WriteLine("Checkout failed or cart is empty.");
                        }
                        else
                        {
                            orderSequence++;

                            // Pesanan masuk antrian gudang.
                            warehouse.ReceiveOrder(order);

                            // Catat penjualan untuk laporan (LINQ).
                            salesReport.Record(order);

                            // Simpan riwayat pembelian dari isi pesanan.
                            for (int i = 0; i < order.Products.Count; i++)
                            {
                                historyProducts[historyCount] = (Product)order.Products[i];
                                historyQuantities[historyCount] = order.Quantities[i];
                                historyReturned[historyCount] = 0;
                                historyPrices[historyCount] = order.Products[i].FinalPrice();
                                historyDates[historyCount] = DateTime.Today;
                                historyCount++;
                            }

                            Console.WriteLine();
                            Console.WriteLine("=== CHECKOUT SUCCESSFUL ===");
                            Console.WriteLine("Order ID     : " + order.OrderId);
                            Console.WriteLine("Order Total  : Rp" + order.Subtotal.ToString("N0"));
                            Console.WriteLine("Discount     : Rp" + order.Discount.ToString("N0"));
                            Console.WriteLine("Shipping Cost: Rp" + order.Shipping.ToString("N0"));
                            Console.WriteLine("Total Payment: Rp" + order.TotalPayment.ToString("N0"));
                            Console.WriteLine("Buyer points : " + buyer.Points);
                            Console.WriteLine("Stock has been updated.");
                            Console.WriteLine(
                                "Order sent to warehouse. Orders in queue: " + warehouse.PendingCount);
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
                    Console.WriteLine();
                    Console.WriteLine("=== WAREHOUSE QUEUE (ASYNC) ===");
                    Console.WriteLine("Pending orders: " + warehouse.PendingCount);
                    Console.WriteLine();

                    // Proses antrian satu per satu secara async
                    // (verifikasi pembayaran -> pengemasan -> pengiriman).
                    await warehouse.ProcessAllAsync();
                }
                else if (choice == "7")
                {
                    salesReport.PrintBestSeller();
                }
                else if (choice == "8")
                {
                    salesReport.PrintSalesByType();
                }
                else if (choice == "9")
                {
                    store.PrintLowStock();
                }
                else if (choice == "10")
                {
                    isRunning = false;
                    Console.WriteLine("Thank you for shopping!");

                }
            }
        }
    }
}
