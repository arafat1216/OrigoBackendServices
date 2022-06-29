using System;
using System.Collections.Generic;

namespace AssetServices.ServiceModel
{
    public class ReturnDeviceDTO
    {
        public Guid AssetLifeCycleId { get; init; }
        public IList<EmailPersonAttributeDTO>? Managers { get; init; }
        public Guid CallerId { get; set; }
        public Guid ReturnLocationId { get; set; }
    }
}
