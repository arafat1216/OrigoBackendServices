using AssetServices.DomainEvents;
using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AssetServices.Models
{
    public class CustomerLabel : Entity
    {
        /// <summary>
        /// The external Id of this Label
        /// </summary>
        public Guid ExternalId { get; protected set; }

        /// <summary>
        /// The customer this Label belongs to
        /// </summary>
        public Guid CustomerId { get; protected set; }

        /// <summary>
        /// The Label data
        /// <see cref="Label"/>
        /// </summary>
        [JsonInclude]
        public Label Label { get; protected set; }

        /// <summary>
        /// All Asset lifecycles having this customer label set. Needed for many-to-many mapping.
        /// </summary>
        [JsonIgnore]
        public ICollection<AssetLifecycle> AssetLifecycles { get; set; }

        [JsonConstructor]
        public CustomerLabel()
        { }

        /// <summary>
        /// Assign the given label to the given customer
        /// </summary>
        
        public CustomerLabel(Guid customerId, Guid callerId, Label label)
        {
            ExternalId = Guid.NewGuid();
            CustomerId = customerId;
            Label = label;
            CreatedDate = DateTime.UtcNow;
            LastUpdatedDate = DateTime.UtcNow;
            CreatedBy = callerId;
            UpdatedBy = callerId;
            IsDeleted = false;
        }

        /// <summary>
        /// Used when updating a CustomerLabel.
        /// CreatedBy & CreatedDate is intentionally not set, as the object already exists.
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="customerId"></param>
        /// <param name="callerId"></param>
        /// <param name="label"></param>
        public CustomerLabel(Guid externalId, Guid customerId, Guid callerId, Label label)
        {
            ExternalId = externalId;
            CustomerId = customerId;
            Label = label;
            LastUpdatedDate = DateTime.UtcNow;
            UpdatedBy = callerId;
        }


        /// <summary>
        /// Update the label attribute
        /// </summary>
        /// <param name="label"></param>
        public void PatchLabel(Guid callerId, Label label)
        {
            var previousLabel = Label;
            Label = label;
            LastUpdatedDate = DateTime.UtcNow;
            UpdatedBy = callerId;
        }

        /// <summary>
        /// Soft delete this from database.
        /// Entity still exists, but will not show up when fetching CustomerLabels from repository
        /// </summary>
        /// <param name="callerId"></param>
        public void Delete(Guid callerId)
        {
            AddDomainEvent(new CustomerLabelDeletedDomainEvent(this, true));
        }
    }
}
