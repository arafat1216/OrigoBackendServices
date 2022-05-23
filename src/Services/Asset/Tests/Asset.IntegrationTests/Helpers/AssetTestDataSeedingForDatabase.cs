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
    public static readonly Guid DEPARTMENT_ID = new("6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72");
    public static readonly Guid DEPARTMENT_TWO_ID = new("fe625c35-91d0-448e-a803-0dcbbd97f1d5");
    public static readonly Guid COMPANY_ID = new("cab4bb77-3471-4ab3-ae5e-2d4fce450f36");
    public static readonly Guid ASSETHOLDER_ONE_ID = new("6d16a4cb-4733-44de-b23b-0eb9e8ae6590");

    public static void InitialiseDbForTests(AssetsContext dbContext)
    {
        dbContext.Database.EnsureCreated();
        var assetOne = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012345", "Samsung", "Samsung Galaxy S20",
            new List<AssetImei> { new(500119468586675) }, "B26EDC46046B");

        var assetTwo = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012364", "Apple", "Apple iPhone 8",
            new List<AssetImei> { new(546366434558702) }, "487027C99FA1");

        var assetThree = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012399", "Samsung", "Samsung Galaxy S21",
            new List<AssetImei> { new(512217111821626) }, "840F1D0C06AD");

        var assetFour = new MobilePhone(Guid.NewGuid(), CALLER_ID, "123456789012397", "Apple", "iPhone 11 Pro",
            new List<AssetImei> { new(512217111821624) }, "840F1D0C06AB");

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
            AssetLifecycleStatus = AssetLifecycleStatus.Available,
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

        var lifeCycleSetting = new LifeCycleSetting(COMPANY_ID, 1, true, 700M, Guid.Empty);

        dbContext.Users.AddRange(userOne);
        dbContext.Assets.AddRange(assetOne, assetTwo, assetThree);
        dbContext.AssetLifeCycles.AddRange(assetLifecycleOne, assetLifecycleTwo, assetLifecycleThree,
            assetLifecycleFour, assetLifecycleFive);
        dbContext.LifeCycleSettings.AddRange(lifeCycleSetting);

        var label = new CustomerLabel(COMPANY_ID, CALLER_ID, new Label("CompanyOne", LabelColor.Lightblue));
        dbContext.CustomerLabels.Add(label);

        dbContext.SaveChanges();
    }

    public static void ResetDbForTests(AssetsContext dbContext)
    {
        var lifeCycleSettings = dbContext.LifeCycleSettings.ToArray();
        var assets = dbContext.Assets.ToArray();
        var mobilePhones = dbContext.MobilePhones.ToArray();
        var assetLifeCycles = dbContext.AssetLifeCycles.ToArray();
        var users = dbContext.Users.ToArray();
        var labels = dbContext.CustomerLabels.ToArray();

        dbContext.LifeCycleSettings.RemoveRange(lifeCycleSettings);
        dbContext.AssetLifeCycles.RemoveRange(assetLifeCycles);
        dbContext.MobilePhones.RemoveRange(mobilePhones);
        dbContext.Assets.RemoveRange(assets);
        dbContext.Users.RemoveRange(users);
        dbContext.CustomerLabels.RemoveRange(labels);

        dbContext.SaveChanges();

        InitialiseDbForTests(dbContext);
    }
}