using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TokoOnline_New.BCart_Checkout;
using TokoOnline_New.TokoOnline_New.BCart_Checkout;

namespace TokoOnline_New
{
    public class Asynchronous
    {
        public static async Task VerifyPaymentAsync(Order order)
        {
            Console.WriteLine($"Order {order.OrderId}: Verifying payment...");
            await Task.Delay(1000);
            Console.WriteLine("Payment verified.");
        }

        public static async Task PackOrderAsync(Order order)
        {
            Console.WriteLine($"Order {order.OrderId}: Packing order...");
            await Task.Delay(2000);
            Console.WriteLine("Order packed.");
        }

        public static async Task ShipOrderAsync(Order order)
        {
            Console.WriteLine($"Order {order.OrderId}: Shipping order...");
            await Task.Delay(1000);
            Console.WriteLine("Order shipped.");
        }

        public static async Task WarehouseAsync(Queue<Order> warehouseQueue)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (warehouseQueue.Count > 0)
            {
                Order order = warehouseQueue.Dequeue();

                await VerifyPaymentAsync(order);
                await PackOrderAsync(order);
                await ShipOrderAsync(order);

                Console.WriteLine($"Order {order.OrderId} completed.");
                Console.WriteLine();
            }

            stopwatch.Stop();

            Console.WriteLine(
                $"Total processing time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}