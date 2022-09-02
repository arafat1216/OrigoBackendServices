using HardwareServiceOrderServices.Conmodo.Mappings;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using System.Reflection;

namespace HardwareServiceOrder.Conmodo.UnitTests.Mappings
{
    /// <summary>
    ///     Tests for the <see cref="EventMapper"/> class.
    /// </summary>
    public class EventMapperTests
    {

        /// <summary>
        ///     Basic test for <see cref="EventMapper.FromConmodo(IEnumerable{Event}, bool?)"/>
        /// </summary>
        /// <param name="replacementDeviceAdded"> <see langword="true"/> when a device-replacement is confirmed, <see langword="false"/> if
        ///     no device-replacement is confirmed, and <see langword="null"/> if we don't know yet. </param>
        /// <param name="historicalEvents"> The historical events we need to check. </param>
        /// <param name="expectedResponse"> The result we expect to get back. </param>
        [Theory]
        [MemberData(nameof(FromConmodo_Data))]
        internal void FromConmodo(bool? replacementDeviceAdded, IEnumerable<Event> historicalEvents, IEnumerable<ExternalServiceEventDTO> expectedResponse)
        {
            EventMapper eventMapper = new();
            var actual = eventMapper.FromConmodo(historicalEvents, replacementDeviceAdded);

            // Order the lists by timestamp to sync up the orders
            expectedResponse = expectedResponse.OrderByDescending(e => e.Timestamp);
            actual = actual.OrderByDescending(e => e.Timestamp);

            // Asserts
            Assert.Equal(expectedResponse.Count(), actual.Count());

            for (int i = 0; i < actual.Count(); i++)
            {
                Assert.Equal(expectedResponse.ElementAt(i).ServiceStatusId, actual.ElementAt(i).ServiceStatusId);
                Assert.Equal(expectedResponse.ElementAt(i).Timestamp, actual.ElementAt(i).Timestamp);
            }
        }


        /// <summary>
        ///     Basic test for <see cref="EventMapper.ContainsAnyCompletedEvents(IEnumerable{Event})"/>
        /// </summary>
        /// <param name="historicalEvents"> The list of events we are testing against. </param>
        /// <param name="expectedResponse"> The <see cref="bool"/> value we expect to receive back from the method that's being tested. </param>
        [Theory]
        [MemberData(nameof(ContainsAnyCompletedEvents1_Data))]
        internal void ContainsAnyCompletedEvents1(IEnumerable<Event> historicalEvents, bool expectedResponse)
        {
            EventMapper eventMapper = new();
            var actual = eventMapper.ContainsAnyCompletedEvents(historicalEvents);

            Assert.Equal(expectedResponse, actual);
        }


        /// <summary>
        ///      Basic test for <see cref="EventMapper.ContainsAnyCompletedEvents(IEnumerable{Event})"/>
        /// </summary>
        /// <param name="serviceEvent"> The list of events we are testing against. </param>
        /// <param name="expectedResponse"> The <see cref="bool"/> value we expect to receive back from the method that's being tested. </param>
        [Theory]
        [MemberData(nameof(ContainsAnyCompletedEvents2_Data))]
        public void ContainsAnyCompletedEvents2(IEnumerable<ExternalServiceEventDTO> serviceEvent, bool expectedResponse)
        {
            EventMapper eventMapper = new();
            var actual = eventMapper.ContainsAnyCompletedEvents(serviceEvent);

            Assert.Equal(expectedResponse, actual);
        }


        /// <summary>
        ///     A special test-case that handles testing of the private method <see cref="EventMapper"/>.GetCompletedStatusFromEventHistory(). 
        ///     This makes sure we end up with the expected mapping in certain special mapping scenarios, such as when we have two end-statuses.
        /// </summary>
        /// <remarks>
        ///     This uses the testing-data provided by <see cref="GetCompletedStatusFromEventHistory_Data"/>.
        /// </remarks>
        /// <param name="replacementDeviceAdded"> <see langword="true"/> when a device-replacement is confirmed, <see langword="false"/> if
        ///     no device-replacement is confirmed, and <see langword="null"/> if we don't know yet. </param>
        /// <param name="historicalEvents"> The historical events we need to check. </param>
        /// <param name="expectedResponse"> The result we expect to get back. </param>
        /// <exception cref="Exception"> Thrown if we fail <see cref="System.Reflection"/> fails to access the private method. </exception>
        [Theory]
        [MemberData(nameof(GetCompletedStatusFromEventHistory_Data))]
        internal void GetCompletedStatusFromEventHistory(bool? replacementDeviceAdded, IEnumerable<Event> historicalEvents, int expectedResponse)
        {
            // No need to test all trigger IDs, as we are only interested in checking the mappings
            int conmodoEventId = (int)ConmodoEventIdEnum.Levert_Kunde;

            EventMapper eventMapper = new();

            // Using reflections to retrieve the private method
            MethodInfo? methodInfo = typeof(EventMapper).GetMethod("GetCompletedStatusFromEventHistory", BindingFlags.NonPublic | BindingFlags.Instance);

            if (methodInfo is null)
                throw new Exception("Reflections could not access the method. Please check the name parameter in case the method's name has changed.");

            // Adding parameters to the private method
            object[] parameters = { conmodoEventId, historicalEvents, replacementDeviceAdded! };

            // Call the private method and convert the result
            object? result = methodInfo.Invoke(eventMapper, parameters);
            int? convertedResult = (int?)result;

            Assert.Equal(expectedResponse, convertedResult);
        }


