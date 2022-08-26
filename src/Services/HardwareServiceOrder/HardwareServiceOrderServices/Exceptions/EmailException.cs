using Common.Enums;

namespace HardwareServiceOrderServices.Exceptions
{
    public class EmailException : HardwareServiceOrderException
    {
        public EmailException(string message, Guid traceId, Exception? innerException = null) : base(message, traceId, OrigoErrorCodes.EmailError, innerException)
        {
        }
    }
}
