using Common.Enums;

namespace SubscriptionManagementServices.Exceptions;

public class CustomerReferenceFieldMissingException : SubscriptionManagementException
{
    public CustomerReferenceFieldMissingException(string missingFieldName, Guid trackingId,
        Exception? innerException = null) : base($"The field name {missingFieldName} is not valid for this customer.",
        trackingId, OrigoErrorCodes.CustomerReferenceFieldMissing, innerException)
    {
    }
}