using DAL.Models;
using EcoAppApi.DTOs;

namespace EcoAppApi.Utils
{
    public interface IOrderService
    {

        Task<(List<OrderDto>, int)> GetOrdersAsync(
            string? userId, string? userEmail, string sortBy, bool descending, int page, int pageSize
            );
        Task<OrderDto?> GetMyOrderAsync(string userId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, string userId);
        //Task<Order> CreateOrderAsync(CreateOrderDto orderDto, int userId);
        //Task<List<Order>> GetAllOrdersForadminasync();

        //Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto orderDto);
    }
}
