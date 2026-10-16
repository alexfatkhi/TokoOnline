using TokoOnline_New.AProduct;
using System.Collections.Generic;

namespace TokoOnline_New.BCart_Checkout
{
    public class Repository<TEntity> where TEntity : Product
    {
        private List<TEntity> products = new List<TEntity>();

        public void Add(TEntity product)
        {
            products.Add(product);
        }

        public List<TEntity> GetAll()
        {
            return products;
        }
    }
}