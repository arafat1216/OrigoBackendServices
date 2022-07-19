using Common.Enums;
using Microsoft.Extensions.Logging;
using System;

namespace AssetServices.Exceptions
{
    public class InvalidOperationForInactiveState :  AssetException
    {
        public InvalidOperationForInactiveState(string currentStatus,Guid assetId, Guid traceId, Exception? innerException = null) : base($"Asset is not in Active state, current status {currentStatus} for asset with id: {assetId}",
            traceId, OrigoErrorCodes.InvalidOperationForInactiveState, innerException)
        {
        }
    }
}
