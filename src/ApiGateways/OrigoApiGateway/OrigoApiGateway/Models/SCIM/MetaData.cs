namespace OrigoApiGateway.Models.SCIM;

/// <summary>
///     From https://www.rfc-editor.org/rfc/rfc7643
///     A complex attribute containing resource metadata.  All "meta"
///     sub-attributes are assigned by the service provider (have a
///     "mutability" of "readOnly"), and all of these sub-attributes have
///     a "returned" characteristic of "default".  This attribute SHALL be
///     ignored when provided by clients
/// </summary>
public class MetaData
{
    /// <summary>
    ///     The name of the resource type of the resource.  This
    ///     attribute has a mutability of "readOnly" and "caseExact" as
    ///     "true".
    /// </summary>
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    ///     The "DateTime" that the resource was added to the service
    ///     provider.This attribute MUST be a DateTime.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    ///     The most recent DateTime that the details of this
    ///     resource were updated at the service provider.If this
    ///     resource has never been modified since its initial creation,
    ///     the value MUST be the same as the value of "created".
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    ///     The URI of the resource being returned.  This value MUST
    ///     be the same as the "Content-Location" HTTP response header(see
    ///     Section 3.1.4.2 of[RFC7231]).
    /// </summary>
    public Uri Location { get; set; }

    /// <summary>
    ///     The version of the resource being returned.  This value
    ///     must be the same as the entity-tag (ETag) HTTP response header
    ///     (see Sections 2.1 and 2.3 of [RFC7232]).  This attribute has
    ///     "caseExact" as "true".  Service provider support for this
    ///     attribute is optional and subject to the service provider's
    ///     support for versioning (see Section 3.14 of [RFC7644]).  If a
    ///     service provider provides "version" (entity-tag) for a
    ///     representation and the generation of that entity-tag does not
    ///     satisfy all of the characteristics of a strong validator (see
    ///     Section 2.1 of [RFC7232]), then the origin server MUST mark the
    ///     "version" (entity-tag) as weak by prefixing its opaque value
    ///     with "W/" (case sensitive).
    /// </summary>
    public string Version { get; set; } = string.Empty;
}