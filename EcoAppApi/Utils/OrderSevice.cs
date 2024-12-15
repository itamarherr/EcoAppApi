using DAL.Data;
using DAL.enums;
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
        var user = await _context.Users.FindAsync(orderDto.UserId);
        var product = await _context.Products.FindAsync(orderDto.ProductId);

        if (user == null || product == null)
        {
            throw new ArgumentException("Invalid UserId or ProductId");
        }


        var order = new Order
        {
            UserId = orderDto.UserId,
            ProductId = orderDto.ProductId,
            AdditionalNotes = orderDto.AdditionalNotes,
            TotalPrice = orderDto.TotalPrice ?? product.Price, // Default to Product Price if TotalPrice is null
            DateForConsultancy = orderDto.DateForConsultancy ?? DateTime.Now,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.pending.ToString(), // Use enum for consistency
            //ImageUrl = orderDto.ImageUrl
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }




    public async Task<List<OrderDto>> GetAllOrdersForAdminAsync()
    {
        var orders = await _orderRepository.GetAllOrdersForAdminAsync();

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserEmail = order.User?.Email ?? "N/A",
            ServiceType = order.Product?.Name ?? "Unspecified",
            Status = Enum.TryParse(order.Status, true, out OrderStatus status)
                ? status
                : OrderStatus.pending,
            CreatedAt = order.CreatedAt,
            AdditionalNotes = order.AdditionalNotes
        }).ToList();
    }

    public Task<List<Order>> GetAllOrdersForadminasync()
    {
        throw new NotImplementedException();
    }
}