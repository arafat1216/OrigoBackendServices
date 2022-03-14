using Common.Enums;

namespace SubscriptionManagementServices.Exceptions;

[Serializable]
public class InvalidCustomerReferenceInputDataException : SubscriptionManagementException
{
    public InvalidCustomerReferenceInputDataException(string message, Guid trackingId, Exception? innerException = null)
        : base(message, trackingId, OrigoErrorCodes.CustomerReferenceFieldMissing, innerException)
    {
    }
}