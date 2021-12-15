using System;
using System.Net;
using System.Net.Http;

namespace OrigoApiGateway.Exceptions
{
    public class MicroserviceErrorResponseException : HttpRequestException
    {
        public MicroserviceErrorResponseException(string? message) : base(message)
        {
        }

        public MicroserviceErrorResponseException(string? message, Exception? inner) : base(message, inner)
        {
        }

        public MicroserviceErrorResponseException(string? message, HttpStatusCode? statusCode) : base(message, null, statusCode)
        {
        }

        public MicroserviceErrorResponseException(string? message, Exception? inner, HttpStatusCode? statusCode) : base(message, inner, statusCode)
        {
        }
    }
}
