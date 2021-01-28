using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoAssetServices.Models;
using OrigoAssetServices.Services;

namespace OrigoAssetServices.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(ILogger<AssetsController> logger, IAssetServices assetServices)
        {
            _logger = logger;
            AssetServices = assetServices;
        }

        public IAssetServices AssetServices { get; }

        [HttpGet]
        public IEnumerable<Asset> Get()
        {
            return AssetServices.GetAssetsForUser(1); 
        }
    }
}
