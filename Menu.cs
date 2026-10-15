
using System;
using Project_OOP;
using Project_OOP.AProduct;

namespace ProjectOOP_TokoOnline
{
    public class Menu
    {
        private Member[] registeredMembers;
        private Product[] catalog;

        private Cart cart = new Cart();

        private Transaction[] transactionHistory = new Transaction[10];

        public Menu(Member[] registeredMembers, Product[] catalog)
        {
            this.registeredMembers = registeredMembers;
            this.catalog = catalog;
        }

        public void Start()
        {
            Console.Clear();

            Customer customer = IdentifyCustomer();

            Console.WriteLine();
            Console.WriteLine(
                "Welcome to our online shop, " + customer.CustomerName + "!"
            );

            WaitForUser();

            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();

                ShowMainMenu();

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Please enter a valid number.");
                    WaitForUser();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ShowProducts();
                        WaitForUser();
                        break;

                    case 2:
                        AddProductToCart();
                        WaitForUser();
                        break;

                    case 3:
                        cart.ShowItems();
                        WaitForUser();
                        break;

                    case 4:
                        Checkout(customer);
                        WaitForUser();
                        break;

                    case 5:
                        ReturnProduct();
                        WaitForUser();
                        break;

                    case 6:
                        isRunning = false;
                        Console.WriteLine("Thank you for shopping!");
                        break;

                    default:
                        Console.WriteLine("Invalid menu choice.");
                        WaitForUser();
                        break;
                }
            }
        }

        private Customer IdentifyCustomer()
        {
            string name = "";

            while (name == "")
            {
                Console.Write("Enter your name: ");
                name = (Console.ReadLine() ?? "").Trim();

                if (name == "")
                {
                    Console.WriteLine("Name cannot be empty.");
                }
            }

            for (int i = 0; i < registeredMembers.Length; i++)
            {
                //narik object member sementara
                Member member = registeredMembers[i];

                if (member.CustomerName == name)
                {
                    Console.WriteLine("Membership status: Member");
                    Console.WriteLine("Welcome back, " + member.CustomerName + "!");
                    Console.WriteLine("Your points: " + member.Point);

                    return member;
                }
            }

            Console.WriteLine("Membership status: Regular Customer");

            Console.Write("Enter your address: ");
            string address = (Console.ReadLine() ?? "").Trim();

            Customer newCustomer = new Customer(name, address);

            return newCustomer;
        }

        private void ShowMainMenu()
        {
            Console.WriteLine("========== ONLINE SHOP ==========");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Add Product to Cart");
            Console.WriteLine("3. View Cart");
            Console.WriteLine("4. Checkout");
            Console.WriteLine("5. Return Product");
            Console.WriteLine("6. Exit");
            Console.WriteLine("=================================");
            Console.Write("Choose: ");
        }

        private void ShowProducts()
        {
            Console.WriteLine();
            Console.WriteLine("========== PRODUCT CATALOG ==========");

            for (int i = 0; i < catalog.Length; i++)
            {
                // ngambil object product
                Product product = catalog[i];

                Console.WriteLine(product.GetCatalogInfo());

                if (product is Food)
                {
                    Food food = (Food)product;

                    Console.WriteLine(
                        "Days until expired: " + food.DaysUntilExpired
                    );

                    if (food.IsExpired())
                    {
                        Console.WriteLine("Status: Expired");
                    }
                }
            }
        }

        private void AddProductToCart()
        {
            ShowProducts();

            Console.WriteLine();
            Console.Write("Enter product code: ");
            string code = (Console.ReadLine() ?? "").Trim();

            Product selectedProduct = FindProduct(code);

            if (selectedProduct == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }

            Console.Write("Enter quantity: ");

            if (!int.TryParse(Console.ReadLine(), out int quantity))
            {
                Console.WriteLine("Quantity must be a number.");
                return;
            }

            bool added = cart.AddItem(selectedProduct, quantity);

            if (added)
            {
                Console.WriteLine("Product added to cart.");
            }
            else
            {
                Console.WriteLine(
                    "Cannot add product. Check quantity, stock, " +
                    "expiration date, or cart capacity."
                );
            }
        }

        private void Checkout(Customer customer)
        {
            decimal subtotal = cart.CalculateTotal();

            if (subtotal == 0)
            {
                Console.WriteLine("Your cart is empty.");
                return;
            }

            int emptyIndex = -1;

            for (int i = 0; i < transactionHistory.Length; i++)
            {
                if (transactionHistory[i] == null)
                {
                    emptyIndex = i;
                    break;
                }
            }

            if (emptyIndex == -1)
            {
                Console.WriteLine("Transaction history is full.");
                return;
            }

            Transaction newTransaction = cart.Checkout(customer);

            if (newTransaction == null)
            {
                Console.WriteLine("Checkout failed. Please check the errors above.");
                return;
            }

            transactionHistory[emptyIndex] = newTransaction;

            Console.WriteLine();
            Console.WriteLine("Checkout successful!");

            newTransaction.PrintReceipt();

            if (customer is Member)
            {
                Member member = (Member)customer;

                Console.WriteLine("Your total points: " + member.Point);
            }

            Console.WriteLine();
            Console.WriteLine("Stock has been updated.");
        }

        private void ReturnProduct()
        {
            Console.WriteLine();
            Console.WriteLine("========== TRANSACTION HISTORY ==========");

            bool hasTransaction = false;

            for (int i = 0; i < transactionHistory.Length; i++)
            {
                Transaction transaction = transactionHistory[i];

                if (transaction != null)
                {
                    hasTransaction = true;

                    Customer customer = transaction.Customer;
                    string customerName = customer.CustomerName;

                    DateTime purchaseDate = transaction.PurchaseDate;
                    decimal totalPayment = transaction.TotalPayment;

                    Console.WriteLine(
                        $"{i + 1}. {customerName} | {purchaseDate:dd/MM/yyyy} | Rp{totalPayment:N0}"
                    );
                }
            }

            if (!hasTransaction)
            {
                Console.WriteLine("No completed transactions.");
                return;
            }

            Console.WriteLine();
            Console.Write("Choose transaction number: ");

            if (!int.TryParse( Console.ReadLine(), out int transactionNumber))
            {
                Console.WriteLine("Invalid transaction number.");
                return;
            }

            if (transactionNumber < 1 || transactionNumber > transactionHistory.Length)
            {
                Console.WriteLine("Transaction not found.");
                return;
            }

            Transaction selectedTransaction = transactionHistory[transactionNumber - 1];

            if (selectedTransaction == null)
            {
                Console.WriteLine("Transaction not found.");
                return;
            }

            Console.WriteLine();

            selectedTransaction.PrintReceipt();

            Console.WriteLine();
            Console.WriteLine(
                "Enter the item number shown on the receipt."
            );

            Console.Write("Item number to return: ");

            if (!int.TryParse(Console.ReadLine(), out int itemNumber))
            {
                Console.WriteLine("Invalid item number.");
                return;
            }

            if (itemNumber < 1 ||
                itemNumber > selectedTransaction.ItemCount)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            int itemIndex = itemNumber - 1;

            bool returnSuccessful =
                selectedTransaction.ProcessReturn(itemIndex);

            if (returnSuccessful)
            {
                decimal refund =
                    selectedTransaction.GetRefundAmount(itemIndex);

                Console.WriteLine();
                Console.WriteLine("Return successful.");
                Console.WriteLine("Refund: Rp" + refund.ToString("N0"));
                Console.WriteLine("Product stock has been restored.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Return rejected. The product may not be returnable, " +
                    "the 7-day period may have passed, or it was already returned."
                );
            }
        }

        private Product? FindProduct(string code)
        {
            for (int i = 0; i < catalog.Length; i++)
            { 
                //merujuk ke satu objectnyh
                Product product = catalog[i];

                if (product.Code == code)
                {
                    return product;
                }
            }

            return null;
        }

        private void WaitForUser()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
