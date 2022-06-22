using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HardwareServiceOrder.API.Extensions
{
    /// <summary>
    ///     Adds support for the <see cref="DateOnly"/> and <see cref="TimeOnly"/> data-types in the Swagger UI. <para>
    ///     
    ///     When the extension is registered with Swagger, these data-types will correctly be presented as '<c>string</c>'
    ///     values instead of a complex object, with the correct <c>format</c> specification also added to the OpenAPI-schema
    ///     (either '<c>date</c>' or '<c>time</c>'). </para>
    /// </summary>
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
