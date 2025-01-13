using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DALContext _context;

        public OrderRepository(DALContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Order>> GetPaginatedOrdersAsync(string? userId, string? userEmail, string sortBy, bool descending, int page, int pageSize)
        {

            //var query = _context.Orders.Include(o => o.UserId).AsQueryable();
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(o => o.User.Id == userId);

            if (!string.IsNullOrEmpty(userEmail))
                query = query.Where(o => o.User.Email.Contains(userEmail));

            query = descending
        ? query.OrderByDescending(o => EF.Property<object>(o, sortBy))
        : query.OrderBy(o => EF.Property<object>(o, sortBy));

            // query = descending
            //? query.OrderByDescending(o => EF.Property<object>(o, sortBy))
            //: query.OrderBy(o => EF.Property<object>(o, sortBy));

            return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            //.Select(o => new OrderDto
            //{
            //    Id = o.Id,
            //    UserEmail = o.User != null ? o.User.Email : "Unknown", // Explicitly select User.Email
            //    CreatedAt = o.CreatedAt,
            //    TotalPrice = o.TotalPrice
            //})
            .ToListAsync();
        }

        public async Task<int> GetTotalOrdersCountAsync(string? userId, string? userEmail)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(o => o.User.Id == userId);

            if (!string.IsNullOrEmpty(userEmail))
                query = query.Where(o => o.User.Email.Contains(userEmail));

            return await query.CountAsync();
        }

        //public async Task<Order?> GetLatestOrderByUserAsync(string userId)
        //{
        //    return await _context.Orders
        //        .Where(o => o.UserId == userId)
        //        .Include(o => o.User)
        //        .Include(o => o.Product)
        //        .OrderByDescending(o => o.CreatedAt)
        //        .FirstOrDefaultAsync();
        //}
        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order; // Return the saved order with its ID populated
        }

        //public async Task SaveChangesAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}

        public async Task<Order?> GetLatestOrderAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.Product)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
      .Include(o => o.User)   
      .Include(o => o.Product) 
      .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
