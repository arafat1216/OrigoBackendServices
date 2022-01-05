using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Asset changed IMEI number.";
        }
    }
}
