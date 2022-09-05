using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Services;

namespace OrigoApiGateway.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class ApiKeyAuthentication : Attribute, IAsyncActionFilter
    {

        private const string ApiKey = "XXX-AUTHENTICATED-KEY";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKey, out var apiKeyVal))
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api Key was not provided"
                };
                return;
            }

            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>("TechstepCoreWebhook:ApiKey");
            if (!apiKey.Equals(apiKeyVal))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api Key is not valid"
                };
                return;
            }
            await next();
        }
    }
}
