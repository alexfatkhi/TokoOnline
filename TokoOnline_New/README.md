# TokoOnline_New — Implementasi Kasus 3

Dokumen ini menjelaskan apa saja yang dikerjakan untuk **Kasus 3 (Toko Online: voucher + antrian gudang)**.
Semua perubahan **hanya di folder `TokoOnline_New`**, project lain tidak disentuh.

> **Toko online mulai memakai voucher dan antrian pesanan untuk gudang.**

## Penjelasan per poin soal

Tiap titik (•) pada soal dijelaskan di bawah: maksudnya, cara kerjanya, dan di file mana diimplementasikan.

### Poin 1 — Exception keranjang (stok, kedaluwarsa, penuh)
> Menambah produk melebihi stok, membeli makanan kedaluwarsa, dan keranjang penuh akan melempar exception.

Ketiga aturan ini dicek saat produk ditambahkan ke keranjang, dan masing-masing **melempar custom exception** sendiri (bukan sekadar `return false`). Ketiga class dibuat di folder `CException/`, turunan dari `Exception`, dan punya **dua constructor**: satu tanpa parameter (pesan default) dan satu menerima `message` (pesan custom yang menyebut nama produk / sisa stok).

- **Stok habis / qty melebihi stok** → `OutOfStockException` (`'X' is out of stock.` / `Not enough stock for 'X'. Available: n.`).
- **Makanan kedaluwarsa** → `ExpiredProductException` (`'X' is expired and cannot be purchased.`), untuk produk `Food` dengan `DaysUntilExpiry <= 0`.
- **Keranjang penuh** → `CartFullException` (`Cart is full. Maximum 10 product types allowed.`), saat jenis produk mencapai `MaxItemTypes` (10).

**File:**
- `CException/OutOfStockException.cs`, `CException/ExpiredProductException.cs`, `CException/CartFullException.cs` — definisi ketiga custom exception.
- `BCart&Checkout/Cart.cs` — `Cart.Add(...)` yang melempar ketiga custom exception sesuai kasus (stok habis juga dicek ulang di `Cart.Checkout` → `OutOfStockException`).
- `Program.cs` — menu **"2. Add Product to Cart"** membungkus `cart.Add(...)` dengan **`try/catch`**: `catch (ExpiredProductException)`, `catch (OutOfStockException)`, `catch (CartFullException)`, plus `catch (ArgumentException)` untuk input tidak valid.

### Poin 2 — Harga tidak valid melempar `ArgumentException`
> Harga 0 atau negatif melempar `ArgumentException`.

Saat produk dibuat, harga divalidasi di konstruktor induk. Kalau `price <= 0`, langsung dilempar `ArgumentException`.

**File:**
- `AProduct/Product.cs` — konstruktor `Product(...)` berisi validasi `if (price <= 0) throw new ArgumentException(...)`.

### Poin 3 — Katalog pakai `List`, pencarian kode pakai `Dictionary`
> Katalog memakai `List` dan pencarian produk lewat kode memakai `Dictionary`.

Katalog disimpan di `List<T>` (urutan tampil terjaga), sementara pencarian produk berdasarkan kode memakai `Dictionary<string, T>` supaya cepat (O(1)).

**File:**
- `AProduct/Repository.cs` — menyimpan `List<T> items` + `Dictionary<string, T> byCode`. Pencarian lewat `GetByCode(code)`.
- `BCart&Checkout/Store.cs` — `FindProduct` memanggil `Dictionary` di repository; `PrintCatalog` memakai `List` dari repository.

### Poin 4 — Voucher diskon 10% sekali pakai, disimpan di `HashSet`
> Voucher diskon 10% hanya bisa dipakai sekali per kode; kode yang sudah terpakai disimpan di `HashSet`.

Voucher memberi diskon 10%. Kode yang valid dan kode yang sudah terpakai sama-sama disimpan dalam `HashSet`. Begitu sebuah kode dipakai, kode itu masuk `usedCodes` dan tidak bisa dipakai lagi.

**File:**
- `BCart&Checkout/VoucherService.cs` — `HashSet` `validCodes` & `usedCodes`, method `TryRedeem(...)`.
- `Program.cs` — saat checkout, meminta kode voucher dan memanggil `voucherService.TryRedeem(...)`.

### Poin 5 — Pesanan checkout masuk `Queue` untuk gudang
> Pesanan yang sudah checkout masuk `Queue` untuk diproses gudang.

Hasil checkout dibungkus jadi objek `Order`, lalu dimasukkan ke antrian gudang yang bersifat **FIFO** (yang lebih dulu checkout lebih dulu diproses).

**File:**
- `BCart&Checkout/Order.cs` — objek satu pesanan (item, subtotal, diskon, ongkir, total, id, tanggal).
- `BCart&Checkout/Warehouse.cs` — menampung pesanan dalam `Queue<Order>` (`ReceiveOrder` / `ProcessNext`).
- `Program.cs` — setelah checkout memanggil `warehouse.ReceiveOrder(order)`; menu **"6. Process Warehouse Queue"** memproses pesanan dari antrian.

### Poin 6 — `Repository<T>` dengan constraint ke class induk produk
> Penyimpanan produk memakai `Repository<T>` dengan constraint ke class induk produk.

Penyimpanan produk dibuat generic dengan batasan `where T : Product`, sehingga hanya tipe turunan `Product` yang boleh disimpan.

**File:**
- `AProduct/Repository.cs` — `public class Repository<T> where T : Product`.
- `Program.cs` — produk dimuat ke `Repository<Product>` lalu diberikan ke `Store`.

## Ringkasan Kasus 3

