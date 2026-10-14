using System;
using Project_OOP;
using Project_OOP.AProduct;

namespace ProjectOOP_TokoOnline
{
    // Console UI layer that wires the user to the domain classes.
    public class Menu
    {
        // One completed purchase line, kept so it can be returned later.
        private class PurchaseRecord
        {
            public Product Product { get; }
            public int Quantity { get; }
            public bool Returned { get; set; }

            public PurchaseRecord(Product product, int quantity)
            {
                Product = product;
                Quantity = quantity;
            }
        }

        private readonly Member[] registeredMembers;
        private readonly Product[] catalog;

        public Menu(Member[] registeredMembers, Product[] catalog)
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
                        cart.PrintContents();
                        break;

                    case 4:
                        Checkout(currentCustomer);
                        break;

                    case 5:
                        ReturnProduct(currentCustomer);
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

                Console.WriteLine(product.GetCatalogInfo());

                if (product is Food food)
                {
                    Console.WriteLine(
                        $"    Days until expired: {food.DaysUntilExpired}" +
                        (food.IsExpired() ? " (Expired)" : "")
                    );
                }
            }
        }

        private void AddProductToCart()
        {
            Console.Write("Enter product code: ");
            string code = (Console.ReadLine() ?? "").Trim();

            Product? selectedProduct = FindProduct(code);

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
                    "Cannot add product. Reason: insufficient stock, " +
                    "invalid quantity, expired food, or cart is full."
                );
            }
        }

        private void Checkout(Customer customer)
        {
            if (cart.IsEmpty())
            {
                Console.WriteLine("Your cart is empty.");
                return;
            }

            // Remember the purchased lines (for returns) before the cart is cleared.
            foreach (CartItem line in cart.GetItems())
            {
                purchases.Add(new PurchaseRecord(line.Product, line.Quantity));
            }

            Console.WriteLine();
            customer.Checkout(cart);
        }

        private void ReturnProduct(Customer customer)
        {
            Console.WriteLine();
            Console.WriteLine("===== PURCHASED ITEMS =====");

            bool hasReturnable = false;

            for (int i = 0; i < purchases.Count; i++)
            {
                PurchaseRecord record = purchases[i];
                string status = record.Returned ? " (Returned)" : "";

                Console.WriteLine(
                    $"{i + 1}. {record.Product.Name} x {record.Quantity} " +
                    $"[{record.Product.GetCategory()}]{status}"
                );

                if (!record.Returned)
                {
                    hasReturnable = true;
                }
            }

            if (!hasReturnable)
            {
                Console.WriteLine("No items available to return.");
                return;
            }

            Console.Write("Choose item number to return: ");

            if (!int.TryParse(Console.ReadLine(), out int itemNumber) ||
                itemNumber < 1 ||
                itemNumber > purchases.Count)
            {
                Console.WriteLine("Invalid item number.");
                return;
            }

            PurchaseRecord selected = purchases[itemNumber - 1];

            if (selected.Returned)
            {
                Console.WriteLine("This item has already been returned.");
                return;
            }

            Console.Write("Days since purchase: ");

            if (!int.TryParse(Console.ReadLine(), out int days))
            {
                Console.WriteLine("Please enter a valid number of days.");
                return;
            }

            if (customer.ReturnProduct(selected.Product, days))
            {
                // Return approved: restore stock and refund the money.
                selected.Product.RestoreStock(selected.Quantity);
                selected.Returned = true;

                decimal refund =
                    selected.Product.GetFinalPrice() * selected.Quantity;

                Console.WriteLine("Return successful.");
                Console.WriteLine($"Refund: Rp{refund:N0}");
                Console.WriteLine(
                    $"Stock of {selected.Product.Name} is now {selected.Product.Stock}."
                );
            }
            else
            {
                Console.WriteLine(
                    "Return rejected: food cannot be returned, or the " +
                    "7-day return window has passed."
                );
            }
        }

        private Product? FindProduct(string code)
        {
            for (int i = 0; i < catalog.Length; i++)
            {
                if (catalog[i].Code.Equals(
                    code,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return catalog[i];
                }
            }

            return null;
        }
    }
}
