using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;

namespace AssetServices
{
    public static class VATConfiguration
    {
        public static decimal Amount;
        public static void Initialize(IConfiguration Configuration)
        {
            var value = Configuration.GetSection("VAT:Amount").Value;
            var culture = new CultureInfo("en-US");
            Amount = decimal.Parse(value, culture);
             
        }
    }
}
