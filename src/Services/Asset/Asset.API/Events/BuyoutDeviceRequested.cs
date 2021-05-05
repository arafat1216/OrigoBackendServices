using System;

namespace Asset.API.Events
{
    public class BuyoutDeviceRequested : CustomerEvent
    {
        public BuyoutDeviceRequested(Guid customerId, string assetId, string requestedBy, string requestedFor, DateTime requestedTime, decimal buyoutPrice)
        : base(customerId)
        {
            AssetId = assetId;
            RequestedBy = requestedBy;
            RequestedFor = requestedFor;
            RequestedTime = requestedTime;
            BuyoutPrice = buyoutPrice;
        }

        public string AssetId { get; }
        public string RequestedBy { get; }
        public string RequestedFor { get; }
        public DateTime RequestedTime { get; }
        public decimal BuyoutPrice { get; }
    }
}