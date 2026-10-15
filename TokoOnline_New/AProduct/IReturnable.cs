using System;

namespace Project_OOP.AProduct
{
    // Kontrak untuk produk yang bisa diretur Electronic & Clothing.
    public interface IReturnable
    {
        int ReturnDayLimit { get; }        // batas hari retur
        bool CanReturn(int days);          // cek masih boleh retur
    }
}
