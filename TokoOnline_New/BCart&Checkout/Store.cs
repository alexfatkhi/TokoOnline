
using System;
using TokoOnline_New.AProduct;

namespace TokoOnline_New.BCart_Checkout
{
    public class Store
    {
        private Repository<Product> repository;
        private Dictionary<string, Product> productsByCode;
        private HashSet<string> availableVouchers = new HashSet<string>()
        {
            "HEMAT10",
            "BELANJA10",
            "ILHAM10"
        };

        private HashSet<string> usedVouchers = new HashSet<string>();


        public Store(Product[] products)
        {
            repository = new Repository<Product>();
            productsByCode = new Dictionary<string, Product>();

            for (int i = 0; i < products.Length; i++)
            {
                Product product = products[i];

                repository.Add(product);

                productsByCode.Add(
                    product.Code.ToUpper(),
                    product
                );
            }
        }

        public Store(Repository<Product> repository)
        {
            this.repository = repository;
        }

        public Product FindProduct(string code)
        {
            Product product;

            if (productsByCode.TryGetValue(code.Trim().ToUpper(),out product))
            {
                return product;
            }

            return null;
        }

        public void PrintCatalog()
        {
            List<Product> products = repository.GetAll();

            Console.WriteLine("=== PRODUCT CATALOG ===");

            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($@"{products[i].Code}  |  {products[i].Name} | Rp {products[i].Price.ToString("N0")} | Stock: {products[i].Stock}");
            }
        }
        public int Refund(Product item, int days)
        {
            if (item == null)
            {
                return 0;
            }

            if (item is IReturnable)
            {
                IReturnable returnable = (IReturnable)item;

                if (returnable.CanReturn(days))
                {
                    item.AddStock(1);
                    return item.FinalPrice();
                }
            }

            return 0;
        }

        public int Voucher(string code, int total)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return total;
            }

            code = code.ToUpper();

            if (usedVouchers.Contains(code))
            {
                Console.WriteLine("Voucher has already been used");
                return total;
            }

            usedVouchers.Add(code);

            int discount = total * 10 / 100;
            Console.WriteLine($"Voucher applied. Discount Rp {discount}");

            return total - discount;
        }
    }
}
