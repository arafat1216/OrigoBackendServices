namespace OrigoApiGateway.Models.SCIM;

/// <summary>
///     Each resource (or JSON object) in SCIM has a resource type
///     ("meta.resourceType"; see Section 3.1) that defines the resource's
///     core attribute schema and any attribute extension schema, as well
///     as the endpoint where objects of the same type may be found.
/// </summary>
public class Resource
{
    /// <summary>
    ///     The "schemas" attribute is a REQUIRED attribute and is an array of
    ///     Strings containing URIs that are used to indicate the namespaces
    ///     of the SCIM schemas that define the attributes present in the
    ///     current JSON structure.  This attribute may be used by parsers to
    ///     define the attributes present in the JSON structure that is the
    ///     body to an HTTP request or response.  Each String value must be a
    ///     unique URI.  All representations of SCIM schemas MUST include a
    ///     non-empty array with value(s) of the URIs supported by that
    ///     representation.  The "schemas" attribute for a resource MUST only
    ///     contain values defined as "schema" and "schemaExtensions" for the
    ///     resource's defined "resourceType".  Duplicate values MUST NOT be
    ///     included.  Value order is not specified and MUST NOT impact
    ///     behavior.
    /// </summary>
    public List<string> Schemas { get; set; } = new();

    /// <summary>
    ///     A unique identifier for a SCIM resource as defined by the service
    ///     provider.  Each representation of the resource MUST include a
    ///     non-empty "id" value.  This identifier MUST be unique across the
    ///     SCIM service provider's entire set of resources.  It MUST be a
    ///     stable, non-reassignable identifier that does not change when the
    ///     same resource is returned in subsequent requests.  The value of
    ///     the "id" attribute is always issued by the service provider and
    ///     MUST NOT be specified by the client.  The string "bulkId" is a
    ///     reserved keyword and MUST NOT be used within any unique identifier
    ///     value.  The attribute characteristics are "caseExact" as "true", a
    ///     mutability of "readOnly", and a "returned" characteristic of
    ///     "always".  See Section 9 for additional considerations regarding
    ///     privacy.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     A String that is an identifier for the resource as defined by the
    ///     provisioning client.  The "externalId" may simplify identification
    ///     of a resource between the provisioning client and the service
    ///     provider by allowing the client to use a filter to locate the
    ///     resource with an identifier from the provisioning domain,
    ///     obviating the need to store a local mapping between the
    ///     provisioning domain's identifier of the resource and the
    ///     identifier used by the service provider.  Each resource MAY
    ///     include a non-empty "externalId" value.  The value of the
    ///     "externalId" attribute is always issued by the provisioning client
    ///     and MUST NOT be specified by the service provider.  The service
    ///     provider MUST always interpret the externalId as scoped to the
    ///     provisioning domain.  While the server does not enforce
    ///     uniqueness, it is assumed that the value's uniqueness is
    ///     controlled by the client setting the value.
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    ///     <inheritdoc cref="MetaData" />
    /// </summary>
    public MetaData Meta { get; set; } = new();
}
