﻿using DAL.Data;
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

   
}







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
