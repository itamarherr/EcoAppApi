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

    /// <summary>
    /// Tests fetching an order by its ID from the repository.
    /// Ensures the order exists and its properties match the expected values.
    /// </summary>
    [Fact] 
    public async Task GetOrder_ShouldReturnCorrectOrder()
    {
        // Arrange: Seed the database with test data.
        await SeedTestDataAsync();

        // Act: Retrieve the order with ID 1 from the repository.
        var order = await _orderRepository.GetOrderByIdAsync(1);

        // Assert: Verify that the order exists and properties match the expected values.
        Assert.NotNull(order); // Ensure the order is not null.
        Assert.Equal(3, order.NumberOfTrees);// Verify the number of trees matches the seeded data.
        Assert.Equal("City", order.City); // Verify the city matches the seeded data.
    }

    /// <summary>
    /// Tests the creation of a new order.
    /// Ensures the order is saved correctly and all properties match the expected values.
    /// </summary>
    [Fact]
    public async Task CreateOrder_ShouldAddOrderToDatabase()
    {
        // Arrange: Seed the database with test data and define a new order to create.
        await SeedTestDataAsync();
        

        var createOrderDto = new CreateOrderDto
        {
            UserId = "user1", // Existing user ID from the seeded data.
            ProductId = 1, // Existing product ID from the seeded data.
            NumberOfTrees = 5, // Custom number of trees for the new order.
            City = "City",
            Street = "Street",
            Number = 10,
            ConsultancyType = Purpose.BeforeConstruction,
            IsPrivateArea = true,
            DateForConsultancy = DateTime.UtcNow,
            StatusType = OrderStatus.pending
            // TotalPrice is calculated by the PricingService; the value in CreateOrderDto is ignored
        };

        // Act: Call the CreateOrderAsync method.
        var result = await _orderService.CreateOrderAsync(createOrderDto, "user1");

        // Assert: Verify the order was created and properties match expectations.
        Assert.NotNull(result); // Ensure the result is not null.
        Assert.Equal("user1", result.UserId); // Verify the user ID matches.
        Assert.Equal(1200, result.TotalPrice); // Verify the total price is calculated correctly.
        Assert.Equal(5, result.NumberOfTrees); // Verify the number of trees matches.
        Assert.Equal("City", result.City); // Verify the city matches the input.
    }

    /// <summary>
    /// Ensures that creating an order with an invalid user ID throws an exception.
    /// </summary>
    [Fact]
    public async Task CreateOrder_ShouldFailInvalidUserId()
    {
        // Arrange: Define an invalid user and order details.
        var createOrderDTo = new CreateOrderDto
        {
            UserId = "invalidUser",
            ProductId = 1,
            NumberOfTrees = 5,
            City = "City",
            Street = "Street",
            Number = 10,
            ConsultancyType = Purpose.BeforeConstruction,
            IsPrivateArea = true,
            DateForConsultancy = DateTime.UtcNow,
            StatusType = OrderStatus.pending
        };

        // Act & Assert: Verify an exception is thrown.
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _orderService.CreateOrderAsync(createOrderDTo, "invalidUser");
        });
    }

    /// <summary>
    /// Tests the PricingService to ensure it calculates the total price based on
    /// consultancy type, number of trees, and whether it's a private area.
    /// </summary>
    [Theory]
    [InlineData(Purpose.BeforeConstruction, 5, true, 1200)] // Expected price for private area with 5 trees.
    [InlineData(Purpose.Dislocations, 10, false, 2340)]  // Expected price for public area with 10 trees.    
    [InlineData(Purpose.TreesIllness, 1, true, 2000)]  // Expected price for private area with 1 tree.   
    public void PricingService_ShouldCalculateTotalPriceCorrectly(
     Purpose consultancyType, int numberOfTrees, bool isPrivate, decimal expectedPrice)
    {
        // Arrange: Initialize the PricingService.
        var pricingService = new PricingService();

        // Act: Calculate the total price.
        var totalPrice = pricingService.CalculatePrice(consultancyType, numberOfTrees, isPrivate);

        // Assert: Verify the price matches the expected value.
        Assert.Equal(expectedPrice, totalPrice);
    }

    /// <summary>
    /// Tests if the UpdateOrderAsync method correctly updates the details of an existing order.
    /// </summary>
    [Fact]
    public async Task UpdateOrder_ShouldModifyOrderDetails()
    {
        // Arrange: Define updated order details.
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

        // Act: Update the order.
        var updatedOrder = await _orderService.UpdateOrderAsync(1, updateOrderDto);

        // Assert: Verify the update was successful.
        Assert.True( updatedOrder);    
    }

    /// <summary>
    /// Tests if UpdateOrderAsync correctly handles optional fields.
    /// Input: An update request with only the AdminNotes field.
    /// Expected: 
    /// - AdminNotes is updated.
    /// - Other fields remain unchanged with their original values.
    /// </summary>
    [Fact]
    public async Task UpdatOrder_shouldHandleOptionalFieldsCorrectly()
    {
        await SeedTestDataAsync();
        var updateOrderDto = new UpdateOrderDto
        {
            Id = 1,
            AdminNotes = "Optional fields test",
        };
        var result = await _orderService.UpdateOrderAsync(1, updateOrderDto);

        Assert.True(result);


        var updateOrder = await _orderRepository.GetOrderByIdAsync(1);
        Assert.NotNull(updateOrder);
        Assert.Equal("Optional fields test", updateOrder.AdminNotes);
        Assert.Equal(150, updateOrder.TotalPrice);
        Assert.Equal(OrderStatus.pending, updateOrder.StatusType);
    }

    /// <summary>
    /// Verifies that updating an order with an invalid ID throws an ArgumentException.
    /// </summary>
    [Fact]
    public async Task UpdateOrder_ShouldFailForInvalidOrderId()
    {
        // Arrange: Define an UpdateOrderDto with an invalid order ID.
        var updateOrderDto = new UpdateOrderDto
        {
            Id = 9999, // מזהה שלא קיים
            AdminNotes = "Testing invalid order ID",
            TotalPrice = 300,
            NumberOfTrees = 10,
            City = "Test City",
            Street = "Test Street",
            Number = 5,
            ConsultancyType = Purpose.TreesIllness,
            IsPrivateArea = true,
            DateForConsultancy = DateTime.UtcNow.AddDays(5),
            StatusType = OrderStatus.pending
        };

        // Act & Assert: Expect an ArgumentException when attempting to update.
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _orderService.UpdateOrderAsync(updateOrderDto.Id, updateOrderDto);
        });
    }

    /// <summary>
    /// Ensures that attempting to update an order with an invalid date
    /// (e.g., a past date) does not update the DateForConsultancy field.
    /// Valid fields in the DTO are updated as expected.
    /// </summary>
    [Fact]
    public async Task Update_ShouldNotChangeDateForInvalidDate()
    {
        // Arrange: Seed test data and prepare an UpdateOrderDto with an invalid date.
        await SeedTestDataAsync();
        var updateOrderDto = new UpdateOrderDto
        {
            Id = 1,
            AdminNotes = "Invalid date test",
            DateForConsultancy = DateTime.UtcNow.AddDays(-1) // Invalid past date.

        };

        // Act: Attempt to update the order.
        await _orderService.UpdateOrderAsync(1, updateOrderDto);

        // Assert: Verify that the DateForConsultancy was not updated.
        var updateOrder = await _orderRepository.GetOrderByIdAsync(1);
        Assert.NotNull(updateOrder);
        Assert.NotEqual(updateOrder.DateForConsultancy, updateOrderDto.DateForConsultancy);
    }

    /// <summary>
    /// Verifies that updating an order with invalid data (e.g., empty AdminNotes 
    /// or an invalid date) throws an ArgumentException and the update is rejected.
    /// </summary>
    [Fact]
    public async Task UpdateOrder_ShouldFailForInvalidData()
    {
        // Arrange: Seed test data and prepare an UpdateOrderDto with invalid data.
        await SeedTestDataAsync();

        var updateOrderDto = new UpdateOrderDto
        {
            Id = 1, // Valid order ID from the seeded data.
            AdminNotes = "", // Invalid data
            DateForConsultancy = DateTime.UtcNow.AddDays(-1) // Invalid past date.
        };

        // Act & Assert: Expect an ArgumentException when attempting to update.
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _orderService.UpdateOrderAsync(1, updateOrderDto);
        });
    }
    /// <summary>
    /// Verifies that an existing order can be deleted from the database.
    /// Ensures that the order no longer exists after deletion.
    /// </summary>
    [Fact]
    public async Task DeleteOrder_ShouldRemoveOrderFromDatabase()
    {
        await SeedTestDataAsync();

        // Arrange: Ensure there is an existing order for the user.
        var existingOrder = await _orderRepository.GetLatestOrderAsync("user1");
        Assert.NotNull(existingOrder);  // The order must exist before we can delete it.

        // Act: Delete the latest order.
        var result = await _orderService.DeleteLastOrderAsync("user1");

        // Assert: Ensure the deletion was successful.
        Assert.True(result);


        // Additional Assert: Ensure the order no longer exists in the database.
        var deleteOrder = await _orderRepository.GetOrderByIdAsync(existingOrder.Id);

        Assert.NotNull(existingOrder); // The order should be null as it was deleted.

    }

    /// <summary>
    /// Ensures that attempting to delete an order for a user with no existing orders
    /// throws an ArgumentException.
    /// </summary>
    [Fact]
    public async Task DeleteOrder_ShouldReturnFalseIfNoOrderExists()
    {
        // Arrange: Ensure the database is empty (no orders exist).
        var orders = await _context.Orders.ToListAsync();
        Assert.Empty(orders);

        // Act & Assert: Attempt to delete an order for a user with no orders, and expect an exception.
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _orderService.DeleteLastOrderAsync("nonexistentUser");
        });
    }
  
    /// <summary>
    /// Seeds test data into the in-memory database for use in tests.
    /// This includes a user, a product, and an order.
    /// </summary>
    /// <param name="userId">The ID of the user to seed.</param>
    /// <param name="productId">The ID of the product to seed.</param>
    /// <param name="city">The city for the order.</param>
    /// <param name="street">The street for the order.</param>
    /// <param name="number">The house number for the order.</param>
    private async Task SeedTestDataAsync(
          string userId = "user1",
          string userName = "Test User",
          string productName = "Oak Consultancy",
          int productId = 1,
          decimal productPrice = 1100,
          int orderId = 1,
          int numberOfTrees = 3,
          string city = "City",
          string street = "Street",
          int number = 10,
          Purpose consultancyType = Purpose.BeforeConstruction,
          bool isPrivateArea = true,
          DateTime? dateForConsultancy = null,
          OrderStatus statusType = OrderStatus.pending
        )
    {
       
        var user = new AppUser { Id = "user1", UserName = "Test User" };
        _context.Users.Add(user);

        // Arrange: Add a product to the database
        var product = new Product
        {
            Id = 1,
            Name = "Oak Consultancy",
            Description = "This service provides expert consultation for tree health and relocation",
            Price = 1100,
            Editing = false
        };
        _context.Products.Add(product);

        // Arrange: Add an order to the database
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
            CreatedAt = DateTime.UtcNow,
            StatusType = OrderStatus.pending
        };
 
        _context.Orders.Add(order);

        // Act: Save all changes to the in-memory database
        await _context.SaveChangesAsync();
    }
}
