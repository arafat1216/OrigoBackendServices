using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HardwareServiceOrder.API
{
    /// <summary>
    ///     Adds HTTP-header parameters to all Swagger API endpoints.
    /// </summary>
    public class SwaggerHeaderParameters : IOperationFilter
    {

        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            // Add the "X-Authenticated-User" to all endpoints in Swagger.
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "X-Authenticated-User",
                In = ParameterLocation.Header,
                Required = false,
                Description = "The userID of the user that performed the request.",
                Schema = new OpenApiSchema() { Type = "string", Format = "uuid" }
            });
        }
    }
}