using System;
using System.Runtime.Serialization;

namespace CustomerServices.Exceptions
{
    [Serializable]
    public class UserNameIsInUseException : Exception
    {
        public UserNameIsInUseException()
        {
        }

        public UserNameIsInUseException(string message) : base(message)
        {
        }

        public UserNameIsInUseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNameIsInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
