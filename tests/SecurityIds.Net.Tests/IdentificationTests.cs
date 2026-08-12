using SecurityIds;

namespace SecurityIds.Tests;

public class IdentificationTests
{
    [Theory]
    [InlineData("US0378331005", SecurityIdentifierType.Isin)]
    [InlineData("US38259P7069", SecurityIdentifierType.Isin)]
    [InlineData("459200101", SecurityIdentifierType.Cusip)]
    [InlineData("68389X105", SecurityIdentifierType.Cusip)]
    [InlineData("0263494", SecurityIdentifierType.Sedol)]
    [InlineData("B0YBKL9", SecurityIdentifierType.Sedol)]
    [InlineData("BBG000BLNNH6", SecurityIdentifierType.Figi)]
    public void TryIdentify_detects_the_correct_scheme_for_valid_identifiers(string value, SecurityIdentifierType expected)
    {
        Assert.Equal(expected, SecurityIdentifiers.TryIdentify(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-an-identifier")]
    [InlineData("0263495")]
    [InlineData("459200100")]
    [InlineData("US0378331004")]
    [InlineData("BBG000BLNNH0")]
    [InlineData("123456789012")]
    public void TryIdentify_returns_none_for_unrecognized_or_invalid_values(string? value)
    {
        Assert.Equal(SecurityIdentifierType.None, SecurityIdentifiers.TryIdentify(value));
    }

    [Fact]
    public void TryIdentify_prefers_figi_over_isin_for_the_bloomberg_g_marker_pattern()
    {
        Assert.Equal(SecurityIdentifierType.Figi, SecurityIdentifiers.TryIdentify("BBG000BLNNH6"));
    }
}
