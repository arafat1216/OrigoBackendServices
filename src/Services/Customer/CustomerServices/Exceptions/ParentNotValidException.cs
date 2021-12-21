using System;

namespace CustomerServices.Exceptions
{
    public class ParentNotValidException : Exception
    {
        public ParentNotValidException()
        {
        }

        public ParentNotValidException(string message) : base(message)
        {
        }

        public ParentNotValidException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
