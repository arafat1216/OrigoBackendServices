namespace HardwareServiceOrderServices.Models
{
    public enum ServiceTypeEnum
    {
        /// <summary>
        ///     A <see langword="null"/> equivalent enum-value. This indicates a missing value or a bad/default value-mapping.
        ///     This value should always be treated as an error, as it should not never actually be assigned.
        /// </summary>
        Null = 0,

        Recycle = 1,

        Remarketing = 2,

        SUR = 3,

        SWAP = 4,
    }
}
