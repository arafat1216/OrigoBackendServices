using System;
using System.Runtime.Serialization;

namespace CustomerServices.Exceptions
{
    [Serializable]
    public class UserNameDoesNotExistException : Exception
    {
        public UserNameDoesNotExistException()
        {
        }

        public UserNameDoesNotExistException(string message) : base(message)
        {
        }

        public UserNameDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNameDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
