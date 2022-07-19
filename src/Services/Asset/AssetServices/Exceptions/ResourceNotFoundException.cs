using Common.Enums;
using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class ResourceNotFoundException : AssetException
    {
        public ResourceNotFoundException(string message, Guid traceId, Exception? innerException = null) : base(message, traceId, OrigoErrorCodes.ResourceNotFound, innerException)
        {
        }
    }
}
