using System;
using System.Runtime.Serialization;

namespace OrigoApiGateway.Exceptions
{
    [Serializable]
    public class AssetException : Exception
    {

        public AssetException()
        {
        }

        public AssetException(Exception exception)
        {
        }

        public AssetException(string message) : base(message)
        {
        }

        public AssetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AssetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
