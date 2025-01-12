using EcoAppApi.Calculations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoAppApi.xUnitTests.DataTests
{
    public class CalculatorTests
    {
        [Fact]
        public void Add_GivenTwointValues_ReturnsInt()
        {
            var cal = new Calculator();
            var result = cal.Add(1, 2); 
            Assert.Equal(3, result);
        }
        [Fact]
        public void AddDouble_GivenTwoDoubleValues_ReturnsInt()
        {
            var cal = new Calculator();
            var result = cal.AddDouble(1.20, 3.5);
            Assert.Equal(4.7, result, 2 );
        }
    }
}
