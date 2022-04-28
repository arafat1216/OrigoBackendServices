namespace HardwareServiceOrderServices.Models
{
    public enum ServiceStateEnum
    {
        /// <summary>
        ///     A <see langword="null"/> equivalent enum-value. This indicates a missing value or a bad/default value-mapping.
        ///     This value should always be treated as an error, as it should not never actually be assigned.
        /// </summary>
        Null = 0,

        /// <summary>
        ///     We have received a state we don't support, or that we don't have proper mapping for. Someone should investigate 
        ///     as it may involve some manual data or code fixes!
        /// </summary>
        Unknown = 1,
        
        /// <summary>
        ///     The service-order has been canceled.
        /// </summary>
        Canceled = 2,

        /// <summary>
        ///     The service is registered/ordered, but it's currently not received by the service-provider.
        /// </summary>
        Registered = 3,

        /// <summary>
        ///     The service-provider has received the hardware.
        /// </summary>
        Ongoing = 4,

        /// <summary>
        ///     The service has been closed or completed.
        /// </summary>
        Completed = 5,


    }
}
