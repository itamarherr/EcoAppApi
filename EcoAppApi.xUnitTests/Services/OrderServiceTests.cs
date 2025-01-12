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

namespace EcoAppApi.xUnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly IOrderRepository _orderRepository; // ממשק הריפו
        private readonly IOrderService _orderService;       // ממשק השירות
        private readonly DALContext _context;

        public OrderServiceTests() 
        {
            _context = GetInMemoryDbContext();
            _orderRepository = new  OrderRepository(_context);
            _orderService = new OrderService(_orderRepository, new PricingService(), _context);
            
        }
        private DALContext GetInMemoryDbContext()
        {
            // Create a new options instance for the DbContext
            var options = new DbContextOptionsBuilder<DALContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for isolation
                .EnableSensitiveDataLogging()
                .Options;

            // Return a new instance of the context with the in-memory options
            return new DALContext(options);
        }
        [Fact]
        public async Task CreateOrder_ShouldAddOrderToDatabase()
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
            _context.Users.Add(user);
            _context.Products.Add(product); 
            await _context.SaveChangesAsync();

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
    }
}
