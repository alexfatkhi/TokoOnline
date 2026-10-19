using System;

namespace TokoOnline_New.CException
{
    // Dilempar saat keranjang sudah penuh (jenis produk mencapai batas maksimum).
    public class CartFullException : Exception
    {
        // pesan yang difault kalau throw tanpa pesan
        public CartFullException()
            : base("Cart is full. Maximum product types reached.")
        {
        }

        // Pesan custom mis. menyebut batas maksimum.
        public CartFullException(string message) : base(message)
        {
            
        }
    }
}
