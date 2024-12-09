using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DALContext _context; 

     
        public OrderRepository(DALContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetAllOrdersForAdminAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
