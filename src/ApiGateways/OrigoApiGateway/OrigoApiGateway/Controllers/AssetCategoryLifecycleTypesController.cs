using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
    // AssetCategoryLifecycleTypes should only be available through a given customer
    [Route("/origoapi/v{version:apiVersion}/[controller]")]
    public class AssetCategoryLifecycleTypesController : ControllerBase
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<AssetsController> _logger;
        private readonly IAssetCategoryLifecycleTypesServices _assetCategoryLifecycleTypesServices;

        public AssetCategoryLifecycleTypesController(ILogger<AssetsController> logger, IAssetCategoryLifecycleTypesServices assetCategoryLifecycleTypesServices)
        {
            _logger = logger;
            _assetCategoryLifecycleTypesServices = assetCategoryLifecycleTypesServices;
        }
    }
}
