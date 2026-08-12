using SecurityIds;

namespace SecurityIds.Tests;

public class FigiValidationTests
{
    private const string OracleFigi = "BBG000BLNNH6";
    private const string OracleFigiPayload = "BBG000BLNNH";
    private const int OracleFigiCheckDigit = 6;

    // Apple Inc's published FIGI, independently hand-verified against the same algorithm
    // (per-character doubling by position, decimal digit sum, modulus 10). A second real-world
    // oracle corroborating the check-digit algorithm beyond the single Bloomberg example above.
    private const string SecondOracleFigi = "BBG000B9XRY4";
    private const string SecondOracleFigiPayload = "BBG000B9XRY";
    private const int SecondOracleFigiCheckDigit = 4;

    [Fact]
    public void IsValidFigi_accepts_published_oracle_example()
    {
        Assert.True(SecurityIdentifiers.IsValidFigi(OracleFigi));
    }

    [Fact]
    public void IsValidFigi_accepts_second_published_oracle_example()
    {
        Assert.True(SecurityIdentifiers.IsValidFigi(SecondOracleFigi));
    }

    [Theory]
    [InlineData("BBG000BLNNH0")]
    [InlineData("BBG000BLNNH5")]
    [InlineData("BBG000BLNNH7")]
    public void IsValidFigi_rejects_oracle_example_with_corrupted_check_digit(string corrupted)
    {
        Assert.False(SecurityIdentifiers.IsValidFigi(corrupted));
    }

    [Theory]
    [InlineData("BBG000BLNMH6")]
    [InlineData("BBG100BLNNH6")]
    public void IsValidFigi_rejects_oracle_example_with_corrupted_body(string corrupted)
    {
        Assert.False(SecurityIdentifiers.IsValidFigi(corrupted));
    }

    [Theory]
    [InlineData("BSG000BLNNH6")]
    [InlineData("BBF000BLNNH6")]
    public void IsValidFigi_rejects_values_missing_the_fixed_third_character(string value)
    {
        Assert.False(SecurityIdentifiers.IsValidFigi(value));
    }

    [Theory]
    [InlineData("BBG000BLNNA3")]
    [InlineData("BBGE00BLNNH2")]
    [InlineData("BBG000ULNNH1")]
    public void IsValidFigi_rejects_values_containing_a_vowel(string value)
    {
        // The OMG FIGI specification restricts every character after the fixed 'G' marker to
        // upper case consonants and digits; vowels are excluded by design.
        Assert.False(SecurityIdentifiers.IsValidFigi(value));
    }

    [Theory]
    [InlineData("BSG000BLNNH1")]
    [InlineData("BMG000BLNNH1")]
    [InlineData("GGG000BLNNH1")]
    [InlineData("GBG000BLNNH1")]
    [InlineData("GHG000BLNNH1")]
    [InlineData("KYG000BLNNH1")]
    [InlineData("VGG000BLNNH1")]
    public void IsValidFigi_rejects_reserved_two_letter_prefixes(string value)
    {
        // BS, BM, GG, GB, GH, KY and VG are reserved to avoid colliding with an ISIN country code.
        Assert.False(SecurityIdentifiers.IsValidFigi(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("BBG000BLNNH")]
    [InlineData("BBG000BLNNH66")]
    [InlineData("bbg000blnnh6")]
    [InlineData("BBG000BLNN!6")]
    public void IsValidFigi_rejects_malformed_input(string? value)
    {
        Assert.False(SecurityIdentifiers.IsValidFigi(value));
    }

    [Fact]
    public void ComputeFigiCheckDigit_matches_oracle_example()
    {
        Assert.Equal(OracleFigiCheckDigit, SecurityIdentifiers.ComputeFigiCheckDigit(OracleFigiPayload));
    }

    [Fact]
    public void ComputeFigiCheckDigit_matches_second_oracle_example()
    {
        Assert.Equal(SecondOracleFigiCheckDigit, SecurityIdentifiers.ComputeFigiCheckDigit(SecondOracleFigiPayload));
    }

    [Fact]
    public void ComputeFigiCheckDigit_throws_for_null_payload()
    {
        Assert.Throws<ArgumentNullException>(() => SecurityIdentifiers.ComputeFigiCheckDigit(null!));
    }

    [Theory]
    [InlineData("BBG000BLNN")]
    [InlineData("BBF000BLNNH")]
    [InlineData("BBG000BLN!H")]
    [InlineData("BBG000BLNNA")]
    [InlineData("BSG000BLNNH")]
    public void ComputeFigiCheckDigit_throws_for_malformed_payload(string payload)
    {
        Assert.Throws<ArgumentException>(() => SecurityIdentifiers.ComputeFigiCheckDigit(payload));
    }
}
