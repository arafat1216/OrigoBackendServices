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

            // If the HttpMethod is POST, PUT, PATCH or DELETE, add the header-filter
            if (string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Post.Method, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Put.Method, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Patch.Method, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Delete.Method, StringComparison.InvariantCultureIgnoreCase)
            )
            {
                // Add the "X-Authenticated-UserId" to all related endpoints in Swagger.
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "X-Authenticated-UserId",
                    Description = "The <code>userID</code> of the user/caller that triggered the request.<br/><br/>" +

                                  "In supported endpoints, the <code>CreatedBy</code>, <code>UpdatedBy</code> and <code>DeletedBy</code> values is automatically handled, and uses this ID to record the user that made changes in the database.<br/><br/>" +

                                  "The following caller IDs exist and are reserved for special cases:<br/>" +
                                  "<ul>" +
                                    "<li><code>00000000-0000-0000-0000-000000000000</code> - the caller/user-ID is unknown.</li>" +
                                    "<li><code>00000000-0000-0000-0000-000000000001</code> - the system's caller/user-ID. Used for automated or M2M processes that's not initiated by a user.</li>" +
                                  "</ul>",
                    
                    In = ParameterLocation.Header,
                    Required = true,

                    Schema = new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "uuid",
                    },

                    // Add a example if we are in debug, as this indicates local environment, and it adds a pre-filled value the developer
                    // can use, making it easier to do testing.
#if DEBUG
                    //Example = new OpenApiString("00000000-0000-0000-0000-000000000002")
#endif
                });
            }
        }
    }
}