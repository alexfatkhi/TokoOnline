using System;

namespace TokoOnline_New.CException
{
    // Dilempar saat stok produk habis atau qty melebihi stok yang tersedia.

    public class OutOfStockException : Exception
    {
        // Pesan default kalau dithrow tanpa pesan.
        public OutOfStockException() : base("Produk sudah habis")
        {

        }

        public OutOfStockException(string message) : base(message)
        {
        }
    }
}
