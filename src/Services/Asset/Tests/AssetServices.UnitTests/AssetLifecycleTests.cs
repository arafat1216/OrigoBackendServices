using System;
using System.Collections.Generic;
using System.Linq;
using AssetServices.DomainEvents.AssetLifecycleEvents;
using AssetServices.Exceptions;
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
            Runtime = 36,
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

    [Fact]
    public void AssignLifecycleHolder_NoHolderSetThenSetUser_CheckDomainEventsCreated()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        var user = new User() { ExternalId = userId };

        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        // Act
        assetLifecycle.AssignAssetLifecycleHolder(user, null, Guid.Empty);

        // Assert
        Assert.Equal(2, assetLifecycle.DomainEvents.Count);
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(AssignContractHolderToAssetLifeCycleDomainEvent));
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(UpdateAssetLifecycleStatusDomainEvent));
        Assert.Equal(userId, assetLifecycle.ContractHolderUser!.ExternalId);
        Assert.Null(assetLifecycle.ManagedByDepartmentId);
    }

    [Fact]
    public void AssignLifecycleHolder_PersonalHolderSetThenSetUser_CheckDomainEventsCreated()
    {
        // Arrange
        Guid newUserId = Guid.NewGuid();
        var user = new User() { ExternalId = newUserId };

        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional,
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        // Act
        assetLifecycle.AssignAssetLifecycleHolder(user, null, Guid.Empty);

        // Assert
        Assert.Equal(2, assetLifecycle.DomainEvents.Count);
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(AssignContractHolderToAssetLifeCycleDomainEvent));
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(UpdateAssetLifecycleStatusDomainEvent));
        Assert.Equal(newUserId, assetLifecycle.ContractHolderUser!.ExternalId);
        Assert.Null(assetLifecycle.ManagedByDepartmentId);
    }

    [Fact]
    public void AssignLifecycleHolder_PersonalHolderWithDepartment_CheckDepartmentSet()
    {
        // Arrange
        Guid newUserId = Guid.NewGuid();
        Guid newDepartmentId = Guid.NewGuid();
        var user = new User() { ExternalId = newUserId };

        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional,
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        // Act
        assetLifecycle.AssignAssetLifecycleHolder(user, newDepartmentId, Guid.Empty);

        // Assert
        Assert.Equal(newDepartmentId, assetLifecycle.ManagedByDepartmentId);
    }

    [Fact]
    public void CreateAsset_IsSentToRepair_CheckDomainEventsCreated()
    {
        // Arrange
        Guid callerId = Guid.NewGuid();
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        // Act
        assetLifecycle.IsSentToRepair(callerId);

        // Assert
        Assert.Equal(2, assetLifecycle.DomainEvents.Count);
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(UpdateAssetLifecycleStatusDomainEvent));
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(AssetSentToRepairDomainEvent));
    }

    [Fact]
    public void CreateAsset_HasBeenStolen_CheckDomainEventsCreated()
    {
        // Arrange
        Guid callerId = Guid.NewGuid();
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        // Act
        assetLifecycle.HasBeenStolen(callerId);

        // Assert
        Assert.Equal(2, assetLifecycle.DomainEvents.Count);
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(UpdateAssetLifecycleStatusDomainEvent));
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(AssetHasBeenStolenDomainEvent));
    }

    [Fact]
    public void CreateAsset_HasBeenStolen_CheckStateAndStatus()
    {
        // Arrange
        Guid callerId = Guid.NewGuid();
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        // Act
        assetLifecycle.HasBeenStolen(callerId);

        // Assert
        Assert.Equal(AssetLifecycleStatus.Stolen, assetLifecycle.AssetLifecycleStatus);
        Assert.False(assetLifecycle.IsActiveState);
    }

    [Fact]
    public void CreateAsset_Transactional_CheckStateAndStatus()
    {
        // Arrange
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional,
        };

        // Act
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        // Assert
        Assert.Equal(AssetLifecycleStatus.InputRequired, assetLifecycle.AssetLifecycleStatus);
        Assert.True(assetLifecycle.IsActiveState);
    }
    [Fact]
    public void RepairCompleted_ChangeStatusAndCheckDomainEventsCreated()
    {
        // Arrange
        Guid callerId = Guid.NewGuid();
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        //Act
        assetLifecycle.IsSentToRepair(callerId);
        assetLifecycle.RepairCompleted(callerId, false);

        //Assert
        Assert.Equal(AssetLifecycleStatus.InUse, assetLifecycle.AssetLifecycleStatus);
        Assert.Equal(4, assetLifecycle.DomainEvents.Count);
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(UpdateAssetLifecycleStatusDomainEvent));
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(AssetSentToRepairDomainEvent));
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(AssetRepairCompletedDomainEvent));

    }

    [Fact]
    public void RepairCompleted_ShouldBeAllowedButNotChange_AlsoWhenStatusIsNotRepair()
    {
        // Arrange
        Guid callerId = Guid.NewGuid();
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);
        Models.Asset asset = new MobilePhone(Guid.NewGuid(), callerId, "123Serial", "Apple", "IPhone X",
                    new List<AssetImei>() { new AssetImei(332226834371155) }, "01:56:23:98:45:AB");
        assetLifecycle.AssignAsset(asset, callerId);
        assetLifecycle.AssignAssetLifecycleHolder(null, Guid.NewGuid(), callerId);

        bool discarded = false;

        Assert.Equal(AssetLifecycleStatus.InUse, assetLifecycle.AssetLifecycleStatus);

        //Act
        assetLifecycle.RepairCompleted(callerId, discarded);

        //Assert
        Assert.Equal(AssetLifecycleStatus.InUse, assetLifecycle.AssetLifecycleStatus);
        Assert.Equal(3, assetLifecycle.DomainEvents.Count);
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(UpdateAssetLifecycleStatusDomainEvent));
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(AssignAssetToAssetLifeCycleDomainEvent));
        Assert.Contains(assetLifecycle.DomainEvents, e => e.GetType() == typeof(AssignDepartmentAssetLifecycleDomainEvent));
    }
    [Fact]
    public void RepairCompleted_ThrowInvalidAssetDataException_WhenStatusIsNotValid()
    {
        // Arrange
        Guid callerId = Guid.NewGuid();
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        //Act
        assetLifecycle.HasBeenStolen(callerId);

        //Assert
        var exception = Assert.Throws<InvalidAssetDataException>(
            () => assetLifecycle.RepairCompleted(callerId, false));

        Assert.Equal("Invalid asset lifecycle status: Stolen for completing return.", exception.Message);
    }
    [Fact]
    public void IsSentToRepair_ThrowInvalidAssetDataException_WhenStatusIsNotValid()
    {
        // Arrange
        Guid callerId = Guid.NewGuid();
        var createAssetLifecycleDTO = new CreateAssetLifecycleDTO
        {
            LifecycleType = LifecycleType.Transactional
        };
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(createAssetLifecycleDTO);

        //Act
        assetLifecycle.HasBeenStolen(callerId);

        //Assert
        var exception = Assert.Throws<InvalidAssetDataException>(
            () => assetLifecycle.IsSentToRepair(callerId));

        Assert.Equal("Invalid asset lifecycle status: Stolen for sending asset lifecycle on repair.", exception.Message);
    }
}