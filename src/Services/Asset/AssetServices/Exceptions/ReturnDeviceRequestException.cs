using Common.Enums;
using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class ReturnDeviceRequestException : AssetException
    {
        public ReturnDeviceRequestException(string message, Guid traceId, Exception? innerException = null) : base(message, traceId, OrigoErrorCodes.ReturnDeviceError, innerException)
    {
    }
}
}
