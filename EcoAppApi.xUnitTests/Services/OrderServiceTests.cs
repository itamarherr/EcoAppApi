using DAL.Data;
using DAL.enums;
using DAL.Models;
using EcoAppApi.DTOs;
using EcoAppApi.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoAppApi.xUnitTests.Services;

public class OrderServiceTests
{
    private readonly IOrderRepository _orderRepository; 
    private readonly IOrderService _orderService;      
    private readonly DALContext _context;

    public OrderServiceTests() 
    {
        _context = GetInMemoryDbContext();
        _orderRepository = new  OrderRepository(_context);
        _orderService = new OrderService(_orderRepository, new PricingService(), _context);
        
    }
    private DALContext GetInMemoryDbContext()
    {
      
        var options = new DbContextOptionsBuilder<DALContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .EnableSensitiveDataLogging()
            .Options;

        return new DALContext(options);
    }
    [Fact]
    public async Task CreateOrder_ShouldAddOrderToDatabase()
    {

        await SeedTestDataAsync();

        var createOrderDto = new CreateOrderDto
        {
            UserId = "user1",
            ProductId = 1,
            NumberOfTrees = 5,
            City = "City",
            Street = "Street",
            Number = 10,
            ConsultancyType = Purpose.BeforeConstruction,
            IsPrivateArea = true,
            DateForConsultancy = DateTime.UtcNow,
            StatusType = OrderStatus.pending
            // TotalPrice is calculated by the PricingService; the value in CreateOrderDto is ignored

        };
        var result = await _orderService.CreateOrderAsync(createOrderDto, "user1");
        Assert.NotNull(result);
        Assert.Equal("user1", result.UserId);
        Assert.Equal(1200, result.TotalPrice);
        Assert.Equal(5, result.NumberOfTrees);
        Assert.Equal("City", result.City);
    }

    [Theory]
    [InlineData(Purpose.BeforeConstruction, 5, true, 1200)] 
    [InlineData(Purpose.Dislocations, 10, false, 2340)]     
    [InlineData(Purpose.TreesIllness, 1, true, 2000)]       
    public void PricingService_ShouldCalculateTotalPriceCorrectly(
     Purpose consultancyType, int numberOfTrees, bool isPrivate, decimal expectedPrice)
    {
     
        var pricingService = new PricingService();

       
        var totalPrice = pricingService.CalculatePrice(consultancyType, numberOfTrees, isPrivate);

       
        Assert.Equal(expectedPrice, totalPrice);
    }
    [Fact]
    public async Task UpdateOrder_ShouldModifyOrderDetails()
    {

        await SeedTestDataAsync();

        var updateOrderDto = new UpdateOrderDto
        {
            AdminNotes = "Updated by admin",
            TotalPrice = 200,
            NumberOfTrees = 5,
            City = "New City",
            Street = "New Street",
            Number = 20,
            ConsultancyType = Purpose.Dislocations,
            IsPrivateArea = false,
            DateForConsultancy = DateTime.UtcNow.AddDays(1),
            StatusType = OrderStatus.completed

        };
        var updatedOrder = await _orderService.UpdateOrderAsync(1, updateOrderDto);
        Assert.True( updatedOrder);
       
    }

    [Fact]
    public async Task DeleteOrder_ShouldRemoveorderFromDatabase()
    {
        await SeedTestDataAsync();
        var existingOrder = await _orderRepository.GetLatestOrderAsync("user1");
        Assert.NotNull(existingOrder);

        var result = await _orderService.DeleteLastOrderAsync("user1");
        Assert.True(result);

        var deleteOrder = await _orderRepository.GetOrderByIdAsync(existingOrder.Id);

        Assert.NotNull(existingOrder);

    }
    [Fact]
    public async Task DeleteOrder_ShouldReturnFalseIfNoOrderExists()
    {
        var orders = await _context.Orders.ToListAsync();
        Assert.Empty(orders);
        // Act: Attempt to delete an order for a user with no orders
        var result = await _orderService.DeleteLastOrderAsync("nonexistentUser");

        // Assert: Ensure the result is false
        Assert.False(result);
    }
    private async Task SeedTestDataAsync()
    {
        var user = new AppUser { Id = "user1", UserName = "Test User" };
        var product = new Product
        {
            Id = 1,
            Name = "Oak Consultancy",
            Description = "This service provides expert consultation for tree health and relocation",
            Price = 1100,
            Editing = false
        };
        var order = new Order
        {
            UserId = "user1",
            ProductId = 1,
            TotalPrice = 150,
            NumberOfTrees = 3,
            City = "City",
            Street = "Street",
            Number = 10,
            ConsultancyType = Purpose.BeforeConstruction,
            IsPrivateArea = true,
            DateForConsultancy = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        _context.Products.Add(product);
        _context.Orders.Add(order);

        await _context.SaveChangesAsync();
    }
}
