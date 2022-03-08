using PhoneNumbers;

namespace Common.Utilities
{
    public static class PhoneNumberUtility
    {
        private static readonly PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

        public static bool ValidatePhoneNumber(string mobileNumber,string countryCode)
        {
            try 
            { 
                var upperCaseCountryCode = countryCode?.ToUpper();

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
