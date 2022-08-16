using System.Text.Json.Serialization;

#nullable enable

namespace OrigoApiGateway.Models
{
    /// <summary>
    ///     This is a subset of the <c><see cref="NewOrganization"/></c> entity, where a few of the unrelated properties have been hidden.
    /// </summary>
    public record NewPartnerOrganization : NewOrganization
    {
        // We are overriding (using new keyword) some existing properties so we can hide them from the de-serializer.

        // Partner's cant have a parent.
        [JsonIgnore]
        public new Guid? ParentId { get; private init; } = null;

        // We don't want to re-use existing locations.
        [JsonIgnore]
        public new Guid? PrimaryLocation { get; init; } = null;

        // This is deprecated and not actually used. Will be removed when it's removed from 'NewOrganization'
        [JsonIgnore]
        [Obsolete("This is not used..")]
        [EmailAddress]
        public new string? ContactEmail { get; private set; } = null;

        // Partner's can't have another partner.
        [JsonIgnore]
        public new Guid? PartnerId { get; private set; } = null;
    }
}
