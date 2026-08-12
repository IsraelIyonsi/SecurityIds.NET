namespace SecurityIds.Internal;

internal static class SedolAlgorithm
{
    private static readonly int[] Weights = [1, 3, 1, 7, 3, 9];

    internal static bool IsValid(string value)
    {
        if (value.Length != IdentifierConstants.SedolLength)
        {
            return false;
        }

        if (!TryGetBaseValues(value[..IdentifierConstants.SedolBaseLength], out var values))
        {
            return false;
        }

        if (!AlphanumericCode.TryGetValue(value[IdentifierConstants.SedolBaseLength], out var checkDigit)
            || checkDigit >= IdentifierConstants.DigitCount)
        {
            return false;
        }

        return CheckDigitFromValues(values!) == checkDigit;
    }

    internal static int ComputeCheckDigit(string basePart)
    {
        if (basePart.Length != IdentifierConstants.SedolBaseLength)
        {
            throw new ArgumentException(
                $"SEDOL base must be exactly {IdentifierConstants.SedolBaseLength} characters.",
                nameof(basePart));
        }

        if (!TryGetBaseValues(basePart, out var values))
        {
            throw new ArgumentException(
                "SEDOL base must contain only the digits 0-9 or the letters A-Z excluding vowels.",
                nameof(basePart));
        }

        return CheckDigitFromValues(values!);
    }

    private static bool TryGetBaseValues(string basePart, out List<int>? values)
    {
        var collected = new List<int>(basePart.Length);
        foreach (var character in basePart)
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
            sum += values[index] * Weights[index];
        }

        return (IdentifierConstants.CheckDigitModulus - sum % IdentifierConstants.CheckDigitModulus)
               % IdentifierConstants.CheckDigitModulus;
    }
}
