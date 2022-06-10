#if DEBUG

using Common.Converters;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Infrastructure;
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

        private readonly HardwareServiceOrderContext _context;
        private readonly HardwareServiceOrderRepository _repo;

        public TestController(ProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;

            _context = new HardwareServiceOrderContextFactory().CreateDbContext(Array.Empty<string>());
            _repo = new HardwareServiceOrderRepository(_context);
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
            var orderResponse = await providerInterface.CreateRepairOrderAsync(repairOrder, (int)ServiceTypeEnum.SUR, Guid.Empty.ToString());

            return Ok(orderResponse);
        }


        [HttpGet("getRepairOrder")]
        public async Task<ActionResult> GetRepairOrder([FromQuery] Guid orderId, [FromQuery] int providerId = 1, string? apiUsername = "52079706", string? apiPassword = null)
        {
            var providerInterface = await _providerFactory.GetRepairProviderAsync(providerId, apiUsername, apiPassword);
            var response = await providerInterface.GetRepairOrderAsync(orderId.ToString(), null);

            return Ok(response);
        }

        /// <summary>
        ///     Creates a new dummy service-order
        /// </summary>
        /// <returns></returns>
        [HttpPost("repo-test/serviceorder/add")]
        public async Task<ActionResult> asdasd()
        {
            var CUSTOMER_ONE_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD62");

            var deliveryAddress = new DeliveryAddress(
                RecipientTypeEnum.Personal,
                "Recipient",
                "Address1",
                "Address2",
                "PostalCode",
                "City",
                "NO"
            );


            var order2 = new HardwareServiceOrderServices.Models.HardwareServiceOrder()
            {
                CustomerId = CUSTOMER_ONE_ID,
                Owner = new ContactDetails(Guid.NewGuid(), "Firstname", "test@test.com"),
                AssetLifecycleId = Guid.NewGuid(),
                DeliveryAddress = deliveryAddress,
                ServiceProviderId = 3,
                ServiceProviderOrderId1 = Guid.NewGuid().ToString(),
                ServiceProviderOrderId2 = null,
                ExternalServiceManagementLink = "http://localhost",
                ServiceTypeId = (int)ServiceTypeEnum.SUR,
                StatusId = (int)ServiceStatusEnum.Ongoing,
                UserDescription = "A problem!"
            };

            var result = await _context.HardwareServiceOrders.AddAsync(order2);
            await _context.SaveChangesAsync();

            return Ok(result.Entity);
        }

        [HttpGet("repo-test/event/get")]
        public async Task<ActionResult> Tests([FromQuery] bool add = false)
        {
            //var result = await _repo.GetServiceEventsForOrder(1);
            ServiceEvent serviceEvent1 = new()
            {
                ServiceStatusId = 2,
                Timestamp = DateTimeOffset.UtcNow,
            };

            if (add)
            {
                var order = await _context.HardwareServiceOrders.FindAsync(7);
                order.AddServiceEvent(serviceEvent1);
                //order.ServiceEvents.Add(serviceEvent1);
                await _context.SaveChangesAsync();
            }

            var result = await _context.HardwareServiceOrders.FindAsync(7);
            return Ok(result);
        }



        [HttpGet("updateInsertTest")]
        public async Task<ActionResult> TestValueAssignment()
        {
            ServiceStatus status = new()
            {

            };

            var result = await _context.ServiceStatuses.AddAsync(status);
            await _context.SaveChangesAsync();
            return Ok(result.Entity);
        }


    }
}

#endif