using System;
using System.Collections.Generic;

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

        // Ambil & proses satu pesanan paling depan. null kalau antrian kosong.
        public Order ProcessNext()
        {
            if (orderQueue.Count == 0)
            {
                return null;
            }

            return orderQueue.Dequeue();
        }
    }
}
