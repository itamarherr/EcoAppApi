using DAL.enums;
using EcoAppApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoAppApi.xUnitTests.Services
{
    public class PricingServiceTests 
    {
        [Theory]
        [InlineData(Purpose.BeforeConstruction, 5, true, 1200)] // Private area with 5 trees
        [InlineData(Purpose.Dislocations, 10, false, 2340)]      // Public area with 10 trees
        [InlineData(Purpose.TreesIllness, 1, true, 2000)]        // Private area with 1 tree
        public void PricingService_ShouldCalculateTotalPriceCorrectly(
        Purpose consultancyType, int numberOfTrees, bool isPrivate, decimal expectedPrice)
        {
            // Arrange
            var pricingService = new PricingService();

            // Act
            var totalPrice = pricingService.CalculatePrice(consultancyType, numberOfTrees, isPrivate);

            // Assert
            Assert.Equal(expectedPrice, totalPrice);
        }
    }
}
