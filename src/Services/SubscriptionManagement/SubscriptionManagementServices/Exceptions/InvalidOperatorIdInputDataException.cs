using Common.Enums;

namespace SubscriptionManagementServices.Exceptions;

[Serializable]
public class InvalidOperatorIdInputDataException : SubscriptionManagementException
{
    public InvalidOperatorIdInputDataException(int operatorId, Guid trackingId, Exception? innerException = null) :
        base($"No operator with OperatorId {operatorId} found.", trackingId, OrigoErrorCodes.InvalidOperatorId,
            innerException)
    {
    }
}