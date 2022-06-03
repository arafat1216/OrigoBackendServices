using System;
using System.Collections.Generic;

namespace AssetServices.ServiceModel;

    public class AssetLifeCycleRepairCompleted
    {
        public bool Discarded { get; set; }
        public IEnumerable<string?> NewImei { get; set; } = new List<string>();
        public string? NewSerialNumber { get; set; }
        public Guid CallerId { get; set; }
}

