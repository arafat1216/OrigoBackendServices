using System;
using System.Collections.Generic;
using System.Linq;
using AssetServices.Infrastructure;
using AssetServices.Models;
using Common.Enums;

namespace Asset.IntegrationTests.Helpers;

/// <summary>
///     Seeds the database with given values and makes it easier to run independent tests.
///     Ideas taken from the Pluralsight course:
///     https://app.pluralsight.com/library/courses/integration-testing-asp-dot-net-core-applications-best-practices/table-of-contents
/// </summary>
internal static class AssetTestDataSeedingForDatabase
{
    public static readonly Guid ORGANIZATION_ID = Guid.Parse("7adbd9fa-97d1-11ec-8500-00155d64bd3d");
    private static readonly Guid CALLER_ID = new("da031680-abb0-11ec-849b-00155d3196a5");
    public static readonly Guid ASSETLIFECYCLE_ONE_ID = new("4e7413da-54c9-4f79-b882-f66ce48e5074");
    public static readonly Guid ASSETLIFECYCLE_TWO_ID = new("6c38b551-a5c2-4f53-8df8-221bf8485c61");
    public static readonly Guid ASSETLIFECYCLE_THREE_ID = new("80665d26-90b4-4a3a-a20d-686b64466f32");
    public static readonly Guid ASSETLIFECYCLE_FOUR_ID = new("bdb4c26c-33fd-40d7-a237-e74728609c1c");
    public static readonly Guid ASSETLIFECYCLE_FIVE_ID = new("4315bba8-698f-4ddd-aee2-82554c91721f");
    public static readonly Guid ASSETLIFECYCLE_SIX_ID = new("a066d864-e0c4-11ec-a601-00155dd40b8e");
    public static readonly Guid ASSETLIFECYCLE_SEVEN_ID = new("b3c62ea0-e0c4-11ec-a915-00155dd40b8e");
    public static readonly Guid ASSETLIFECYCLE_EIGHT_ID = new("5e1b365a-0012-4fd1-a451-8a9f4b1de812");
    public static readonly Guid ASSETLIFECYCLE_NINE_ID = new("b1378497-87f5-4204-bd8a-ab1be0b3536c");



    public static readonly Guid DEPARTMENT_ID = new("6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72");
    public static readonly Guid DEPARTMENT_TWO_ID = new("fe625c35-91d0-448e-a803-0dcbbd97f1d5");
    public static readonly Guid COMPANY_ID = new("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");
    public static readonly Guid COMPANY_ID_TWO = new("dab4bb77-3471-4ab3-ae5e-2d4fce450f37");
    public static readonly Guid ASSETHOLDER_ONE_ID = new("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");

    public static void InitialiseDbForTests(AssetsContext dbContext)
    {
        dbContext.Database.EnsureCreated();
        var assetOne = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012345", "Samsung", "Samsung Galaxy S20",
            new List<AssetImei> { new(500119468586675) }, "01-23-45-67-89-AB");

