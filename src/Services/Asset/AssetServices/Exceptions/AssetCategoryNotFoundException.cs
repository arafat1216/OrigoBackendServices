using Common.Enums;
using System;

namespace AssetServices.Exceptions
{
    public class AssetCategoryNotFoundException : AssetException
    {
        public AssetCategoryNotFoundException(string categoryNotFound, Guid traceId,
        Exception? innerException = null) : base($"{categoryNotFound}", traceId, OrigoErrorCodes.AssetCategoryNotFound, innerException)
        {
        }
    }
}
