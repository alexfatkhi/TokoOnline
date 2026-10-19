using System;

namespace TokoOnline_New.CException
{
    // Dilempar saat produk makanan sudah kedaluwarsa dan tidak bisa dibeli.
    // Constructor
    public class ExpiredProductException : Exception
    {
        // Pesan default kalau di-throw tanpa pesan.
        public ExpiredProductException()
            : base("Product is expired and cannot be purchased.")
        {
        }

        //pesan custom
        public ExpiredProductException(string message) : base(message)
        {
        }
    }
}
