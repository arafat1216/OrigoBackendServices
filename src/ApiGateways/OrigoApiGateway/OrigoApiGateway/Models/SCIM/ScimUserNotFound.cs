using System.Net;

namespace OrigoApiGateway.Models.SCIM;

public class ScimUserNotFound
{
    public List<string> Schemas { get; private set; } = new() { "urn:ietf:params:scim:api:messages:2.0:Error" };

    public string Detail { get; set; } = "User not found";

    public HttpStatusCode Status { get; set; } = HttpStatusCode.NotFound;
}