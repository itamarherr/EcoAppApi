using DAL.Models;

namespace DAL.Data;

public interface IOrderRepository
{
    Task<List<Order>> GetAllOrdersForAdminAsync();
}
