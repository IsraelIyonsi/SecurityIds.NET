# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-08-12

### Added

- `SecurityIdentifiers` static API: `IsValidIsin`, `IsValidCusip`, `IsValidSedol`, `IsValidFigi`, `TryIdentify`, `ComputeIsinCheckDigit`, `ComputeCusipCheckDigit`, `ComputeSedolCheckDigit`, `ComputeFigiCheckDigit`, and `IsinFromCusip`.
- ISIN (ISO 6166) validation and check-digit computation: letters expanded to two digits each (`A`=10 through `Z`=35), standard Luhn algorithm over the resulting digit string.
- CUSIP validation and check-digit computation: modulus 10 double-add-double, with `*`, `@`, `#` supported as extended issue-number characters.
- SEDOL validation and check-digit computation: weighted modulus 10 with weights `1, 3, 1, 7, 3, 9`, vowels excluded from the valid character set.
- FIGI validation and check-digit computation: per-character doubling by position followed by a decimal digit sum, modulus 10, with the fixed third character `G` enforced.
- `TryIdentify` disambiguates 12-character ISIN and FIGI values using the FIGI's fixed third character together with each scheme's check digit.
- `IsinFromCusip` builds a full ISIN from a CUSIP and a two-letter country code, validating the source CUSIP's own check digit first.
- Verified against published worked examples for all four schemes: Apple Inc and Alphabet Inc Class C for ISIN, IBM, Apple Inc and Oracle Corp for CUSIP, a digit-only and a letter-containing example for SEDOL, and a documented FIGI worked example.
- Zero runtime dependencies; Native AOT and trimming compatible.
- SourceLink (GitHub), deterministic CI builds and `.snupkg` symbol packages.
