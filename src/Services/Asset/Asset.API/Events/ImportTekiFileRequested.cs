using System;

namespace Asset.API.Events
{
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