using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class InactiveDeviceRequestException : Exception
    {
        public InactiveDeviceRequestException(ILogger logger)
        {
            logger.LogError(this, "Error in exception");
        }
        public InactiveDeviceRequestException(string message) : base(message)
        {
        }
        public InactiveDeviceRequestException(string message, ILogger logger) : base(message)
        {
            logger.LogError(this, message);
        }
    }
}
