using DAL.Data;
using DAL.enums;
using EcoAppApi.DTOs;
using EcoAppApi.Utils;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoAppApi.xUnitTests.Services;

public class OrderUpdateTests : TestBase
{
    private readonly DALContext _context;
    private readonly IOrderService _orderService;
    private readonly IOrderRepository _orderRepository;

    public OrderUpdateTests()
    {
        _context = GetInMemoryDbContext();
        _orderRepository = new OrderRepository(_context);
        _orderService = new OrderService(_orderRepository, new PricingService(), _context);

    }
    /// <summary>
    /// Tests if the UpdateOrderAsync method correctly updates the details of an existing order.
    /// </summary>
    [Fact]
    public async Task UpdateOrder_ShouldModifyOrderDetails()
    {
        // Arrange: Define updated order details.
        await SeedTestDataAsync(_context);

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
        Assert.True(updatedOrder);
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
        await SeedTestDataAsync(_context);
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
            Id = 9999, // Not exist
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
        await SeedTestDataAsync(_context);
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
        await SeedTestDataAsync(_context);

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

}