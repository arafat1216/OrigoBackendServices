using System;
using Dolittle.SDK;
using Dolittle.SDK.Tenancy;
using AssetServices.Events;
using AssetServices.Handlers;

namespace ImportTekiFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = Client
                    .ForMicroservice("7a570285-d114-43d7-8705-f96ce12b5ea9")
                    .WithRuntimeOn("localhost", 50053)
                    .WithEventTypes(eventTypes =>
                        eventTypes.Register<ImportTekiFileRequested>())
                    .WithEventHandlers(builder =>
                        builder.RegisterEventHandler<ImportTekiFileRequestedHandler>())
                    .Build();

            // client
            //     .AggregateOf<Asset>("560d1a25-646c-41d6-bd4f-4374f982c2a4", _ => _.ForTenant(TenantId.Development))
            //     .Perform(asset => asset. kitchen.PrepareDish("Bean Blaster Taco", "Mr. Taco"));

            var importTekiFileRequested = new ImportTekiFileRequested("Rolf", DateTime.UtcNow);
            client.EventStore
                .ForTenant(Guid.Parse("7d287d4b-2916-43d6-aced-87de26fd6ae3"))
                .Commit(eventsBuilder =>
                    eventsBuilder
                        .CreateEvent(importTekiFileRequested)
                        .FromEventSource("560d1a25-646c-41d6-bd4f-4374f982c2a4"));

            // Blocks until the EventHandlers are finished, i.e. forever
            client.Start().Wait();
        }
    }
}