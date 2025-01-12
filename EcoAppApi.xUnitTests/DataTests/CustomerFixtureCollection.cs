using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using EcoAppApi.Calculations;

namespace EcoAppApi.xUnitTests.DataTests;

[CollectionDefinition("Customer")]
public class CustomerFixtureCollection : ICollectionFixture<CustomerFixture>
{
}
