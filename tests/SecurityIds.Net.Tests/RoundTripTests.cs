using SecurityIds;

namespace SecurityIds.Tests;

public class RoundTripTests
{
    public static readonly TheoryData<string> IsinPayloads = new()
    {
        "US037833100",
        "GB000B19NLV",
        "DE000BASF11",
        "FR001400ABC",
    };

    [Theory]
    [MemberData(nameof(IsinPayloads))]
    public void Isin_round_trips_through_compute_and_validate(string payload)
    {
        var checkDigit = SecurityIdentifiers.ComputeIsinCheckDigit(payload);
        var isin = payload + checkDigit;
        Assert.True(SecurityIdentifiers.IsValidIsin(isin));
    }

    public static readonly TheoryData<string> CusipBases = new()
    {
        "45920010",
        "03783310",
        "68389X10",
        "AB*CD@E#",
    };

    [Theory]
    [MemberData(nameof(CusipBases))]
    public void Cusip_round_trips_through_compute_and_validate(string basePart)
    {
        var checkDigit = SecurityIdentifiers.ComputeCusipCheckDigit(basePart);
        var cusip = basePart + checkDigit;
        Assert.True(SecurityIdentifiers.IsValidCusip(cusip));
    }

    public static readonly TheoryData<string> SedolBases = new()
    {
        "026349",
        "B0YBKL",
        "702752",
    };

    [Theory]
    [MemberData(nameof(SedolBases))]
    public void Sedol_round_trips_through_compute_and_validate(string basePart)
    {
        var checkDigit = SecurityIdentifiers.ComputeSedolCheckDigit(basePart);
        var sedol = basePart + checkDigit;
        Assert.True(SecurityIdentifiers.IsValidSedol(sedol));
    }

    public static readonly TheoryData<string> FigiPayloads = new()
    {
        "BBG000BLNNH",
        "BBG000BXPZJ",
        "ZZG00012345",
    };

    [Theory]
    [MemberData(nameof(FigiPayloads))]
    public void Figi_round_trips_through_compute_and_validate(string payload)
    {
        var checkDigit = SecurityIdentifiers.ComputeFigiCheckDigit(payload);
        var figi = payload + checkDigit;
        Assert.True(SecurityIdentifiers.IsValidFigi(figi));
    }

    [Theory]
    [MemberData(nameof(IsinPayloads))]
    public void Isin_single_digit_substitution_always_invalidates(string payload)
    {
        // The Luhn algorithm is guaranteed to detect any single-decimal-digit substitution, so
        // this exercises every digit position (letter positions are covered by the dedicated
        // corrupted-body oracle tests instead, since swapping a letter changes how many digits
        // it expands to and is not a same-width single-digit substitution).
        var checkDigit = SecurityIdentifiers.ComputeIsinCheckDigit(payload);
        var isin = payload + checkDigit;

        for (var position = 0; position < isin.Length; position++)
        {
            if (isin[position] is < '0' or > '9')
            {
                continue;
            }

            foreach (var replacement in DigitAlphabet)
            {
                if (replacement == isin[position])
                {
                    continue;
                }

                var corrupted = isin.ToCharArray();
                corrupted[position] = replacement;
                Assert.False(SecurityIdentifiers.IsValidIsin(new string(corrupted)));
            }
        }
    }

    private static readonly char[] DigitAlphabet = "0123456789".ToCharArray();
}
