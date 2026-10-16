using System;

namespace TokoOnline_New.AProduct
{
    // Kontrak untuk produk yang bisa diretur Electronic & Clothing.
    public interface IReturnable
    {
        int ReturnDayLimit { get; }        // batas hari retur
        bool CanReturn(int days);          // cek masih boleh retur
    }
}
