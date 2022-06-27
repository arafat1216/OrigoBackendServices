using HardwareServiceOrderServices.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.Controllers
{
    /// <summary>
    ///     A health-controller used for ensuring the microservice is reached, and that the requests are valid.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IApiRequesterService _apiRequesterService;


        public HealthController(IApiRequesterService apiRequesterService)
        {
            _apiRequesterService = apiRequesterService;
        }


        private object GetResponseObject()
        {
            return new
            {
                AuthenticatedUserId = _apiRequesterService.AuthenticatedUserId
            };
        }


        /// <summary>
        ///     Perform a test-connection using a GET request.
        /// </summary>
        /// <remarks>
        ///     See if the GET request is accepted or rejected. Rejected requests are likely a result of universally
        ///     required information missing from the request, such as a missing HTTP-header.
        /// </remarks>
        /// <returns> The success or error code for the HTTP request. </returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<ActionResult> TestGetRequestAsync()
        {
            var context = HttpContext;
            return Ok(GetResponseObject());
        }

        /// <summary>
        ///     Perform a test-connection using a POST request.
        /// </summary>
        /// <remarks>
        ///     See if the POST request is accepted or rejected. Rejected requests are likely a result of universally
        ///     required information missing from the request, such as a missing HTTP-header.
        /// </remarks>
        /// <returns> The success or error code for the HTTP request. </returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<ActionResult> TestPostRequestAsync()
        {
            var context = HttpContext;
            return Ok(GetResponseObject());
        }

        /// <summary>
        ///     Perform a test-connection using a PUT request.
        /// </summary>
        /// <remarks>
        ///     See if the PUT request is accepted or rejected. Rejected requests are likely a result of universally
        ///     required information missing from the request, such as a missing HTTP-header.
        /// </remarks>
        /// <returns> The success or error code for the HTTP request. </returns>
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<ActionResult> TestPutRequestAsync()
        {
            var context = HttpContext;
            return Ok(GetResponseObject());
        }

        /// <summary>
        ///     Perform a test-connection using a PATCH request.
        /// </summary>
        /// <remarks>
        ///     See if the PATCH request is accepted or rejected. Rejected requests are likely a result of universally
        ///     required information missing from the request, such as a missing HTTP-header.
        /// </remarks>
        /// <returns> The success or error code for the HTTP request. </returns>
        [HttpPatch]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<ActionResult> TestPatchRequestAsync()
        {
            var context = HttpContext;
            return Ok(GetResponseObject());
        }

        /// <summary>
        ///     Perform a test-connection using a DELETE request.
        /// </summary>
        /// <remarks>
        ///     See if the DELETE request is accepted or rejected. Rejected requests are likely a result of universally
        ///     required information missing from the request, such as a missing HTTP-header.
        /// </remarks>
        /// <returns> The success or error code for the HTTP request. </returns>
        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK)]
        public async Task<ActionResult> TestDeleteRequestAsync()
        {
            var context = HttpContext;
            return Ok(GetResponseObject());
        }
    }
}
