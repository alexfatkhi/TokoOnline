using System;

namespace Project_OOP.AProduct
{
    // Abstract parent for every product category (Electronic, Outfit, Food).
    public abstract class Product
    {
        public string Code { get; }          // immutable product code
        public string Name { get; }
        public decimal Price { get; }         // normal price, must be > 0
        public int Stock { get; private set; } // must never be negative

        // Protected constructor: only subclasses can build a Product.
        // Validation here produces the "harga 0 / stok negatif" messages.
        protected Product(string code, string name, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("Validation: product code cannot be empty.");
            }

            if (price <= 0)
            {
                throw new ArgumentException(
                    $"Validation: price of '{name}' must be greater than 0 (got {price})."
                );
            }

            if (stock < 0)
            {
                throw new ArgumentException(
                    $"Validation: stock of '{name}' cannot be negative (got {stock})."
                );
            }

            Code = code;
            Name = name;
            Price = price;
            Stock = stock;
        }

        // Overridden by each category.
        public abstract string GetCategory();
        public abstract decimal GetFinalPrice();
        public abstract bool CanBeReturned();

        // Reduce stock on checkout. Invalid amounts are ignored.
        public void ReduceStock(int qty)
        {
            if (qty <= 0 || qty > Stock)
            {
                return;
            }

            Stock -= qty;
        }

        // Restore stock on a successful return.
        public void RestoreStock(int qty)
        {
            if (qty <= 0)
            {
                return;
            }

            Stock += qty;
        }

        // Single-line catalog entry: code, name, category, normal + final price, stock.
        public string GetCatalogInfo()
        {
            return $"{Code} | {Name} | {GetCategory()} | " +
                   $"Normal: Rp{Price:N0} | Final: Rp{GetFinalPrice():N0} | " +
                   $"Stock: {Stock}";
        }
    }
}
