namespace OrigoApiGateway.Models.SCIM;

#nullable enable


/// <summary>
/// The different parts of a name, honorifics, and full name formatting for a ScimUser.
/// </summary>
public record ScimName
{
    /// <summary>
    /// Fully formatte name, including honoriffics. E.g."Ms. Barbara Joan Jensen III"
    /// </summary>
    public string? Formatted { get; set; }

    /// <summary>
    /// Last name, e.g. "Jensen"
    /// </summary>
    public string FamilyName { get; set; }

    /// <summary>
    /// First name, e.g. "Barbara"
    /// </summary>
    public string GivenName { get; set; }

    /// <summary>
    /// Middle name, e.g. "Joan"
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Honorary prefix, e.g. "Mr.", "Mrs.", "Dr.", et.al.
    /// </summary>
    public string? HonorificPrefix { get; set; }

    /// <summary>
    /// Honorary suffix, e.g. "Junior", "The third", "IV"
    /// </summary>
    public string? HonorificSuffix { get; set; }
}
