namespace HardwareServiceOrderServices.Models
{
    public enum ServiceTypeEnum
    {
        /// <summary>
        ///     A <see langword="null"/> equivalent enum-value. This indicates a missing value or a bad/default value-mapping.
        ///     This value should always be treated as an error, as it should not never actually be assigned.
        /// </summary>
        Null = 0,

        /// <summary>
        ///     Not yet implemented.
        /// </summary>
        Recycle = 1,

        /// <summary>
        ///     Not yet implemented.
        /// </summary>
        Remarketing = 2,

        /// <summary>
        ///     Same Unit Repair. A repair is done on the device that is sent in.
        /// </summary>
        SUR = 3,


        SWAP = 4,
    }
}
