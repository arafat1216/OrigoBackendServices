using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class AssetExpireRequestException : Exception
    {
        public AssetExpireRequestException(ILogger logger)
        {
            logger.LogError(this, "Error in exception");
        }
        public AssetExpireRequestException(string messages) : base(messages)
        {
        }

        public AssetExpireRequestException(string message, ILogger logger) : base(message)
        {
            logger.LogError(this, message);
        }
    }
}
