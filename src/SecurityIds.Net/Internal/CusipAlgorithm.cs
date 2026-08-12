namespace SecurityIds.Internal;

internal static class CusipAlgorithm
{
    private const int FirstDoubledPosition = 2;

    internal static bool IsValid(string value)
    {
        if (value.Length != IdentifierConstants.CusipLength)
        {
            return false;
        }

        if (!TryGetBaseValues(value[..IdentifierConstants.CusipBaseLength], out var values))
        {
            return false;
        }

        if (!AlphanumericCode.TryGetValue(value[IdentifierConstants.CusipBaseLength], out var checkDigit)
            || checkDigit >= IdentifierConstants.DigitCount)
        {
            return false;
        }

        return CheckDigitFromValues(values!) == checkDigit;
    }

    internal static int ComputeCheckDigit(string basePart)
    {
        if (basePart.Length != IdentifierConstants.CusipBaseLength)
        {
            throw new ArgumentException(
                $"CUSIP base must be exactly {IdentifierConstants.CusipBaseLength} characters.",
                nameof(basePart));
        }

        if (!TryGetBaseValues(basePart, out var values))
        {
            throw new ArgumentException(
                "CUSIP base must contain only the digits 0-9, the letters A-Z, or one of '*', '@', '#'.",
                nameof(basePart));
        }

        return CheckDigitFromValues(values!);
    }

    private static bool TryGetBaseValues(string basePart, out List<int>? values)
    {
        var collected = new List<int>(basePart.Length);
        foreach (var character in basePart)
        {
            if (!AlphanumericCode.TryGetCusipValue(character, out var value))
            {
                values = null;
                return false;
            }

            collected.Add(value);
        }

        values = collected;
        return true;
    }

    private static int CheckDigitFromValues(IReadOnlyList<int> values)
    {
        var sum = 0;
        for (var index = 0; index < values.Count; index++)
        {
            var position = index + 1;
            var value = values[index];
            if (position % FirstDoubledPosition == 0)
            {
                value = AlphanumericCode.SumOfDecimalDigits(value * FirstDoubledPosition);
            }

            sum += value;
        }

        return (IdentifierConstants.CheckDigitModulus - sum % IdentifierConstants.CheckDigitModulus)
               % IdentifierConstants.CheckDigitModulus;
    }
}
