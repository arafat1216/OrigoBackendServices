using System.Runtime.Serialization;

namespace SubscriptionManagementServices.Exceptions
{

    [Serializable]
    internal class InvalidSimException : SubscriptionManagementException
    {
        public InvalidSimException()
        {
        }

        public InvalidSimException(Exception exception)
        {
        }

        public InvalidSimException(string message) : base(message)
        {
        }

        public InvalidSimException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSimException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
