using Microsoft.Extensions.Logging;
using OrigoAssetServices.Models;
using System;
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

        public IEnumerable<Asset> GetAssetsForUser(Guid userId)
        {
            return new List<Asset>() {
                new Asset {
                    AssetId = new Guid("361f11bb-c8e5-42ca-a008-7e1cfba893ad"),
                    AssetName = "iPhone",
                    AssetHolder = new AssetHolder{
                        AssetHolderId = Guid.NewGuid(),
                        CompanyId = Guid.NewGuid(),
                        DepartmentId = Guid.NewGuid()
                    },
                    Imei = "353041092945465",
                    Vendor = "Apple"
                },
                new Asset {
                    AssetId = new Guid("75e887ac-6ea4-4cfe-b49d-fdc3dba450a9"),
                    AssetName = "iPhone XS",
                    AssetHolder = new AssetHolder{
                        AssetHolderId = Guid.NewGuid(),
                        CompanyId = Guid.NewGuid(),
                        DepartmentId = Guid.NewGuid()
                    },
                    Imei = "357229098092251",
                    Vendor = "Apple"
                }
            };
        }
    }
}
