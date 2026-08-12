# SecurityIds.NET

Validate and identify financial security identifiers in .NET: ISIN, CUSIP, SEDOL and FIGI, with check-digit verification. Zero external dependencies.

Every security traded anywhere in the world carries one or more of these codes, and every one of them is protected by a check digit meant to catch typos and transcription errors before they turn into a trade on the wrong instrument. Getting the check-digit algorithm wrong is easy: ISIN, CUSIP, SEDOL and FIGI each use a different scheme, with different character-to-value mappings and different weighting rules. There is no maintained, dependency-free .NET package that implements all four correctly and ties them together with a single detection entry point. SecurityIds.NET is that package: strict, verified against published worked examples, and dependency-free.

Where you need it:

- Validating instrument identifiers on data entry, before they reach a booking or settlement system
- Detecting which identifier scheme a free-text reference string belongs to
- Deriving a US ISIN from a CUSIP for reference data pipelines that only carry one of the two
- Post-trade and reference-data reconciliation, where a single transposed character otherwise fails silently

## Install

```
dotnet add package SecurityIds.Net
```

## Quickstart

```csharp
using SecurityIds;

SecurityIdentifiers.IsValidIsin("US0378331005");   // true, Apple Inc
SecurityIdentifiers.IsValidCusip("037833100");     // true, Apple Inc
SecurityIdentifiers.IsValidSedol("0263494");       // true
SecurityIdentifiers.IsValidFigi("BBG000BLNNH6");   // true

SecurityIdentifiers.IsValidIsin("US0378331006");   // false, corrupted check digit
```

## Detecting an unknown identifier

```csharp
using SecurityIds;

SecurityIdentifierType type = SecurityIdentifiers.TryIdentify("US0378331005");
// SecurityIdentifierType.Isin

SecurityIdentifierType none = SecurityIdentifiers.TryIdentify("not-an-identifier");
// SecurityIdentifierType.None
```

`TryIdentify` distinguishes a 12-character ISIN from a 12-character FIGI using the FIGI's fixed third character (`G`) together with each scheme's own check digit, so a single call is enough for free-text reference data that mixes identifier types.

## Deriving an ISIN from a CUSIP

```csharp
using SecurityIds;

string isin = SecurityIdentifiers.IsinFromCusip("037833100", "US");
// US0378331005
```

`IsinFromCusip` validates the CUSIP's own check digit first, then prefixes the two-letter country code and computes the ISIN check digit over the combined 11 characters.

## Computing a check digit directly

```csharp
using SecurityIds;

int isinCheckDigit  = SecurityIdentifiers.ComputeIsinCheckDigit("US037833100");   // 5
int cusipCheckDigit = SecurityIdentifiers.ComputeCusipCheckDigit("45920010");     // 1
int sedolCheckDigit = SecurityIdentifiers.ComputeSedolCheckDigit("026349");       // 4
int figiCheckDigit  = SecurityIdentifiers.ComputeFigiCheckDigit("BBG000BLNNH");   // 6
```

## The four schemes

| Scheme | Length | Structure | Check digit |
|---|---|---|---|
| ISIN (ISO 6166) | 12 | 2-letter country code + 9-character NSIN + 1 numeric check digit | Letters expanded to two digits each (`A`=10 ... `Z`=35), then the standard Luhn algorithm |
| CUSIP | 9 | 6-character issuer number + 2-character issue number + 1 check digit | Modulus 10 double-add-double; letters `A`-`Z` value 10-35, plus `*`, `@`, `#` as 36-38 |
| SEDOL | 7 | 6-character alphanumeric base (vowels never appear) + 1 check digit | Weighted modulus 10 with weights `1, 3, 1, 7, 3, 9` |
| FIGI | 12 | 2-character prefix (excluding the reserved combinations `BS`, `BM`, `GG`, `GB`, `GH`, `KY`, `VG`) + fixed letter `G` + 8-character consonant/digit body (vowels never appear) + 1 check digit | Every second character (by position) doubled, digit-summed, modulus 10 |

The ISIN check digit position (the 12th character) must itself be a digit `0`-`9`; a letter in that position is rejected even if it happens to expand to a Luhn-sum-preserving digit pair. The FIGI character set excludes the vowels `A`, `E`, `I`, `O`, `U` from every position after the fixed `G`, matching the OMG FIGI specification, which also reserves certain two-letter prefixes to avoid colliding with an ISIN country code.

All four `IsValid*` methods are case-sensitive and require the exact character set for their scheme; they return `false` rather than throwing for `null`, wrong length, or an invalid character. The `Compute*CheckDigit` methods throw `ArgumentNullException` or `ArgumentException` on malformed input, since a caller asking for a check digit is expected to already have a well-formed base value.

## Verified against published examples

Each scheme's implementation is checked, by hand and in the test suite, against a worked example with a known, independently verifiable check digit:

- **ISIN**: `US0378331005` (Apple Inc, digits-only NSIN) and `US38259P7069` (Alphabet Inc Class C, NSIN contains a letter)
- **CUSIP**: `459200101` (IBM) and `037833100` (Apple Inc), plus `68389X105` (Oracle Corp, issue number contains a letter)
- **SEDOL**: `0263494` (digits-only base) and `B0YBKL9` (base contains letters)
- **FIGI**: `BBG000BLNNH6` and `BBG000B9XRY4` (Apple Inc)

The FIGI check-digit algorithm is documented less consistently across secondary sources than the other three. This implementation follows the description published by the Object Management Group (per-character doubling by position, followed by a decimal digit sum, modulus 10), and its output matches both worked FIGI examples above digit for digit, from two independent issuers. Corroborate independently against the OMG FIGI specification before relying on it for anything where a mismatch would be costly, such as auto-generating identifiers rather than merely validating ones you already have.

## Dependencies and AOT

Zero runtime dependencies. The library is pure managed code operating on `string` and `char`, with no reflection, no unmanaged calls and no third-party packages, so it is fully compatible with Native AOT and trimming.

## License

MIT. See [LICENSE](LICENSE).
