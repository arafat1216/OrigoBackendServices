

namespace SubscriptionManagementServices.Exceptions
{
    public class InvalidPhoneNumberException : SubscriptionManagementException
    {
        public InvalidPhoneNumberException()
        {
        }

        public InvalidPhoneNumberException(string message) : base(message)
        {
        }

        public InvalidPhoneNumberException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

