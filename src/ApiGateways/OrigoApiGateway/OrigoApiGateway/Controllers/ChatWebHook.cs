using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace OrigoApiGateway.Controllers
{
    // This is not the final solution. Testing out with frontend that it works or not
    [ApiController]
    [ApiVersion("1.0")]
    [Route("/origoapi/v{version:apiVersion}/[controller]")]
    public class ChatWebHook : ControllerBase
    {
        private ILogger<ChatWebHook> _logger { get; }
        private IHttpClientFactory _httpClientFactory { get; }

        public ChatWebHook(ILogger<ChatWebHook> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult> GetEmbeddedData()
        {
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            "https://techstep.secure.force.com/Chat/embeddedService/sidebarApp.app?aura.format=JSON&aura.formatAdapter=LIGHTNING_OUT&guestUserLang=en-US&eswConfigDeveloperName=Origo_EN")
            {
                Headers =
            {
                { HeaderNames.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8" },
                { HeaderNames.Host, "techstep.secure.force.com" }
            }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var jsonData = await httpResponseMessage.Content.ReadAsStringAsync();
                return Ok(jsonData);
            }
            else
            {
                return Ok("error");
            }
        }
    }
}
