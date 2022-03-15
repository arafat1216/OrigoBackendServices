using AssetServices.Models;
using Common.Logging;
using System.Collections.Generic;

namespace AssetServices.DomainEvents
{
    public class IMEIChangedDomainEvent<T> : BaseEvent where T : HardwareAsset
    {
        public IMEIChangedDomainEvent(T asset, IList<long> previousIMEI) : base(asset.ExternalId)
        {
            Asset = asset;
            //PreviousIMEI = previousIMEI;
        }
        public T Asset { get; protected set; }
        //public IList<long> PreviousIMEI { get; protected set; }
        public override string EventMessage()
        {
            return $"Asset changed IMEI number.";
        }
    }
}
