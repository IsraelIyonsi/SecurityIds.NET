using SecurityIds.Internal;

namespace SecurityIds;

/// <summary>
/// Validates, identifies and computes check digits for the standard financial security
/// identifier schemes: ISIN (ISO 6166), CUSIP, SEDOL and FIGI.
/// </summary>
/// <remarks>
/// Every method treats its input literally: values must already be upper case and must not
/// contain surrounding whitespace. No normalization is performed on behalf of the caller.
/// </remarks>
public static class SecurityIdentifiers
{
    /// <summary>
    /// Determines whether <paramref name="value"/> is a structurally well-formed and
    /// check-digit-valid ISIN (ISO 6166).
    /// </summary>
    /// <param name="value">The candidate identifier, expected to be 12 upper case characters.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is a valid ISIN; otherwise <see langword="false"/>.</returns>
    public static bool IsValidIsin(string? value) => value is not null && IsinAlgorithm.IsValid(value);

    /// <summary>
    /// Determines whether <paramref name="value"/> is a structurally well-formed and
    /// check-digit-valid CUSIP.
    /// </summary>
    /// <param name="value">The candidate identifier, expected to be 9 upper case characters.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is a valid CUSIP; otherwise <see langword="false"/>.</returns>
    public static bool IsValidCusip(string? value) => value is not null && CusipAlgorithm.IsValid(value);

    /// <summary>
    /// Determines whether <paramref name="value"/> is a structurally well-formed and
    /// check-digit-valid SEDOL.
    /// </summary>
    /// <param name="value">The candidate identifier, expected to be 7 upper case characters.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is a valid SEDOL; otherwise <see langword="false"/>.</returns>
    public static bool IsValidSedol(string? value) => value is not null && SedolAlgorithm.IsValid(value);

    /// <summary>
    /// Determines whether <paramref name="value"/> is a structurally well-formed and
    /// check-digit-valid FIGI.
    /// </summary>
    /// <param name="value">The candidate identifier, expected to be 12 upper case characters.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is a valid FIGI; otherwise <see langword="false"/>.</returns>
    public static bool IsValidFigi(string? value) => value is not null && FigiAlgorithm.IsValid(value);

    /// <summary>
    /// Detects which security identifier scheme <paramref name="text"/> belongs to, based on
    /// its length, structure and check digit.
    /// </summary>
    /// <param name="text">The candidate identifier text.</param>
    /// <returns>
    /// The detected <see cref="SecurityIdentifierType"/>, or <see cref="SecurityIdentifierType.None"/>
    /// if <paramref name="text"/> does not validate against any known scheme.
    /// </returns>
    /// <remarks>
    /// ISIN and FIGI are both 12 characters long. A FIGI always carries the fixed letter
    /// <c>G</c> as its third character, so a 12-character value is classified as
    /// <see cref="SecurityIdentifierType.Figi"/> when that structural marker is present and its
    /// check digit validates, and as <see cref="SecurityIdentifierType.Isin"/> otherwise.
    /// </remarks>
    public static SecurityIdentifierType TryIdentify(string? text)
    {
        if (text is null)
        {
            return SecurityIdentifierType.None;
        }

        return text.Length switch
        {
            IdentifierConstants.SedolLength when SedolAlgorithm.IsValid(text) => SecurityIdentifierType.Sedol,
            IdentifierConstants.CusipLength when CusipAlgorithm.IsValid(text) => SecurityIdentifierType.Cusip,
            IdentifierConstants.IsinLength => IdentifyTwelveCharacterValue(text),
            _ => SecurityIdentifierType.None
        };
    }

    private static SecurityIdentifierType IdentifyTwelveCharacterValue(string text)
    {
        if (FigiAlgorithm.IsWellFormed(text) && FigiAlgorithm.IsValid(text))
        {
            return SecurityIdentifierType.Figi;
        }

        return IsinAlgorithm.IsValid(text) ? SecurityIdentifierType.Isin : SecurityIdentifierType.None;
    }

