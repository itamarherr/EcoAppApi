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

        var orders = await _orderRepository.GetOrdersAsync(userId, userEmail, sortBy, descending, page, pageSize);

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
            CreatedAt = o.CreatedAt

        }).ToList(), totalItems);
      
    }
}
    //private readonly PricingService _pricingService;
    //private readonly DALContext _context = context;



    //public async Task<Order> createOrderAsync(CreateOrderProductDto orderDto)
    //{
    //    var user = await _context.Users.FindAsync(orderDto.UserId);
    //    var product = await _context.Products.FindAsync(orderDto.ProductId);

    //    if (user == null || product == null)
    //    {
    //        throw new ArgumentException("Invalid UserId or ProductId");
    //    }

    //    var order = new Order
    //    {
    //        UserId = orderDto.UserId.ToString(),
    //        ProductId = orderDto.ProductId,
    //        AdditionalNotes = orderDto.AdditionalNotes,
    //        TotalPrice = orderDto.TotalPrice ?? product.Price, // Default to Product Price if TotalPrice is null
    //        DateForConsultancy = orderDto.DateForConsultancy ?? DateTime.Now,
    //        CreatedAt = DateTime.UtcNow,
    //        //StatusType = orderDto.StatusType 
    //        //ImageUrl = orderDto.ImageUrl
    //    };

    //    _context.Orders.Add(order);
    //    await _context.SaveChangesAsync();
    //    return order;
    //}


    //public async Task<List<OrderDto>> GetAllOrdersForAdminAsync()
    //{
    //    var orders = await _orderRepository.GetAllOrdersForAdminAsync();

    //    return orders.Select(order => new OrderDto
    //    {
    //        Id = order.Id,
    //        UserEmail = order.User?.Email ?? "N/A",
    //        ServiceType = order.Product?.Name ?? "Unspecified",
    //        StatusType = order.StatusType,

    //        CreatedAt = order.CreatedAt,
    //        AdditionalNotes = order.AdditionalNotes
    //    }).ToList();
    //}

    //public Task<List<Order>> GetAllOrdersForadminasync()
    //{
    //    throw new NotImplementedException();
    //}

    //public async Task<OrderDto> UpdateOrderAsync(string id, UpdateOrderDto orderDto)
    //{
    //    var order = await _context.Orders.FindAsync(id);
    //    if (order == null)
    //    {
    //        throw new KeyNotFoundException("Order not found");
    //    }

    //    order.StatusType = order.StatusType;
    //    order.TotalPrice = orderDto.TotalPrice ?? order.TotalPrice;
    //    order.AdditionalNotes = orderDto.AdditionalNotes ?? order.AdditionalNotes;
    //    order.DateForConsultancy = orderDto.DateForConsultancy ?? order.DateForConsultancy;
    //    // Update other fields similarly
    //    await _context.SaveChangesAsync();

    //    return order.ToDto();
    //}
