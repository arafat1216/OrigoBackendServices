using Common.Enums;

namespace SubscriptionManagementServices.Exceptions;

public class InvalidPhoneNumberException : SubscriptionManagementException
{
    public InvalidPhoneNumberException(string mobileNumber, string country, Guid trackingId,
        Exception? innerException = null) : base(
        $"Phone number {mobileNumber} not valid for countrycode {country}.", trackingId,
        OrigoErrorCodes.InvalidPhoneNumber, innerException)
    {
    }
}