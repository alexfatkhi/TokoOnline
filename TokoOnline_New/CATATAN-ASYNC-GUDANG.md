# Catatan: Proses Gudang secara Asynchronous

Soal: **Gudang memproses pesanan dari antrian satu per satu secara async:
verifikasi pembayaran, pengemasan, dan pengiriman (disimulasikan dengan `Task.Delay`).**

Dokumen ini menjelaskan cara pengerjaannya, mengikuti gaya `async/await` di catatanmu.

---

## File yang diubah (lokasi direktori)

Semua di dalam folder project:
`TokoOnline-new3/TokoOnline/TokoOnline_New/`

| No | Lokasi file (path) | Status | Bagian yang diubah/ditambah |
|----|--------------------|--------|------------------------------|
| 1 | `TokoOnline_New/BCart&Checkout/Warehouse.cs` | **DIEDIT** | Tambah method `ProcessNextAsync()` & `ProcessAllAsync()` (async, `Task.Delay`). Tambah `using System.Threading.Tasks;`. |
| 2 | `TokoOnline_New/Program.cs` | **DIEDIT** | Signature `Main` → `static async Task Main(...)`, tambah `using System.Threading.Tasks;`, handler menu `6` → `await warehouse.ProcessAllAsync();`. |

Path lengkap di komputer ini:

- `/Users/alex/Documents/magang/C#/TokoOnline-new3/TokoOnline/TokoOnline_New/BCart&Checkout/Warehouse.cs`
- `/Users/alex/Documents/magang/C#/TokoOnline-new3/TokoOnline/TokoOnline_New/Program.cs`

---

## Ide dasar (sesuai catatan)

- **`Task`** mewakili pekerjaan yang hasilnya bisa disediakan nanti.
- **`async/await`** = cara menulis kode asynchronous supaya terbaca seperti kode biasa.
- **`await`** menunggu sebuah `Task` selesai **tanpa membuat thread diam (non-blocking)** —
  selama menunggu, thread dilepas dan bisa mengerjakan hal lain.
- **`Task.Delay(ms)`** = menunggu secara async (bukan `Thread.Sleep` yang blocking).
  Di sini dipakai untuk **mensimulasikan** proses yang makan waktu
  (verifikasi bayar, kemas, kirim).
- Pesanan diambil dari **`Queue<Order>`** → **FIFO**: yang lebih dulu checkout, lebih dulu diproses.

---

## 1. `Warehouse.ProcessNextAsync()` — proses SATU pesanan, 3 tahap

Mengambil satu pesanan paling depan dari antrian, lalu menjalankan 3 tahap
**berurutan dengan `await`**:

```csharp
public async Task<Order> ProcessNextAsync()
{
    if (orderQueue.Count == 0)
    {
        return null;
    }

    Order order = orderQueue.Dequeue();      // ambil pesanan terdepan (FIFO)

    // Tahap 1: verifikasi pembayaran
    Console.WriteLine("  [1/3] Verifikasi pembayaran...");
    await Task.Delay(1000);                  // simulasi proses (non-blocking)
    Console.WriteLine("        Pembayaran terverifikasi.");

    // Tahap 2: pengemasan
    Console.WriteLine("  [2/3] Pengemasan barang...");
    await Task.Delay(1000);
    Console.WriteLine("        Barang selesai dikemas.");

    // Tahap 3: pengiriman
    Console.WriteLine("  [3/3] Pengiriman...");
    await Task.Delay(1000);
    Console.WriteLine("        Pesanan dikirim dari gudang.");

    return order;
}
```

Kenapa `async Task<Order>`? Karena method ini **asynchronous** (`async`) dan
**mengembalikan nilai** `Order` — sesuai catatan: return type `Task<T>` untuk
async yang membawa hasil.

---

## 2. `Warehouse.ProcessAllAsync()` — proses SEMUA, SATU PER SATU

Soal minta "satu per satu". Jadi kita `await` satu pesanan **sampai selesai**,
baru lanjut ke pesanan berikutnya (sequential, bukan paralel):

```csharp
public async Task ProcessAllAsync()
{
    if (orderQueue.Count == 0)
    {
        Console.WriteLine("No orders to process.");
        return;
    }

    while (orderQueue.Count > 0)
    {
        Order order = await ProcessNextAsync();   // tunggu 1 pesanan beres dulu

        Console.WriteLine($"Selesai: {order.OrderId} | Total Payment: Rp{order.TotalPayment:N0}");
        Console.WriteLine($"Sisa antrian: {PendingCount}");
        Console.WriteLine();
    }

    Console.WriteLine("Semua pesanan sudah diproses.");
}
```

Return type `Task` (tanpa `<T>`) karena method ini async tapi **tidak mengembalikan nilai**.

---

## 3. `Program.cs` — menjalankan dari menu

Agar bisa memakai `await` di dalam `Main`, `Main` diubah menjadi **`async Task`**:

```csharp
static async Task Main(string[] args)   // sebelumnya: static void Main(...)
```

Lalu menu `6` memanggilnya:

```csharp
else if (choice == "6")
{
    Console.WriteLine("=== WAREHOUSE QUEUE (ASYNC) ===");
    Console.WriteLine("Pending orders: " + warehouse.PendingCount);

    await warehouse.ProcessAllAsync();   // proses antrian satu per satu secara async
}
```

---

## Contoh output (2 pesanan di antrian)

```
=== WAREHOUSE QUEUE (ASYNC) ===
Pending orders: 2

Memproses ORD-001 untuk Fatkhi:
  - Mouse x1
  [1/3] Verifikasi pembayaran...
        Pembayaran terverifikasi.
  [2/3] Pengemasan barang...
        Barang selesai dikemas.
  [3/3] Pengiriman...
        Pesanan dikirim dari gudang.
Selesai: ORD-001 | Total Payment: Rp115.000
Sisa antrian: 1

Memproses ORD-002 untuk Fatkhi:
  ...
Semua pesanan sudah diproses.
```

Tiap tahap ada jeda ~1 detik (`Task.Delay(1000)`), jadi satu pesanan ~3 detik,
dan pesanan berikutnya baru mulai setelah yang sebelumnya selesai.

---

## Catatan penting / pilihan desain

- **Kenapa `Task.Delay`, bukan `Thread.Sleep`?**
  `Thread.Sleep` memblokir thread (blocking). `Task.Delay` menunggu secara async
  (non-blocking) — persis poin di catatanmu. Untuk aplikasi nyata (API, DB, file),
  inilah yang membuat program tidak "beku" saat menunggu.

- **Satu per satu (sequential) vs paralel.**
  Soal minta **satu per satu**, jadi dipakai `await` berurutan. Kalau mau semua
  pesanan diproses **bersamaan**, bisa pakai `Task.WhenAll(...)` (dari catatanmu) —
  tapi itu mengubah makna soal, jadi tidak dipakai di sini.

- **Pengembangan lanjutan (opsional):** bisa ditambah `CancellationToken`
  (seperti bagian *Cancellation* di catatanmu) agar proses gudang bisa dibatalkan
  di tengah jalan atau timeout. Belum dipasang karena soal tidak memintanya.

---

## Cara menjalankan

```bash
cd TokoOnline-new3/TokoOnline/TokoOnline_New
dotnet run
```

Login → beli produk (menu `2`) → checkout (menu `4`) → ulangi untuk beberapa pesanan →
pilih menu **`6`** untuk memproses antrian gudang secara async.