    /// <summary>
    /// Computes the ISO 6166 Luhn check digit for an 11-character ISIN payload
    /// (2-letter country code followed by the 9-character national security identifying number).
    /// </summary>
    /// <param name="payload">The 11-character country code and NSIN, without the check digit.</param>
    /// <returns>The computed check digit, in the range 0 to 9.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="payload"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="payload"/> is not 11 characters, does not start with a two-letter
    /// country code, or contains a character other than a digit or an upper case letter.
    /// </exception>
    public static int ComputeIsinCheckDigit(string payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return IsinAlgorithm.ComputeCheckDigit(payload);
    }

    /// <summary>
    /// Computes the modulus 10 double-add-double check digit for an 8-character CUSIP base
    /// (6-character issuer number followed by the 2-character issue number).
    /// </summary>
    /// <param name="basePart">The 8-character CUSIP base, without the check digit.</param>
    /// <returns>The computed check digit, in the range 0 to 9.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="basePart"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="basePart"/> is not 8 characters, or contains a character other than a
    /// digit, an upper case letter, or one of <c>*</c>, <c>@</c>, <c>#</c>.
    /// </exception>
    public static int ComputeCusipCheckDigit(string basePart)
    {
        ArgumentNullException.ThrowIfNull(basePart);
        return CusipAlgorithm.ComputeCheckDigit(basePart);
    }

    /// <summary>
    /// Computes the weighted modulus 10 check digit for a 6-character SEDOL base.
    /// </summary>
    /// <param name="basePart">The 6-character SEDOL base, without the check digit.</param>
    /// <returns>The computed check digit, in the range 0 to 9.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="basePart"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="basePart"/> is not 6 characters, or contains a character other than a
    /// digit or an upper case letter that is not one of the vowels A, E, I, O, U.
    /// </exception>
    public static int ComputeSedolCheckDigit(string basePart)
    {
        ArgumentNullException.ThrowIfNull(basePart);
        return SedolAlgorithm.ComputeCheckDigit(basePart);
    }

    /// <summary>
    /// Computes the FIGI check digit for an 11-character FIGI payload (2-character prefix,
    /// the fixed letter <c>G</c>, and 8 further alphanumeric characters).
    /// </summary>
    /// <param name="payload">The 11-character FIGI payload, without the check digit.</param>
    /// <returns>The computed check digit, in the range 0 to 9.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="payload"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="payload"/> is not 11 characters, does not carry <c>G</c> as its third
    /// character, or contains a character other than a digit or an upper case letter.
    /// </exception>
    public static int ComputeFigiCheckDigit(string payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        return FigiAlgorithm.ComputeCheckDigit(payload);
    }

    /// <summary>
    /// Builds the ISIN for a security given its CUSIP and the two-letter ISO 3166-1 country
    /// code that should prefix it.
    /// </summary>
    /// <param name="cusip">A valid 9-character CUSIP, including its own check digit.</param>
    /// <param name="countryCode">The two-letter upper case ISO 3166-1 alpha-2 country code.</param>
    /// <returns>The resulting 12-character ISIN.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="cusip"/> or <paramref name="countryCode"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="cusip"/> is not a valid CUSIP, or <paramref name="countryCode"/> is not
    /// exactly two upper case letters.
    /// </exception>
    public static string IsinFromCusip(string cusip, string countryCode)
    {
        ArgumentNullException.ThrowIfNull(cusip);
        ArgumentNullException.ThrowIfNull(countryCode);

        if (!CusipAlgorithm.IsValid(cusip))
        {
            throw new ArgumentException("The CUSIP is not valid.", nameof(cusip));
        }

        if (countryCode.Length != IdentifierConstants.IsinCountryCodeLength
            || !AlphanumericCode.IsUpperCaseLetter(countryCode[0])
            || !AlphanumericCode.IsUpperCaseLetter(countryCode[1]))
        {
            throw new ArgumentException(
                "The country code must be exactly two upper case letters.",
                nameof(countryCode));
        }

        var payload = countryCode + cusip;
        var checkDigit = IsinAlgorithm.ComputeCheckDigit(payload);
        return payload + checkDigit;
    }
}
