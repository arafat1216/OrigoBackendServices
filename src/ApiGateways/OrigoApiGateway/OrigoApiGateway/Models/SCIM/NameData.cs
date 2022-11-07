namespace OrigoApiGateway.Models.SCIM;

/// <summary>
///     The components of the user's name.  Service providers MAY return
///     just the full name as a single string in the formatted
///     sub-attribute, or they MAY return just the individual component
///     attributes using the other sub-attributes, or they MAY return
///     both.If both variants are returned, they SHOULD be describing
///     the same name, with the formatted name indicating how the
///     component attributes should be combined.
/// </summary>
public class NameData
{
    /// <summary>
    ///     The full name, including all middle names, titles, and
    ///     suffixes as appropriate, formatted for display(e.g.,
    ///     "Ms. Barbara Jane Jensen, III").
    /// </summary>
    public string Formatted { get => $"{GivenName} {MiddleName} {FamilyName}"; }

    /// <summary>
    ///     The family name of the User, or last name in most
    ///     Western languages(e.g., "Jensen" given the full name
    ///     "Ms. Barbara Jane Jensen, III").
    /// </summary>
    public string FamilyName { get; set; } = string.Empty;

    /// <summary>
    ///     The given name of the User, or first name in most
    ///     Western languages(e.g., "Barbara" given the full name
    ///     "Ms. Barbara Jane Jensen, III").
    /// </summary>
    public string GivenName { get; set; } = string.Empty;

    /// <summary>
    ///     The middle name(s) of the User(e.g., "Jane" given the
    ///     full name "Ms. Barbara Jane Jensen, III").
    /// </summary>
    public string MiddleName { get; set; } = string.Empty;
}