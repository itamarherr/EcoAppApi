using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public sealed class ServicesRepository(DALContext context) : Repository<Service>(context)
    {
        public override IEnumerable<Service> GetAll()
        {
            return context.Services.Include(p => p.Category);
        }

        public override Service? GetById(int id)
        {
            return context.Services.Include(p => p.Category).SingleOrDefault(p => p.Id == id);
        }
    }
}
