using System;
using System.Runtime.Serialization;


namespace Common.Exceptions
{
        
        public class InvalidSimException : Exception
        {
            public InvalidSimException()
            {
            }

            public InvalidSimException(Exception exception)
            {
            }

            public InvalidSimException(string message) : base(message)
            {
            }

            public InvalidSimException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected InvalidSimException(SerializationInfo info, StreamingContext context) : base(info,
                context)
            {
            }
        }
    }

