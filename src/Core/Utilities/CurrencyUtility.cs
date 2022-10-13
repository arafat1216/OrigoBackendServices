using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class CurrencyUtility
    {
        /// <summary>
        /// Validate currency code (ISO-4217)
        /// </summary>
        /// <param name="currencyCode">Upper case</param>
        /// <returns>Currency code is valid or not</returns>
        public static bool Validate(string currencyCode)
        {
            
            return CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(ct => new RegionInfo(ct.LCID))
                .Any(ri => ri.ISOCurrencySymbol == currencyCode);
        }


        /// <summary>
        /// Get currency code by region/country code 
        /// </summary>
        /// <param name="regionCode">Region/Country code (ISO-3166)</param>
        /// <returns>Currency code(ISO-4217), return empty if region/country code is invalid</returns>
        public static string GetCurrencyCode(string regionCode)
        {
            try
            {
                RegionInfo ri = new RegionInfo(regionCode);
                return ri.ISOCurrencySymbol;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
