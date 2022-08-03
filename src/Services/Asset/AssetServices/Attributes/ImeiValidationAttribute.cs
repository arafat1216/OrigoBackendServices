using AssetServices.Models;
using AssetServices.Utility;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AssetServices.Attributes;

public class ImeiValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return false;
        }
        var inputValue = value as HardwareAsset;
        return inputValue != null && AssetValidatorUtility.ValidateImeis(string.Join(',', inputValue.Imeis.Select(i=>i.Imei)));
    }
}