using AssetServices.Models;
using AssetServices.Utility;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.Attributes
{
    public class ImeiValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var inputValue = value as Asset;
            var isValid = false;

            if (AssetValidatorUtility.ValidateImeis(inputValue.Imei))
            {
                isValid = true;
            }

            return isValid;
        }
    }
}
