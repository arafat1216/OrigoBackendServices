#if DEBUG

using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Mvc;

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


        [HttpPost("create/1")]
        public async Task<ActionResult> CreateTest1()
        {
            try
            {
                //var serviceProvider = new HardwareServiceOrderServices.Conmodo.ProviderServices("", "", "52079706");

                DeliveryAddressDTO address = new()
                {
                    Recipient = "Recipient",
                    Address1 = "Address1",
                    Address2 = null,
                    PostalCode = "6060",
                    City = "City",
                    Country = "NO",
                };

                AssetInfoDTO asset = new()
                {
                    AssetCategoryId = 1,
                    Brand = "Samsung",
                    Model = "Galaxy S7",
                    Imei = null,
                    SerialNumber = null,
                    Accessories = null,
                    PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
                };

                NewExternalRepairOrderDTO repairOrder = new(Guid.NewGuid(), "Firstname", "Lastname", null, "test@test.com", Guid.NewGuid(), null, address, asset, "Something is wrong!");

                //var result = await serviceProvider.CreateRepairOrderAsync(repairOrder, (int)ServiceTypeEnum.SUR, Guid.NewGuid().ToString());

                //return Ok(result);

                return Ok();
            }
            catch (Exception e)
            {
                throw;
            }
        }


        [HttpPost("create/2")]
        public async Task<ActionResult> CreateTest2()
        {
            try
            {
                var providerInterface = await _providerFactory.GetRepairProviderAsync(1);

                return Ok();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        /// <summary>
        ///     Create a repair order
        /// </summary>
        /// <param name="repairOrder"></param>
        /// <returns></returns>
        [HttpPost("create/3")]
        public async Task<ActionResult> CreateTest3([FromBody] NewExternalRepairOrderDTO repairOrder)
        {
            var providerInterface = await _providerFactory.GetRepairProviderAsync(1, "52079706");
            var orderResponse = providerInterface.CreateRepairOrderAsync(repairOrder, (int)ServiceTypeEnum.SUR, Guid.Empty.ToString());

            return Ok(orderResponse);
        }


        [HttpGet("update/1")]
        public async Task<ActionResult> UpdateTest1()
        {
            try
            {
                var providerInterface = await _providerFactory.GetRepairProviderAsync(1, "username", "password");

                var result = await providerInterface.GetUpdatedRepairOrdersAsync(DateTimeOffset.Parse("2010-01-01"));
                return Ok(result);
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}

#endif