using System.Text.Json.Serialization;

namespace OrigoApiGateway.Models.SCIM;


/// <summary>
///     A list of groups to which the user belongs, either through direct
///     membership, through nested groups, or dynamically calculated.The
///     values are meant to enable expression of common group-based or
///     role-based access control models, although no explicit
///     authorization model is defined.It is intended that the semantics
///     of group membership and any behavior or authorization granted as a
///     result of membership are defined by the service provider.  The
///     canonical types "direct" and "indirect" are defined to describe
///     how the group membership was derived.  Direct group membership
///     indicates that the user is directly associated with the group and
///     SHOULD indicate that clients may modify membership through the
///     "Group" resource.Indirect membership indicates that user
///     membership is transitive or dynamic and implies that clients
///     cannot modify indirect group membership through the "Group"
///     resource but MAY modify direct group membership through the
///     "Group" resource, which may influence indirect memberships.If
///     the SCIM service provider exposes a "Group" resource, the "value"
///     sub-attribute MUST be the "id", and the "$ref" sub-attribute must
///     be the URI of the corresponding "Group" resources to which the
///     user belongs.  Since this attribute has a mutability of
///     "readOnly", group membership changes MUST be applied via the
///     "Group" Resource (Section 4.2).  This attribute has a mutability
///     of "readOnly"
/// </summary>
public class Group
{
    /// <summary>
    ///     The group Id, e.g. "e9e30dba-f08f-4109-8486-d5c6a331660a"
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// The URI of the corresponding "Group" resources to which the user belongs.
    /// E.g. "https://example.com/v2/Groups/e9e30dba-f08f-4109-8486-d5c6a331660a"
    /// </summary>
    [JsonPropertyName("$ref")]
    public string Ref { get; set; }

    /// <summary>
    ///     The display name of the group, e.g. "Tour Guides".
    ///     This may also correspond with a customer Id in the name of the Okta group.
    /// </summary>
    public string Display { get; set; }
}
