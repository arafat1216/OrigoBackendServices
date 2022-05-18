using Common.Model;

namespace SubscriptionManagementServices.Exceptions
{
    [Serializable]
    public class SubscriptionManagementException : Exception
    {
        readonly ErrorResponse _errorResponse;

        public SubscriptionManagementException(string message, Guid traceId, Common.Enums.OrigoErrorCodes errorCode, Exception? innerException) : base(message, innerException)
        {
            var subscriptionManagementInnerException = innerException as SubscriptionManagementException;
            List<ErrorResponseDetail> innerExceptionErrorResponseDetails = new();
            if (subscriptionManagementInnerException is not null)
            {
                innerExceptionErrorResponseDetails = subscriptionManagementInnerException._errorResponse.Details.ToList();
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

        public ErrorResponse ErrorResponse => _errorResponse;
    }
}