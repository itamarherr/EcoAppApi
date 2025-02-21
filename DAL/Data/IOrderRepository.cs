using DAL.Models;

namespace DAL.Data;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> SearchOrdersAsync(string query);
    Task<List<Order>> GetPaginatedOrdersAsync(string? userId, string? userEmail, string sortBy, bool descending, int page, int pageSize);
    Task<int> GetTotalOrdersCountAsync(string? userId, string? userEmail);
    Task<Order?> GetLatestOrderAsync(string userId);
    Task<Order> CreateOrderAsync(Order order);
    Task<Order> GetOrderByIdAsync(int id);
    Task DeleteMyOrderAsync(Order order);
    Task DeleteOrderByIdAsync(int orderId);

}
