using AssetServices.DomainEvents;
using Common.Seedwork;
using System;

namespace AssetServices.Models
{
    /// <summary>
    ///  Bridge between Assets owning n Labels, and Labels being assigned to x Assets
    /// </summary>
    public class AssetLabel : Entity
    {
        public AssetLabel()
        { }


        public AssetLabel(int assetId, int customerLabelId, Guid callerId)
        {
            ExternalId = Guid.NewGuid();
            AssetId = assetId;
            LabelId = customerLabelId;
            CreatedBy = callerId;
            CreatedDate = DateTime.UtcNow;
            LastUpdatedDate = DateTime.UtcNow;
            IsDeleted = false;
            AddDomainEvent(new AssetLabelCreatedDomainEvent(ExternalId, AssetId, LabelId));
        }

        public void SetActiveStatus(Guid callerId, bool deactivate)
        {
            var previousValue = IsDeleted;
            IsDeleted = deactivate;
            if (deactivate)
                DeletedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            UpdatedBy = callerId;
            AddDomainEvent(new AssetLabelChangedDomainEvent(ExternalId, AssetId, LabelId, previousValue));
        }

        public Guid ExternalId { get; protected set; }

        // AssetLabel belongs to 1 Asset
        public int AssetId { get; protected set; }
        public virtual Asset Asset { get; set; }

        // AssetLabel belongs to 1 CustomerLabel
        public int LabelId { get; protected set; }
        public virtual CustomerLabel Label {get; set;}
    }
}
