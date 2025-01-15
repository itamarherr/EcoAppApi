using DAL.Data;
using DAL.enums;
using DAL.Models;
using EcoAppApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace EcoAppApi.xUnitTests.Services;
public abstract class TestBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderService _orderService;
    private readonly DALContext _context;

    public TestBase()
    {
        _context = GetInMemoryDbContext ();
        _orderRepository = new OrderRepository (_context);
        _orderService = new OrderService (_orderRepository, new PricingService (), _context);

    }
    protected DALContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DALContext> ()
            .UseInMemoryDatabase (databaseName: Guid.NewGuid ().ToString ())
            .EnableSensitiveDataLogging ()
            .Options;

        return new DALContext (options);
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
    protected async Task SeedTestDataAsync(
          DALContext context,
          int numberOfOrders = 1,
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
        context.Users.Add (user);

        // Arrange: Add a product to the database
        var product = new Product
        {
            Id = 1,
            Name = "Oak Consultancy",
            Description = "This service provides expert consultation for tree health and relocation",
            Price = 1100,
            Editing = false
        };
        context.Products.Add (product);

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

        context.Orders.Add (order);

        // Act: Save all changes to the in-memory database
        await context.SaveChangesAsync ();
    }
}
