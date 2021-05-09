using System;
using System.Collections.Generic;
using System.Linq;
using AssetServices.Models;
using Microsoft.Extensions.Logging;

namespace AssetServices
{
    public class AssetServices : IAssetServices
    {
        public AssetServices(ILogger<AssetServices> logger, AssetsContext assetContext)
        {
            Logger = logger;
            AssetContext = assetContext;
        }

        private ILogger<AssetServices> Logger { get; }
        private AssetsContext AssetContext { get; }

        public IList<Asset> GetAssetsForUser(Guid userId)
        {
            Logger.LogInformation($"Assets from {userId} retrieved.");
            return AssetContext.Assets.Where(a => a.AssetHolderId == userId).ToList();
        }
    }
}
