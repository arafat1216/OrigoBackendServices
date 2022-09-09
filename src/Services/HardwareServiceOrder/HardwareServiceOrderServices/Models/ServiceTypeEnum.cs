namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     A fixed, pre-populated list of <see cref="ServiceType"/> IDs.
    /// </summary>
    public enum ServiceTypeEnum
    {
        /// <summary>
        ///     A <see langword="null"/> equivalent enum-value. This indicates a missing value or a bad/default value-mapping.
        ///     This value should always be treated as an error, as it should not never actually be used or assigned.
        /// </summary>
        Null = 0,

        /// <summary>
        ///     Not yet implemented.
        /// </summary>
        [Obsolete("Currently not implemented / out of scope")]
        Recycle = 1,

        /// <summary>
        ///     A type of return where the device is returned to an external service-provider. The service-provider does
        ///     an assessment of the device, and the customer get a cash-back based on the asset's estimated value. 
        ///     
        ///     <para>
        ///     Once the service-provider has taken over the device, it will be wiped, refurbish, then finally sold on the
        ///     2nd hand market. </para>
        /// </summary>
        Remarketing = 2,

        /// <summary>
        ///     SUR ('Same Unit Repair') is the 'normal' repair type. This means that the asset that was sent in, is also the
        ///     asset that will be repaired and returned (some exceptions apply, such as warranty-replacements).
        /// </summary>
        SUR = 3,

        /// <summary>
        ///     Not yet implemented.
        /// </summary>
        [Obsolete("Currently not implemented / out of scope")]
        PreSwap = 4,
    }
}
