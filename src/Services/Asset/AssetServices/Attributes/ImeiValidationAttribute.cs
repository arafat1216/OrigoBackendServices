using AssetServices.Models;
using AssetServices.Utility;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.Attributes
{
    public class ImeiValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var inputValue = value as HardwareSuperType;
            var isValid = false;

            if (AssetValidatorUtility.ValidateImeis(string.Join(',', inputValue.Imeis)))
            {
                isValid = true;
            }

            return isValid;
        }
    }
}
