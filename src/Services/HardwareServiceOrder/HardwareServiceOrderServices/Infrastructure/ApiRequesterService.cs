using Microsoft.AspNetCore.Http;

namespace HardwareServiceOrderServices.Infrastructure
{
    /// <summary>
    ///     The dependency-injected implementation of <see cref="IApiRequesterService"/>.
    /// </summary>
    public class ApiRequesterService : IApiRequesterService
    {
        /// <inheritdoc/>
        public Guid AuthenticatedUserId { get; private set; }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiRequesterService"/>.
        /// </summary>
        /// <param name="contextAccessor"> A dependency-injected <see cref="IHttpContextAccessor"/>. </param>
        /// <exception cref="Exception"> Thrown whenever a required request-header is missing or invalid. </exception>
        public ApiRequesterService(IHttpContextAccessor contextAccessor)
        {
            var _httpContext = contextAccessor.HttpContext;

            if (_httpContext is not null)
            {
                bool userIsParsed = _httpContext.Request.Headers.TryGetValue("X-Authenticated-User", out var userId);

                if (userIsParsed)
                {
                    SetAuthenticatedUser(userId);
                }
                else
                {
                    // TODO: This should eventually be added back in, but first we need to add the to the API gateway calls.
                    //throw new Exception("The header 'X-Authenticated-User' is missing.");
                }
            }
        }


        /// <summary>
        ///     Parses a provided header string-value, and sets the <see cref="AuthenticatedUserId"/>.
        /// </summary>
        /// <param name="userId"> The ID that should be parsed and set as the <see cref="AuthenticatedUserId"/>. </param>
        /// <exception cref="Exception"> Thrown when the <paramref name="userId"/> fails to be parsed (invalid value). </exception>
        private void SetAuthenticatedUser(string userId)
        {
            bool callerIdParsed = Guid.TryParse(userId, out Guid result);

            if (callerIdParsed)
                AuthenticatedUserId = result;
            else
                throw new Exception("The value in 'X-Authenticated-User' is not valid.");
        }



    }
}