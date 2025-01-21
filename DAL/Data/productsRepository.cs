using DAL.Models;

namespace DAL.Data
{
    public sealed class ProductsRepository(DALContext context) : Repository<Product> (context)
    {
        public override IEnumerable<Product> GetAll()
        {
            return context.Products;
        }

        public override Product? GetById(int id)
        {
            return context.Products.SingleOrDefault (p => p.Id == id);
        }
    }
}


