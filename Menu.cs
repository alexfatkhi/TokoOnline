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

        private Transaction?[] transactionHistory =
            new Transaction?[10];

        public Menu(
            Member[] registeredMembers,
            Product[] catalog)
        {
            this.registeredMembers = registeredMembers;
            this.catalog = catalog;
        }

        public void Start()
        {
            Customer currentCustomer = IdentifyCustomer();

            Console.WriteLine();
            Console.WriteLine(
                $"Welcome to our online shop, {currentCustomer.CustomerName}!"
            );

            bool isRunning = true;

            while (isRunning)
            {
                ShowMainMenu();

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Please enter a valid number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        ShowProducts();
                        break;

                    case 2:
                        AddProductToCart();
                        break;

                    case 3:
                        cart.ShowItems();
                        break;

                    case 4:
                        Checkout(currentCustomer);
                        break;

                    case 5:
                        ReturnProduct();
                        break;

                    case 6:
                        isRunning = false;
                        Console.WriteLine("Thank you for shopping!");
                        break;

                    default:
                        Console.WriteLine("Invalid menu choice.");
                        break;
                }
            }
        }

        private Customer IdentifyCustomer()
        {
            string name;

            do
            {
                Console.Write("Enter your name: ");
                name = (Console.ReadLine() ?? "").Trim();

                if (name == "")
                {
                    Console.WriteLine("Name cannot be empty.");
                }

            } while (name == "");

            for (int i = 0; i < registeredMembers.Length; i++)
            {
                if (registeredMembers[i].CustomerName.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase))
                {
                    Member member = registeredMembers[i];

                    Console.WriteLine("Membership status: Member");
                    Console.WriteLine($"Welcome back, {member.CustomerName}!");
                    Console.WriteLine($"Your points: {member.Point}");

                    return member;
                }
            }

            Console.WriteLine("Membership status: Regular Customer");

            Console.Write("Enter your address: ");
            string address = (Console.ReadLine() ?? "").Trim();

            return new Customer(name, address);
        }

        private void ShowMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("===== ONLINE SHOP =====");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Add to Cart");
            Console.WriteLine("3. View Cart");
            Console.WriteLine("4. Checkout");
            Console.WriteLine("5. Return Product");
            Console.WriteLine("6. Exit");
            Console.Write("Choose: ");
        }

        private void ShowProducts()
        {
            Console.WriteLine();
            Console.WriteLine("===== PRODUCT CATALOG =====");

            for (int i = 0; i < catalog.Length; i++)
            {
                Product product = catalog[i];

                Console.WriteLine(
                    $"{product.Code} | {product.ProductName} | " +
                    $"Price: Rp{product.FinalPrice():N0} | " +
                    $"Stock: {product.Stock}"
                );

                if (product is Food food)
                {
                    Console.WriteLine($"Expiry Date: {food.ExpiryDate:d}");

                    if (!food.CanBePurchased())
                    {
                        Console.WriteLine("Status: Expired");
                    }
                }
            }
        }

        private void AddProductToCart()
        {
            Console.Write("Enter product code: ");
            string code = (Console.ReadLine() ?? "").Trim();

            Product? selectedProduct = null;

            for (int i = 0; i < catalog.Length; i++)
            {
                if (catalog[i].Code.Equals(
                    code,
                    StringComparison.OrdinalIgnoreCase))
                {
                    selectedProduct = catalog[i];
                    break;
                }
            }

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

            if (cart.AddItem(selectedProduct, quantity))
            {
                Console.WriteLine("Product added to cart.");
            }
            else
            {
                Console.WriteLine(
                    "Cannot add product. Check stock, quantity, " +
                    "expiration date, or cart capacity."
                );
            }
        }

        private void Checkout(Customer customer)
        {
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

            Transaction? transaction = cart.Checkout(customer);

            if (transaction == null)
            {
                Console.WriteLine("Checkout failed.");
                return;
            }

            transactionHistory[emptyIndex] = transaction;

            Console.WriteLine("Checkout successful!");
            transaction.PrintReceipt();

            if (customer is Member member)
            {
                Console.WriteLine($"Your current points: {member.Point}");
            }
        }

        private void ReturnProduct()
        {
            bool hasTransaction = false;

            Console.WriteLine();
            Console.WriteLine("===== TRANSACTION HISTORY =====");

            for (int i = 0; i < transactionHistory.Length; i++)
            {
                Transaction? transaction = transactionHistory[i];

                if (transaction != null)
                {
                    hasTransaction = true;

                    Console.WriteLine(
                        $"{i + 1}. {transaction.Customer.CustomerName} | " +
                        $"{transaction.PurchaseDate:d} | " +
                        $"Rp{transaction.TotalPayment:N0}"
                    );
                }
            }

            if (!hasTransaction)
            {
                Console.WriteLine("No completed transactions.");
                return;
            }

            Console.Write("Choose transaction number: ");

            if (!int.TryParse(
                Console.ReadLine(),
                out int transactionNumber))
            {
                Console.WriteLine("Invalid transaction number.");
                return;
            }

            if (transactionNumber < 1 ||
                transactionNumber > transactionHistory.Length)
            {
                Console.WriteLine("Transaction not found.");
                return;
            }

            Transaction? transactionToReturn =
                transactionHistory[transactionNumber - 1];

            if (transactionToReturn == null)
            {
                Console.WriteLine("Transaction not found.");
                return;
            }

            transactionToReturn.PrintReceipt();

            Console.Write("Enter item number to return: ");

            if (!int.TryParse(
                Console.ReadLine(),
                out int itemNumber))
            {
                Console.WriteLine("Invalid item number.");
                return;
            }

            if (itemNumber < 1 ||
                itemNumber > transactionToReturn.ItemCount)
            {
                Console.WriteLine("Item not found.");
                return;
            }

            int itemIndex = itemNumber - 1;

            if (transactionToReturn.ProcessReturn(itemIndex))
            {
                decimal refund =
                    transactionToReturn.GetRefundAmount(itemIndex);

                Console.WriteLine("Return successful.");
                Console.WriteLine($"Refund: Rp{refund:N0}");
            }
            else
            {
                Console.WriteLine(
                    "Return failed. Check the return policy " +
                    "or whether the item was already returned."
                );
            }
        }
    }
}
