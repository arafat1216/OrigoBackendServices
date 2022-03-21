using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MediatR;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Common.Seedwork
{
    /// <summary>
    ///     A base-entity that implements common properties and methods.
    /// </summary>
    public abstract class Entity
    {
        int? _requestedHashCode;

        /// <summary>
        ///     The entities internal database ID.
        /// </summary>
        public virtual int Id { get; protected set; }

        /// <summary>
        ///     A timestamp for when the entity was first created.
        /// </summary>
        [JsonInclude]
        public DateTime CreatedDate { get; protected set; } = DateTime.UtcNow;

        /// <summary>
        ///     The external ID of the user that originally created this entity.
        /// 
        ///     <list type="bullet">
        ///         <item>For automated actions performed by the system, the GUID will be "<c>00000000-0000-0000-0000-000000000001</c>"
        ///         (<see cref="Extensions.GuidExtension.SystemUserId(Guid)">Guid.SystemUserId</see> extension). </item>
        ///     
        ///         <item>If the user is unknown, the GUID will be "<c>00000000-0000-0000-0000-000000000000</c>" (<see cref="Guid.Empty"/>). </item>
        ///     </list>
        /// </summary>
        [JsonInclude]
        public Guid CreatedBy { get; protected set; }

        /// <summary>
        ///     A timestamp for the last time the database-entity was updated.
        /// </summary>
        [JsonInclude]
        public DateTime LastUpdatedDate { get; protected set; }

        /// <summary>
        ///     The external ID of the user that performed the last update on this entity.
        /// 
        ///     <list type="bullet">
        ///         <item>For automated actions performed by the system, the GUID will be "<c>00000000-0000-0000-0000-000000000001</c>"
        ///         (<see cref="Extensions.GuidExtension.SystemUserId(Guid)">Guid.SystemUserId</see> extension). </item>
        ///     
        ///         <item>If the user is unknown, the GUID will be "<c>00000000-0000-0000-0000-000000000000</c>" (<see cref="Guid.Empty"/>). </item>
        ///     </list>
        /// </summary>
        [JsonInclude]
        public Guid UpdatedBy { get; protected set; }

        /// <summary>
        ///     When <see langword="true"/> the item has been soft deleted. Externally, all soft deleted entries should generally be excluded from results and
        ///     treated as non-existent.
        /// </summary>
        [JsonInclude]
        public bool IsDeleted { get; protected set; }

        /// <summary>
        ///     The external ID of the user that deleted this entity.
        /// 
        ///     <list type="bullet">
        ///         <item>For automated actions performed by the system, the GUID will be "<c>00000000-0000-0000-0000-000000000001</c>"
        ///         (<see cref="Extensions.GuidExtension.SystemUserId(Guid)">Guid.SystemUserId</see> extension). </item>
        ///     
        ///         <item>If the user is unknown, the GUID will be "<c>00000000-0000-0000-0000-000000000000</c>" (<see cref="Guid.Empty"/>). </item>
        ///         
        ///         <item>If the entity is not deleted, this will be <see langword="null"/>. </item>
        ///     </list>
        /// </summary>
        [JsonInclude]
        public Guid? DeletedBy { get; protected set; }

        private List<INotification> _domainEvents;

        [JsonIgnore]
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

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

        public bool IsTransient()
        {
            return Id == default;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || IsTransient())
                return false;
            else
                return item.Id == Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                _requestedHashCode ??= Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            else
                // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
                return base.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return left?.Equals(right) ?? Equals(right, null);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
