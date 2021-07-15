using System;
using AssetServices.Infrastructure;
using AssetServices.Models;
using Common.Enums;
using Microsoft.EntityFrameworkCore;

// ReSharper disable InconsistentNaming

namespace AssetServices.UnitTests
{
    public class AssetBaseTest
    {
        private readonly Guid ASSET_ONE_ID = new("4e7413da-54c9-4f79-b882-f66ce48e5074");
        private readonly Guid ASSET_TWO_ID = new("6c38b551-a5c2-4f53-8df8-221bf8485c61");
        private readonly Guid ASSET_THREE_ID = new("80665d26-90b4-4a3a-a20d-686b64466f32");
        protected readonly Guid COMPANY_ID = new("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");
        protected readonly Guid ASSET_CATEGORY_ID = new Guid("9a9dd12f-e523-4fae-a9df-dddda5c719de");

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

            var assetCategory = new AssetCategory(ASSET_CATEGORY_ID, "Mobile phones", true);
            context.Add(assetCategory);
            context.SaveChanges();

            var assetOne = new Asset(ASSET_ONE_ID, COMPANY_ID,  "123456789012345",
                assetCategory, "Samsung", "Samsung Galaxy S20",
                LifecycleType.Leasing, new DateTime(2021, 4, 1), ASSETHOLDER_ONE_ID, true, "500119468586675", "B26EDC46046B");

            var assetTwo = new Asset(ASSET_TWO_ID, COMPANY_ID,  "123456789012364",
                assetCategory, "Apple", "Apple iPhone 8",
                LifecycleType.Leasing, new DateTime(2021, 5, 1), ASSETHOLDER_TWO_ID, true, "546366434558702", "487027C99FA1");

            var assetThree = new Asset(ASSET_THREE_ID, COMPANY_ID,  "123456789012399",
                assetCategory, "Samsung", "Samsung Galaxy S21",
                LifecycleType.Leasing, new DateTime(2021, 6, 1), ASSETHOLDER_ONE_ID, true, "512217111821626", "840F1D0C06AD");

            var assetOther = new Asset(Guid.NewGuid(), Guid.NewGuid(), "123457789012399",
                assetCategory, "Samsung", "Samsung Galaxy S21",
                LifecycleType.Leasing, new DateTime(2021, 6, 1), Guid.NewGuid(), true, "308757706784653", "2E423AD72484");

            context.AddRange(assetOne, assetTwo, assetThree, assetOther);

            context.SaveChanges();
        }
    }
}