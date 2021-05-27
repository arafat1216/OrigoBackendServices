using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Serialization;

namespace OrigoApiGateway.Exceptions
{
    public class AssetConnectionException : LoggedException
    {
        public AssetConnectionException(ILogger logger) : base(logger)
        {
        }

        public AssetConnectionException(string message, ILogger logger) : base(message, logger)
        {
        }

        public AssetConnectionException(string message, Exception innerException, ILogger logger) : base(message, innerException, logger)
        {
        }

        protected AssetConnectionException(SerializationInfo info, StreamingContext context, ILogger logger) : base(info, context, logger)
        {
        }
    }
}
