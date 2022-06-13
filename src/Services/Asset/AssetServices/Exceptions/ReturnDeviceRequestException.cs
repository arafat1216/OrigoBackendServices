using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class ReturnDeviceRequestException : Exception
    {
        public ReturnDeviceRequestException(ILogger logger)
        {
            logger.LogError(this, "Error in exception");
        }

        public ReturnDeviceRequestException(string message, ILogger logger) : base(message)
        {
            logger.LogError(this, message);
        }
    }
}
