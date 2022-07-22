using Common.Enums;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Utilities
{
    /// <summary>
    /// Will add the header for the authenticated user to be used by swagger.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AuthenticatedUserHeaderFilter : IOperationFilter
    {
        /// <summary>
        /// Called by swagger to set up extra parameter.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var httpMethod = context.ApiDescription.HttpMethod;
            if (httpMethod == "GET") return;

            operation.Parameters ??= new List<OpenApiParameter>();
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = HeaderNames.AuthenticatedUser,
                In = ParameterLocation.Header,
                Required = true,
                Description = "uuid for authenticated user"
            });
        }
    }
}
