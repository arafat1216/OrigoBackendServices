using System;

namespace CustomerServices.Exceptions
{
    public class RequiredFieldIsEmptyException : Exception
    {
        public RequiredFieldIsEmptyException()
        {
        }

        public RequiredFieldIsEmptyException(string message) : base(message)
        {
        }

        public RequiredFieldIsEmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