        var assetTwo = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012364", "Apple", "Apple iPhone 8",
            new List<AssetImei> { new(546366434558702) }, "01:23:45:67:89:AB");

        var assetThree = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Samsung", "Samsung Galaxy S21",
            new List<AssetImei> { new(512217111821626) }, "0123.4567.89AB");

        var assetFour = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012397", "Apple", "iPhone 11 Pro",
            new List<AssetImei> { new(512217111821624) }, "01:23:00:67:89:AB");

        var assetFive = new Tablet(Guid.NewGuid(), CALLER_ID, "123456789012397", "Apple", "iPhone 11 Pro",
            new List<AssetImei> { new(512217111821624) }, "0123.4567.89AB");

        var labelOne = new CustomerLabel(COMPANY_ID, COMPANY_ID, CALLER_ID, new Label("CompanyOne", LabelColor.Lightblue));
        var labelTwo = new CustomerLabel(COMPANY_ID_TWO, COMPANY_ID, CALLER_ID, new Label("CompanyOne", LabelColor.Lightblue));

        var userOne = new User { ExternalId = ASSETHOLDER_ONE_ID };

        var assetLifecycleOne = new AssetLifecycle(ASSETLIFECYCLE_ONE_ID)
        {
            CustomerId = COMPANY_ID,
            Alias = "alias_0",
            AssetLifecycleStatus = AssetLifecycleStatus.InputRequired,
            AssetLifecycleType = LifecycleType.Transactional
        };
        assetLifecycleOne.AssignAsset(assetOne, CALLER_ID);
        assetLifecycleOne.AssignAssetLifecycleHolder(userOne, null, CALLER_ID);

        var assetLifecycleTwo = new AssetLifecycle(ASSETLIFECYCLE_TWO_ID)
        {
            CustomerId = COMPANY_ID,
            Alias = "alias_1",
            AssetLifecycleStatus = AssetLifecycleStatus.Available,
            AssetLifecycleType = LifecycleType.Transactional
        };
        assetLifecycleTwo.AssignAsset(assetTwo, CALLER_ID);

        var assetLifecycleThree = new AssetLifecycle(ASSETLIFECYCLE_THREE_ID)
        {
            CustomerId = COMPANY_ID,
            Alias = "alias_2",
            AssetLifecycleStatus = AssetLifecycleStatus.InUse,
            AssetLifecycleType = LifecycleType.Transactional
        };
        assetLifecycleThree.AssignAsset(assetThree, CALLER_ID);
        assetLifecycleThree.AssignAssetLifecycleHolder(null,DEPARTMENT_ID, CALLER_ID);

        var assetLifecycleFour = new AssetLifecycle(ASSETLIFECYCLE_FOUR_ID)
        {
            CustomerId = COMPANY_ID,
            Alias = "alias_3",
            AssetLifecycleStatus = AssetLifecycleStatus.Repair,
            AssetLifecycleType = LifecycleType.Transactional
        };
        assetLifecycleFour.AssignAsset(assetFour, CALLER_ID);
        assetLifecycleFour.AssignAssetLifecycleHolder(userOne,null, CALLER_ID);

        var assetLifecycleFive = new AssetLifecycle(ASSETLIFECYCLE_FIVE_ID)
        {
            CustomerId = COMPANY_ID,
            Alias = "alias_4",
            AssetLifecycleStatus = AssetLifecycleStatus.Available,
            AssetLifecycleType = LifecycleType.Transactional
        };
        assetLifecycleFive.AssignAsset(assetFour, CALLER_ID);
        assetLifecycleFive.AssignAssetLifecycleHolder(null,DEPARTMENT_ID, CALLER_ID);

        var assetLifecycleSix = new AssetLifecycle(ASSETLIFECYCLE_SIX_ID)
        {
            CustomerId = COMPANY_ID,
            Alias = "alias_5",
            AssetLifecycleStatus = AssetLifecycleStatus.Active,
            AssetLifecycleType = LifecycleType.NoLifecycle
        };
        assetLifecycleSix.AssignAsset(assetFour, CALLER_ID);
        assetLifecycleSix.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
        assetLifecycleSix.AssignCustomerLabel(labelOne, CALLER_ID);


        var assetLifecycleSeven = new AssetLifecycle(ASSETLIFECYCLE_SEVEN_ID)
        {
            CustomerId = COMPANY_ID,
            Alias = "alias_6",
            AssetLifecycleStatus = AssetLifecycleStatus.Recycled,
            AssetLifecycleType = LifecycleType.NoLifecycle
        };
        assetLifecycleSeven.AssignAsset(assetFive, CALLER_ID);
        assetLifecycleSeven.AssignAssetLifecycleHolder(null, DEPARTMENT_ID, CALLER_ID);
        assetLifecycleSeven.AssignCustomerLabel(labelTwo, CALLER_ID);

        var assetLifecycleEight = new AssetLifecycle(ASSETLIFECYCLE_EIGHT_ID)
        {
            CustomerId = COMPANY_ID_TWO,
            Alias = "alias_7",
            AssetLifecycleStatus = AssetLifecycleStatus.Repair,
            AssetLifecycleType = LifecycleType.Transactional
        };
        assetLifecycleEight.AssignAsset(assetTwo, CALLER_ID);

        var assetLifecycleNine = new AssetLifecycle(ASSETLIFECYCLE_NINE_ID)
        {
            CustomerId = COMPANY_ID,
            Alias = "alias_8",
            AssetLifecycleStatus = AssetLifecycleStatus.InputRequired,
            AssetLifecycleType = LifecycleType.BYOD
        };
        assetLifecycleNine.AssignAsset(assetTwo, CALLER_ID);

        var disposeSetting = new DisposeSetting("example@techstep.no", Guid.Empty);
        var returnLocation = new ReturnLocation("Return Location","Return to Mr. on 3rd Floor",Guid.NewGuid());
        disposeSetting.AddReturnLocation(returnLocation, COMPANY_ID, Guid.Empty);
        var lifeCycleSettingOne = new LifeCycleSetting(1, true, 700M, 24, Guid.Empty);
        var customerSettingOne = new CustomerSettings(COMPANY_ID, new List<LifeCycleSetting>() { lifeCycleSettingOne }, disposeSetting);

        var lifeCycleSettingTwo = new LifeCycleSetting(1, true, 700M, 24, Guid.Empty);
        var customerSettingTwo = new CustomerSettings(ORGANIZATION_ID, new List<LifeCycleSetting>() { lifeCycleSettingTwo });


        dbContext.Users.AddRange(userOne);
        dbContext.Assets.AddRange(assetOne, assetTwo, assetThree,assetFour ,assetFive);
        dbContext.AssetLifeCycles.AddRange(assetLifecycleOne, assetLifecycleTwo, assetLifecycleThree,
            assetLifecycleFour, assetLifecycleFive, assetLifecycleSix, assetLifecycleSeven, assetLifecycleEight, assetLifecycleNine);
        dbContext.CustomerSettings.AddRange(customerSettingOne, customerSettingTwo);

        var label = new CustomerLabel(COMPANY_ID, CALLER_ID, new Label("CompanyOne", LabelColor.Lightblue));

        dbContext.CustomerLabels.AddRange(labelOne, labelTwo);

        dbContext.SaveChanges();
    }

    public static void ResetDbForTests(AssetsContext dbContext)
    {
        var customerSettings = dbContext.CustomerSettings.ToArray();
        var assets = dbContext.Assets.ToArray();
        var mobilePhones = dbContext.MobilePhones.ToArray();
        var assetLifeCycles = dbContext.AssetLifeCycles.ToArray();
        var users = dbContext.Users.ToArray();
        var labels = dbContext.CustomerLabels.ToArray();

        dbContext.CustomerSettings.RemoveRange(customerSettings);
        dbContext.AssetLifeCycles.RemoveRange(assetLifeCycles);
        dbContext.MobilePhones.RemoveRange(mobilePhones);
        dbContext.Assets.RemoveRange(assets);
        dbContext.Users.RemoveRange(users);
        dbContext.CustomerLabels.RemoveRange(labels);

        dbContext.SaveChanges();

        InitialiseDbForTests(dbContext);
    }
}