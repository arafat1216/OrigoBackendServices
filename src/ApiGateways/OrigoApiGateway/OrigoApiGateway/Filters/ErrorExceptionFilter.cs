﻿using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Exceptions;
using System;
using System.Net;

namespace OrigoApiGateway.Filters
{
    public class ErrorExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ErrorExceptionFilter> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        public ErrorExceptionFilter(ILogger<ErrorExceptionFilter> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            _logger.LogError(exception, $"{exception.Message}");

            if (exception is ArgumentException or SubscriptionManagementException or ProductCatalogException or AssetException)
            {
                context.Result = new BadRequestObjectResult(exception.Message);
                
                return;
            }

            if(exception is ResourceNotFoundException)
            {
                context.Result = new NotFoundObjectResult(exception.Message);
                return;
            }

            if(exception is UnauthorizedAccessException)
            {
                context.Result = new UnauthorizedObjectResult(exception.Message);
                return;
            }

            if(exception is Azure.RequestFailedException or BadHttpRequestException or NotSupportedException)
            {
                context.Result = new BadRequestObjectResult(exception.Message);
                return;
            }

            if (exception is HttpRequestException ex)
            {
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        {
                            context.Result = new NotFoundObjectResult(exception.Message);
                            return;
                        }
                    default:
                        {
                            context.Result = new BadRequestObjectResult(exception.Message);
                            return;
                        }
                }
            }

            if (exception is InvalidOrganizationNumberException)
            {
                context.Result = new ConflictObjectResult(exception.Message);
                return;
            }

            if (exception is MicroserviceErrorResponseException mexception)
            {
                if (mexception.StatusCode.HasValue && mexception.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    _logger.LogError(exception,
                            "Encountered an unexpected exception.\nUnique location ID: 731B4BE3-D358-4104-AAF7-96041BD07102.\nMessage:\n{0}",
                            exception.Message);

                // Try and parse the error code, and set it to 500 if it's null.
                int statusCode = mexception.StatusCode.HasValue ? (int)mexception.StatusCode : 500;

                // If the status code was 207 (Multi Status), or it was 300 or above, then it's been explicitly thrown by the
                // internal micro-service, meaning we can forward it to the frontend.
                // Important: Error 500 is excluded, since this is the error-code thrown by exceptions.
                if (statusCode == 207 || (statusCode != 500 && statusCode >= 300))
                {
                    context.Result = new ObjectResult(statusCode);
                    return;
                }
                else
                {
                    context.Result = new StatusCodeResult(500);
                    return;
                }
            }

            var errorMessage = _hostEnvironment.IsDevelopment() ?
                exception.StackTrace : "Unexpected error occurred. Please try again.";

            context.HttpContext.Response.StatusCode = 500;

            context.Result = new JsonResult(errorMessage);

            context.ExceptionHandled = true;
        }
    }
}
