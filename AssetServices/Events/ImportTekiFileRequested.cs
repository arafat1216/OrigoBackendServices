using System;
using Dolittle.SDK.Events;

namespace AssetServices.Events
{
    [EventType("8e3ff3ee-50e3-4ff1-87dc-0699920105fd")]
    public class ImportTekiFileRequested : SystemEvent
    {
        public ImportTekiFileRequested(string requestedBy, DateTime requestedTime)
        {
            RequestedBy = requestedBy;
            RequestedTime = requestedTime;
        }

        public string RequestedBy { get; }
        public DateTime RequestedTime { get; }
    }
}