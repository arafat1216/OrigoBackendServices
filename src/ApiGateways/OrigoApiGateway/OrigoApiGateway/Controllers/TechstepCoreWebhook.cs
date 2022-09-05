
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.TechstepCoreWebhook;
using OrigoApiGateway.Services;
using OrigoApiGateway.Attributes;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Tags("SwaggerName")] //find a cryptic name for swagger also?
    [Route("/origoapi/v{version:apiVersion}/customers")]
    
    public class TechstepCoreWebhook : ControllerBase
    {
        private ILogger<TechstepCoreWebhook> _logger { get; }
        private ICustomerServices _customerServices { get; }

        public TechstepCoreWebhook(ILogger<TechstepCoreWebhook> logger, ICustomerServices customerServices)
        {
            _logger = logger;
            _customerServices = customerServices;
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ApiKeyAuthentication]
        [Route("844c0935-8768-4292-878e-73b915ebf7f6")]
        public async Task<ActionResult> ReceiveCustomerUpdateFromTechstepCore([FromBody] TechstepCoreCustomerUpdate TechstepCoreUpdate)
        {
            try
            {


                await _customerServices.UpdateCustomerFromTechstepCore(TechstepCoreUpdate);

                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return Ok();
            }
        }
    }
}
