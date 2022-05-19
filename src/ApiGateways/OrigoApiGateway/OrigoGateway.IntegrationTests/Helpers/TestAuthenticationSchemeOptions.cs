using Microsoft.AspNetCore.Authentication;

namespace OrigoGateway.IntegrationTests.Helpers
{
    public class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public string Email { get; set; }
    }
}