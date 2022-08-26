using Common.Model;

namespace HardwareServiceOrderServices.Exceptions
{
    [Serializable]
    public class HardwareServiceOrderException : Exception
    {
        readonly ErrorResponse _errorResponse;
        public ErrorResponse ErrorResponse => _errorResponse;
        public HardwareServiceOrderException(string message, Guid traceId, Common.Enums.OrigoErrorCodes errorCode, Exception? innerException) : base(message, innerException)
        {
            var assetInnerException = innerException as HardwareServiceOrderException;
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
