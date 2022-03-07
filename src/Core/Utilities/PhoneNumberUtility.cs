using PhoneNumbers;

namespace Common.Utilities
{
    public class PhoneNumberUtility
    {
        private static PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

        public static bool ValidatePhoneNumber(string mobileNumber,string countryCode)
        {
            try 
            { 
                var upperCaseCountryCode = countryCode != null ? countryCode.ToUpper() : null;

                if (countryCode != null && countryCode.Contains("nb"))
                {
                    upperCaseCountryCode = "NO";
                }

                PhoneNumber number = phoneNumberUtil.Parse(mobileNumber, upperCaseCountryCode);
                
            return phoneNumberUtil.IsValidNumberForRegion(number, upperCaseCountryCode);

            }
            catch (NumberParseException)
            {
                return false;
            }
        }

    }
}
