using Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Common.Infrastructure;

/// <summary>
///     A dependency-injected implementation of <see cref="IApiRequesterService" />.
/// </summary>
public class ApiRequesterService : IApiRequesterService
{
    /// <inheritdoc />
    public Guid AuthenticatedUserId { get; private set; }


    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiRequesterService" />.
    /// </summary>
    /// <param name="contextAccessor"> A dependency-injected <see cref="IHttpContextAccessor" />. </param>
    public ApiRequesterService(IHttpContextAccessor contextAccessor)
    {
        if (contextAccessor.HttpContext is not null)
        {
            CheckAuthenticatedUserHeader(contextAccessor.HttpContext.Request);
        }
    }

    /// <summary>
    ///     Ensures that the 'X-Authenticated-UserId' is present and valid on all HTTP-methods that performs write-operations.
    /// </summary>
    /// <param name="httpRequest"> The incoming HTTP request. </param>
    /// <exception cref="HttpHeaderException"> Thrown when the required header parameter is missing or invalid. </exception>
    private void CheckAuthenticatedUserHeader(in HttpRequest httpRequest)
    {
        // If we are able to parse and extract a value from the header: set the value.
        if (httpRequest.Headers.TryGetValue("X-Authenticated-UserId", out var userId))
        {
            SetAuthenticatedUser(userId);
        }
        else
        {
            // If the header-value was not set, and it's for a write operation (PUT/PATCH/POST methods), we should throw an exception as it's required
            // when automatically assigning 'CreatedBy', 'UpdatedBy', etc.
            if (string.Equals(httpRequest.Method, HttpMethod.Post.Method,
                    StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(httpRequest.Method, HttpMethod.Put.Method, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(httpRequest.Method, HttpMethod.Patch.Method, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new HttpHeaderException("X-Authenticated-UserId",
                    "The HTTP header 'X-Authenticated-UserId' is missing.");
            }
        }
    }


    /// <summary>
    ///     Parses a provided header string-value, and sets the <see cref="AuthenticatedUserId" />.
    /// </summary>
    /// <param name="userId"> The ID that should be parsed and set as the <see cref="AuthenticatedUserId" />. </param>
    /// <exception cref="Exception"> Thrown when the <paramref name="userId" /> fails to be parsed (invalid value). </exception>
    private void SetAuthenticatedUser(string userId)
    {
        var parsedId = Guid.TryParse(userId, out var result);

        if (parsedId)
        {
            AuthenticatedUserId = result;
        }
        else
        {
            throw new HttpHeaderException("X-Authenticated-UserId",
                "The HTTP header 'X-Authenticated-UserId' is not valid.");
        }
    }
}