using Microsoft.AspNetCore.Http;

namespace HardwareServiceOrderServices.Infrastructure
{
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
                    throw new Exception("The header 'X-Authenticated-User' is missing.");
                }
            }
        }


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