        #region MemberData methods (methods for providing test-data)

        /// <summary>
        ///     Provides the test-data (data-carrier) for the <see cref="FromConmodo(bool?, IEnumerable{Event}, int)"/> test.
        /// </summary>
        /// <returns> The testing data to be used by the caller. </returns>
        public static IEnumerable<object[]> FromConmodo_Data()
        {
            // All test-items must have three parameters: 
            //      1) bool? replacementDeviceAdded
            //      2) IEnumerable<Event> historicalEvents
            //      3) IEnumerable<ExternalServiceEventDTO> expectedResponse

            DateTimeOffset timestamp = DateTimeOffset.Now;

            // 1 of 3: Test remapping with no replacement
            yield return new object[] {
                false,
                new List<Event> {
                    new() { EventId = (int)ConmodoEventIdEnum.Mottatt_Servicested, EventName = "Mottatt Serviceverksted", EventDateTime = timestamp.AddDays(-2) },
                    new() { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar, EventName = "Betalbar", EventDateTime = timestamp.AddHours(-12) },
                    new() { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti, EventName = "Garanti", EventDateTime = timestamp.AddHours(-12) },
                    new() { EventId = (int)ConmodoEventIdEnum.Pakke_utlevert_fra_posten, EventName = "Pakke utlevert fra posten", EventDateTime = timestamp },
                },
                new List<ExternalServiceEventDTO>
                {
                    new(ServiceStatusEnum.Ongoing, timestamp.AddDays(-2)),
                    new(ServiceStatusEnum.CompletedRepaired, timestamp),
                },
            };

            // 2 of 3: Test remapping with unknown replacement
            yield return new object[] {
                null!,
                new List<Event> {
                    new() { EventId = (int)ConmodoEventIdEnum.Mottatt_Servicested, EventName = "Mottatt Serviceverksted", EventDateTime = timestamp.AddDays(-2) },
                    new() { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar, EventName = "Betalbar", EventDateTime = timestamp.AddHours(-12) },
                    new() { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti, EventName = "Garanti", EventDateTime = timestamp.AddHours(-12) },
                    new() { EventId = (int)ConmodoEventIdEnum.Pakke_utlevert_fra_posten, EventName = "Pakke utlevert fra posten", EventDateTime = timestamp },
                },
                new List<ExternalServiceEventDTO>
                {
                    new(ServiceStatusEnum.Ongoing, timestamp.AddDays(-2)),
                    new(ServiceStatusEnum.CompletedRepaired, timestamp),
                },
            };

            // 3 of 3: Test remapping with replacement
            yield return new object[] {
                true,
                new List<Event> {
                    new() { EventId = (int)ConmodoEventIdEnum.Mottatt_Servicested, EventName = "Mottatt Serviceverksted", EventDateTime = timestamp.AddDays(-2) },
                    new() { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar, EventName = "Betalbar", EventDateTime = timestamp.AddHours(-12) },
                    new() { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti, EventName = "Garanti", EventDateTime = timestamp.AddHours(-12) },
                    new() { EventId = (int)ConmodoEventIdEnum.Pakke_utlevert_fra_posten, EventName = "Pakke utlevert fra posten", EventDateTime = timestamp },
                },
                new List<ExternalServiceEventDTO>
                {
                    new(ServiceStatusEnum.Ongoing, timestamp.AddDays(-2)),
                    new(ServiceStatusEnum.CompletedReplaced, timestamp),
                },
            };

            // Unsupported event
            yield return new object[] {
                null!,
                new List<Event> {
                    new Event { EventId = (int)ConmodoEventIdEnum.Mottatt_Forhandler, EventName = "Motatt Forhandler", EventDateTime = timestamp },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar, EventName = "Betalbar", EventDateTime = timestamp.AddHours(-5) },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti, EventName = "Garanti", EventDateTime = timestamp.AddHours(-6) },
                },
                new List<ExternalServiceEventDTO>
                {
                    new(ServiceStatusEnum.Unknown, timestamp),
                },
            };

            // Non-existing event
            yield return new object[] {
                null!,
                new List<Event> {
                    new Event { EventId = 99999999, EventDateTime = timestamp },
                },
                new List<ExternalServiceEventDTO>
                {
                    new(ServiceStatusEnum.Unknown, timestamp)
                },
            };

