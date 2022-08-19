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
        public readonly Guid ASSETLIFECYCLE_ONE_ID = new("4e7413da-54c9-4f79-b882-f66ce48e5074");
        protected readonly Guid ASSETLIFECYCLE_TWO_ID = new("6c38b551-a5c2-4f53-8df8-221bf8485c61");
        protected readonly Guid ASSETLIFECYCLE_THREE_ID = new("80665d26-90b4-4a3a-a20d-686b64466f32");
        protected readonly Guid ASSETLIFECYCLE_FOUR_ID = new("bdb4c26c-33fd-40d7-a237-e74728609c1c");
        protected readonly Guid ASSETLIFECYCLE_FIVE_ID = new("4315bba8-698f-4ddd-aee2-82554c91721f");
        protected readonly Guid ASSETLIFECYCLE_SIX_ID = new("4515bba8-698f-4ddd-aee2-82554c91721f");
        protected readonly Guid ASSETLIFECYCLE_SEVEN_ID = new("1dee1675-0a94-4571-be24-d87ce0fb986a");
        
        protected readonly Guid ASSETLIFECYCLE_EIGHT_ID = new("b6afee47-f4a7-4cc7-b890-4eaf2b36acd4");
        protected readonly Guid ASSETLIFECYCLE_NINE_ID = new("a729805b-e341-4c45-97cf-3ea76722d026");
        protected readonly Guid ASSETLIFECYCLE_TEN_ID = new("370911d5-8545-41a3-af83-3ae376a88775");
        protected readonly Guid ASSETLIFECYCLE_ELEVEN_ID = new("6c7d1ca7-8c3a-4dda-b47c-eb539fdaa6ba");
        protected readonly Guid ASSETLIFECYCLE_TWELVE_ID = new("35ce5192-dbf9-458b-a0ec-c72875a227b7");

        protected readonly Guid COMPANY_ID = new("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");
        protected readonly Guid DEPARTMENT_ID = new("6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72");
        protected readonly int ASSET_CATEGORY_ID = 1;

        protected readonly Guid LABEL_ONE_ID = new("BA92FC18-9399-4AC1-9BEB-57DCE85FF657");
        protected readonly Guid LABEL_TWO_ID = new("D3EF00AB-C3B6-4751-982F-BF66738BC068");
        protected readonly Guid LABEL_THREE_ID = new("7f24a300-2f8a-4f3c-bbba-208ec483d36c");

        protected readonly Guid ASSETHOLDER_ONE_ID = new("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");
        public readonly Guid ASSETHOLDER_TWO_ID = new("c90c616f-b165-408b-b90b-9facbbd63eb1");
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
            var userTwo = new User { ExternalId = ASSETHOLDER_TWO_ID, Name = "Atish" };
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetCategory = new AssetCategory(ASSET_CATEGORY_ID, null, new List<AssetCategoryTranslation>());

            var assetOne = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012345", "Samsung", "Samsung Galaxy S20", new List<AssetImei>() { new AssetImei(500119468586675) }, "B26EDC46046B");

            var assetTwo = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012364", "Apple", "Apple iPhone 8", new List<AssetImei>() { new AssetImei(546366434558702) }, "487027C99FA1");

            var assetThree = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Samsung", "Samsung Galaxy S21", new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD");

            var assetFour = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Samsung", "Samsung Galaxy S21", new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD");

            var assetFive = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Apple", "iPhone 12", new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD");

            var assetSix = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Apple", "iPhone 12", new List<AssetImei>() { new AssetImei(512217111821626) }, "840F1D0C06AD");

            var assetSeven = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123458789012399", "Samsung", "Samsung Galaxy S21", new List<AssetImei>() { new AssetImei(335958985460224) }, "A93FE191233B");

            var assetOther = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123457789012399", "Samsung", "Samsung Galaxy S21", new List<AssetImei>() { new AssetImei(308757706784653) }, "2E423AD72484");

            var labelOne = new CustomerLabel(LABEL_ONE_ID, COMPANY_ID, Guid.Empty, new Label("Label_1", LabelColor.Blue));
            var labelTwo = new CustomerLabel(LABEL_TWO_ID, COMPANY_ID, Guid.Empty, new Label("Label_2", LabelColor.Green));
            var labelThree = new CustomerLabel(LABEL_THREE_ID, COMPANY_ID, Guid.Empty, new Label("Label_3", LabelColor.Orange));

            var assetLifecycleOne = new AssetLifecycle(ASSETLIFECYCLE_ONE_ID) { CustomerId = COMPANY_ID, Alias = "alias_0", AssetLifecycleStatus = AssetLifecycleStatus.Active, AssetLifecycleType = LifecycleType.NoLifecycle };
            assetLifecycleOne.AssignAsset(assetOne, CALLER_ID);

            var assetLifecycleTwo = new AssetLifecycle(ASSETLIFECYCLE_TWO_ID) { CustomerId = COMPANY_ID, Alias = "alias_1", AssetLifecycleStatus = AssetLifecycleStatus.InputRequired, AssetLifecycleType = LifecycleType.Transactional, StartPeriod = DateTime.UtcNow.AddMonths(-12), EndPeriod = DateTime.UtcNow };
            assetLifecycleTwo.AssignAsset(assetTwo, CALLER_ID);
            assetLifecycleTwo.AssignAssetLifecycleHolder(userTwo, null, CALLER_ID);

            var assetLifecycleThree = new AssetLifecycle(ASSETLIFECYCLE_THREE_ID) { CustomerId = COMPANY_ID, Alias = "alias_2", AssetLifecycleStatus = AssetLifecycleStatus.InputRequired, AssetLifecycleType = LifecycleType.Transactional, StartPeriod = DateTime.UtcNow.AddMonths(-12), EndPeriod = DateTime.UtcNow };
            assetLifecycleTwo.AssignAsset(assetThree, CALLER_ID);

            var assetLifecycleFour = new AssetLifecycle() { CustomerId = COMPANY_ID, Alias = "alias_3", AssetLifecycleStatus = AssetLifecycleStatus.InputRequired, AssetLifecycleType = LifecycleType.Transactional };
            assetLifecycleFour.AssignAsset(assetFour, CALLER_ID);
            assetLifecycleFour.AssignAssetLifecycleHolder(userOne, null, CALLER_ID);
            assetLifecycleFour.HasBeenStolen(CALLER_ID);
            assetLifecycleFour.AssignCustomerLabel(labelOne, CALLER_ID);

            var assetLifecycleFive = new AssetLifecycle(ASSETLIFECYCLE_FOUR_ID) { CustomerId = COMPANY_ID, Alias = "alias_4", AssetLifecycleStatus = AssetLifecycleStatus.InputRequired, AssetLifecycleType = LifecycleType.Transactional };
            assetLifecycleFive.AssignAsset(assetFive, CALLER_ID);
            assetLifecycleFive.AssignAssetLifecycleHolder(userOne, null, CALLER_ID);
            assetLifecycleFive.MakeAssetAvailable(CALLER_ID);
            assetLifecycleFive.AssignCustomerLabel(labelOne, CALLER_ID);

            var assetLifecycleSix = new AssetLifecycle(ASSETLIFECYCLE_FIVE_ID) { CustomerId = COMPANY_ID, Alias = "alias_5", AssetLifecycleStatus = AssetLifecycleStatus.InputRequired, AssetLifecycleType = LifecycleType.Transactional, StartPeriod = DateTime.UtcNow.AddMonths(-12), EndPeriod = DateTime.UtcNow };
            assetLifecycleSix.AssignAsset(assetSix, CALLER_ID);
            assetLifecycleSix.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
            assetLifecycleSix.AssignCustomerLabel(labelTwo, CALLER_ID);

            var assetLifecycleSeven = new AssetLifecycle(ASSETLIFECYCLE_SIX_ID) { CustomerId = COMPANY_ID, Alias = "alias_6", AssetLifecycleStatus = AssetLifecycleStatus.Active, AssetLifecycleType = LifecycleType.NoLifecycle };
            assetLifecycleSeven.AssignAsset(assetSix, CALLER_ID);
            assetLifecycleSeven.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
            assetLifecycleSeven.AssignCustomerLabel(labelTwo, CALLER_ID);

            var assetLifecycleOther = new AssetLifecycle { CustomerId = Guid.NewGuid(), Alias = "alias_4", AssetLifecycleStatus = AssetLifecycleStatus.InputRequired, AssetLifecycleType = LifecycleType.Transactional };
            assetLifecycleOther.AssignAsset(assetOther, CALLER_ID);
            assetLifecycleOther.AssignAssetLifecycleHolder(new User { ExternalId = Guid.NewGuid() }, null, CALLER_ID);

            var assetLifecycleEight = new AssetLifecycle(ASSETLIFECYCLE_SEVEN_ID) { CustomerId = COMPANY_ID, Alias = "alias_7", AssetLifecycleStatus = AssetLifecycleStatus.Active, AssetLifecycleType = LifecycleType.Transactional };
            assetLifecycleEight.AssignAsset(assetSeven, CALLER_ID);
            assetLifecycleEight.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
            assetLifecycleEight.IsSentToRepair(CALLER_ID);

            //Pending recycle
            var assetLifecycleNine = new AssetLifecycle(ASSETLIFECYCLE_EIGHT_ID) { CustomerId = COMPANY_ID, Alias = "alias_8", AssetLifecycleStatus = AssetLifecycleStatus.InUse, AssetLifecycleType = LifecycleType.Transactional };
            assetLifecycleNine.AssignAsset(assetSeven, CALLER_ID);
            assetLifecycleNine.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
            assetLifecycleNine.SetPendingRecycledStatus(CALLER_ID);

            //Recycled 
            var assetLifecycleTen = new AssetLifecycle(ASSETLIFECYCLE_NINE_ID) { CustomerId = COMPANY_ID, Alias = "alias_9", AssetLifecycleStatus = AssetLifecycleStatus.InUse, AssetLifecycleType = LifecycleType.Transactional };
            assetLifecycleTen.AssignAsset(assetSeven, CALLER_ID);
            assetLifecycleTen.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
            assetLifecycleTen.SetPendingRecycledStatus(CALLER_ID);
            assetLifecycleTen.SetRecycledStatus(CALLER_ID);

            //Expired soon
            var assetLifecycleEleven = new AssetLifecycle(ASSETLIFECYCLE_TEN_ID) { CustomerId = COMPANY_ID, Alias = "alias_10", AssetLifecycleStatus = AssetLifecycleStatus.InUse, AssetLifecycleType = LifecycleType.Transactional };
            assetLifecycleEleven.AssignAsset(assetSeven, CALLER_ID);
            assetLifecycleEleven.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
            assetLifecycleEleven.MakeAssetExpiresSoon(CALLER_ID);

            //Expired 
            var assetLifecycleTwelve = new AssetLifecycle(ASSETLIFECYCLE_ELEVEN_ID) { CustomerId = COMPANY_ID, Alias = "alias_11", AssetLifecycleStatus = AssetLifecycleStatus.InUse, AssetLifecycleType = LifecycleType.Transactional };
            assetLifecycleTwelve.AssignAsset(assetSeven, CALLER_ID);
            assetLifecycleTwelve.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
            assetLifecycleTwelve.MakeAssetExpiresSoon(CALLER_ID);
            assetLifecycleTwelve.MakeAssetExpired(CALLER_ID);

            //Pending Return
            var assetLifecycleThirteen = new AssetLifecycle(ASSETLIFECYCLE_TWELVE_ID) { CustomerId = COMPANY_ID, Alias = "alias_12", AssetLifecycleStatus = AssetLifecycleStatus.InUse, AssetLifecycleType = LifecycleType.Transactional };
            assetLifecycleThirteen.AssignAsset(assetSeven, CALLER_ID);
            assetLifecycleThirteen.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
            assetLifecycleThirteen.MakeAssetExpiresSoon(CALLER_ID);
            assetLifecycleThirteen.MakeReturnRequest(CALLER_ID);


            var lifeCycleSetting = new LifeCycleSetting(1, true, 500M, 24, Guid.Empty);
            var customerSetting = new CustomerSettings(COMPANY_ID, new List<LifeCycleSetting>() { lifeCycleSetting });


            assetLifecycleTwo.AssignCustomerLabel(labelOne, CALLER_ID);

            context.Users.AddRange(userOne, userTwo);
            context.Assets.AddRange(assetOne, assetTwo, assetThree, assetFour, assetFive, assetSix, assetOther);
            context.CustomerLabels.AddRange(labelOne, labelTwo, labelThree);
            context.AssetLifeCycles.AddRange(assetLifecycleOne, assetLifecycleTwo, assetLifecycleThree, assetLifecycleFour, assetLifecycleFive, assetLifecycleSix, assetLifecycleSeven, assetLifecycleOther, assetLifecycleEight,
                assetLifecycleNine, assetLifecycleTen, assetLifecycleEleven, assetLifecycleTwelve, assetLifecycleThirteen);
            context.CustomerSettings.AddRange(customerSetting);
            context.SaveChanges();
        }
    }
}