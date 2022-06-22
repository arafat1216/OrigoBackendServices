using System.Text.RegularExpressions;

namespace HardwareServiceOrderServices.Attributes
{
    /// <summary>
    ///     Applies IMEI validation to a <see cref="string"/> or <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <remarks>
    ///     This only checks if existing values are valid. <br/>
    ///       - To prevent <see langword="null"/> values, use <see cref="RequiredAttribute"/> and/or nullable-notations. <br/>
    ///       - Use <see cref="MinLengthAttribute"/> to enforce the number of items in a <see cref="IEnumerable{T}"/>.
    /// </remarks>
    public class ImeiAttribute : ValidationAttribute
    {
        private static readonly string _imeiRegexPattern = "^[0-9]{14,15}$";


        /// <inheritdoc/>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // No need to validate nulls. This should be done using the [Required] attribute and/or nullable annotations.
            if (value is null)
                return ValidationResult.Success;
            // IEnumerable (JS arrays)
            else if (value is IEnumerable<string>)
                return ValidateValue((IEnumerable<string>)value);
            // Good ol' strings
            else if (value is string)
                return ValidateValue((string)value);

            // If it don't fall into one of the above, it's likely wrongly tagged, so we just skip the check and return success.
            return ValidationResult.Success;
        }


        /// <summary>
        ///     Validate single-value string inputs.
        /// </summary>
        /// <param name="value"> The input to be validated. </param>
        /// <returns> The validation result. </returns>
        private static ValidationResult? ValidateValue(string value)
        {
            if (IsValidImei(value))
                return new ValidationResult($"The value '{value}' is not a valid IMEI number.");

            return ValidationResult.Success;
        }

        /// <summary>
        ///     Validate the value of all items contained in a IEnumerable.
        /// </summary>
        /// <param name="value"> The input to be validated. </param>
        /// <returns> The validation result. </returns>
        private static ValidationResult? ValidateValue(IEnumerable<string> value)
        {
            // Ensure all items in the enumerator are valid
            foreach (var listItem in value)
            {
                if (!IsValidImei(listItem))
                    return new ValidationResult($"The value '{listItem}' is not a valid IMEI number.");
            }

            return ValidationResult.Success;
        }


        /// <summary>
        ///     Checks if a given string contains a valid IMEI-number.
        /// </summary>
        /// <param name="imei"> The content that should be validated. </param>
        /// <returns> Returns <see langword="true"/> if the string contains a valid IMEI number. 
        ///     Otherwise it returns <see langword="false"/>. </returns>
        private static bool IsValidImei(in string imei)
        {
            // TODO: Extend this to also check the value/validate the IMEI using the Luhn-algorithm.
            return Regex.IsMatch(imei, _imeiRegexPattern);
        }

    }
}
