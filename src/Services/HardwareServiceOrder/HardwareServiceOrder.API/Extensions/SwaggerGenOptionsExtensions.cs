using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HardwareServiceOrder.API.Extensions
{
    public static class SwaggerGenOptionsExtensions
    {
        /// <summary>
        ///     Maps <see cref="DateOnly"/> to string.
        /// </summary>
        public static void UseDateOnlyStringConverters(this SwaggerGenOptions options)
        {
            options.MapType<DateOnly>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "date"
            });
        }

        /// <summary>
        ///     Maps <see cref="TimeOnly"/> to string.
        /// </summary>
        public static void UseTimeOnlyStringConverters(this SwaggerGenOptions options)
        {
            options.MapType<TimeOnly>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "time",
                Example = OpenApiAnyFactory.CreateFromJson("\"13:45:42.0000000\"")
            });
        }
    }
}
