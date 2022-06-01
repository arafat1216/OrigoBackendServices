#if DEBUG

using Common.Converters;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace HardwareServiceOrder.API.Controllers
{

    /// <summary>
    ///     A temporary controller used for testing during development.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2737:\"catch\" clauses should do more than rethrow", Justification = "<Pending>")]
    public class TestController : ControllerBase
    {

        private readonly ProviderFactory _providerFactory;

        public TestController(ProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }


        /// <summary>
        ///     Retrieve a given provider-interface.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/providerInterface")]
        public async Task<ActionResult> GetProviderInterface([FromQuery] int providerId = 1)
        {
            try
            {
                var providerInterface = await _providerFactory.GetRepairProviderAsync(providerId);

                return Ok();
            }
            catch (Exception e)
            {
                throw;
            }
        }


        /// <summary>
        ///     Trigger the "get updated orders" method
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="apiUsername"></param>
        /// <param name="apiPassword"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        [HttpGet("update/getAllUpdatedOrders")]
        public async Task<ActionResult> GetUpdatedRepairOrders([FromQuery] int providerId = 1, string? apiUsername = "52079706", string? apiPassword = null, string since = "2010-01-01")
        {
            try
            {
                var providerInterface = await _providerFactory.GetRepairProviderAsync(providerId, apiUsername, apiPassword);

                var result = await providerInterface.GetUpdatedRepairOrdersAsync(DateTimeOffset.Parse(since));
                return Ok(result);
            }
            catch (Exception e)
            {
                throw;
            }
        }


        /// <summary>
        ///     Create a new repair order
        /// </summary>
        /// <param name="repairOrder"></param>
        /// <param name="providerId"></param>
        /// <param name="apiUsername"></param>
        /// <param name="apiPassword"></param>
        /// <returns></returns>
        [HttpPost("create/repairOrder")]
        public async Task<ActionResult> CreateRepairOrder([FromBody] NewExternalRepairOrderDTO repairOrder, [FromQuery] int providerId = 1, string? apiUsername = "52079706", string? apiPassword = null)
        {
            var providerInterface = await _providerFactory.GetRepairProviderAsync(providerId, apiUsername, apiPassword);
            var orderResponse = providerInterface.CreateRepairOrderAsync(repairOrder, (int)ServiceTypeEnum.SUR, Guid.Empty.ToString());

            return Ok(orderResponse);
        }


        [HttpGet("serializer")]
        public async Task<ActionResult> TestSerialization()
        {
            HttpClient client = new HttpClient();
            var result = await client.GetAsync("https://localhost:7263/api/v1/test/data/1");
            var responseBody = await result.Content.ReadFromJsonAsync<TestEntity>();

            return Ok(responseBody);
        }

        [HttpPost("serializer")]
        public async Task<ActionResult> TestPostSerialization()
        {
            var a = new TestEntity()
            {
                Date = DateOnly.Parse("2020-01-01")
            };

            HttpClient client = new HttpClient();
            var result = await client.PostAsJsonAsync("https://localhost:7263/api/v1/test/data/1", a);
            var responseBody = await result.Content.ReadFromJsonAsync<TestEntity>();

            return Ok(responseBody);
        }

        [HttpGet("data/1")]
        public async Task<TestEntity> GetDate()
        {
            return new TestEntity()
            {
                Date = DateOnly.Parse("2020-01-01")
            };
        }

        [HttpPost("data/1")]
        public async Task<ActionResult> PostData(TestEntity testEntity)
        {
            return Ok(testEntity);
        }

    }

    public class TestEntity
    {
        [JsonConverter(typeof(DateOnlyNullableJsonConverter))]
        public DateOnly? Date { get; set; }
    }
}

#endif