using SecurityIds;

namespace SecurityIds.Tests;

public class FigiValidationTests
{
    private const string OracleFigi = "BBG000BLNNH6";
    private const string OracleFigiPayload = "BBG000BLNNH";
    private const int OracleFigiCheckDigit = 6;

    [Fact]
    public void IsValidFigi_accepts_published_oracle_example()
    {
        Assert.True(SecurityIdentifiers.IsValidFigi(OracleFigi));
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
    public void ComputeFigiCheckDigit_throws_for_null_payload()
    {
        Assert.Throws<ArgumentNullException>(() => SecurityIdentifiers.ComputeFigiCheckDigit(null!));
    }

    [Theory]
    [InlineData("BBG000BLNN")]
    [InlineData("BBF000BLNNH")]
    [InlineData("BBG000BLN!H")]
    public void ComputeFigiCheckDigit_throws_for_malformed_payload(string payload)
    {
        Assert.Throws<ArgumentException>(() => SecurityIdentifiers.ComputeFigiCheckDigit(payload));
    }
}
