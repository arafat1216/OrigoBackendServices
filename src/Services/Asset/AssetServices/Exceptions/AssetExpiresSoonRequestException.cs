using Common.Enums;
using System;

namespace AssetServices.Exceptions
{
    public class AssetExpiresSoonRequestException : AssetException
    {
        public AssetExpiresSoonRequestException(string message, Guid traceId, Exception? innerException = null) : base(message, traceId, OrigoErrorCodes.ExpiresSoonError, innerException)
        {
        }
    }
}
