using DAL.Models;

namespace DAL.Data;


    public sealed class CategoryRepository(DALContext context) : Repository<Category>(context)
    {

    }

