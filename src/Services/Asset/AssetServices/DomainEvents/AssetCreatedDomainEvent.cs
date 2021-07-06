using MediatR;
using System;

namespace AssetServices.DomainEvents
{
    public class AssetCreatedDomainEvent : INotification
    {
        public Guid AssetId { get; }
        public Guid CustomerId { get; }
        public string SerialNumber { get; }
        public Guid AssetCategoryId { get; }
        public string Brand { get; }
        public string Model { get; }
        public string LifecycleType { get; }
        public DateTime PurchaseDate { get; }
        public Guid? ManagedByDepartmentId { get; }
        public Guid? AssetHolderId { get; }
        public bool IsActive { get; }
    }
}
