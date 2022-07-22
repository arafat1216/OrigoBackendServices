using System.Net;
using System.Security.Claims;

namespace OrigoApiGateway.Authorization;

/// <summary>
/// This handler is added to every HttpClient call and adds the authenticated caller id
/// as a header to be processed by the backend microservices.
/// </summary>
public class AddCallerIdHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string AUTHENTICATED_USER_ID = "X-Authenticated-UserId";

    public AddCallerIdHeaderHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,  CancellationToken cancellationToken)
    {
        var callerId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
        if (string.IsNullOrEmpty(callerId))
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("The user is missing an id.")
            };
        }

        if (request.Headers.Contains(AUTHENTICATED_USER_ID))
        {
            request.Headers.Remove(AUTHENTICATED_USER_ID);
        }
        request.Headers.Add(AUTHENTICATED_USER_ID, callerId);
        return await base.SendAsync(request, cancellationToken);
    }
}