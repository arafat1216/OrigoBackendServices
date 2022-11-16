using System;
using AssetServices.Models;
using Common.Enums;
using Common.Logging;

namespace AssetServices.DomainEvents.EndOfLifeCycleEvents
{
    public class ReportDeviceDomainEvent : BaseEvent
    {
        public ReportDeviceDomainEvent(AssetLifecycle assetLifecycle, ReportCategory reportCategory, Guid callerId, AssetLifecycleStatus previousAssetLifecycleStatus) : base(assetLifecycle.ExternalId)
        {
            AssetLifecycle = assetLifecycle;
            CallerId = callerId;
            ReportCategory = reportCategory;
            PreviousAssetLifecycleStatus = previousAssetLifecycleStatus;
        }

        public AssetLifecycle AssetLifecycle { get; protected set; }
        public Guid CallerId { get; protected set; }
        public ReportCategory ReportCategory { get; protected set; }
        public AssetLifecycleStatus PreviousAssetLifecycleStatus { get; protected set; }

        public override string EventMessage()
        {
            return $"User Id: {CallerId} has Reported '{ReportCategory}' for the asset Id:{AssetLifecycle.ExternalId} with status: {PreviousAssetLifecycleStatus};";
        }
    }
}
