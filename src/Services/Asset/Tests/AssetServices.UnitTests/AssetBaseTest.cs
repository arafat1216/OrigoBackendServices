using System;
using AssetServices.Infrastructure;
using AssetServices.Models;
using Common.Enums;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Common.Logging;
using MediatR;
using Moq;
using System.Collections.ObjectModel;

// ReSharper disable InconsistentNaming

namespace AssetServices.UnitTests
{
    public class AssetBaseTest
    {
        private readonly Guid ASSET_ONE_ID = new("4e7413da-54c9-4f79-b882-f66ce48e5074");
        private readonly Guid ASSET_TWO_ID = new("6c38b551-a5c2-4f53-8df8-221bf8485c61");
        private readonly Guid ASSET_THREE_ID = new("80665d26-90b4-4a3a-a20d-686b64466f32");
        protected readonly Guid COMPANY_ID = new("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");
        protected readonly int ASSET_CATEGORY_ID = 1;

        protected readonly Guid LABEL_ONE_ID = new("BA92FC18-9399-4AC1-9BEB-57DCE85FF657");
        protected readonly Guid LABEL_TWO_ID = new("D3EF00AB-C3B6-4751-982F-BF66738BC068");

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

            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetCategory = assetRepository.GetAssetCategoryAsync(ASSET_CATEGORY_ID).Result;

            var assetOne = new MobilePhone(ASSET_ONE_ID, COMPANY_ID, "alias_0", assetCategory, "123456789012345", "Samsung", "Samsung Galaxy S20",
                LifecycleType.Leasing, new DateTime(2021, 4, 1), ASSETHOLDER_ONE_ID, new List<AssetImei>() { new AssetImei(500119468586675) }, "B26EDC46046B", AssetStatus.OnRepair, string.Empty, "Tag_0", "Description_0");

            var assetTwo = new MobilePhone(ASSET_TWO_ID, COMPANY_ID, "alias_1", assetCategory, "123456789012364", "Apple", "Apple iPhone 8",
                LifecycleType.Leasing, new DateTime(2021, 5, 1), ASSETHOLDER_TWO_ID, new List<AssetImei>() { new AssetImei(546366434558702) }, "487027C99FA1", AssetStatus.Inactive, "Note_1", "Tag_1", "Description_1", null);

            var assetThree = new MobilePhone(ASSET_THREE_ID, COMPANY_ID, "alias_2", assetCategory, "123456789012399", "Samsung", "Samsung Galaxy S21",
                LifecycleType.Leasing, new DateTime(2021, 6, 1), ASSETHOLDER_ONE_ID, new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD", AssetStatus.Active, "Company phone", "Company", "This is a company owned device");

            var assetOther = new MobilePhone(Guid.NewGuid(), Guid.NewGuid(), "alias_3", assetCategory, "123457789012399", "Samsung", "Samsung Galaxy S21",
                LifecycleType.Leasing, new DateTime(2021, 6, 1), Guid.NewGuid(), new List<AssetImei>() { new AssetImei(308757706784653) }, "2E423AD72484", AssetStatus.Active, "Note_3", "Tag_3", "Description_3");

            var labelOne = new CustomerLabel(LABEL_ONE_ID, COMPANY_ID, new Label("Label_1", LabelColor.Blue));
            var labelTwo = new CustomerLabel(LABEL_TWO_ID, COMPANY_ID, new Label("Label_2", LabelColor.Green));


            context.Assets.AddRange(assetOne, assetTwo, assetThree, assetOther);
            context.CustomerLabels.AddRange(labelOne, labelTwo);
            context.SaveChanges();
        }
    }
}