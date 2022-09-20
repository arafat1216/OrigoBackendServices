using Microsoft.Extensions.Logging;
using System;

namespace OrigoApiGateway.Exceptions
{
    public class PendingBuyoutException : Exception
    {
        public PendingBuyoutException(ILogger logger)
        {
            logger.LogError(this, "Error in exception");
        }

        public PendingBuyoutException(string message, ILogger logger) : base(message)
        {
            logger.LogError(this, message);
        }
    }
}
