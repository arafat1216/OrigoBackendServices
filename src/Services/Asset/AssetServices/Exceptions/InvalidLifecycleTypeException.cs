using Common.Enums;
using System;

namespace AssetServices.Exceptions
{
    public class InvalidLifecycleTypeException : AssetException
    {
        public InvalidLifecycleTypeException(string message, Guid traceId, Exception? innerException = null) : base($"Invalid invalidLifecycle type, {message}", traceId, OrigoErrorCodes.InvalidLifecycleType, innerException)
        {
        }
    }
}
