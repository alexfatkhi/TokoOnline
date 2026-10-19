using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TokoOnline_New.BCart_Checkout
{
    // Gudang memproses pesanan dengan prinsip FIFO lewat Queue:
    // pesanan yang lebih dulu checkout, lebih dulu diproses.
    public class Warehouse
    {
        private readonly Queue<Order> orderQueue = new Queue<Order>();

        // Jumlah pesanan yang masih menunggu diproses.
        public int PendingCount => orderQueue.Count;

        // Pesanan masuk antrian setelah checkout.
        public void ReceiveOrder(Order order)
        {
            if (order == null)
            {
                return;
            }

            orderQueue.Enqueue(order);
        }

        // Proses SATU pesanan paling depan secara ASYNC.
        // Tiga tahap dijalankan berurutan dengan await (non-blocking):
        // verifikasi pembayaran -> pengemasan -> pengiriman.
        // Tiap tahap disimulasikan dengan Task.Delay (seolah proses makan waktu).
        // Mengembalikan pesanan yang diproses, atau null kalau antrian kosong.
        public async Task<Order> ProcessNextAsync()
        {
            if (orderQueue.Count == 0)
            {
                return null;
            }

            Order order = orderQueue.Dequeue();

            Console.WriteLine($"Memproses {order.OrderId} untuk {order.BuyerName}:");
            for (int i = 0; i < order.Products.Count; i++)
            {
                Console.WriteLine($"  - {order.Products[i].Name} x{order.Quantities[i]}");
            }

            // 1. verifikasi pembayaran.
            Console.WriteLine("1. Verifikasi pembayaran...");
            await Task.Delay(1000);
            Console.WriteLine("Pembayaran terverifikasi.");

            // 2. pengemasan.
            Console.WriteLine("2. Pengemasan barang...");
            await Task.Delay(1000);
            Console.WriteLine("Barang selesai dikemas.");

            // 3. pengiriman.
            Console.WriteLine("3. Pengiriman...");
            await Task.Delay(1000);
            Console.WriteLine("Pesanan dikirim dari gudang.");

            return order;
        }

        // Proses SEMUA pesanan di antrian, SATU PER SATU (FIFO) secara async.
        // Menunggu satu pesanan selesai dulu (await) baru lanjut ke pesanan berikutnya.
        public async Task ProcessAllAsync()
        {
            if (orderQueue.Count == 0)
            {
                Console.WriteLine("No orders to process.");
                return;
            }

            while (orderQueue.Count > 0)
            {
                Order order = await ProcessNextAsync();

                Console.WriteLine(
                    $"Selesai: {order.OrderId} | Total Payment: Rp{order.TotalPayment:N0}");
                Console.WriteLine($"Sisa antrian: {PendingCount}");
                Console.WriteLine();
            }

            Console.WriteLine("Semua pesanan sudah diproses.");
        }
    }
}
