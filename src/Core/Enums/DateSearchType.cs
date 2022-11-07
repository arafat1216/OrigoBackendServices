namespace Common.Enums
{
    /// <summary>
    ///     Contains identifiers for the various search-strategies.
    /// </summary>
    public enum DateSearchType
    {
        /// <summary>
        /// The results is on the exact date.
        /// </summary>
        ExcactDate = 1,

        /// <summary>
        /// The result is on, or before the selected date.
        /// </summary>
        OnOrAfterDate = 2,

        /// <summary>
        /// The result is on, or after the selected date.
        /// </summary>
        OnOrBeforeDate = 3,
    }
}

