using DAL.enums;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DALContext _context;

        public OrderRepository(DALContext context)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
        }

        public async Task<IEnumerable<Order>> SearchOrdersAsync(string query)
        {
            query = query.Trim ().ToLower ();
            var allOrders = await _context.Orders
         .Include (o => o.User)
         .ToListAsync (); //  Ensure we are getting the full list

            Console.WriteLine ($"Total Orders in DB: {allOrders.Count}");


            var filteredOrders = allOrders.Where (o =>
                o.Id.ToString ().Contains (query) ||
                (o.User?.Email?.ToLower ().Contains (query) ?? false) ||
                o.City.ToLower ().Contains (query) ||
                (Enum.GetName (typeof (Purpose), o.ConsultancyType)?.ToLower () ?? "").Contains (query)
            ).ToList ();

            Console.WriteLine ($"Search Results Count: {filteredOrders.Count}");

            return filteredOrders;
        }

        public async Task<List<Order>> GetPaginatedOrdersAsync(string? userId, string? userEmail, string sortBy, bool descending, int page, int pageSize)
        {
            var query = _context.Orders
                .Include (o => o.User)
                .Include (o => o.Product)
                .AsQueryable ();

            if (!string.IsNullOrEmpty (userId))
                query = query.Where (o => o.User.Id == userId);

            if (!string.IsNullOrEmpty (userEmail))
                query = query.Where (o => o.User.Email.Contains (userEmail));

            query = descending
           ? query.OrderByDescending (o => EF.Property<object> (o, sortBy))
           : query.OrderBy (o => EF.Property<object> (o, sortBy));

            return await query
            .Skip ((page - 1) * pageSize)
            .Take (pageSize)
            .ToListAsync ();
        }

        public async Task<int> GetTotalOrdersCountAsync(string? userId, string? userEmail)
        {
            var query = _context.Orders.AsQueryable ();

            if (!string.IsNullOrEmpty (userId))
                query = query.Where (o => o.User.Id == userId);

            if (!string.IsNullOrEmpty (userEmail))
                query = query.Where (o => o.User.Email.Contains (userEmail));

            return await query.CountAsync ();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add (order);
            await _context.SaveChangesAsync ();
            return order;
        }

        // This method is for regular users to fetch their latest order only
        public async Task<Order?> GetLatestOrderAsync(string userId)
        {
            return await _context.Orders
                .Where (o => o.UserId == userId)
                .Include (o => o.User)
                .Include (o => o.Product)
                .OrderByDescending (o => o.CreatedAt)
                .FirstOrDefaultAsync ();
        }
        // This method is for admin use to fetch any order by its ID
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
          .Include (o => o.User)
          .Include (o => o.Product)
          .FirstOrDefaultAsync (o => o.Id == id);
        }

        public async Task DeleteOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync (orderId);
            if (order == null)
            {
                throw new InvalidOperationException ("Order not found.");
            }
            _context.Orders.Remove (order);
            await _context.SaveChangesAsync ();
        }

        public async Task DeleteMyOrderAsync(Order order)
        {
            _context.Orders.Remove (order);
            await _context.SaveChangesAsync (); ;
        }
    }
}
