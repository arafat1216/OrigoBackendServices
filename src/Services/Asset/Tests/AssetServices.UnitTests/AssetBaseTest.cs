using System;
using AssetServices.Infrastructure;
using AssetServices.Models;
using Common.Enums;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Common.Logging;
using MediatR;
using Moq;

// ReSharper disable InconsistentNaming

namespace AssetServices.UnitTests
{
    public class AssetBaseTest
    {
        private readonly Guid ASSETLIFECYCLE_ONE_ID = new("4e7413da-54c9-4f79-b882-f66ce48e5074");
        private readonly Guid ASSETLIFECYCLE_TWO_ID = new("6c38b551-a5c2-4f53-8df8-221bf8485c61");
        protected readonly Guid ASSETLIFECYCLE_THREE_ID = new("80665d26-90b4-4a3a-a20d-686b64466f32");
        protected readonly Guid ASSETLIFECYCLE_FOUR_ID = new("bdb4c26c-33fd-40d7-a237-e74728609c1c");
        protected readonly Guid ASSETLIFECYCLE_FIVE_ID = new("4315bba8-698f-4ddd-aee2-82554c91721f");
        protected readonly Guid COMPANY_ID = new("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");
        protected readonly Guid DEPARTMENT_ID = new("6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72");
        protected readonly int ASSET_CATEGORY_ID = 1;

        protected readonly Guid LABEL_ONE_ID = new("BA92FC18-9399-4AC1-9BEB-57DCE85FF657");
        protected readonly Guid LABEL_TWO_ID = new("D3EF00AB-C3B6-4751-982F-BF66738BC068");

        protected readonly Guid ASSETHOLDER_ONE_ID = new("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");
        private readonly Guid ASSETHOLDER_TWO_ID = new();
        protected readonly Guid CALLER_ID = new("da031680-abb0-11ec-849b-00155d3196a5");

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

            var userOne = new User { ExternalId = ASSETHOLDER_ONE_ID };
            var userTwo = new User { ExternalId = ASSETHOLDER_TWO_ID };
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetCategory = new AssetCategory(ASSET_CATEGORY_ID, null, new List<AssetCategoryTranslation>());

            var assetOne = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012345", "Samsung", "Samsung Galaxy S20", new List<AssetImei>() { new AssetImei(500119468586675) }, "B26EDC46046B");

            var assetTwo = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012364", "Apple", "Apple iPhone 8", new List<AssetImei>() { new AssetImei(546366434558702) }, "487027C99FA1");

            var assetThree = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Samsung", "Samsung Galaxy S21", new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD");
            
            var assetFour = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Samsung", "Samsung Galaxy S21", new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD");
            
            var assetFive = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Apple", "iPhone 12", new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD");
            
            var assetSix = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Apple", "iPhone 12", new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD");
            
            var assetOther = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123457789012399", "Samsung", "Samsung Galaxy S21", new List<AssetImei>() { new AssetImei(308757706784653) }, "2E423AD72484");

            var labelOne = new CustomerLabel(LABEL_ONE_ID, COMPANY_ID, Guid.Empty, new Label("Label_1", LabelColor.Blue));
            var labelTwo = new CustomerLabel(LABEL_TWO_ID, COMPANY_ID, Guid.Empty, new Label("Label_2", LabelColor.Green));

            var assetLifecycleOne = new AssetLifecycle(ASSETLIFECYCLE_ONE_ID) { CustomerId = COMPANY_ID, Alias = "alias_0", AssetLifecycleStatus = AssetLifecycleStatus.InputRequired};
            assetLifecycleOne.AssignAsset(assetOne, CALLER_ID);
            assetLifecycleOne.AssignContractHolder(userOne, CALLER_ID);

            var assetLifecycleTwo = new AssetLifecycle(ASSETLIFECYCLE_TWO_ID) { CustomerId = COMPANY_ID, Alias = "alias_1", AssetLifecycleStatus = AssetLifecycleStatus.Available };
            assetLifecycleTwo.AssignAsset(assetTwo, CALLER_ID);
            assetLifecycleTwo.AssignContractHolder(userTwo, CALLER_ID);

            var assetLifecycleThree = new AssetLifecycle(ASSETLIFECYCLE_THREE_ID) { CustomerId = COMPANY_ID, Alias = "alias_2", AssetLifecycleStatus = AssetLifecycleStatus.Active };
            assetLifecycleThree.AssignAsset(assetThree, CALLER_ID);
            assetLifecycleThree.AssignContractHolder(userOne, CALLER_ID);
            assetLifecycleThree.AssignDepartment(DEPARTMENT_ID, CALLER_ID);

            var assetLifecycleFour = new AssetLifecycle { CustomerId = COMPANY_ID, Alias = "alias_3", AssetLifecycleStatus = AssetLifecycleStatus.Stolen };
            assetLifecycleFour.AssignAsset(assetFour, CALLER_ID);
            assetLifecycleFour.AssignContractHolder(userOne, CALLER_ID);

            var assetLifecycleFive = new AssetLifecycle(ASSETLIFECYCLE_FOUR_ID) { CustomerId = COMPANY_ID, Alias = "alias_4", AssetLifecycleStatus = AssetLifecycleStatus.Available };
            assetLifecycleFive.AssignAsset(assetFive, CALLER_ID);
            assetLifecycleFive.AssignContractHolder(userOne, CALLER_ID);

            var assetLifecycleSix = new AssetLifecycle(ASSETLIFECYCLE_FIVE_ID) { CustomerId = COMPANY_ID, Alias = "alias_5", AssetLifecycleStatus = AssetLifecycleStatus.Available };
            assetLifecycleSix.AssignAsset(assetSix, CALLER_ID);
            assetLifecycleSix.AssignContractHolder(userOne, CALLER_ID);
            assetLifecycleSix.AssignDepartment(DEPARTMENT_ID, CALLER_ID);


            var assetLifecycleOther = new AssetLifecycle { CustomerId = Guid.NewGuid(), Alias = "alias_4", AssetLifecycleStatus = AssetLifecycleStatus.Active};
            assetLifecycleOther.AssignAsset(assetOther, CALLER_ID);
            assetLifecycleOther.AssignContractHolder(new User{ExternalId = Guid.NewGuid()}, CALLER_ID);

            var lifeCycleSetting = new LifeCycleSetting(COMPANY_ID, true, Guid.Empty);
            lifeCycleSetting.SetMinBuyoutPrice(700M, 1, Guid.Empty);

            context.Users.AddRange(userOne, userTwo);
            context.Assets.AddRange(assetOne, assetTwo, assetThree, assetFour, assetFive, assetSix, assetOther);
            context.CustomerLabels.AddRange(labelOne, labelTwo);
            context.AssetLifeCycles.AddRange(assetLifecycleOne, assetLifecycleTwo, assetLifecycleThree, assetLifecycleFour, assetLifecycleFive, assetLifecycleSix, assetLifecycleOther);
            context.LifeCycleSettings.AddRange(lifeCycleSetting);
            context.SaveChanges();
        }
    }
}