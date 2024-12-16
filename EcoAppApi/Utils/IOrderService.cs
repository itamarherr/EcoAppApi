using DAL.Models;
using EcoAppApi.DTOs;

namespace EcoAppApi.Utils
{
    public interface IOrderService
    {
        Task<Order> createOrderAsync(CreateOrderProductDto orderDto);
        Task<List<Order>> GetAllOrdersForadminasync();

        Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto orderDto);
    }
}
