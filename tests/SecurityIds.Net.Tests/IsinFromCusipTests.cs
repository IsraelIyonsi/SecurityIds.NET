using SecurityIds;

namespace SecurityIds.Tests;

public class IsinFromCusipTests
{
    [Theory]
    [InlineData("037833100", "US", "US0378331005")]
    [InlineData("459200101", "US", "US4592001014")]
    [InlineData("68389X105", "US", "US68389X1054")]
    public void IsinFromCusip_builds_the_expected_isin(string cusip, string countryCode, string expectedIsin)
    {
        Assert.Equal(expectedIsin, SecurityIdentifiers.IsinFromCusip(cusip, countryCode));
    }

    [Fact]
    public void IsinFromCusip_result_is_itself_a_valid_isin()
    {
        var isin = SecurityIdentifiers.IsinFromCusip("037833100", "US");
        Assert.True(SecurityIdentifiers.IsValidIsin(isin));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("037833101")]
    [InlineData("03783310")]
    public void IsinFromCusip_throws_for_invalid_cusip(string? cusip)
    {
        if (cusip is null)
        {
            Assert.Throws<ArgumentNullException>(() => SecurityIdentifiers.IsinFromCusip(cusip!, "US"));
        }
        else
        {
            Assert.Throws<ArgumentException>(() => SecurityIdentifiers.IsinFromCusip(cusip, "US"));
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("U")]
    [InlineData("USA")]
    [InlineData("u1")]
    [InlineData("12")]
    public void IsinFromCusip_throws_for_invalid_country_code(string? countryCode)
    {
        if (countryCode is null)
        {
            Assert.Throws<ArgumentNullException>(() => SecurityIdentifiers.IsinFromCusip("037833100", countryCode!));
        }
        else
        {
            Assert.Throws<ArgumentException>(() => SecurityIdentifiers.IsinFromCusip("037833100", countryCode));
        }
    }
}
