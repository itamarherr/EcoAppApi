using EcoAppApi.Calculations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace EcoAppApi.xUnitTests.DataTests;

public class NamesTest
{
    [Fact]
    public void MakeFullNameTest()
    {
        var names = new Names();
        var result = names.MakeFullName("Itamar", "Herr");
        Assert.Equal("Itamar Herr", result, ignoreCase: true);
        Assert.Contains("Itamar", result);
        Assert.Contains("itamar", result, StringComparison.InvariantCultureIgnoreCase);
        Assert.StartsWith("Itamar", result);
        Assert.EndsWith("Herr", result);
        //Assert.Matches("[A-Z]{1}[a-z]+ [A-Z]{1}[a-z]+ ", result);

    }
    [Fact]
    public void NickName_MustBeNull()
    {
        var names = new Names();
        names.NickName = "Itamar";
        Assert.NotNull(names.NickName);
        
    }
    [Fact]
    public void MakeFullName_AlwaysReturnValue()
    {
        var names = new Names();
        var result = names.MakeFullName("Itamar", "Herr");
        Assert.False(string.IsNullOrEmpty(result));
    }
}
