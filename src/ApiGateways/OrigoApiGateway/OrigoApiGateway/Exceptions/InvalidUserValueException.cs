using System;
using System.Runtime.Serialization;

namespace OrigoApiGateway.Exceptions
{
    [Serializable]
    public class InvalidUserValueException : Exception
    {
        public InvalidUserValueException()
        {
        }

        public InvalidUserValueException(string message) : base(message)
        {
        }

        public InvalidUserValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidUserValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
