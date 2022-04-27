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
         * State: Closed / Completed
         */

        /// <summary>
        ///     The service-request is closed, and the asset has been returned to the user unrepaired.
        /// </summary>
        ClosedNotRepaired = 10,

        /// <summary>
        ///     The service-request is closed, and the asset has been returned to the user repaired.
        /// </summary>
        ClosedRepaired = 11,

        /// <summary>
        ///     The service-request is closed, and the asset has been returned to the user repaired - free of charge - as a part of the device's warranty.
        /// </summary>
        ClosedRepairedOnWarranty = 12,

        /// <summary>
        ///     The service-request is closed, and a replacement/swapped device has been returned to the user.
        /// </summary>
        ClosedReplaced = 13,

        /// <summary>
        ///     The service-request is closed, and a replacement/swapped device has been returned to the user - free of charge - as a part of the device's warranty.
        /// </summary>
        ClosedReplacedOnWarranty = 14,

        /// <summary>
        ///     The service-request is closed because the service-provider credited/refunded the asset.
        /// </summary>
        ClosedCredited = 15,


    }
}
