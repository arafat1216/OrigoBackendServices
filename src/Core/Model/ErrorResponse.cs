using System.Collections.Generic;
using System.Net;

namespace Common.Model
{
    /// <summary>
    /// The error object used to communicate information about
    /// the type of error which has happened across microservice boundaries.
    /// </summary>
    public class ErrorResponse
    {
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
        public string TraceId { get; set; }
        /// <summary>
        /// Detailed descriptions of one or more errors.
        /// </summary>
        public IReadOnlyCollection<ErrorResponseDetail> Details { get; set; }
    }
}