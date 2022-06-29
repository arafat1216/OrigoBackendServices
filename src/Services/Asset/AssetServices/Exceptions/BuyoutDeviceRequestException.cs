using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class BuyoutDeviceRequestException : Exception
    {
        public BuyoutDeviceRequestException(ILogger logger)
        {
            logger.LogError(this, "Error in exception");
        }

        public BuyoutDeviceRequestException(string message, ILogger logger) : base(message)
        {
            logger.LogError(this, message);
        }
        public BuyoutDeviceRequestException(string message) : base(message)
        {
        }
    }
}
