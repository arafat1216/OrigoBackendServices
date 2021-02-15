using OrigoAssetServices.Models;
using System;
using System.Collections.Generic;

namespace OrigoAssetServices.Services
{
    public interface IAssetServices
    {
        IList<Asset> GetAssetsForUser(Guid userId);
    }
}