using System;
using System.Runtime.Serialization;

namespace CustomerServices.Exceptions
{
    [Serializable]
    public class UserNameDoNotExistException : Exception
    {
        public UserNameDoNotExistException()
        {
        }

        public UserNameDoNotExistException(string message) : base(message)
        {
        }

        public UserNameDoNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNameDoNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
