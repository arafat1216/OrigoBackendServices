using System;
using Microsoft.EntityFrameworkCore;
using AssetServices.Models;
// ReSharper disable InconsistentNaming

namespace AssetServicesTests
{
    public class AssetBaseTest
    {
        private readonly Guid ASSET_ONE_ID = new("4e7413da-54c9-4f79-b882-f66ce48e5074");
        private readonly Guid ASSET_TWO_ID = new("6c38b551-a5c2-4f53-8df8-221bf8485c61");
        private readonly Guid ASSET_THREE_ID = new("80665d26-90b4-4a3a-a20d-686b64466f32");

        protected readonly Guid ASSETHOLDER_ONE_ID = new("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");
        private readonly Guid ASSETHOLDER_TWO_ID = new();

        protected AssetBaseTest(DbContextOptions<AssetsContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        protected DbContextOptions<AssetsContext> ContextOptions { get; }

        private void Seed()
        {
            using var context = new AssetsContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var assetOne = new AssetServices.Models.Asset
            {
                AssetId = ASSET_ONE_ID,
                AssetName = "Asset 1",
                AssetHolderId = ASSETHOLDER_ONE_ID
            };

            var assetTwo = new AssetServices.Models.Asset
            {
                AssetId = ASSET_TWO_ID,
                AssetName = "Asset 2",
                AssetHolderId = ASSETHOLDER_ONE_ID
            };

            var assetThree = new AssetServices.Models.Asset
            {
                AssetId = ASSET_THREE_ID,
                AssetName = "Asset 3",
                AssetHolderId = ASSETHOLDER_TWO_ID
            };

            context.AddRange(assetOne, assetTwo, assetThree);

            context.SaveChanges();
        }
    }
}