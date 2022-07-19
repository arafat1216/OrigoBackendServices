using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetServices.Exceptions
{
    [Serializable]
    public class AssetException : Exception
    {
        readonly ErrorResponse _errorResponse;
        public ErrorResponse ErrorResponse => _errorResponse;
        public AssetException(string message, Guid traceId, Common.Enums.OrigoErrorCodes errorCode, Exception? innerException) : base(message, innerException)
        {
            var assetInnerException = innerException as AssetException;
            List<ErrorResponseDetail> innerExceptionErrorResponseDetails = new();
            if (assetInnerException is not null)
            {
                innerExceptionErrorResponseDetails = assetInnerException._errorResponse.Details.ToList();
            }
            _errorResponse = new ErrorResponse
            {
                Message = message,
                HttpStatusCode = System.Net.HttpStatusCode.BadRequest,
                TraceId = Guid.NewGuid()
            };
            innerExceptionErrorResponseDetails.Add(new ErrorResponseDetail(errorCode, message, traceId));
            _errorResponse.AddErrorResponseDetails(innerExceptionErrorResponseDetails);
        }
    }
}
