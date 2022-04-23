using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace AssetServices;

public static class VATConfiguration
{
    public static decimal Amount { get; private set; }

    public static void Initialize(IConfiguration configuration)
    {
        var value = configuration.GetSection("VAT:Amount").Value;
        var culture = new CultureInfo("en-US");
        Amount = decimal.Parse(value, culture);
    }
}