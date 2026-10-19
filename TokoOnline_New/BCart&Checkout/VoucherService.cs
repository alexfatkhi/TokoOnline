using System;
using System.Collections.Generic;

namespace TokoOnline_New.BCart_Checkout
{
    // voucher diskon 10%, Tiap kode hanya boleh dipakai sekali, Kode yang sudah terpakai disimpan di HashSet supaya tidak bisa dipakai lagi.
    public class VoucherService
    {
        public const int DiscountPercent = 10;

        //--- 
        private readonly HashSet<string> validCodes;
        private readonly HashSet<string> usedCodes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public VoucherService(IEnumerable<string> codes)
        {
            validCodes = new HashSet<string>(codes, StringComparer.OrdinalIgnoreCase);
        }

        public bool Exists(string code)
        {
            return code != null && validCodes.Contains(code);
        }

        public bool AlreadyUsed(string code)
        {
            return code != null && usedCodes.Contains(code);
        }
 

        // Coba pakai voucher pada subtotal.
        // kalau true, berhasil, discount & message terisi, kode ditandai terpakai.
        // kalau false gagal ,kosong /tidak valid, message menjelaskan.
        public bool TryRedeem(string code, int subtotal, out int discount, out string message)
        {
            discount = 0;

            if (string.IsNullOrWhiteSpace(code))
            {
                message = "No voucher used.";
                return false;
            }

            if (!Exists(code))
            {
                message = $"Voucher '{code}' is not valid.";
                return false;
            }

            if (AlreadyUsed(code))
            {
                message = $"Voucher '{code}' has already been used.";
                return false;
            }

            usedCodes.Add(code);
            discount = subtotal * DiscountPercent / 100;
            message =
                $"Voucher '{code}' applied. Discount {DiscountPercent}% (-Rp{discount:N0}).";

            return true;
        }
    }
}
