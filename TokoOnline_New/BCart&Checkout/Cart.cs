using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace TokoOnline_New.BCart_Checkout
{
    public class Cart
    {
        public Buyer Buyer { get; private set }
        private Product[] product = new Product[10];
        private int[] quantities = new int[10];
        private int itemCount = 0;

        public bool Add(Product product)
        {
            return Add(product, 1);
        }

        public bool Add(Product product, int qty)
        {
            if (product == null || qty <= 0)
            {
                return false;
            }

            if (!product.CanBuy())
            {
                return false;
            }

            if (qty > product.Stock)
            {
                return false;
            }

            for (int i = 0; i < itemCount; i++)
            {
                if (products[i].Code == product.Code)
                {
                    int newQuantity = quantities[i] + qty;

                    if (newQuantity > product.Stock)
                    {
                        return false;
                    }

                    quantities[i] = newQuantity;
                    return true;
                }
            }

            if (itemCount >= products.Length)
            {
                return false;
            }

            products[itemCount] = product;
            quantities[itemCount] = qty;
            itemCount++;

            return true;
        }

        public int CalculateTotal()
        {
            int total = 0;
            
            for (int i=0; i < itemCount; i++)
            {
                int subtotal = products[i].FinalPrice() * quantities[i];
                total += subtotal;
            }

            return total;

        }

        public int CalculateShipping()
        {
            int subtotal = CalculateTotal();

            if (subtotal == 0 || subtotal >= 250000)
            {
                return 0;
            }

            return 15000;

        }

        public int Checkout()
        {
            if (itemCount == 0)
            {
                return 0;
            }

            for (int i = 0; i < itemCount; i++)
            {

                
            }
        }

        public void PrintItems()
        {

        }


    }
}
