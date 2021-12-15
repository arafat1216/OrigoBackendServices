using AssetServices.Models;
using AssetServices.Utility;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AssetServices.Attributes
{
    public class ImeiValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var inputValue = value as HardwareAsset;
            var isValid = false;

            if (AssetValidatorUtility.ValidateImeis(string.Join(',', inputValue.Imeis.Select(i=>i.Imei))))
            {
                isValid = true;
            }

            return isValid;
        }
    }
}
