namespace HardwareServiceOrderServices.Models
{
    public enum ServiceStatusEnum
    {
        /// <summary>
        ///     A <see langword="null"/> equivalent enum-value. This indicates a missing value or a bad/default value-mapping.
        ///     This value should always be treated as an error, as it should not never actually be assigned.
        /// </summary>
        Null = 0,


        /*
         * State: Unknown
         */

        /// <summary>
        ///     We have received a status we don't support, or that we don't have proper mapping for. Someone should investigate 
        ///     as it may involve some manual data or code fixes!
        /// </summary>
        Unknown = 1,


        /*
         * State: Canceled
         */

        /// <summary>
        ///     The service-request was been canceled.
        /// </summary>
        Canceled = 2,


        /*
         * State: Registered
         */

        /// <summary>
        ///     A service has been requested.
        /// </summary>
        Registered = 3,

        /// <summary>
        ///     The asset is on it's way to the service-provider.
        /// </summary>
        RegisteredInTransit = 4,

        // NOT USED BY CONMODO - RESERVED FOR FUTURE PROVIDERS!
        /// <summary>
        ///     The user needs to perform an action.
        /// </summary>
        RegisteredUserActionNeeded = 5,


        /*
         * State: Ongoing
         */

        /// <summary>
        ///     The asset has been received by the service-provider, but the service is not yet completed.
        /// </summary>
        Ongoing = 6,

        /// <summary>
        ///     The service is currently pending a user-action.
        /// </summary>
        OngoingUserActionNeeded = 7,

        /// <summary>
        ///     The asset is currently being back to the user.
        /// </summary>
        OngoingInTransit = 8,

        /// <summary>
        ///     The asset is waiting for the user at a pickup location.
        /// </summary>
        OngoingReadyForPickup = 9,


        /*
         * State: Completed
         */

        /// <summary>
        ///     The service-request is closed, and the asset has been returned to the user unrepaired.
        /// </summary>
        CompletedNotRepaired = 10,

        /// <summary>
        ///     The service-request is closed, and the asset has been returned to the user repaired.
        /// </summary>
        CompletedRepaired = 11,

        /// <summary>
        ///     The service-request is closed, and the asset has been returned to the user repaired - free of charge - as a part of the device's warranty.
        /// </summary>
        CompletedRepairedOnWarranty = 12,

        /// <summary>
        ///     The service-request is closed, and a replacement/swapped device has been returned to the user.
        /// </summary>
        CompletedReplaced = 13,

        /// <summary>
        ///     The service-request is closed, and a replacement/swapped device has been returned to the user - free of charge - as a part of the device's warranty.
        /// </summary>
        CompletedReplacedOnWarranty = 14,

        /// <summary>
        ///     The service-request is closed because the service-provider credited/refunded the asset.
        /// </summary>
        CompletedCredited = 15,


    }
}
