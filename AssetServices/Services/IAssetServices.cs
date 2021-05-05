using AssetServices.Models;
using System;
using System.Collections.Generic;

namespace AssetServices.Services
{
    public interface IAssetServices
    {
        IList<Asset> GetAssetsForUser(Guid userId);
    }
}