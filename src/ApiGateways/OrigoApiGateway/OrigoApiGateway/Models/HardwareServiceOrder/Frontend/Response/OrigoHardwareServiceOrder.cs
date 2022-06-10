﻿using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response
{
    public class OrigoHardwareServiceOrder
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public Guid Owner { get; set; }
        public string ServiceProvider { get; set; }
        public IEnumerable<ServiceEvent> Events { get; set; }
        public Guid AssetLifecycleId { get; set; }
    }
}
