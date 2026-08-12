namespace SecurityIds.Internal;

internal static class IsinAlgorithm
{
    internal static bool IsWellFormed(string value)
    {
        return value.Length == IdentifierConstants.IsinLength
               && HasLetterCountryPrefix(value)
               && TryExpandToDigits(value, out _);
    }

    private static bool HasLetterCountryPrefix(string value)
    {
        for (var index = 0; index < IdentifierConstants.IsinCountryCodeLength; index++)
        {
            if (!AlphanumericCode.IsUpperCaseLetter(value[index]))
            {
                return false;
            }
        }

        return true;
    }

    internal static bool IsValid(string value)
    {
        if (!IsWellFormed(value))
        {
            return false;
        }

        TryExpandToDigits(value, out var digits);
        return WeightedSum(digits!, doubleRightmostDigit: false) % IdentifierConstants.CheckDigitModulus == 0;
    }

    internal static int ComputeCheckDigit(string payload)
    {
        if (payload.Length != IdentifierConstants.IsinPayloadLength)
        {
            throw new ArgumentException(
                $"ISIN payload must be exactly {IdentifierConstants.IsinPayloadLength} characters.",
                nameof(payload));
        }

        if (!HasLetterCountryPrefix(payload))
        {
            throw new ArgumentException(
                "ISIN payload must start with a two-letter country code.",
                nameof(payload));
        }

        if (!TryExpandToDigits(payload, out var digits))
        {
            throw new ArgumentException(
                "ISIN payload must contain only the digits 0-9 and the letters A-Z.",
                nameof(payload));
        }

        var sum = WeightedSum(digits!, doubleRightmostDigit: true);
        return (IdentifierConstants.CheckDigitModulus - sum % IdentifierConstants.CheckDigitModulus)
               % IdentifierConstants.CheckDigitModulus;
    }

    private static bool TryExpandToDigits(string value, out List<int>? digits)
    {
        var expanded = new List<int>(value.Length * 2);
        foreach (var character in value)
        {
            if (!AlphanumericCode.TryGetValue(character, out var charValue))
            {
                digits = null;
                return false;
            }

            if (charValue < IdentifierConstants.DigitCount)
            {
                expanded.Add(charValue);
            }
            else
            {
                expanded.Add(charValue / IdentifierConstants.DigitCount);
                expanded.Add(charValue % IdentifierConstants.DigitCount);
            }
        }

        digits = expanded;
        return true;
    }

    private static int WeightedSum(IReadOnlyList<int> digits, bool doubleRightmostDigit)
    {
        var sum = 0;
        var shouldDouble = doubleRightmostDigit;
        for (var index = digits.Count - 1; index >= 0; index--)
        {
            var digitValue = digits[index];
            if (shouldDouble)
            {
                digitValue = AlphanumericCode.SumOfDecimalDigits(digitValue * 2);
            }

            sum += digitValue;
            shouldDouble = !shouldDouble;
        }

        return sum;
    }
}
