using AssetServices.Models;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.DomainEvents
{
    public class MacAddressChangedDomainEvent<T> : BaseEvent where T : HardwareAsset
    {
        public MacAddressChangedDomainEvent(T asset, string macAddress, Guid callerId)
        {
            Asset = asset;
            MacAddress = macAddress;
            CallerId = callerId;
        }

        public T Asset { get; protected set; }
        public string MacAddress { get; protected set; }
        public Guid CallerId { get; protected set; }
    }
}
