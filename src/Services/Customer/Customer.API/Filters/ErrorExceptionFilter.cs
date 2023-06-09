﻿using CustomerServices.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Customer.API.Filters
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
            _logger.LogError(exception.Message, exception);

            if (exception is ArgumentException)
            {
                context.Result = new BadRequestObjectResult(exception.Message);
                return;
            }

            if (exception is DuplicateException)
            {
                context.Result = new ConflictObjectResult(exception.Message);
                return;
            }
            

            if (exception is CustomerNotFoundException || exception is UserNotFoundException)
            {
                context.Result = new NotFoundObjectResult(exception.Message);
                return;
            }

            var errorMessage = _hostEnvironment.IsDevelopment() ?
                exception.StackTrace : "Unexpected error occurred. Please try again.";

            context.HttpContext.Response.StatusCode = 500;

            context.Result = new JsonResult(errorMessage);

            context.ExceptionHandled = true;
        }
    }
}
