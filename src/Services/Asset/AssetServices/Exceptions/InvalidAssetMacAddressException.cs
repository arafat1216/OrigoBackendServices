using Common.Enums;
using System;

namespace AssetServices.Exceptions
{
    public class InvalidAssetMacAddressException : AssetException
    {
        public InvalidAssetMacAddressException(string macAddress, Guid traceId, Exception? innerException = null) : base($"Invalid mac address {macAddress}", traceId, OrigoErrorCodes.InvalidMacAddress, innerException)
        {
        }
    }
}
