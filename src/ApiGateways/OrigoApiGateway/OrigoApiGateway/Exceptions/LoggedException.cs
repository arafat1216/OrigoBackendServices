using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Serialization;

namespace OrigoApiGateway.Exceptions
{
    public class LoggedException : Exception
    {
        public LoggedException(ILogger logger)
        {
            logger.LogError(this, "Error in exception");
        }

        public LoggedException(string message, ILogger logger) : base(message)
        {
            logger.LogError(this, message);
        }

        public LoggedException(string message, Exception innerException, ILogger logger) : base(message, innerException)
        {
            logger.LogError(this, message);
        }

        protected LoggedException(SerializationInfo info, StreamingContext context, ILogger logger) : base(info, context)
        {
            logger.LogError(this, info.ToString());
        }
    }
}
