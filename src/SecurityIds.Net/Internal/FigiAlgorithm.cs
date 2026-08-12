namespace SecurityIds.Internal;

internal static class FigiAlgorithm
{
    private const int DoublingRemainder = 1;

    private static readonly HashSet<string> ForbiddenPrefixes = new(StringComparer.Ordinal)
    {
        "BS", "BM", "GG", "GB", "GH", "KY", "VG"
    };

    internal static bool IsWellFormed(string value)
    {
        return value.Length == IdentifierConstants.FigiLength
               && HasFixedThirdCharacter(value)
               && HasAllowedPrefix(value)
               && TryGetPayloadValues(value[..IdentifierConstants.FigiPayloadLength], out _);
    }

    internal static bool IsValid(string value)
    {
        if (!IsWellFormed(value))
        {
            return false;
        }

        if (!AlphanumericCode.TryGetValue(value[IdentifierConstants.FigiPayloadLength], out var checkDigit)
            || checkDigit >= IdentifierConstants.DigitCount)
        {
            return false;
        }

        TryGetPayloadValues(value[..IdentifierConstants.FigiPayloadLength], out var values);
        return CheckDigitFromValues(values!) == checkDigit;
    }

    internal static int ComputeCheckDigit(string payload)
    {
        if (payload.Length != IdentifierConstants.FigiPayloadLength)
        {
            throw new ArgumentException(
                $"FIGI payload must be exactly {IdentifierConstants.FigiPayloadLength} characters.",
                nameof(payload));
        }

        if (!HasFixedThirdCharacter(payload))
        {
            throw new ArgumentException(
                $"FIGI payload must have '{IdentifierConstants.FigiFixedThirdCharacter}' as its third character.",
                nameof(payload));
        }

        if (!HasAllowedPrefix(payload))
        {
            throw new ArgumentException(
                "FIGI payload must not start with one of the reserved two-letter prefixes " +
                "(BS, BM, GG, GB, GH, KY, VG) that would collide with an ISIN country code.",
                nameof(payload));
        }

        if (!TryGetPayloadValues(payload, out var values))
        {
            throw new ArgumentException(
                "FIGI payload must contain only the digits 0-9 and the upper case consonants (vowels A, E, I, O, U are not permitted).",
                nameof(payload));
        }

        return CheckDigitFromValues(values!);
    }

    private static bool HasFixedThirdCharacter(string value)
    {
        return value[IdentifierConstants.FigiPrefixLength] == IdentifierConstants.FigiFixedThirdCharacter;
    }

    private static bool HasAllowedPrefix(string value)
    {
        return !ForbiddenPrefixes.Contains(value[..IdentifierConstants.FigiPrefixLength]);
    }

    private static bool TryGetPayloadValues(string payload, out List<int>? values)
    {
        var collected = new List<int>(payload.Length);
        foreach (var character in payload)
        {
            if (IsVowel(character) || !AlphanumericCode.TryGetValue(character, out var value))
            {
                values = null;
                return false;
            }

            collected.Add(value);
        }

        values = collected;
        return true;
    }

    private static bool IsVowel(char character) => character is 'A' or 'E' or 'I' or 'O' or 'U';

    private static int CheckDigitFromValues(IReadOnlyList<int> values)
    {
        var sum = 0;
        for (var index = 0; index < values.Count; index++)
        {
            var value = values[index];
            if (index % 2 == DoublingRemainder)
            {
                value *= 2;
            }

            sum += AlphanumericCode.SumOfDecimalDigits(value);
        }

        return (IdentifierConstants.CheckDigitModulus - sum % IdentifierConstants.CheckDigitModulus)
               % IdentifierConstants.CheckDigitModulus;
    }
}
