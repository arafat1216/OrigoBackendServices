using System.Text;
using System.Text.Json.Serialization;


#nullable enable
namespace OrigoApiGateway.Models.SCIM;

/// <summary>
/// High level model of a SCIM User.
/// urn:ietf:params:scim:schemas:extension:enterprise:2.0:User
/// </summary>
public record ScimUser
{
    public List<string> Schemas { get; private set; } = new() { "urn:ietf:params:scim:schemas:core:2.0:User" };

    /// <summary>
    /// The username of the user, the unique identifier within the system across the entire set of users. 
    /// This is not recommended to be the same as the user's email due to potential email changes, but within Techstep Lifecycle the username == email.
    /// </summary>
    public string UserName { get; set; } = "";
    
    /// <summary>
    /// The identifier of the user in the external system, e.g. the Okta user Id
    /// </summary>
    public string ExternalId { get; set; }

    /// <inheritdoc />
    public ScimName Name { get; set; }

    /// <summary>
    ///  Display name of user, usually name.GivenName + name.FamilyName
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Potential nick name of user, e.g. name.GivenName "Robert" -> Nick name "Bobby
    /// Currently not relevant for Techstep Lifecycle.
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// The URL that hat points to a location
    /// representing the user's online profile (e.g., a web page)
    /// </summary>
    public string? ProfileUrl { get; set; }

    /// <summary>
    /// E.g. "Junior Developer", "Vice President"
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Identifies the relationship betweeen the organization and the user.
    /// E.g. "Employee", "Contractor", "Unknown"
    /// </summary>
    public string? UserType { get; set; }

    /// <summary>
    /// The format of the value is the same as the HTTP Accept-Language header field
    /// </summary>
    public string? PreferredLanguage { get; set; }

    /// <summary>
    /// E.g. "en-US", "nb-NO", "fr"
    /// </summary>
    public string? Locale { get; set; }

    /// <summary>
    /// IANA Time Zone database format, e.g. "America/Los_Angeles"
    /// </summary>
    public string? TimeZone { get; set; }

    /// <summary>
    /// The user's administrative status
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// A list of a ScimUser's emails.
    /// Only a single one of these will be used, but the protocol requires this be defined as a list.
    /// </summary>
    public List<string> Emails { get; set; }

    /// <summary>
    /// A ScimUser's phone numbers.
    /// </summary>
    public List<string>? PhoneNumbers { get; set; }

    /// <summary>
    /// A ScimUser's assigned (Okta) Groups.
    /// This will e.g. contain a Group with an organizationId as its name.
    /// </summary>
    public List<string> Groups { get; set; }
}
