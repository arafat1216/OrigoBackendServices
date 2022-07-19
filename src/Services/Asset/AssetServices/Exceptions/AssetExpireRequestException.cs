using Common.Enums;
using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class AssetExpireRequestException : AssetException
    {
        public AssetExpireRequestException(string message, Guid traceId, Exception? innerException = null) : base(message, traceId, OrigoErrorCodes.ExpiredError, innerException)
        {
        }

    }
}
