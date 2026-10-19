using System;
using System.Collections.Generic;

namespace Project_OOP.AProduct
{
    //Penyimpanan produk generic dengan constraint ke class induk Product.

    //---
    public class Repository<T> where T : Product
    {
        //---
        private readonly List<T> items = new List<T>();
        //---
        private readonly Dictionary<string, T> byCode =
            new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

        public int Count => items.Count;

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (byCode.ContainsKey(item.Code))
            {
                throw new ArgumentException(
                    $"Product code '{item.Code}' already exists in repository.");
            }

            items.Add(item);
            byCode[item.Code] = item;
        }

        // Cari produk lewat kode. null kalau tidak ketemu.
        public T GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            return byCode.TryGetValue(code, out T found) ? found : null;
        }

        // Seluruh isi katalog (urutan List).
        public IReadOnlyList<T> GetAll()
        {
            return items;
        }
    }
}
