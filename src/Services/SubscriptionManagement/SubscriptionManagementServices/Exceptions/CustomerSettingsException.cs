using Common.Enums;

namespace SubscriptionManagementServices.Exceptions;

public class CustomerSettingsException : SubscriptionManagementException
{
    public CustomerSettingsException(string message, Guid trackingId, Exception? innerException = null) : base(message,
        trackingId, OrigoErrorCodes.CustomerSettingsError, innerException)
    {
    }
}