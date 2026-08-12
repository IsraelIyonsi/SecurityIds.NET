using SecurityIds;

namespace SecurityIds.Tests;

public class CusipValidationTests
{
    public static readonly TheoryData<string, string> OracleCusips = new()
    {
        // Modulus 10 double-add-double worked example: IBM common stock.
        { "459200101", "IBM" },
        // Modulus 10 double-add-double worked example: Apple Inc common stock.
        { "037833100", "Apple Inc" },
        // Modulus 10 double-add-double worked example: Oracle Corp common stock, issue number contains a letter.
        { "68389X105", "Oracle Corp" },
    };

    [Theory]
    [MemberData(nameof(OracleCusips))]
    public void IsValidCusip_accepts_published_oracle_examples(string cusip, string label)
    {
        Assert.True(SecurityIdentifiers.IsValidCusip(cusip), label);
    }

    [Theory]
    [InlineData("459200100")]
    [InlineData("459200102")]
    [InlineData("037833101")]
    [InlineData("68389X104")]
    public void IsValidCusip_rejects_oracle_examples_with_corrupted_check_digit(string corrupted)
    {
        Assert.False(SecurityIdentifiers.IsValidCusip(corrupted));
    }

    [Theory]
    [InlineData("459210101")]
    [InlineData("68389Y105")]
    public void IsValidCusip_rejects_oracle_examples_with_corrupted_body(string corrupted)
    {
        Assert.False(SecurityIdentifiers.IsValidCusip(corrupted));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("45920010")]
    [InlineData("4592001011")]
    [InlineData("459200101 ")]
    [InlineData("45920010!")]
    [InlineData("459200i01")]
    public void IsValidCusip_rejects_malformed_input(string? value)
    {
        Assert.False(SecurityIdentifiers.IsValidCusip(value));
    }

    [Theory]
    [InlineData("45920010*")]
    [InlineData("45920010@")]
    [InlineData("45920010#")]
    public void IsValidCusip_rejects_extended_characters_with_wrong_check_digit(string value)
    {
        Assert.False(SecurityIdentifiers.IsValidCusip(value));
    }

    [Theory]
    [InlineData("45920010", 1)]
    [InlineData("03783310", 0)]
    [InlineData("68389X10", 5)]
    public void ComputeCusipCheckDigit_matches_oracle_examples(string basePart, int expectedCheckDigit)
    {
        Assert.Equal(expectedCheckDigit, SecurityIdentifiers.ComputeCusipCheckDigit(basePart));
    }

    [Fact]
    public void ComputeCusipCheckDigit_throws_for_null_base()
    {
        Assert.Throws<ArgumentNullException>(() => SecurityIdentifiers.ComputeCusipCheckDigit(null!));
    }

    [Theory]
    [InlineData("4592001")]
    [InlineData("459200100")]
    [InlineData("4592001!")]
    public void ComputeCusipCheckDigit_throws_for_malformed_base(string basePart)
    {
        Assert.Throws<ArgumentException>(() => SecurityIdentifiers.ComputeCusipCheckDigit(basePart));
    }
}
