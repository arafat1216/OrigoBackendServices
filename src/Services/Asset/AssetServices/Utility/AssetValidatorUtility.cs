using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetServices.Utility
{
    public static class AssetValidatorUtility
    {

        /// <summary>
        /// Validate a string of imeis (csv format).
        /// </summary>
        /// <param name="imeis"></param>
        /// <returns></returns>
        public static bool ValidateImeis(string imeis)
        {
            foreach (string e in imeis.Split(','))
            {
                if (!ValidateImei(e))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validate an imei using Luhn's algorithm
        /// https://stackoverflow.com/questions/25229648/is-it-possible-to-validate-imei-number/25229800#25229800
        /// </summary>
        /// <param name="imei"></param>
        /// <returns></returns>
        public static bool ValidateImei(string imei)
        {
            //validate length
            if (string.IsNullOrEmpty(imei))
                return false;


            // is not a number
            if (!long.TryParse(imei, out _))
                return false;


            int sumDigits = 0;
            int diffValue = 0;
            int sumRounded = 0;

            // temp value
            int d = 0;

            // 1. Get validation number (last number of imei)
            int validationDigit = int.Parse(imei[imei.Length - 1].ToString());


            // 2. Double values in every second value of remaining imei
            imei = imei.Substring(0, imei.Length - 1);
            for (int i = imei.Length - 1; i >= 0; i--)
            {
                string currentNumber = imei.Substring(i, 1);

                d = Convert.ToInt32(currentNumber);
                if (i % 2 != 0)
                {
                    // Double value and add it: 9 -> 18: value to add is 1+8, not 18.
                    d = d * 2;
                    string dString = Convert.ToString(d);
                    d = 0;
                    foreach (char e in dString)
                    {
                        d = d + int.Parse(e.ToString());
                    }
                }
                sumDigits += d;
            }

            // Round up to neares multiplicative of 10: 52 -> 60
            sumRounded = (int)Math.Ceiling(((double)sumDigits / 10)) * 10;

            diffValue = sumRounded - sumDigits;

            return diffValue == validationDigit;
        }

        public static List<long> MakeUniqueIMEIList(IList<long> imei)
        {
            return imei.Distinct().ToList();
        }
    }
}
