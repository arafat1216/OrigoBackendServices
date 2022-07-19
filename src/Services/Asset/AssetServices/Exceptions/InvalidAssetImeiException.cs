using Common.Enums;
using System;

namespace AssetServices.Exceptions
{
    public class InvalidAssetImeiException : AssetException
    {
        public InvalidAssetImeiException(string message, Guid traceId, Exception? innerException = null) : base($"Invalid imei: {message}", traceId, OrigoErrorCodes.InvalidImei, innerException)
        {
        }
    }
}
