using System;

namespace Common.Exceptions
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
    }
}

