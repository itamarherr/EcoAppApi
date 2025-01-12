using EcoAppApi.Calculations;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using Xunit.Abstractions;
namespace EcoAppApi.xUnitTests.DataTests
{
    public class CollectionsFixture : IDisposable
    {
        public static Collections Coll => new Collections();

        public void Dispose()
        {
           //clean
        }
    }

    public class CollectionsTests :  IClassFixture<CollectionsFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly CollectionsFixture _collctionsFixture;
        private readonly MemoryStream memoryStream;

        public CollectionsTests(ITestOutputHelper testOutputHelper, CollectionsFixture collctionsFixture)
        {
            _testOutputHelper = testOutputHelper;
            _collctionsFixture = collctionsFixture;
            _testOutputHelper.WriteLine("Constractor");
        }
        [Fact]
        [Trait("Category", "Fibo")]
        public void FiboNumbersNotIncludeZero()
        {
            _testOutputHelper.WriteLine("FiboNumbersNotIncludeZero");
            var coll = CollectionsFixture.Coll;
            Assert.All(coll.FiboNumbers, n => Assert.NotEqual(0, n));
        }
        [Fact]
        [Trait("Category", "Fibo")]
        public void FiboNumbersInclude13()
        {
            _testOutputHelper.WriteLine("FiboNumbersInclude13");
            var coll = CollectionsFixture.Coll;
            var result = coll.FiboNumbers;
            Assert.Contains(13, result);
        }
        //[Fact]
        //[Trait("Category", "Fibo")]
        //public void FiboNumbersNotInclude4()
        //{
        //    var coll = new Collctions();
        //    var result = coll.FiboNumbers;
        //    Assert.DoesNotContain(4, result);
        //}
        [Fact]
        [Trait("Category", "Fibo")]
        public void CheckCollection()
        {
            _testOutputHelper.WriteLine("CheckCollection. Test Starting at{ 0}");
            var expectedCollection = new List<int>() { 1, 1, 2, 3, 5, 8, 13};
            _testOutputHelper.WriteLine("Creating an insteance of Collection calss..");
            var coll = CollectionsFixture.Coll;
            var result = coll.FiboNumbers;
            Assert.Equal(expectedCollection, result);
        }
        //[Fact]
        //public void IsOdd_GivenOddValue_ReturnsTrue()
        //{
        //    var coll = new Collections();
        //    var result = coll.IsOdd(1);
        //    Assert.True(result);
        //}
        //[Fact]
        //public void IsOdd_GivenEvenValue_ReturnsFalse()
        //{
        //    var coll = new Collections();
        //    var result = coll.IsOdd(2);
        //    Assert.False(result);
        //}
        //[Theory]
        //[InlineData(1, true)]
        //[InlineData(200, false)]
        //public void IsOdd_TestOddAndEven(int value, bool expected)
        //{
        //    var coll = new Collections();
        //    var result = coll.IsOdd(value);
        //    Assert.Equal(expected, result);
        //}
        [Theory]
        //[MemberData(nameof(TestDataShare.IsOddOrEvenExternalData), MemberType = typeof(TestDataShare))]
        [IsOddOrEvenData]
        public void IsOdd_TestOddAndEven(int value, bool expected)
        {
            var coll = new Collections();
            var result = coll.IsOdd(value);
            Assert.Equal(expected, result);
        }
    }
}
