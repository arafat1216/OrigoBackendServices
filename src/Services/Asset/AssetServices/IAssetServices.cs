using System;
using System.Collections.Generic;
using AssetServices.Models;

namespace AssetServices
{
    public interface IAssetServices
    {
        IList<Asset> GetAssetsForUser(Guid userId);
    }
}