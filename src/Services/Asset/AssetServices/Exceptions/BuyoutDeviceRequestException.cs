using Common.Enums;
using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class BuyoutDeviceRequestException : AssetException
    {
        public BuyoutDeviceRequestException(string message, Guid traceId, Exception? innerException = null) : base(message, traceId, OrigoErrorCodes.BuyoutDeviceError, innerException)
        {
        }
    }
}
