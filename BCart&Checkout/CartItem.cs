using Project_OOP.AProduct;
using System;
using System.Collections.Generic;
using System.Text;

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
            return Product.FinalPrice() * Quantity;
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
