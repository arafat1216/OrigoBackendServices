using System.Net;

namespace Common.Model
{
    /// <summary>
    /// The error object used to communicate information about
    /// the type of error which has happened across microservice boundaries.
    /// </summary>
    public class ErrorResponse
    {
        private readonly List<ErrorResponseDetail> _details = new();
        /// <summary>
        /// Type of error according to http status code list.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
        /// <summary>
        ///  An overall description of the error.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Tracing information.
        /// </summary>
        public Guid TraceId { get; set; }
        /// <summary>
        /// Detailed descriptions of one or more errors.
        /// </summary>
        public IReadOnlyCollection<ErrorResponseDetail> Details => _details.AsReadOnly();

        public void AddErrorResponseDetails(IList<ErrorResponseDetail> errorResponseDetails)
        {
            _details.AddRange(errorResponseDetails);
        }
    }
}