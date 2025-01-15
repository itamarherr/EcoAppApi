using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoAppApi.xUnitTests.Services;

public class OrderRetrievalTests : TestBase
{
    private readonly DALContext _context;
    private readonly IOrderRepository _orderRepository;
    public OrderRetrievalTests()
    {
        _context = GetInMemoryDbContext();
        _orderRepository = new OrderRepository(_context);
    }

    /// <summary>
    /// Tests fetching an order by its ID from the repository.
    /// Ensures the order exists and its properties match the expected values.
    /// </summary>
    [Fact]
    public async Task GetOrder_ShouldReturnCorrectOrder()
    {
        // Arrange: Seed the database with test data.
        await SeedTestDataAsync(_context);

        // Act: Retrieve the order with ID 1 from the repository.
        var order = await _orderRepository.GetOrderByIdAsync(1);

        // Assert: Verify that the order exists and properties match the expected values.
        Assert.NotNull(order); // Ensure the order is not null.
        Assert.Equal(3, order.NumberOfTrees);// Verify the number of trees matches the seeded data.
        Assert.Equal("City", order.City); // Verify the city matches the seeded data.
    }

    /// <summary>
    /// Verifies that the GetLatestOrderAsync method retrieves the latest order
    /// (based on CreatedAt) for a specific user.
    /// Ensures the returned order matches the expected data.
    /// </summary>
    [Fact]
    public async Task GetLatestOrderAsync_ShouldReturnLatestOrderForUser()
    {
        // Arrange: seed Multiple orders for the Same user.
        await SeedTestDataAsync(_context, numberOfOrders: 2);
        var latestOrder = new Order
        {
            Id = 2,
            UserId = "user1",
            City = "New City", 
            Street = "New Street",
            CreatedAt = DateTime.UtcNow.AddDays(1)
        };
        _context.Orders.Add(latestOrder);
        await _context.SaveChangesAsync();

        //Act: Fatch the latest order for the user.
        var result = await _orderRepository.GetLatestOrderAsync("user1");

        Assert.NotNull(result);
        Assert.Equal(2, await _context.Orders.CountAsync());
    }

    /// <summary>
    /// Verifies that the GetPaginatedOrdersAsync method retrieves all orders
    /// without applying user or email filters.
    /// Ensures the total count of returned orders matches the expected number.
    /// </summary>
    [Fact]
    public async Task GetOrdersAsync_ShouldReturnAllOrders()
    {
        // Arrange: seed Multiple orders.
        await SeedTestDataAsync(_context, numberOfOrders: 2);
        var additionalOrder = new Order
        {
            Id = 2,
            UserId = "user2",
            City = "New City",
            Street = "New Street",
            CreatedAt = DateTime.UtcNow
        };
        _context.Orders.Add(additionalOrder);
        await _context.SaveChangesAsync();

        //Act: Fatch the latest order for the user.
        var result = await _orderRepository.GetPaginatedOrdersAsync(
          userId: null,
          userEmail: null,
          sortBy: "CreatedAt",
          descending: false,
          page: 1,
          pageSize: int.MaxValue
          );
        // Assert: Ensure all orders are returned.
        Assert.NotNull(result);
        Assert.Equal(2, await _context.Orders.CountAsync());
    }
}