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
    /// <see cref="EntityFramework.SaveContextChangesInterceptor"/>
    public abstract class Auditable
    {
        /*
         * Important Remarks:
         * 
         * None of the setters in this class should be settable or accessible outside for this abstract class. The values are intended 
         * to be automatically assigned/updated by the DBContext's "SaveContextChangesInterceptor"-class, using reflections to allow access
         * to the private setters. This is done to prevent any accidental assignment/override on the values.
         */

        /// <summary>
        ///     Initializes a new instance of the <see cref="Auditable"/> class. This is intended to be used with
        ///     the <see cref="EntityFramework.SaveContextChangesInterceptor"/> for automatically applying values 
        ///     to the private setters.
        /// </summary>
        protected Auditable()
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Auditable"/> class, intended for use with unit-testing frameworks.
        /// </summary>
        /// <param name="createdBy"> The user-id for person that first created the entity. </param>
        /// <param name="dateCreated"> A timestamp of when the entity was first created. </param>
        /// <param name="updatedBy"> The user-id for last user that updated the entity. 
        ///     If the entity have never been updated, then the value is <see langword="null"/>. </param>
        /// <param name="dateUpdated"> A timestamp of the last time the entity was updated. 
        ///     If the entity have never been updated, then the value is <see langword="null"/>. </param>
        /// <param name="deletedBy"> The user-id for the person that deleted the entity.
        ///     If the entity is not deleted, then the value is <see langword="null"/>. </param>
        /// <param name="dateDeleted"> A timestamp stating when the entity was deleted.
        ///     If the entity is not deleted, then the value is <see langword="null"/>. </param>
        /// <param name="isDeleted"> A <see cref="bool"/> indicating whether or not the entity is soft-deleted. </param>
        [Obsolete("This is a reserved constructor that should only be used for unit-testing.")]
        protected Auditable(Guid createdBy, DateTimeOffset dateCreated, Guid? updatedBy, DateTimeOffset? dateUpdated, Guid? deletedBy = null, DateTimeOffset? dateDeleted = null, bool isDeleted = false)
        {
            CreatedBy = createdBy;
            DateCreated = dateCreated;
            UpdatedBy = updatedBy;
            DateUpdated = dateUpdated;
            DeletedBy = deletedBy;
            DateDeleted = dateDeleted;
            IsDeleted = isDeleted;
        }


        /// <summary>
        ///     A timestamp for when the database-entity was first created.
        /// </summary>
        [JsonInclude]
        public DateTimeOffset DateCreated { get; private set; }

        /// <summary>
        ///     A timestamp for when the database-entity was last updated.
        /// </summary>
        [JsonInclude]
        public DateTimeOffset? DateUpdated { get; private set; }

        /// <summary>
        ///     A timestamp for when the database-entity was deleted.
        /// </summary>
        [JsonInclude]
        public DateTimeOffset? DateDeleted { get; private set; }

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
        public Guid CreatedBy { get; private set; }

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
        public Guid? UpdatedBy { get; private set; }

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
        public Guid? DeletedBy { get; private set; }

        /// <summary>
        ///     When <see langword="true"/> the item has been soft deleted. Externally, all soft deleted entries should generally be excluded from results and
        ///     treated as non-existent.
        /// </summary>
        [JsonInclude]
        public bool IsDeleted { get; private set; } = false;
    }
}
