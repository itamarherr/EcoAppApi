using EcoAppApi.Calculations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace EcoAppApi.xUnitTests.DataTests
{
    [Collection("Customer")]
    public class CustomerTests
    {
        private readonly CustomerFixture _customerFixture;

        public CustomerTests(CustomerFixture customerFixture) 
        {
            _customerFixture = customerFixture;
        }
        [Fact]
        public void ChecklegForDiscount()
        {
            var customer = _customerFixture.Cust;
            var result = customer.Age;
            Assert.InRange(result, 35, 50);
        }
        [Fact]
        public void GetOrderByNameNotNull()
        {
            var cosumer = _customerFixture.Cust;
           
            var exceptionDetails = Assert.Throws<ArgumentException>(() => cosumer.GetOrderByName(""));
           Assert.Equal("Hello", exceptionDetails.Message);
        }
        [Fact]
        public void LoyalCustomerForOrderG100()
        {
            var customer = CustomerFactory.CreateCustomerInsance(102);
            //Assert.IsType(typeof(Customer), customer);
            var loyalCustomer = Assert.IsType<LoyalCustomer>(customer);
            Assert.Equal(20, loyalCustomer.Discount);
        }

    }
}
