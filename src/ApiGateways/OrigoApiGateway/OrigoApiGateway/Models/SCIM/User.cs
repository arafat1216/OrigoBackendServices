namespace OrigoApiGateway.Models.SCIM;


/// <summary>
///     SCIM provides a resource type for "User" resources.  The core schema
///     for "User" is identified using the following schema URI:
///     "urn:ietf:params:scim:schemas:core:2.0:User".
///     Details can be found here https://www.rfc-editor.org/rfc/rfc7643
/// </summary>
public class User : Resource
{
    public User()
    {
        Schemas.Add("urn:ietf:params:scim:schemas:core:2.0:User");
        Schemas.Add("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User");
        Meta.ResourceType = "User";
        Name = new NameData();
    }

    /// <summary>
    ///     A service provider's unique identifier for the user, typically
    ///     used by the user to directly authenticate to the service provider.
    ///     Often displayed to the user as their unique identifier within the
    ///     system(as opposed to "id" or "externalId", which are generally
    ///     opaque and not user-friendly identifiers).  Each User MUST include
    ///     a non-empty userName value.This identifier MUST be unique across
    ///     the service provider's entire set of Users.  This attribute is
    ///     REQUIRED and is case insensitive.
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    ///     <inheritdoc cref="NameData" />
    /// </summary>
    public NameData Name { get; set; }

    /// <summary>
    ///     The name of the user, suitable for display to end-users.  Each
    ///     user returned MAY include a non-empty displayName value.The name
    ///     SHOULD be the full name of the User being described, if known
    ///     (e.g., "Babs Jensen" or "Ms. Barbara J Jensen, III") but MAY be a
    ///     username or handle, if that is all that is available(e.g.,
    ///     "bjensen").  The value provided SHOULD be the primary textual
    ///     label by which this User is normally displayed by the service
    ///     provider when presenting it to end-users.
    /// </summary>
    public string DisplayName => Name.Formatted;

    /// <summary>
    ///     A Boolean value indicating the user's administrative status.  The
    ///     definitive meaning of this attribute is determined by the service
    ///     provider.As a typical example, a value of true implies that the
    ///     user is able to log in, while a value of false implies that the
    ///     user's account has been suspended.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    ///     <inheritdoc cref="Email" />
    /// </summary>
    public List<Email> Emails { get; set; } = new();

    /// <summary>
    ///     <inheritdoc cref="PhoneNumber" />
    /// </summary>
    public List<PhoneNumber> PhoneNumbers { get; set; } = new();    

    /// <summary>
    ///     <inheritdoc cref="Group" />
    /// </summary>
    public List<Group> Groups { get; set; } = new();
}