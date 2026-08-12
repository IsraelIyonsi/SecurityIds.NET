namespace SecurityIds;

/// <summary>
/// Identifies the financial security identifier scheme that a value belongs to.
/// </summary>
public enum SecurityIdentifierType
{
    /// <summary>
    /// The value did not match any recognized security identifier scheme.
    /// </summary>
    None = 0,

    /// <summary>
    /// International Securities Identification Number, ISO 6166.
    /// </summary>
    Isin,

    /// <summary>
    /// Committee on Uniform Security Identification Procedures identifier.
    /// </summary>
    Cusip,

    /// <summary>
    /// Stock Exchange Daily Official List identifier used in the United Kingdom and Ireland.
    /// </summary>
    Sedol,

    /// <summary>
    /// Financial Instrument Global Identifier.
    /// </summary>
    Figi
}
