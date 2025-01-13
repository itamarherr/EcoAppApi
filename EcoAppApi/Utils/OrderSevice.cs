using DAL.Data;
using DAL.enums;
using DAL.Models;
using EcoAppApi.DTOs;
using Microsoft.OpenApi.Extensions;

namespace EcoAppApi.Utils;

public class OrderService : IOrderService
  
{
    private readonly IOrderRepository _orderRepository;
    private readonly PricingService _pricingService;
    private readonly DALContext _context;

    public OrderService(IOrderRepository orderRepository, PricingService pricingService, DALContext context)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _pricingService = pricingService ?? throw new ArgumentNullException(nameof(pricingService));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<(List<OrderDto>, int)> GetOrdersAsync(string? userId, string? userEmail, string sortBy, bool descending, int page, int pageSize)
    {
        if (_orderRepository == null)
        {
            throw new InvalidOperationException("Order repository is not initialized.");
        }
        Console.WriteLine($"userId: {userId}, userEmail: {userEmail}, sortBy: {sortBy}, descending: {descending}, page: {page}, pageSize: {pageSize}");

        var orders = await _orderRepository.GetPaginatedOrdersAsync(userId, userEmail, sortBy, descending, page, pageSize);

        if (orders == null)
        {
            throw new InvalidOperationException("GetOrdersAsync returned null.");
        }
        var totalItems = await _orderRepository.GetTotalOrdersCountAsync(userId, userEmail);
        return (orders.Select(o => new OrderDto
        {
            Id = o.Id,
            UserEmail = o.User?.Email ?? "Unknown",
            TotalPrice = o.TotalPrice,
            CreatedAt = o.CreatedAt,

          
            UserId = o.UserId,
            UserName = o.User?.UserName ?? "Unknown",
           
            ServiceType = o.Product.Name ?? "Unknown",
            City = o.City,
            Street = o.Street,
            Number = o.Number,
            IsPrivateArea = o.IsPrivateArea,
            DateForConsultancy = o.DateForConsultancy,
            NumberOfTrees = o.NumberOfTrees,
            AdditionalNotes = o.AdditionalNotes,
           
          
            StatusType = o.StatusType,
            ConsultancyType = o.ConsultancyType
        }).ToList(), totalItems);
      
    }

    public async Task<OrderDto?> GetMyOrderAsync(string userId)
    {
        var order = await _orderRepository.GetLatestOrderAsync(userId);

        if (order == null)
            return null;

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            UserName = order.User?.UserName ?? "Unknown",
            CreatedAt = order.CreatedAt,
            ServiceType = order.Product.Name ?? "Unknown",
            City = order.City,
            Street = order.Street,
            Number = order.Number,
            IsPrivateArea = order.IsPrivateArea,
            DateForConsultancy = order.DateForConsultancy,
            NumberOfTrees = order.NumberOfTrees,
            AdditionalNotes = order.AdditionalNotes,
            TotalPrice = order.TotalPrice,
            UserEmail = order.User?.Email ?? "Unknown",
            StatusType = order.StatusType,
            ConsultancyType = order.ConsultancyType
        };
    }
    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, string userId)
    {
        var totalPrice = _pricingService.CalculatePrice(
                 createDto.ConsultancyType,
                 createDto.NumberOfTrees,
                 createDto.IsPrivateArea
                 );
            var user = await _context.Users.FindAsync(userId);
            var product = await _context.Products.FindAsync(createDto.ProductId);

            if (user == null || product == null)
            {
                throw new ArgumentException("Invalid UserId or ProductId");
            }

        var order = new Order
        {
            UserId = userId,
            ProductId = createDto.ProductId,
            //ImageUrl = createDto.ImageUrl,
            AdditionalNotes = createDto.AdditionalNotes,
            TotalPrice = totalPrice,
            NumberOfTrees = createDto.NumberOfTrees,
            City = createDto.City,
            Street = createDto.Street,
            Number = createDto.Number,
            ConsultancyType = createDto.ConsultancyType,
            IsPrivateArea = createDto.IsPrivateArea,
            DateForConsultancy = createDto.DateForConsultancy,
            CreatedAt = DateTime.UtcNow,

            StatusType = createDto.StatusType,

        };
        var saveOrder = await _orderRepository.CreateOrderAsync(order);
        return saveOrder.ToDto();
    }

    public async Task<OrderDto?> GetLastOrderForUpdateAsync(string userId)
    {
        var lastOrder = await _orderRepository.GetLatestOrderAsync(userId);
        return lastOrder?.ToDto(); 
    }

    public async Task<bool> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);
        if (order == null)
        {
            return false;
        }

        ApplyUpdates(order, updateOrderDto);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCurrentUserOrderAsync(string userId, UpdateOrderDto updateOrderDto)
    {
        var order = await _orderRepository.GetOrderByIdAsync(updateOrderDto.Id);
        if (order == null || order.UserId != userId)
        {
            return false;
        }

        ApplyUpdates(order, updateOrderDto);

        await _context.SaveChangesAsync();
        return true;
    }
    private void ApplyUpdates(Order order, UpdateOrderDto updateOrderDto)
    {
        order.AdminNotes = updateOrderDto.AdminNotes;
        order.TotalPrice = updateOrderDto.TotalPrice ?? order.TotalPrice;
        order.AdditionalNotes = updateOrderDto.AdditionalNotes ?? order.AdditionalNotes;
        order.NumberOfTrees = updateOrderDto.NumberOfTrees > 0 ? updateOrderDto.NumberOfTrees : order.NumberOfTrees;
        order.City = !string.IsNullOrEmpty(updateOrderDto.City) ? updateOrderDto.City : order.City;
        order.Street = !string.IsNullOrEmpty(updateOrderDto.Street) ? updateOrderDto.Street : order.Street;
        order.Number = updateOrderDto.Number > 0 ? updateOrderDto.Number : order.Number;
        order.ConsultancyType = updateOrderDto.ConsultancyType;
        order.IsPrivateArea = updateOrderDto.IsPrivateArea;
        order.DateForConsultancy = updateOrderDto.DateForConsultancy;
        order.StatusType = updateOrderDto.StatusType;
    }

    public async Task<bool> DeleteLastOrderAsync(string userId)
    {
        var order = await _orderRepository.GetLatestOrderAsync(userId);

        if (order == null)
        {
            return false; // No order to delete
        }

        await _orderRepository.DeleteOrderAsync(order);
        return true;
    }
}



