using System;

namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    public class HardwareServiceOrderLog
    {
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string ServiceProvider { get; set; }
    }
}
