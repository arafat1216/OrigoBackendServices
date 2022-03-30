using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public record NewOrganization
    {
        public string Name { get; init; }

        public string OrganizationNumber { get; init; }

        public Address Address { get; init; }

        public OrigoContactPerson ContactPerson { get; init; }

        /// <summary>
        ///     If this is a child/sub-organization, then this is the ID of it's parent. If this is a top-level organization, then the value 
        ///     should be <c><see langword="null"/></c>, or omitted from the request.
        /// </summary>
        public Guid? ParentId { get; init; }

        /// <summary>
        ///     <c>Note:</c> This is mutually exclusive with <c><see cref="Location"/></c>. As such, only one of them should be provided. <para>
        /// 
        ///     The ID of an existing location that should be used. </para>
        /// </summary>
        public Guid? PrimaryLocation { get; init; }

        /// <summary>
        ///     <c>Note:</c> This is mutually exclusive with <c><see cref="PrimaryLocation"/></c>. As such, only one of them should be provided. <para>
        /// 
        ///     The new location object that is created and used with the organization. </para>
        /// </summary>
        public NewLocation? Location { get; init; }
    
        [EmailAddress]
        public string ContactEmail { get; init; }

        public string InternalNotes { get; init; }
                
        public NewOrganizationPreferences Preferences { get; init; }
    }
}
