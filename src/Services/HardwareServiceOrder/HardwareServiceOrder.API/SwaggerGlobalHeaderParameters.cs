using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HardwareServiceOrder.API
{
    /// <summary>
    ///     Adds global HTTP-header parameters to all Swagger API endpoints.
    /// </summary>
    public class SwaggerGlobalHeaderParameters : IOperationFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            // If the HttpMethod is POST, PUT or PATCH, add the header-filter
            if (string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Post.Method, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Put.Method, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Patch.Method, StringComparison.InvariantCultureIgnoreCase)
            )
            {
                // Add the "X-Authenticated-UserId" to all related endpoints in Swagger.
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "X-Authenticated-UserId",
                    Description = "The <code>userID</code> of the user/caller that triggered the request.<br/><br/>In supported endpoints, the <code>CreatedBy</code>, <code>UpdatedBy</code> and <code>DeletedBy</code> values is automatically handled, and uses this ID when changes are made in the database.",
                    In = ParameterLocation.Header,
                    Required = false,

                    Schema = new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "uuid"
                    },

                    //Example = new OpenApiString("00000000-0000-0000-0000-000000000002")
                });
            }
        }
    }
}