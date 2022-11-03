#nullable enable

namespace AssetServices.Models
{
    public enum StringSearchType
    {
        /// <summary>
        /// The result should be an exact match.
        /// </summary>
        Equals = 1,

        /// <summary>
        /// The result should start with the value.
        /// </summary>
        StartsWith = 2,

        /// <summary>
        /// The result should contain the value.
        /// </summary>
        Contains = 3
    }

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

