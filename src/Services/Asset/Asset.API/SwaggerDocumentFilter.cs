using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Asset.API
{
    /// <summary>
    /// Add support for url schema http/https
    /// Taken from https://stackoverflow.com/a/65308149
    /// </summary>
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        private readonly string swaggerDocHost;

        public SwaggerDocumentFilter(IHttpContextAccessor httpContextAccessor)
        {
            var host = httpContextAccessor.HttpContext.Request.Host.Value;
            var scheme = httpContextAccessor.HttpContext.Request.Scheme;
            swaggerDocHost = $"{scheme}://{host}";
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Servers.Add(new OpenApiServer { Url = swaggerDocHost });
        }
    }
}