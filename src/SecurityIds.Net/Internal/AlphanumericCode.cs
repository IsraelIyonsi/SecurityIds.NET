namespace SecurityIds.Internal;

internal static class AlphanumericCode
{
    private const int AsteriskValue = 36;
    private const int AtSignValue = 37;
    private const int HashValue = 38;

    internal static bool TryGetValue(char character, out int value)
    {
        if (character is >= '0' and <= '9')
        {
            value = character - '0';
            return true;
        }

        if (character is >= 'A' and <= 'Z')
        {
            value = character - 'A' + IdentifierConstants.LetterBaseValue;
            return true;
        }

        value = default;
        return false;
    }

    internal static bool TryGetCusipValue(char character, out int value)
    {
        if (TryGetValue(character, out value))
        {
            return true;
        }

        switch (character)
        {
            case '*':
                value = AsteriskValue;
                return true;
            case '@':
                value = AtSignValue;
                return true;
            case '#':
                value = HashValue;
                return true;
            default:
                value = default;
                return false;
        }
    }

    internal static bool IsUpperCaseLetter(char character) => character is >= 'A' and <= 'Z';

    internal static int SumOfDecimalDigits(int value)
    {
        var sum = 0;
        var remaining = value;
        while (remaining > 0)
        {
            sum += remaining % IdentifierConstants.DigitCount;
            remaining /= IdentifierConstants.DigitCount;
        }

        return sum;
    }
}
