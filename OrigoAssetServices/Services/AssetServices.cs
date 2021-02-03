using Microsoft.Extensions.Logging;
using OrigoAssetServices.Models;
using System.Collections.Generic;

namespace OrigoAssetServices.Services
{
    public class AssetServices : IAssetServices
    {
        public AssetServices(ILogger<AssetServices> logger, AssetsContext assetContext)
        {
            Logger = logger;
            AssetContext = assetContext;
        }

        public ILogger<AssetServices> Logger { get; }
        public AssetsContext AssetContext { get; }

        public IEnumerable<Asset> GetAssetsForUser(int userId)
        {
            return new List<Asset>() { new Asset { AssetName = "iPhone" } };
        }
    }
}
