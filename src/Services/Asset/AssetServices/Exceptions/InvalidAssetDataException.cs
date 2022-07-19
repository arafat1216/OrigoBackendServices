using Common.Enums;
using System;

namespace AssetServices.Exceptions
{
    public class InvalidAssetDataException : AssetException
    {
        public InvalidAssetDataException(string invalidData, Guid traceId, Exception? innerException = null) : base($"{invalidData}", traceId, OrigoErrorCodes.AssetInvalidData, innerException)
        {
        }
    }
}
