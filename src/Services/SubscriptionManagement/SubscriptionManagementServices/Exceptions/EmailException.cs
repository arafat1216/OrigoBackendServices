using Common.Enums;

namespace SubscriptionManagementServices.Exceptions
{
    public class EmailException : SubscriptionManagementException
    {
        public EmailException(string message, Guid traceId, Exception? innerException = null) : base(message, traceId, OrigoErrorCodes.EmailError, innerException)
        {
        }
    }
}
