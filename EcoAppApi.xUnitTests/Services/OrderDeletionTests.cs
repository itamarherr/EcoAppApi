using DAL.Data;
using EcoAppApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace EcoAppApi.xUnitTests.Services;

public class OrderDeletionTests : TestBase
{
    private readonly DALContext _context;
    private readonly IOrderService _orderService;
    private readonly IOrderRepository _orderRepository;

    public OrderDeletionTests()
    {
        _context = GetInMemoryDbContext ();
        _orderRepository = new OrderRepository (_context);
        _orderService = new OrderService (_orderRepository, new PricingService (), _context);
    }
    /// <summary>
    /// Verifies that an existing order can be deleted from the database.
    /// Ensures that the order no longer exists after deletion.
    /// </summary>
    [Fact]
    public async Task DeleteOrder_ShouldRemoveOrderFromDatabase()
    {
        await SeedTestDataAsync (_context);

        // Arrange: Ensure there is an existing order for the user.
        var existingOrder = await _orderRepository.GetLatestOrderAsync ("user1");
        Assert.NotNull (existingOrder);  // The order must exist before we can delete it.

        // Act: Delete the latest order.
        var result = await _orderService.DeleteLastOrderAsync ("user1");

        // Assert: Ensure the deletion was successful.
        Assert.True (result);

        // Additional Assert: Ensure the order no longer exists in the database.
        var deleteOrder = await _orderRepository.GetOrderByIdAsync (existingOrder.Id);

        Assert.Null (deleteOrder); // The order should be null as it was deleted.
    }

    /// <summary>
    /// Ensures that attempting to delete an order for a user with no existing orders
    /// throws an ArgumentException.
    /// </summary>
    [Fact]
    public async Task DeleteOrder_ShouldReturnFalseIfNoOrderExists()
    {
        // Arrange: Ensure the database is empty (no orders exist).
        var orders = await _context.Orders.ToListAsync ();
        Assert.Empty (orders);

        // Act & Assert: Attempt to delete an order for a user with no orders, and expect an exception.
        await Assert.ThrowsAsync<ArgumentException> (async () =>
        {
            await _orderService.DeleteLastOrderAsync ("nonexistentUser");
        });
    }
    /// <summary>
    /// Verifies that the admin can delete order successfully by its ID.
    /// Ensures the order no longer exists in the database after deletion.
    /// </summary>
    [Fact]
    public async Task DeleteOrderById_ShouldRemoveOrderFromDatabase()
    {
        await SeedTestDataAsync (_context);

        // Arrange: Ensure there is an existing order with a known ID.
        var existingOrder = await _orderRepository.GetOrderByIdAsync (1); // Assuming ID = 1 exists in SeedTestData
        Assert.NotNull (existingOrder); // The order must exist before we can delete it.

        // Act: Delete the order by its ID.
        var result = await _orderService.DeleteOrderByIdAsync (existingOrder.Id);

        // Assert: Ensure the deletion was successful.
        Assert.True (result);

        // Additional Assert: Ensure the order no longer exists in the database.
        var deletedOrder = await _orderRepository.GetOrderByIdAsync (existingOrder.Id);
        Assert.Null (deletedOrder); // The order should be null as it was deleted.
    }
    /// <summary>
    /// Ensures that attempting to delete an order with an invalid or non-existent ID
    /// throws an ArgumentException.
    /// </summary>
    [Fact]
    public async Task DeleteOrderById_ShouldThrowIfOrderDoesNotExist()
    {
        // Arrange: Ensure the database is empty or ID is non-existent.
        var nonExistentId = 9999; // An ID that does not exist
        var orders = await _context.Orders.ToListAsync ();
        Assert.DoesNotContain (orders, o => o.Id == nonExistentId);

        // Act & Assert: Attempt to delete an order by a non-existent ID and expect an exception.
        await Assert.ThrowsAsync<InvalidOperationException> (async () =>
        {
            await _orderService.DeleteOrderByIdAsync (nonExistentId);
        });
    }


}
