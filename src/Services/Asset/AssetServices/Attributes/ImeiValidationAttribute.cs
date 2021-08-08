using AssetServices.Models;
using AssetServices.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
