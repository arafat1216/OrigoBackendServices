using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoAssetServices.Models;
using OrigoAssetServices.Services;

namespace OrigoAssetServices.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("/origoapi/[controller]")]
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
        public IEnumerable<OrigoAsset> Get(Guid userId)
        {
            var assets = AssetServices.GetAssetsForUser(userId);
            var origoAssets = new List<OrigoAsset>();
            foreach (var asset in assets)
            {
                origoAssets.Add(new OrigoAsset
                {
                    Id = asset.AssetId,
                    SubsId = Guid.NewGuid(),
                    DeptId = asset.AssetHolder.DepartmentId,
                    Imei = asset.Imei,
                    Vendor = asset.Vendor,
                    Source = 1,
                    Status = 0,
                    Terminal = asset.AssetName,
                    PhoneNumber = asset.PhoneNumber,
                    UserName = "John Doe",
                    Email = "john@example.com"
                });
            }
            return origoAssets;
        }
    }
}
