using HardwareServiceOrderServices.Conmodo.ApiModels;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Conmodo.Mappings
{
    /// <summary>
    ///     Handles remapping between Conmodo's <see cref="Event"/>s and <see cref="ExternalServiceEventDTO"/>.
    /// </summary>
    internal class EventMapper
    {
        /// <summary>
        ///     Checks if any of the events indicates a "Completed" state.
        /// </summary>
        /// <param name="events"> The list of events that should be checked against. </param>
        /// <returns> Returns <see langword="true"/> if a completed-state was detected. Otherwise it returns <see langword="false"/>. </returns>
        public bool ContainsAnyCompletedEvents(IEnumerable<ExternalServiceEventDTO> events)
        {
            // All IDs for the various completed states we are checking against.
            var validCompletedStatusIds = new List<int>
            {
                (int)ServiceStatusEnum.CompletedNotRepaired,
                (int)ServiceStatusEnum.CompletedRepaired,
                (int)ServiceStatusEnum.CompletedRepairedOnWarranty,
                (int)ServiceStatusEnum.CompletedReplaced,
                (int)ServiceStatusEnum.CompletedReplacedOnWarranty,
                (int)ServiceStatusEnum.CompletedCredited,
                (int)ServiceStatusEnum.CompletedDiscarded
            };

            // Does the completed statuses exist in any of the events?
            bool eventExist = events.Any(e => validCompletedStatusIds.Contains(e.ServiceStatusId));
            return eventExist;
        }


        /// <inheritdoc cref="ContainsAnyCompletedEvents(IEnumerable{ExternalServiceEventDTO})"/>
        public bool ContainsAnyCompletedEvents(IEnumerable<Event> events)
        {
            var convertedEvent = FromConmodo(events, null);
            return ContainsAnyCompletedEvents(convertedEvent);
        }


        /// <summary>
        ///     Remaps a list of Conmodo-events, and converts them to the closest representative that is supported by our solution.
        /// </summary>
        /// <remarks>
        ///     All Conmodo-events that is considered "uninteresting" for our solution will be discarded, and not included in the returned/converted values.
        /// </remarks>
        /// <param name="conmodoEvents"> The list of events that should be re-mapped from Conmodo's events to a corresponding status in our solution. </param>
        /// <param name="replacementDeviceAdded"> A <see cref="bool"/> that tells the re-mapper if a replacement device has been provided. 
        ///     If this is <see langword="null"/> or <see langword="false"/> the system will assume no replacement-devices has been supplied. </param>
        /// <returns> A list containing the remapped events. </returns>
        public IEnumerable<ExternalServiceEventDTO> FromConmodo(IEnumerable<Event> conmodoEvents, bool? replacementDeviceAdded)
        {
            List<ExternalServiceEventDTO> dtoEvents = new();

            foreach (var @event in conmodoEvents)
            {
                // This should really never happen, but is in place for safety as Conmodo's API documentation has the field tagged as nullable.
                // So for safety; if we receive a null-value from Conmodo, let's skip the remapping.
                if (@event.EventId is null)
                    continue;

                int? newStatus;

                // Try and do a quick and simple 1-to-1 mapping from events to statuses.
                try
                {
                    newStatus = GetStatusFromEventId((int)@event.EventId);
                }
                // If the mapping requires deeper checks (e.g. due to historical data requirements), the GetStatusFromEventId() method
                // will throw an exception indicating that we should swap to the method that explicitly checks and remaps these events/statuses.
                catch (ConmodoEventMappingException)
                {
                    newStatus = GetStatusFromEventHistory((int)@event.EventId, conmodoEvents, replacementDeviceAdded);
                }

                // Null indicates an uninteresting event. Skip to the next check as we want to ignore these.
                if (newStatus is null)
                    continue;

                var response = new ExternalServiceEventDTO((int)newStatus, @event.EventDateTime);
                dtoEvents.Add(response);
            }

            return dtoEvents.OrderByDescending(e => e.Timestamp);
        }


        /// <summary>
        ///     Attempts to resolve the correct "Completed" status based on the previous historical details.
        /// </summary>
        /// <remarks>
        ///     <b>NOTE:</b> This is only intended to resolve the correct status for Conmodo's event-IDs '31' and '25433' as these require custom handling!
        /// </remarks>
        /// <param name="conmodoEventId"> Conmodo's event-ID that should be re-mapped. </param>
        /// <param name="historicalEvents"> A list containing all events that should be used when checking for cross-event dependencies. </param>
        /// <param name="replacementDeviceAdded"> A <see cref="bool"/> that tells the re-mapper if a replacement device has been provided. 
        ///     If this is <see langword="null"/> or <see langword="false"/> the system will assume no replacement-devices has been supplied. </param>
        /// <returns> The closest status-ID implemented by the solution, or <see langword="null"/> if the event should be ignored. </returns>
        private int? GetStatusFromEventHistory(int conmodoEventId, in IEnumerable<Event> historicalEvents, bool? replacementDeviceAdded)
        {
            switch (conmodoEventId)
            {
                case 31:                                                                        // Conmodo's description: 'Levert kunde'
                case 25433:                                                                     // Conmodo's description: 'Pakke utlevert fra posten'
                    {
                        // Not Repaired
                        if (historicalEvents.Any(e => e.EventId == 42))                           // Conmodo's description (42): 'Returneres ureparert'
                        {
                            return (int)ServiceStatusEnum.CompletedNotRepaired;
                        }
                        // Repaired
                        else
                        {
                            // Repaired --> Warranty
                            if (historicalEvents.Any(e => e.EventId == 8))                        // Conmodo's description (8): 'Garanti'
                            {
                                // Repaired --> Warranty --> Device is replaced
                                if (replacementDeviceAdded == true)
                                    return (int)ServiceStatusEnum.CompletedReplacedOnWarranty;
                                // Repaired --> Warranty --> Device was repaired
                                else
                                    return (int)ServiceStatusEnum.CompletedRepairedOnWarranty;
                            }
                            // Repaired --> No warranty
                            else
                            {
                                // Repaired --> No warranty --> Device is replaced
                                if (replacementDeviceAdded == true)
                                    return (int)ServiceStatusEnum.CompletedReplaced;
                                // Repaired --> No warranty --> Device was repaired
                                else
                                    return (int)ServiceStatusEnum.CompletedRepaired;
                            }
                        }

                        // If none of the above, then break and run default behavior.
                        // This should never happen, but is a safety-net so we don't accidentally step into other cases...
#pragma warning disable CS0162 // Unreachable code detected
                        break;
#pragma warning restore CS0162 // Unreachable code detected
                    }

                default:
                    break;
            }

            return (int)ServiceStatusEnum.Unknown;
        }


        /// <summary>
        ///     Remaps Conmodo's event-IDs to the closest status-ID that is supported by our solution. If Conmodo's event is uninteresting,
        ///     or it should be ignored, then this method will instead return a <see langword="null"/> value.
        /// </summary>
        /// <param name="conmodoEventId"> Conmodo's event-ID that should be re-mapped. </param>
        /// <returns> The closest status-ID implemented by the solution, or <see langword="null"/> if the event should be ignored. </returns>
        /// <exception cref="ConmodoEventMappingException"> Thrown when we have event/status re-mappings that requires knowledge of previous events. 
        ///     When thrown, the caller should instead try and resolve the event using <see cref="GetStatusFromEventHistory(int, in IEnumerable{Event}, bool?)"/>. </exception>
        /// <see cref="GetStatusFromEventHistory(int, in IEnumerable{Event}, bool?)"/>
        private int? GetStatusFromEventId(int conmodoEventId)
        {
            switch (conmodoEventId)
            {
                /***********************************
                 * State: Registered
                 ***********************************/

                // Events that returns "Registered: Registered":

                case 21:                                                    // Conmodo's description: 'Registrert'
                case 25424:                                                 // Conmodo's description: 'Sendes Serviceverkstedet'
                    return (int)ServiceStatusEnum.Registered;

                // Events that returns "Registered: In Transit"
                case 25431:                                                 // Conmodo's description: 'Pakke innlevert hos posten'
                    return (int)ServiceStatusEnum.RegisteredInTransit;

                // Events that returns "Registered: User Action Needed":
                // <none>


                /***********************************
                 * State: Ongoing
                 ***********************************/

                // Events that returns "Ongoing: Ongoing":
                case 13:                                                    // Conmodo's description: 'Ferdig'
                case 30:                                                    // Conmodo's description: 'Mottatt Forhandler'
                case 25025:                                                 // Conmodo's description: 'Ingen betaling'
                case 25051:                                                 // Conmodo's description: 'Venter på bytteenhet'
                case 25074:                                                 // Conmodo's description: 'Venter salgspakke'
                case 25395:                                                 // Conmodo's description: 'Venter på bekreftelse fra Forsikringsselskap'
                case 25402:                                                 // Conmodo's description: 'Venter på svar fra produsent'
                case 25525:                                                 // Conmodo's description: 'Motatt fra kunde'
                case 25046:                                                 // Conmodo's description: 'Påbegynt'
                case 25448:                                                 // Conmodo's description: 'Svar mottatt'
                case 25451:                                                 // Conmodo's description: 'Retur fra eksternt verksted'
                case 25514:                                                 // Conmodo's description: 'Final test'
                case 25542:                                                 // Conmodo's description: 'Repair started'
                case 25018:                                                 // Conmodo's description: 'Sendes direkte til kunde'
                case 25024:                                                 // Conmodo's description: 'Sendt eksternt verksted'
                case 25546:                                                 // Conmodo's description: 'Received internal shipment'
                case 25545:                                                 // Conmodo's description: 'Sent internal shipment'
                    return (int)ServiceStatusEnum.Ongoing;

                // Events that returns "Ongoing: User Action Needed":
                case 25036:                                                 // Conmodo's description: 'Mangler Kjøpsdok'
                case 25002:                                                 // Conmodo's description: 'Prisoverslag'
                case 25441:                                                 // Conmodo's description: 'Avventer svar fra kunde'
                case 25538:                                                 // Conmodo's description: 'Avventer svar fra forhandler FMiP'
                case 25539:                                                 // Conmodo's description: 'Avventer svar fra kunde FMiP'
                case 25434:                                                 // Conmodo's description: 'Pakke returnert avsender'
                case 25557:                                                 // Conmodo's description: 'Enhet returnert fra mottaker'
                    return (int)ServiceStatusEnum.OngoingUserActionNeeded;

                // Events that returns "Ongoing: In Transit":
                case 25558:                                                 // Conmodo's description: 'Pakke plukket opp av transportør'
                case 25529:                                                 // Conmodo's description: 'Preswap enhet sendt'
                case 25050:                                                 // Conmodo's description: 'Ettersending'
                    return (int)ServiceStatusEnum.OngoingInTransit;

                // Events that returns "Ongoing: Ready For Pickup":
                case 25432:                                                 // Conmodo's description: 'Pakke klar til henting hos posten'
                case 25534:                                                 // Conmodo's description: 'Klar for utlevering'
                    return (int)ServiceStatusEnum.OngoingReadyForPickup;


                /***********************************
                 * State: Canceled
                 ***********************************/
                case 22:                                                    // Conmodo's description: 'Feilregistrering'
                    return (int)ServiceStatusEnum.Canceled;


                /***********************************
                 * State: Completed
                 ***********************************/

                // Events that returns "Completed: Not Repaired":
                // Events that returns "Completed: Repaired":
                // Events that returns "Completed: Repaired On Warranty":
                // Events that returns "Completed: Replaced":
                // Events that returns "Completed: Replaced On Warranty":
                case 31:
                case 25433:
                    throw new ConmodoEventMappingException("These events require additional information. Please use 'GetCompletedStatusFromHistory' to resolve the historical requirements.");

                // Events that returns "Completed: Discarded":
                case 25006:                                                 // Conmodo's description: 'Beholdt app.etter avt.'
                case 25547:                                                 // Conmodo's description: 'Recycle'
                    return (int)ServiceStatusEnum.CompletedDiscarded;

                // Events that returns "Completed: Credited":
                case 25411:                                                 // Conmodo's description: 'Reklamasjon - Krediteres'
                case 25064:                                                 // Conmodo's description: 'Kreditert'
                    return (int)ServiceStatusEnum.CompletedCredited;


                /***********************************
                 * State: Unknown
                 ***********************************/

                // These events are known, but should never occur. If these happen, someone has likely been naughty, and we need to take action!
                // Events that returns "Unknown: Unknown":
                case 2:                                                     // Conmodo's description: 'Delvis Motatt Servicested'
                case 25022:                                                 // Conmodo's description: 'Registrert av Service'
                case 25526:                                                 // Conmodo's description: 'Leveres i butikk'
                    return (int)ServiceStatusEnum.Unknown;


                /************************************
                 * Other / default / ignored values
                 ***********************************/

                // Events that we should ignore, and that therefore should return 'null':

                case 25463:                                                 // Conmodo's description: 'Back to storage'
                case 25443:                                                 // Conmodo's description: 'Annen betaler'
                case 25009:                                                 // Conmodo's description: 'Betalbar'
                case 25045:                                                 // Conmodo's description: 'Betalbar?'
                case 25457:                                                 // Conmodo's description: 'Preswap'
                case 25456:                                                 // Conmodo's description: 'Preswap?'
                case 25008:                                                 // Conmodo's description: 'Forsikring?'
                case 36:                                                    // Conmodo's description: 'Forsikring'
                case 25067:                                                 // Conmodo's description: 'Reklamasjon - Tidligere Service?'
                case 25007:                                                 // Conmodo's description: 'Garanti?'
                    return null;

                // Unmapped events. These all return "Unknown" as they are likely caused by a missing mapping value.
                default:
                    return (int)ServiceStatusEnum.Unknown;
            }
        }
    }
}
