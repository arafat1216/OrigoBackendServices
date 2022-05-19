using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Represents a re-mapped event or status-change from an external service-provider.
    /// </summary>
    public class ExternalServiceEventDTO
    {
        /// <summary>
        ///     An event from the service-provider that has been re-mapped to the most suitable <see cref="ServiceStatus.Id"/> alternative.
        /// </summary>
        /// <see cref="ServiceStatusEnum"/>
        public int ServiceStatusId { get; set; }

        /// <summary>
        ///     The time the status was recorded.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }


        /// <summary>
        ///     System-reserved constructor. Should not be used!
        /// </summary>
        [Obsolete("Reserved constructor for the JSON serializers.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ExternalServiceEventDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ExternalServiceEventDTO"/> class.
        /// </summary>
        /// <param name="serviceStatusId"> The corresponding <see cref="ServiceStatus.Id"/>. </param>
        /// <param name="timestamp"></param>
        public ExternalServiceEventDTO(int serviceStatusId, DateTimeOffset timestamp)
        {
            ServiceStatusId = serviceStatusId;
            Timestamp = timestamp;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ExternalServiceEventDTO"/> class.
        /// </summary>
        /// <param name="serviceStatusId"></param>
        /// <param name="timestamp"></param>
        public ExternalServiceEventDTO(ServiceStatusEnum serviceStatusId, DateTimeOffset timestamp)
        {
            ServiceStatusId = (int)serviceStatusId;
            Timestamp = timestamp;
        }
    }
}
