using Project_OOP.AProduct;

namespace Project_OOP
{
    public class CartItem
    {
        public Product Product { get; }
        public int Quantity { get; private set; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public decimal Subtotal()
        {
            decimal finalPrice = Product.GetFinalPrice();
            decimal subtotal = finalPrice * Quantity;

            return subtotal;
        }

        public bool UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
            {
                return false;
            }

            if (quantity > Product.Stock)
            {
                return false;
            }

            Quantity = quantity;

            return true;
        }
    }
}
