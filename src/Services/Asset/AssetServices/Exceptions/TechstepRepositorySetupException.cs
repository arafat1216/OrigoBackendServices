using Common.Enums;
using System;

namespace AssetServices.Exceptions
{
    public class TechstepRepositorySetupException : AssetException
    {
        public TechstepRepositorySetupException(string message, Guid traceId, Exception? innerException = null) : base(message, traceId, OrigoErrorCodes.ReturnDeviceError, innerException)
    {
    }
}
}
