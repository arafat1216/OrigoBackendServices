using Common.Enums;

namespace SubscriptionManagementServices.Exceptions;

public class InvalidSimException : SubscriptionManagementException
{
    public InvalidSimException(string message, Guid trackingId, Exception? innerException = null) : base(message,
        trackingId, OrigoErrorCodes.InvalidSimCard, innerException)
    {
    }
}