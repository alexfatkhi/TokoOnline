using System;
using System.Collections.Generic;
using Project_OOP.AProduct;

namespace TokoOnline_New.BCart_Checkout
{
    // Satu pesanan hasil checkout yang nantinya diantrikan ke gudang.
    public class Order
    {
        public string OrderId { get; }
        public string BuyerName { get; }
        public IReadOnlyList<Product> Products { get; }
        public IReadOnlyList<int> Quantities { get; }
        public int Subtotal { get; }
        public int Shipping { get; }
        public int Discount { get; }
        public int TotalPayment { get; }
        public DateTime OrderDate { get; }

        public Order(
            string orderId,
            string buyerName,
            List<Product> products,
            List<int> quantities,
            int subtotal,
            int shipping,
            int discount)
        {
            OrderId = orderId;
            BuyerName = buyerName;
            Products = products;
            Quantities = quantities;
            Subtotal = subtotal;
            Shipping = shipping;
            Discount = discount;
            TotalPayment = subtotal + shipping - discount;
            OrderDate = DateTime.Now;
        }

        // Total unit barang (jumlah semua quanty).
        public int TotalItems()
        {
            int total = 0;

            for (int i = 0; i < Quantities.Count; i++)
            {
                total += Quantities[i];
            }

            return total;
        }
    }
}
