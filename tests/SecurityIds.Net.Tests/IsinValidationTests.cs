using SecurityIds;

namespace SecurityIds.Tests;

public class IsinValidationTests
{
    public static readonly TheoryData<string, string> OracleIsins = new()
    {
        // ISO 6166 worked example: Apple Inc common stock.
        { "US0378331005", "Apple Inc" },
        // ISO 6166 worked example: Alphabet Inc class C capital stock, NSIN contains a letter.
        { "US38259P7069", "Alphabet Inc Class C" },
    };

    [Theory]
    [MemberData(nameof(OracleIsins))]
    public void IsValidIsin_accepts_published_oracle_examples(string isin, string label)
    {
        Assert.True(SecurityIdentifiers.IsValidIsin(isin), label);
    }

    [Theory]
    [InlineData("US0378331004")]
    [InlineData("US0378331006")]
    [InlineData("US38259P7068")]
    public void IsValidIsin_rejects_oracle_examples_with_corrupted_check_digit(string corrupted)
    {
        Assert.False(SecurityIdentifiers.IsValidIsin(corrupted));
    }

    [Theory]
    [InlineData("US0478331005")]
    [InlineData("UT0378331005")]
    public void IsValidIsin_rejects_oracle_examples_with_corrupted_body(string corrupted)
    {
        Assert.False(SecurityIdentifiers.IsValidIsin(corrupted));
    }

    [Theory]
    [InlineData("US037833100G")]
    [InlineData("US037833100O")]
    [InlineData("US037833100W")]
    public void IsValidIsin_rejects_a_letter_in_the_check_digit_position(string corrupted)
    {
        // ISO 6166 requires the 12th character to be a numeric digit. A letter that happens to
        // expand to a Luhn-sum-preserving digit pair must not validate as if it were the correct
        // check digit; this is the exact transcription-corruption class the package exists to catch.
        Assert.False(SecurityIdentifiers.IsValidIsin(corrupted));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("US037833100")]
    [InlineData("US03783310055")]
    [InlineData("us0378331005")]
    [InlineData("1S0378331005")]
    [InlineData("U10378331005")]
    [InlineData("US037833100!")]
    public void IsValidIsin_rejects_malformed_input(string? value)
    {
        Assert.False(SecurityIdentifiers.IsValidIsin(value));
    }

    [Theory]
    [InlineData("US037833100", 5)]
    [InlineData("US38259P706", 9)]
    public void ComputeIsinCheckDigit_matches_oracle_examples(string payload, int expectedCheckDigit)
    {
        Assert.Equal(expectedCheckDigit, SecurityIdentifiers.ComputeIsinCheckDigit(payload));
    }

    [Fact]
    public void ComputeIsinCheckDigit_throws_for_null_payload()
    {
        Assert.Throws<ArgumentNullException>(() => SecurityIdentifiers.ComputeIsinCheckDigit(null!));
    }

    [Theory]
    [InlineData("US037833100X")]
    [InlineData("US03783310")]
    [InlineData("1S037833100")]
    public void ComputeIsinCheckDigit_throws_for_malformed_payload(string payload)
    {
        Assert.Throws<ArgumentException>(() => SecurityIdentifiers.ComputeIsinCheckDigit(payload));
    }
}
