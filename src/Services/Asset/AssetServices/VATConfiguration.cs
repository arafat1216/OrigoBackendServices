using Microsoft.Extensions.Configuration;
using System;

namespace AssetServices
{
    public static class VATConfiguration
    {
        public static decimal Amount;
        public static void Initialize(IConfiguration Configuration)
        {
            Amount = Convert.ToDecimal(Configuration.GetSection("VAT:Amount").Value);
        }
    }
}
