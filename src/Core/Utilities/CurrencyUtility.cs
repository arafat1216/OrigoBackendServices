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
        /// <returns></returns>
        public static bool Validate(string currencyCode)
        {
            
            return CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .Select(ct => new RegionInfo(ct.LCID))
                .Any(ri => ri.ISOCurrencySymbol == currencyCode);
        }


        /// <summary>
        /// Get currency code by region/country code (ISO-3166)
        /// </summary>
        /// <param name="regionCode"></param>
        /// <returns></returns>
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
