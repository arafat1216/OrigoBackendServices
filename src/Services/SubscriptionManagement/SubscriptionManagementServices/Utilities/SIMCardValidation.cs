using System.Text.RegularExpressions;

namespace SubscriptionManagementServices.Utilities
{
    public class SIMCardValidation
    {
        
        public static bool ValidateSim(string simNumber)
        {
            //validate length
            if (string.IsNullOrEmpty(simNumber))
                return false;

            //remove wihtspace
            string simNumberTrimmed = Regex.Replace(simNumber, @"\s+", "");

            //Lenght between 18-22 digits
            if (simNumberTrimmed.Length < 18 || simNumberTrimmed.Length > 22)
                return false;

            //the first two digits are always 89
            string firstTwoDigits = simNumberTrimmed.Substring(0, 2);
            if (firstTwoDigits != "89") return false;


            return LuhnAlgorithm(simNumberTrimmed);
        }

        //https://stackoverflow.com/questions/67435345/correct-iccid-algo
        //AssetValidator Imei was not working because of odd number
        public static bool LuhnAlgorithm(string simNumber)
        {

            int sum = 0;
            // Use index i = 0 means the right-most digit, i = 1 is second-right, etc
            for (int i = 0; i < simNumber.Length; i++)
            {
                // Get the digit at the i'th position from the right
                int digit = int.Parse(simNumber[simNumber.Length - i - 1].ToString());

                // If it's in an odd position (starting from the right), then double it.
                if (i % 2 == 1)
                {
                    digit *= 2;

                    // If it's now >= 10, subtract 9
                    if (digit >= 10)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;
            }

            // It's a pass if the result is a multiple of 10
            return sum % 10 == 0;
        }
    }
}