            // Empty input
            yield return new object[] {
                null!,
                new List<Event>(),
                new List<ExternalServiceEventDTO>()
            };
        }


        /// <summary>
        ///     Provides the test-data (data-carrier) for the <see cref="ContainsAnyCompletedEvents(bool?, IEnumerable{Event}, int)"/> test.
        /// </summary>
        /// <returns> The testing data to be used by the caller. </returns>
        public static IEnumerable<object[]> ContainsAnyCompletedEvents1_Data()
        {
            // All test-items must have three parameters: 
            //      1) IEnumerable<Event> historicalEvents
            //      2) int expectedResponse

            yield return new object[] {
                new List<Event> {
                    new Event { EventId = (int)ConmodoEventIdEnum.Mottatt_Forhandler },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti },
                    new Event { EventId = (int)ConmodoEventIdEnum.Pakke_utlevert_fra_posten },
                },
                true
            };

            yield return new object[] {
                new List<Event> {
                    new Event { EventId = (int)ConmodoEventIdEnum.Mottatt_Forhandler },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti },
                },
                false
            };
        }


        /// <summary>
        ///     Provides the test-data (data-carrier) for the <see cref="ContainsAnyCompletedEvents1(ExternalServiceEventDTO, bool)"/> test.
        /// </summary>
        /// <returns> The testing data to be used by the caller. </returns>
        public static IEnumerable<object[]> ContainsAnyCompletedEvents2_Data()
        {
            // All test-items must have three parameters: 
            //      1) IEnumerable<ExternalServiceEventDTO> historicalEvents
            //      2) int expectedResponse

            yield return new object[] {
                new List<ExternalServiceEventDTO>(),
                false
            };

            yield return new object[] {
                new List<ExternalServiceEventDTO> {
                    new ExternalServiceEventDTO (ServiceStatusEnum.Registered, DateTimeOffset.Now),
                },
                false
            };

            yield return new object[] {
                new List<ExternalServiceEventDTO> {
                    new ExternalServiceEventDTO (ServiceStatusEnum.Registered, DateTimeOffset.Now.AddHours(-2)),
                    new ExternalServiceEventDTO (ServiceStatusEnum.CompletedReplacedOnWarranty, DateTimeOffset.Now),
                    new ExternalServiceEventDTO (ServiceStatusEnum.Ongoing, DateTimeOffset.Now.AddHours(-2)),
                },
                true
            };

            yield return new object[] {
                new List<ExternalServiceEventDTO> {
                    new ExternalServiceEventDTO (ServiceStatusEnum.Registered, DateTimeOffset.Now.AddHours(-3)),
                    new ExternalServiceEventDTO (ServiceStatusEnum.Ongoing, DateTimeOffset.Now.AddHours(-2)),
                    new ExternalServiceEventDTO (ServiceStatusEnum.Unknown, DateTimeOffset.Now),
                },
                false
            };
        }


        /// <summary>
        ///     Provides the test-data (data-carrier) for the <see cref="GetCompletedStatusFromEventHistory(bool?, IEnumerable{Event})"/> test.
        /// </summary>
        /// <returns> The testing data to be used by the caller. </returns>
        public static IEnumerable<object[]> GetCompletedStatusFromEventHistory_Data()
        {
            // All test-items must have three parameters: 
            //      1) bool? replacementDeviceAdded
            //      2) IEnumerable<Event> historicalEvents
            //      3) int expectedResponse

            #region Multi-status ('Betalbar' + 'Garanti')

            // Test item 1: Repaired (unknown replacement status) w/double complete status on the repair (parts covered by warranty, parts covered by user)
            yield return new object[] {
                null!,
                new List<Event> {
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti }
                },
                (int)ServiceStatusEnum.CompletedRepaired
            };

            // Test item 2: Repaired device w/double complete status on the repair (parts covered by warranty, parts covered by user)
            yield return new object[] {
                false,
                new List<Event> {
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti }
                },
                (int)ServiceStatusEnum.CompletedRepaired
            };

            // Test item 3: Replaced device w/double complete status on the repair (parts covered by warranty, parts covered by user)
            yield return new object[] {
                true,
                new List<Event> {
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti }
                },
                (int)ServiceStatusEnum.CompletedReplaced
            };

            // Test item 4: Same as '1' but with the event order flipped to ensure we don't pick based on order
            yield return new object[] {
                null!,
                new List<Event> {
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar },
                },
                (int)ServiceStatusEnum.CompletedRepaired
            };

            // Test item 5: Same as '2' but with the event order flipped to ensure we don't pick based on order
            yield return new object[] {
                false,
                new List<Event> {
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar },
                },
                (int)ServiceStatusEnum.CompletedRepaired
            };

            // Test item 6: Same as '3' but with the event order flipped to ensure we don't pick based on order
            yield return new object[] {
                true,
                new List<Event> {
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Garanti },
                    new Event { EventId = (int)ConmodoEventIdEnum.PaymentStatus_Betalbar },
                },
                (int)ServiceStatusEnum.CompletedReplaced
            };

            #endregion Multi-status ('Betalbar' + 'Garanti')
        }

        #endregion
    }
}
