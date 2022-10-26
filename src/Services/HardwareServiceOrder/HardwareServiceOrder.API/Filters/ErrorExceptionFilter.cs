using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using HardwareServiceOrderServices.Exceptions;

namespace HardwareServiceOrder.API.Filters
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

            if (exception is NotImplementedException)
            {
                context.Result = new BadRequestObjectResult(exception.Message);
                return;
            }

            if (exception is NotFoundException notFoundException)
            {
                context.Result = new NotFoundObjectResult(notFoundException.Message);
                return;
            }

            if(exception is EmailException emailException)
            {
                context.Result = new BadRequestObjectResult(exception.Message);
                return;
            }

            if (exception is HttpHeaderException httpHeaderException)
            {
                context.Result = new BadRequestObjectResult(httpHeaderException.Message);
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