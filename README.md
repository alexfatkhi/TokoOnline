> **Catatan:** Versi TokoOnline yang sudah sesuai dengan UML diagram yang diperbaiki ada di folder/direktori **`TokoOnline_New`**.

<img width="932" height="711" alt="image" src="https://github.com/user-attachments/assets/c5fbede2-cd51-4a04-b5d0-dd79788cb3b5" />
<img width="1222" height="364" alt="image" src="https://github.com/user-attachments/assets/0327507c-a4e2-447a-8f0f-e92f0b161546" />

@⁨Ilham⁩ @⁨Muh Fatkhi⁩ 
Kasus 3 - Toko Online
Toko online mulai memakai voucher dan antrian pesanan untuk gudang.
•⁠  ⁠Menambah produk melebihi stok melempar OutOfStockException. Membeli makanan kedaluwarsa melempar ExpiredProductException. Keranjang penuh melempar CartFullException.
•⁠  ⁠Harga 0 atau negatif melempar ArgumentException.
•⁠  ⁠Katalog memakai List dan pencarian produk lewat kode memakai Dictionary.
•⁠  ⁠Voucher diskon 10% hanya bisa dipakai sekali per kode; kode yang sudah terpakai disimpan di HashSet.
•⁠  ⁠Pesanan yang sudah checkout masuk Queue untuk diproses gudang.
•⁠  ⁠Penyimpanan produk memakai Repository<T> dengan constraint ke class induk produk.


kasus 3 - Toko