| # | Permintaan | Status | Di mana |
|---|------------|--------|---------|
| 1 | Menambah produk melebihi stok → exception | ✅ | `OutOfStockException`, dilempar di `Cart.Add` |
| 2 | Membeli makanan kedaluwarsa → exception | ✅ | `ExpiredProductException`, dilempar di `Cart.Add` |
| 3 | Keranjang penuh → exception | ✅ | `CartFullException`, dilempar di `Cart.Add` |
| 4 | Harga 0 atau negatif → `ArgumentException` | ✅ | `AProduct/Product.cs` (sudah ada, dipertahankan) |
| 5 | Katalog pakai `List`, pencarian kode pakai `Dictionary` | ✅ | `AProduct/Repository.cs` dipakai oleh `Store` |
| 6 | Voucher diskon 10% sekali pakai per kode, kode terpakai di `HashSet` | ✅ | `BCart&Checkout/VoucherService.cs` |
| 7 | Pesanan yang checkout masuk `Queue` untuk gudang | ✅ | `BCart&Checkout/Order.cs` + `Warehouse.cs` |
| 8 | Penyimpanan produk pakai `Repository<T>` dengan constraint ke class induk | ✅ | `AProduct/Repository.cs` (`where T : Product`) |

## File baru

- **`CException/OutOfStockException.cs`** — custom exception stok habis / qty melebihi stok.
- **`CException/ExpiredProductException.cs`** — custom exception makanan kedaluwarsa.
- **`CException/CartFullException.cs`** — custom exception keranjang penuh.
- **`AProduct/Repository.cs`** — `Repository<T> where T : Product`. Menyimpan katalog di `List<T>` dan indeks pencarian kode di `Dictionary<string, T>`.
- **`BCart&Checkout/VoucherService.cs`** — voucher diskon 10%, memakai `HashSet` untuk kode valid dan kode yang sudah terpakai.
- **`BCart&Checkout/Order.cs`** — satu pesanan hasil checkout (item, subtotal, diskon, ongkir, total, id, tanggal).
- **`BCart&Checkout/Warehouse.cs`** — gudang yang menampung pesanan dalam `Queue<Order>` (FIFO).

## File yang diubah

### `BCart&Checkout/Cart.cs`
- Penyimpanan internal diganti dari array tetap (`Product[10]`) menjadi `List<Product>` + `List<int>`.
- `Add` sekarang **melempar custom exception** (sebelumnya hanya `return false` + `Console.WriteLine`):
  - makanan kedaluwarsa → `ExpiredProductException`
  - stok habis / qty melebihi stok → `OutOfStockException`
  - jenis produk sudah mencapai `MaxItemTypes` (10) → `CartFullException`
  - produk null / qty ≤ 0 → `ArgumentException`
- `Checkout(string orderId, int discount)` sekarang mengembalikan objek **`Order`** (bukan lagi `int`), menerapkan diskon voucher, mengurangi stok, menambah poin, lalu mengosongkan keranjang. Kalau stok sudah tidak cukup saat checkout, melempar `OutOfStockException`.

### `BCart&Checkout/Store.cs`
- Konstruktor menerima `Repository<Product>` (bukan lagi `Product[]`).
- `FindProduct` mencari lewat `Dictionary` di repository (sebelumnya loop array manual).
- `PrintCatalog` memakai `List` dari repository.

### `Program.cs`
- Produk dimuat ke `Repository<Product>` lalu diberikan ke `Store`.
- Menambah `VoucherService` (kode: `HEMAT10`, `DISKON10`, `GUDANG10`) dan `Warehouse`.
- Menu "Add to Cart" dibungkus `try/catch` untuk menangkap custom exception dari keranjang (`ExpiredProductException`, `OutOfStockException`, `CartFullException`) plus `ArgumentException`.
- Saat checkout: meminta kode voucher (opsional), membuat `Order` di dalam `try/catch` (`OutOfStockException`), dan memasukkannya ke antrian gudang.
- Menu baru **"6. Process Warehouse Queue"** untuk memproses pesanan dari `Queue` (FIFO). Menu Exit pindah ke **7**.
- Riwayat pembelian (untuk fitur retur) kini diisi dari isi `Order`, sehingga array `pending*` yang lama dihapus.

## Cara menjalankan

```bash
cd TokoOnline_New
dotnet run
```

## Contoh hasil uji

- Tambah `FD005` (Dodol, kedaluwarsa) → `'Dodol' is expired and cannot be purchased.`
- Tambah `FD001` qty 100 (stok 8) → `Not enough stock for 'Bread'. Available: 8.`
- Checkout dengan voucher `HEMAT10` → `Discount 10%`, pesanan `ORD-001` masuk antrian gudang.
- Menu 6 → memproses `ORD-001`, antrian kembali kosong.
- Memakai voucher yang sama dua kali → penggunaan kedua ditolak (kode tersimpan di `HashSet`).

## Deskripsi Soal

@Ilham @Muh Fatkhi

**Kasus 3 - Toko Online**

Toko online mulai memakai voucher dan antrian pesanan untuk gudang.

- Menambah produk melebihi stok melempar OutOfStockException. Membeli makanan kedaluwarsa melempar ExpiredProductException. Keranjang penuh melempar CartFullException.
- Harga 0 atau negatif melempar ArgumentException.
- Katalog memakai List dan pencarian produk lewat kode memakai Dictionary.
- Voucher diskon 10% hanya bisa dipakai sekali per kode; kode yang sudah terpakai disimpan di HashSet.
- Pesanan yang sudah checkout masuk Queue untuk diproses gudang.
- Penyimpanan produk memakai Repository<T> dengan constraint ke class induk produk.
