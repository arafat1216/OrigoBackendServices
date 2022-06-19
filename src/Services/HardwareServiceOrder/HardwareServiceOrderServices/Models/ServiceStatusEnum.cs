namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     A fixed, pre-populated list of <see cref="ServiceStatus"/> IDs.
    /// </summary>
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
        ///     as it may involve some manual data or code fixes! <para>
        ///     
        ///     <b>State:</b> Unknown <br/>
        ///     <b>Status:</b> Unknown </para>
        /// </summary>
        Unknown = 1,


        /*
         * State: Canceled
         */

        /// <summary>
        ///     The service-request was been canceled. <para>
        ///     
        ///     <b>State:</b> Canceled <br/>
        ///     <b>Status:</b> Unknown </para>
        /// </summary>
        Canceled = 2,


        /*
         * State: Registered
         */

        /// <summary>
        ///     A service has been requested. <para>
        ///     
        ///     <b>State:</b> Registered <br/>
        ///     <b>Status:</b> Registered </para>
        /// </summary>
        Registered = 3,

        /// <summary>
        ///     The asset is on it's way to the service-provider. <para>
        ///     
        ///     <b>State:</b> Registered <br/>
        ///     <b>Status:</b> In Transit </para>
        /// </summary>
        RegisteredInTransit = 4,

        // NOT USED BY CONMODO - RESERVED FOR FUTURE PROVIDERS!
        /// <summary>
        ///     The user needs to perform an action. <para>
        ///     
        ///     <b>State:</b> Registered <br/>
        ///     <b>Status:</b> User Action Needed </para>
        /// </summary>
        RegisteredUserActionNeeded = 5,


        /*
         * State: Ongoing
         */

        /// <summary>
        ///     The asset has been received by the service-provider, but the service is not yet completed. <para>
        ///     
        ///     <b>State:</b> Ongoing <br/>
        ///     <b>Status:</b> Ongoing </para>
        /// </summary>
        Ongoing = 6,

        /// <summary>
        ///     The service is currently pending a user-action. <para>
        ///     
        ///     <b>State:</b> Ongoing <br/>
        ///     <b>Status:</b> User Action Needed </para>
        /// </summary>
        OngoingUserActionNeeded = 7,

        /// <summary>
        ///     The asset is currently being back to the user. <para>
        ///     
        ///     <b>State:</b> Ongoing <br/>
        ///     <b>Status:</b> In Transit </para>
        /// </summary>
        OngoingInTransit = 8,

        /// <summary>
        ///     The asset is waiting for the user at a pickup location. <para>
        ///     
        ///     <b>State:</b> Ongoing <br/>
        ///     <b>Status:</b> Ready For Pickup </para>
        /// </summary>
        OngoingReadyForPickup = 9,


        /*
         * State: Completed
         */

        /// <summary>
        ///     The service-request is closed, and the asset has been returned to the user unrepaired. <para>
        ///     
        ///     <b>State:</b> Completed <br/>
        ///     <b>Status:</b> Not Repaired </para>
        /// </summary>
        CompletedNotRepaired = 10,

        /// <summary>
        ///     The service-request is closed, and the asset has been returned to the user repaired. <para>
        ///     
        ///     <b>State:</b> Completed <br/>
        ///     <b>Status:</b> Repaired (no warranty) </para>
        /// </summary>
        CompletedRepaired = 11,

        /// <summary>
        ///     The service-request is closed, and the asset has been returned to the user repaired - free of charge - as a part of the device's warranty. <para>
        ///     
        ///     <b>State:</b> Completed <br/>
        ///     <b>Status:</b> Repaired (warranty) </para>
        /// </summary>
        CompletedRepairedOnWarranty = 12,

        /// <summary>
        ///     The service-request is closed, and a replacement/swapped device has been returned to the user. <para>
        ///     
        ///     <b>State:</b> Completed <br/>
        ///     <b>Status:</b> Replaced (no warranty) </para>
        /// </summary>
        CompletedReplaced = 13,

        /// <summary>
        ///     The service-request is closed, and a replacement/swapped device has been returned to the user - free of charge - as a part of the device's warranty. <para>
        ///     
        ///     <b>State:</b> Completed <br/>
        ///     <b>Status:</b> Replaced (warranty) </para>
        /// </summary>
        CompletedReplacedOnWarranty = 14,

        /// <summary>
        ///     The service-request is closed because the service-provider credited/refunded the asset. <para>
        ///     
        ///     <b>State:</b> Completed <br/>
        ///     <b>Status:</b> Credited </para>
        /// </summary>
        CompletedCredited = 15,

        /// <summary>
        ///     The service-request is closed, and the asset has been discarded/recycled. <para>
        ///     
        ///     <b>State:</b> Completed <br/>
        ///     <b>Status:</b> Discarded </para>
        /// </summary>
        CompletedDiscarded = 16
    }
}
