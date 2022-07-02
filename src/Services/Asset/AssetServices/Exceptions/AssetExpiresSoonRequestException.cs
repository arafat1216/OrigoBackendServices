using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class AssetExpiresSoonRequestException : Exception
    {
        public AssetExpiresSoonRequestException(ILogger logger)
        {
            logger.LogError(this, "Error in exception");
        }
        public AssetExpiresSoonRequestException(string messages) : base(messages)
        {
        }

        public AssetExpiresSoonRequestException(string message, ILogger logger) : base(message)
        {
            logger.LogError(this, message);
        }
    }
}
