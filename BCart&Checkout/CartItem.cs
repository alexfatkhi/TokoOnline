using Project_OOP.AProduct;

namespace Project_OOP
{
    // One line in the cart: a product plus the quantity ordered.
    public class CartItem
    {
        public Product Product { get; }
        public int Quantity { get; private set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public string ProductName => Product.Name;

        // Subtotal = final price (after promo / warranty) x quantity.
        public decimal GetSubTotal()
        {
            return Product.GetFinalPrice() * Quantity;
        }

        public bool UpdateQuantity(int quantity)
        {
            if (quantity <= 0 || quantity > Product.Stock)
            {
                return false;
            }

            Quantity = quantity;
            return true;
        }
    }
}
