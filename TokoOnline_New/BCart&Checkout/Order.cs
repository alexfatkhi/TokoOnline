using System;
using System.Collections.Generic;
using System.Text;

namespace TokoOnline_New
{
    using System;

    namespace TokoOnline_New.BCart_Checkout
    {
        public class Order
        {
            public string OrderId { get; private set; }
            public string BuyerName { get; private set; }
            public int TotalPayment { get; private set; }

            public Order(string orderId, string buyerName, int totalPayment)
            {
                OrderId = orderId;
                BuyerName = buyerName;
                TotalPayment = totalPayment;
            }

            public void PrintInfo()
            {
                Console.WriteLine(
                    OrderId + " | " + BuyerName + " | Rp" + TotalPayment.ToString("N0"));
            }
        }
    }
}
