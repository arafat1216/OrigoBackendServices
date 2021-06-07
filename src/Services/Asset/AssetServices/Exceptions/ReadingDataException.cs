using System;
using System.Runtime.Serialization;

namespace AssetServices.Exceptions
{
    [Serializable]
    internal class ReadingDataException : Exception
    {
        private Exception exception;

        public ReadingDataException()
        {
        }

        public ReadingDataException(Exception exception)
        {
            this.exception = exception;
        }

        public ReadingDataException(string message) : base(message)
        {
        }

        public ReadingDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReadingDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}