using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(ILogger logger)
        {
            logger.LogError(this, "Error in exception");
        }

        public ResourceNotFoundException(string message, ILogger logger) : base(message)
        {
            logger.LogError(this, message);
        }
    }
}
