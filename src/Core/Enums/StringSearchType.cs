namespace Common.Enums
{
    /// <summary>
    ///     Contains identifiers for the various search-strategies.
    /// </summary>
    public enum StringSearchType
    {
        /// <summary>
        /// The result must be an exact match.
        /// </summary>
        Equals = 1,

        /// <summary>
        /// The result must start with the value.
        /// </summary>
        StartsWith = 2,

        /// <summary>
        /// The result must contain the value.
        /// </summary>
        Contains = 3
    }
}

