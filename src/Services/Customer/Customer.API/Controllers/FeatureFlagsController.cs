using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Customer.API.WriteModels;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizations/feature-flags")]
    public class FeatureFlagsController : ControllerBase
    {
        private readonly IFeatureFlagServices _featureFlagServices;

        public FeatureFlagsController(IFeatureFlagServices featureFlagServices)
        {
            _featureFlagServices = featureFlagServices;
        }

        /// <summary>
        /// /// Returns all feature flags set for all users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<string>>> GetFeatureFlags()
        {
            var featureFlags = await _featureFlagServices.GetFeatureFlags();
            return Ok(featureFlags);
        }

        /// <summary>
        /// Returns all feature flags also including the ones specific for a customer.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [Route("{customerId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> GetFeatureFlagsForCustomer(Guid customerId)
        {
            var featureFlags = await _featureFlagServices.GetFeatureFlags(customerId);

            return Ok(featureFlags);
        }

        /// <summary>
        /// /// Adds a feature flags. If customer is not specified flag will be set for all users.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<IList<string>>> AddFeatureFlags(NewFeatureFlag newFeatureFlag)
        {
            await _featureFlagServices.AddFeatureFlags(newFeatureFlag.FeatureFlagName, newFeatureFlag.CustomerId);
            return Ok();
        }

        /// <summary>
        /// /// Adds a feature flags. If customer is not specified flag will be set for all users.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<string>>> DeleteFeatureFlags(DeleteFeatureFlag deleteFeatureFlag)
        {
            await _featureFlagServices.DeleteFeatureFlags(deleteFeatureFlag.FeatureFlagName, deleteFeatureFlag.CustomerId);
            return Ok();
        }
    }
}