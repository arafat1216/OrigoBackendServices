namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Provides information about who is the recipient in a address, so the solution can auto-resolve what values should
    ///     be used when filling in various shipping information.
    /// </summary>
    public enum RecipientTypeEnum
    {
        /// <summary>
        ///     This is used to catch default-value inputs, and should never be used for anything but error checking!
        /// </summary>
        [Obsolete("This is only used to prevent/catch default values. If detected, this value will be considered as an error/invalid input.")]
        Null = 0,

        /// <summary>
        ///     The delivery is to a customer/organization address.
        /// </summary>
        Organization = 1,

        /// <summary>
        ///     The delivery is for a user's personal/private/home address.
        /// </summary>
        Personal = 2
    }
}
