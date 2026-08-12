using SecurityIds;

namespace SecurityIds.Tests;

public class SedolValidationTests
{
    public static readonly TheoryData<string, string> OracleSedols = new()
    {
        // Weighted modulus 10 worked example, all-digit base.
        { "0263494", "Digit-only base" },
        // Weighted modulus 10 worked example, letters present in the base.
        { "B0YBKL9", "Letter-containing base" },
    };

    [Theory]
    [MemberData(nameof(OracleSedols))]
    public void IsValidSedol_accepts_published_oracle_examples(string sedol, string label)
    {
        Assert.True(SecurityIdentifiers.IsValidSedol(sedol), label);
    }

    [Theory]
    [InlineData("0263495")]
    [InlineData("0263493")]
    [InlineData("B0YBKL8")]
    public void IsValidSedol_rejects_oracle_examples_with_corrupted_check_digit(string corrupted)
    {
        Assert.False(SecurityIdentifiers.IsValidSedol(corrupted));
    }

    [Theory]
    [InlineData("0263594")]
    [InlineData("B0YBKM9")]
    public void IsValidSedol_rejects_oracle_examples_with_corrupted_body(string corrupted)
    {
        Assert.False(SecurityIdentifiers.IsValidSedol(corrupted));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("026349")]
    [InlineData("02634944")]
    [InlineData("026349a")]
    [InlineData("A263494")]
    [InlineData("E263494")]
    [InlineData("026349!")]
    public void IsValidSedol_rejects_malformed_input(string? value)
    {
        Assert.False(SecurityIdentifiers.IsValidSedol(value));
    }

    [Theory]
    [InlineData("026349", 4)]
    [InlineData("B0YBKL", 9)]
    public void ComputeSedolCheckDigit_matches_oracle_examples(string basePart, int expectedCheckDigit)
    {
        Assert.Equal(expectedCheckDigit, SecurityIdentifiers.ComputeSedolCheckDigit(basePart));
    }

    [Fact]
    public void ComputeSedolCheckDigit_throws_for_null_base()
    {
        Assert.Throws<ArgumentNullException>(() => SecurityIdentifiers.ComputeSedolCheckDigit(null!));
    }

    [Theory]
    [InlineData("02634")]
    [InlineData("0263494")]
    [InlineData("02634E")]
    public void ComputeSedolCheckDigit_throws_for_malformed_base(string basePart)
    {
        Assert.Throws<ArgumentException>(() => SecurityIdentifiers.ComputeSedolCheckDigit(basePart));
    }
}
