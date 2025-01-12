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
        Task<OrderDto?> GetLastOrderForUpdateAsync(string userId);
        Task<bool> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
        Task<bool> UpdateCurrentUserOrderAsync(string userId, UpdateOrderDto updateOrderDto);
        Task<bool> DeleteLastOrderAsync(string userId);
    
    }
}
