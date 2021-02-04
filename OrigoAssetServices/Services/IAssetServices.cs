using OrigoAssetServices.Models;
using System;
using System.Collections.Generic;

namespace OrigoAssetServices.Services
{
    public interface IAssetServices
    {
        IEnumerable<Asset> GetAssetsForUser(Guid userId);
    }
}