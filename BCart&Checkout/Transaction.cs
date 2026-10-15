using Project_OOP.AProduct;
using ProjectOOP_TokoOnline;
using System;

namespace Project_OOP
{
    public class Transaction
    {
        private Product[] productsBought = new Product[10];
        private int[] quantities = new int[10];
        private decimal[] purchasePrices = new decimal[10];
        private bool[] returnedItems = new bool[10];

        public Customer Customer { get; }
        public DateTime PurchaseDate { get; }
        public int ItemCount { get; }

        public decimal Subtotal { get; }
        public decimal ShippingCost { get; }
        public decimal TotalPayment { get; }
        public int PointsEarned { get; }

        public static int TotalTransactions { get; private set; } = 0;

        public Transaction(Customer customer, CartItem[] cartItems, int itemCount, decimal shippingCost, int pointsEarned)
        {
            Customer = customer;
            PurchaseDate = DateTime.Today;
            ItemCount = itemCount;
            ShippingCost = shippingCost;
            PointsEarned = pointsEarned;

            decimal subtotal = 0;

            for (int i = 0; i < itemCount; i++)
            {
                CartItem item = cartItems[i];

                if (item != null)
                {
                    //ngambil data product dan jumlahnya
                    Product product = item.Product;
                    int quantity = item.Quantity;

                    //simpan
                    productsBought[i] = product;
                    quantities[i] = quantity;

                    purchasePrices[i] = product.GetFinalPrice();

                    decimal itemSubtotal = item.Subtotal();
                    subtotal = subtotal + itemSubtotal;
                }
            }

            Subtotal = subtotal;
            TotalPayment = Subtotal + ShippingCost;
                
            TotalTransactions++;
        }

        public bool ProcessReturn(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= ItemCount)
            {
                return false;
            }

            Product product = productsBought[itemIndex];

            if (product == null)
            {
                return false;
            }
            
            if (returnedItems[itemIndex])
            {
                return false;
            }

            int daysSincePurchase = (DateTime.Today - PurchaseDate).Days;

            if (daysSincePurchase < 0 || daysSincePurchase > 7)
            {
                return false;
            }

            bool canBeReturned = product.CanBeReturned();

            if (!canBeReturned)
            {
                return false;
            }

            int quantity = quantities[itemIndex];

            product.RestoreStock(quantity);

            returnedItems[itemIndex] = true;

            return true;
        }

        public decimal GetRefundAmount(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= ItemCount)
            {
                return 0;
            }

            if (!returnedItems[itemIndex])
            {
                return 0;
            }

            decimal price = purchasePrices[itemIndex];
            int quantity = quantities[itemIndex];

            decimal refund = price * quantity;

            return refund;
        }

        public void PrintReceipt()
        {
            Console.WriteLine("===== RECEIPT =====");
            Console.WriteLine("Customer: " + Customer.CustomerName);
            Console.WriteLine("Date: " + PurchaseDate.ToShortDateString());
            Console.WriteLine("-------------------");

            for (int i = 0; i < ItemCount; i++)
            {
                Product? product = productsBought[i];

                if (product != null)
                {
                    string productName = product.Name;
                    int quantity = quantities[i];
                    decimal price = purchasePrices[i];

                    decimal itemTotal = price * quantity;

                    string status = "";

                    if (returnedItems[i])
                    {
                        status = " (Returned)";
                    }

                    Console.WriteLine(
                        $"{productName} x {quantity} " +
                        $"= Rp{itemTotal:N0}{status}"
                    );
                }
            }
                
            Console.WriteLine("-------------------");
            Console.WriteLine($"Subtotal: Rp{Subtotal:N0}");
            Console.WriteLine($"Shipping: Rp{ShippingCost:N0}");
            Console.WriteLine($"Total payment: Rp{TotalPayment:N0}");
            Console.WriteLine($"Points earned: {PointsEarned}");
        }

    }
}
