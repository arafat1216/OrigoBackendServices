using System;
using System.Linq;
using AssetServices.Models;
using AssetServices.ServiceModel;
using Common.Enums;
using Xunit;

namespace AssetServices.UnitTests;

public class AssetLifecycleTests
{
    [Fact]
    public void CreateAsset_WithTransactionalPurchaseDate_CheckStartPeriod()
    {
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            PurchaseDate = new DateTime(2022, 2, 12),
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifeCycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);
        Assert.Equal(new DateTime(2022, 3, 1).ToLongDateString(), assetLifeCycle.StartPeriod!.Value.ToLongDateString());
    }

    [Fact]
    public void CreateAsset_WithPurchaseDateAndNoLifecycle_CheckStartPeriod()
    {
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            PurchaseDate = new DateTime(2022, 2, 12),
            LifecycleType = LifecycleType.NoLifecycle
        };
        var assetLifeCycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);
        Assert.Null(assetLifeCycle.StartPeriod);
    }

    [Fact]
    public void CreateAsset_WithPurchaseDateAndNoLifecycle_CheckEndPeriod()
    {
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            PurchaseDate = new DateTime(2022, 2, 12),
            LifecycleType = LifecycleType.NoLifecycle
        };
        var assetLifeCycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);
        Assert.Null(assetLifeCycle.EndPeriod);
    }

    [Fact]
    public void CreateAsset_WithTransactionalPurchaseDateAndRuntime_CheckEndPeriod()
    {
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            PurchaseDate = new DateTime(2022, 2, 12),
            MonthlySalaryDeductionRuntime = 36,
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifeCycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);
        Assert.Equal(new DateTime(2025, 2, 28).ToLongDateString(), assetLifeCycle.EndPeriod!.Value.ToLongDateString());
    }

    [Fact]
    public void CreateAsset_WithTransactionalSalaryDeduction_CheckTransactionList()
    {
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            PurchaseDate = new DateTime(2022, 2, 12),
            MonthlySalaryDeduction = 120,
            MonthlySalaryDeductionRuntime = 36,
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifeCycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);
        Assert.Equal(36, assetLifeCycle.SalaryDeductionTransactionList.Count);
        Assert.Equal(120, assetLifeCycle.SalaryDeductionTransactionList.First().Amount);
        Assert.Equal(3, assetLifeCycle.SalaryDeductionTransactionList.First().Month);
        Assert.Equal(2022, assetLifeCycle.SalaryDeductionTransactionList.First().Year);
        Assert.False(assetLifeCycle.SalaryDeductionTransactionList.First().Cancelled);

        Assert.Equal(120, assetLifeCycle.SalaryDeductionTransactionList.Last().Amount);
        Assert.Equal(2, assetLifeCycle.SalaryDeductionTransactionList.Last().Month);
        Assert.Equal(2025, assetLifeCycle.SalaryDeductionTransactionList.Last().Year);
        Assert.False(assetLifeCycle.SalaryDeductionTransactionList.Last().Cancelled);
    }
}