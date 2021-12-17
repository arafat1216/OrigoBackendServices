using System;
using System.Net;

namespace CustomerServices.Exceptions
{
    public class OktaException : Exception
    {
        /// <summary>
        ///     If availible, this represents the HTTP status code we recieved back from Okta before the exception occured.
        ///     This defaults to <see cref="HttpStatusCode.InternalServerError"/> if no the value is unavailible, or haven't been set.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.InternalServerError;


        /// <summary> Initializes a new instance of the <see cref="OktaException"/> class. </summary>
        /// <inheritdoc cref="Exception()"/>
        public OktaException()
        {
        }


        /// <summary> Initializes a new instance of the <see cref="OktaException"/> class with a specified error message. </summary>
        /// <inheritdoc cref="Exception(string)"/>
        public OktaException(string message) : base(message)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="OktaException"/> class with a specified error
        ///     message and a reference to the HTTP code for the extenal exception.
        /// </summary>
        /// 
        /// <param name="httpStatusCode">The HTTP status code attached to the response from Okta</param>
        /// 
        /// <inheritdoc cref="OktaException(string)"/>
        public OktaException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="OktaException"/> class with a specified error
        ///     message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// 
        /// <inheritdoc cref="Exception(string, Exception)"/>
        public OktaException(string message, Exception innerException) : base(message, innerException)
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="OktaException"/> class with a specified error message, the HTTP code 
        ///     for the extenal exception, and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// 
        /// <param name="httpStatusCode">The HTTP status code attached to the response from Okta</param>
        /// 
        /// <inheritdoc cref="Exception(string, Exception)"/>
        public OktaException(string message, Exception innerException, HttpStatusCode httpStatusCode) : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
