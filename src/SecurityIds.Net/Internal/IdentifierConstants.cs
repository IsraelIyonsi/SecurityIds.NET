namespace SecurityIds.Internal;

internal static class IdentifierConstants
{
    internal const int DigitCount = 10;
    internal const int LetterBaseValue = 10;
    internal const int LetterCount = 26;
    internal const int CheckDigitModulus = 10;

    internal const int IsinLength = 12;
    internal const int IsinPayloadLength = IsinLength - 1;
    internal const int IsinCountryCodeLength = 2;

    internal const int CusipLength = 9;
    internal const int CusipBaseLength = CusipLength - 1;

    internal const int SedolLength = 7;
    internal const int SedolBaseLength = SedolLength - 1;

    internal const int FigiLength = 12;
    internal const int FigiPayloadLength = FigiLength - 1;
    internal const char FigiFixedThirdCharacter = 'G';
    internal const int FigiPrefixLength = 2;
}
