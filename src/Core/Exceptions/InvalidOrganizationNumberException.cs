using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    public class InvalidOrganizationNumberException : Exception
    {
        public InvalidOrganizationNumberException()
        {
        }

        public InvalidOrganizationNumberException(string message) : base(message)
        {
        }

        public InvalidOrganizationNumberException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidOrganizationNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
