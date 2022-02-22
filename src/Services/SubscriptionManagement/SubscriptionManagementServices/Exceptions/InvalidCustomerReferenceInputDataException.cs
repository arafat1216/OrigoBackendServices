using System.Runtime.Serialization;

namespace SubscriptionManagementServices.Exceptions
{
    [Serializable]
    public class InvalidCustomerReferenceInputDataException : SubscriptionManagementException
    {

        public InvalidCustomerReferenceInputDataException()
        {
        }

        public InvalidCustomerReferenceInputDataException(Exception exception)
        {
        }

        public InvalidCustomerReferenceInputDataException(string message) : base(message)
        {
        }

        public InvalidCustomerReferenceInputDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidCustomerReferenceInputDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}