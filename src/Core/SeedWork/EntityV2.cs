using System.Text.Json.Serialization;
using MediatR;

namespace Common.Seedwork
{
    // Remarks: This is the same as 'Entity', but using 'DateTimeOffset' and without the overrides that resulted in warnings and code-smells from SonarQube.
    // This should be used for new projects, and we will gradually migrate existing projects over to this one.

    /// <summary>
    ///     A base-entity that implements common properties and methods.
    /// </summary>
    public abstract class EntityV2 : Auditable
    {
        /// <summary>
        ///     The entities internal database ID.
        /// </summary>
        public virtual int Id { get; protected set; }

        /// <summary>
        ///     Backing-field for <see cref="DomainEvents"/>
        /// </summary>
        private List<INotification> _domainEvents = new();

        /// <summary>
        ///     Gets the domain events.
        /// </summary>
        /// <value>
        ///     The domain events.
        /// </value>
        [JsonIgnore]
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();


        /// <summary>
        ///     Initializes a new instance of the <see cref="Auditable"/> class.
        /// </summary>
        /// <inheritdoc/>
        protected EntityV2() : base()
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Auditable"/> class that is intended for unit-testing.
        /// </summary>
        /// <inheritdoc/>
        [Obsolete("This is a reserved constructor that should only be used for unit-testing.")]
        protected EntityV2(Guid createdBy, DateTimeOffset dateCreated, Guid? updatedBy, DateTimeOffset? dateUpdated, Guid? deletedBy = null, DateTimeOffset? dateDeleted = null, bool isDeleted = false) : base(createdBy, dateCreated, updatedBy, dateUpdated, deletedBy, dateDeleted, isDeleted)
        {
        }


        /// <summary>
        /// Adds the domain event.
        /// </summary>
        /// <param name="eventItem">The event item.</param>
        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

    }
}
