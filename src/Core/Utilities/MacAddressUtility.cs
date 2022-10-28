using System.Text.RegularExpressions;

namespace Common.Utilities
{
    /// <summary>
    /// A collection of utilities used for validating and working with MAC addresses.
    /// </summary>
    public static class MacAddressUtility
    {

        /// <summary>
        /// Determines if a string is a valid MAC address. This supports dashes or colons as separators.
        /// </summary>
        /// <param name="toBeValidated"> The string that should be validated. </param>
        /// <returns> Returns <see langword="true"/> if the string contains a valid MAC address. Otherwise it returns <see langword="false"/>. </returns>
        public static bool IsValid(string toBeValidated)
        {
            return Regex.IsMatch(toBeValidated, "^(?:[0-9A-Fa-f]{2}[:-]){5}(?:[0-9A-Fa-f]{2})$");
        }
    }
}
