using DAL.enums;
using DAL.Data;
using DAL.Models;
using EcoAppApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoAppApi.Utils;

namespace EcoAppApi.xUnitTests.Services
{
    public class OrderCreationTests : TestBase
    {

        private readonly DALContext _context;
        private readonly IOrderService _orderService;
        public OrderCreationTests()
        {
            _context = GetInMemoryDbContext();
            var orderRepository = new OrderRepository(_context);
            _orderService = new OrderService(orderRepository, new PricingService(), _context);
        }
        /// <summary>
        /// Tests the creation of a new order.
        /// Ensures the order is saved correctly and all properties match the expected values.
        /// </summary>
        [Fact]
        public async Task CreateOrder_ShouldAddOrderToDatabase()
        {
            // Arrange: Seed the database with test data and define a new order to create.
            await SeedTestDataAsync(_context);


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
    }
}
