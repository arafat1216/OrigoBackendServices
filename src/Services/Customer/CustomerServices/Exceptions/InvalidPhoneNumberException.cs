using System;
using System.Runtime.Serialization;


namespace CustomerServices.Exceptions
{
    public class InvalidPhoneNumberException : Exception
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

        protected InvalidPhoneNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
