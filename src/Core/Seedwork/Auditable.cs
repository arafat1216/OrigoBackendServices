using System.Text.Json.Serialization;

namespace Common.Seedwork
{
    /// <summary>
    ///     A generic abstract implementation containing all properties that is used for tracking 
    ///     and setting creation/update/deletion details, such as the corresponding timestamps and 
    ///     the caller that performed the action.
    /// </summary>
    /// <remarks>
    ///     This is a generic model that Entity Framework relies on to auto-assign various settings,
    ///     and should not be tied to any specific design, e.g. event/domain-driven design.
    /// </remarks>
    public abstract class Auditable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Auditable"/> class that is intended for use by Entity Framework, 
        ///     JSON (de-)serializers, and/or testing-frameworks. This should NOT be used in production-code.
        /// </summary>
        [Obsolete("This is a reserved constructor that should only be used by Entity Framework, JSON (de-)serializers, and/or testing-frameworks.")]
        protected Auditable()
        {   
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Auditable"/> class.
        /// </summary>
        /// <param name="callerId">The caller's identifier.</param>
        protected Auditable(Guid callerId)
        {
            CreatedBy = callerId;
            DateCreated = DateTimeOffset.UtcNow;
            IsDeleted = false;
        }


        /// <summary>
        ///     A timestamp for when the database-entity was first created.
        /// </summary>
        [JsonInclude]
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        ///     A timestamp for when the database-entity was last updated.
        /// </summary>
        [JsonInclude]
        public DateTimeOffset? DateUpdated { get; set; }

        /// <summary>
        ///     A timestamp for when the database-entity was deleted.
        /// </summary>
        [JsonInclude]
        public DateTimeOffset? DateDeleted { get; set; }

        /// <summary>
        ///     The external ID of the user that originally created this entity.
        /// 
        ///     <list type="bullet">
        ///         <item>For automated actions performed by the system, the GUID should be "<c>00000000-0000-0000-0000-000000000001</c>"
        ///         (<see cref="Extensions.GuidExtension.SystemUserId(Guid)">Guid.SystemUserId</see> extension). </item>
        ///     
        ///         <item>If the user is unknown, the GUID should be "<c>00000000-0000-0000-0000-000000000000</c>" (<see cref="Guid.Empty"/>). </item>
        ///     </list>
        /// </summary>
        [JsonInclude]
        public Guid CreatedBy { get; set; }

        /// <summary>
        ///     The external ID of the user that performed the last update on this entity.
        /// 
        ///     <list type="bullet">
        ///         <item>For automated actions performed by the system, the GUID should be "<c>00000000-0000-0000-0000-000000000001</c>"
        ///         (<see cref="Extensions.GuidExtension.SystemUserId(Guid)">Guid.SystemUserId</see> extension). </item>
        ///     
        ///         <item>If the user is unknown, the GUID should be "<c>00000000-0000-0000-0000-000000000000</c>" (<see cref="Guid.Empty"/>). </item>
        ///     </list>
        /// </summary>
        [JsonInclude]
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        ///     The external ID of the user that deleted this entity.
        /// 
        ///     <list type="bullet">
        ///         <item>For automated actions performed by the system, the GUID should be "<c>00000000-0000-0000-0000-000000000001</c>"
        ///         (<see cref="Extensions.GuidExtension.SystemUserId(Guid)">Guid.SystemUserId</see> extension). </item>
        ///     
        ///         <item>If the user is unknown, the GUID should be "<c>00000000-0000-0000-0000-000000000000</c>" (<see cref="Guid.Empty"/>). </item>
        ///         
        ///         <item>If the entity is not deleted, this will be <see langword="null"/>. </item>
        ///     </list>
        /// </summary>
        [JsonInclude]
        public Guid? DeletedBy { get; set; }

        /// <summary>
        ///     When <see langword="true"/> the item has been soft deleted. Externally, all soft deleted entries should generally be excluded from results and
        ///     treated as non-existent.
        /// </summary>
        [JsonInclude]
        public bool IsDeleted { get; set; }


        /// <summary>
        ///     Marks the entity as updated. 
        ///     This will update the values in <see cref="UpdatedBy">UpdatedBy</see> and <see cref="DateUpdated">DateUpdated</see>.
        /// </summary>
        /// <param name="callerId"> The caller's identifier. </param>
        public void SetUpdated(Guid callerId)
        {
            this.UpdatedBy = callerId;
            this.DateUpdated = DateTimeOffset.UtcNow;
        }


        /// <summary>
        ///     Marks the entity as (soft) deleted. This will set <see cref="IsDeleted">IsDeleted</see> to <see langword="true"/>,
        ///     and update the values in <see cref="DeletedBy">DeletedBy</see> and <see cref="DateDeleted">DateDeleted</see>.
        /// </summary>
        /// <param name="callerId"> The caller's identifier. </param>
        public void SetDeleted(Guid callerId)
        {
            this.IsDeleted = true;
            this.DeletedBy = callerId;
            this.DateDeleted = DateTimeOffset.UtcNow;
        }


        // Note: This is not used at the moment, so it's set to private to hide it from view.

        /// <summary>
        ///     Undelete the item.
        /// </summary>
        /// <param name="callerId"> The caller's identifier. </param>
        private void UnsetDeleted(Guid callerId)
        {
            this.IsDeleted = false;
            this.DeletedBy = null;
            this.DateDeleted = null;
        }
    }
}
