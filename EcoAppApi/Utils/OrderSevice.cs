using DAL.Data;
using DAL.Models;
using EcoAppApi.DTOs;

namespace EcoAppApi.Utils;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly PricingService _pricingService;
    private readonly DALContext _context;

    public OrderService(
        IOrderRepository orderRepository,
        PricingService pricingService,
        DALContext context)
    {
        _orderRepository = orderRepository;
        _pricingService = pricingService;
        _context = context;
    }

    public async Task<Order> createOrderAsync(CreateOrderProductDto orderDto)
    {

        var order = new Order
        {
            UserId = orderDto.UserId,
            ProductId = orderDto.ProductId,
            AdditionalNotes = orderDto.AdditionalNotes,
            TotalPrice = orderDto.TotalPrice = 0, // Default to 0 if not provided
            OrderDate = DateTime.Now,
            CreatedAt = DateTime.Now,
            LastUpdate = DateTime.Now,
            Status = "pending",
            ImageUrl = orderDto.ImageUrl
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

 


    public async Task<List<Order>> GetAllOrdersForadminasync()
    {
        return await _orderRepository.GetAllOrdersForAdminAsync();
    }
}