using System;
using Microsoft.EntityFrameworkCore;
using AssetServices.Models;

namespace AssetServicesTests
{
    public class AssetBaseTest
    {
        public const int ASSET_ONE_ID = 12;
        public const int ASSET_TWO_ID = 14;
        public const int ASSET_THREE_ID = 15;

        public Guid ASSETHOLDER_ONE_ID = new Guid("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");
        public Guid ASSETHOLDER_TWO_ID = new Guid();

        protected AssetBaseTest(DbContextOptions<AssetsContext> contextOptions)
        {
            ContextOptions = contextOptions;
        }

        protected DbContextOptions<AssetsContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new AssetsContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var assetOne = new AssetServices.Models.Asset
                {
                    Id = ASSET_ONE_ID,
                    AssetHolderId = ASSETHOLDER_ONE_ID
                };

                var assetTwo = new AssetServices.Models.Asset
                {
                    Id = ASSET_TWO_ID,
                    AssetHolderId = ASSETHOLDER_ONE_ID
                };

                var assetThree = new AssetServices.Models.Asset
                {
                    Id = ASSET_THREE_ID,
                    AssetHolderId = ASSETHOLDER_TWO_ID
                };

                context.AddRange(assetOne, assetTwo, assetThree);

                context.SaveChanges();
            }
        }
    }
}