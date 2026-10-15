
using System;
using Project_OOP.AProduct;

namespace TokoOnline_New.BCart_Checkout
{
    public class Store
    {
        private Product[] catalog;
        private int productCount = 0;

        public Store(Product[] products)
        {
            catalog = products;
            productCount = products.Length;
        }

        public Product FindProduct(string code)
        {
            for (int i = 0; i < productCount; i++)
            {
                if (catalog[i].Code == code)
                {
                    return catalog[i];
                }
            }

            return null;
        }

        public void PrintCatalog()
        {
            Console.WriteLine("=== PRODUCT CATALOG ===");

            for (int i = 0; i < productCount; i++)
            {
                catalog[i].Info();
            }
        }

        public int Refund(Product item, int days)
        {
            if (item == null)
            {
                return 0;
            }

            if (item is IReturnable)
            {
                IReturnable returnable = (IReturnable)item;

                if (returnable.CanReturn(days))
                {
                    item.AddStock(1);
                    return item.FinalPrice();
                }
            }

            return 0;
        }
    }
}
