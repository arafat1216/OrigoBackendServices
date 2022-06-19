using System;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    public class ServiceEvent
    {
        public string Status { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
