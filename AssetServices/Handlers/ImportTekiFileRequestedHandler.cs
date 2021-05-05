using System;
using AssetServices.Events;
using Dolittle.SDK.Events;
using Dolittle.SDK.Events.Handling;

namespace AssetServices.Handlers
{
    [EventHandler("f2d366cf-c00a-4479-acc4-851e04b6fbba")]
    public class ImportTekiFileRequestedHandler
    {
        public void Handle(ImportTekiFileRequested @event, EventContext eventContext)
        {
            Console.WriteLine($"Import Teki file requested by {@event.RequestedBy} at {@event.RequestedTime}");
        }
    }
}