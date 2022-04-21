using System;
using System.Collections.Generic;
using System.Linq;
using AssetServices.DomainEvents.AssetLifecycleEvents;
using Common.Enums;
using Common.Seedwork;

namespace AssetServices.Models;

public class AssetLifecycle : Entity, IAggregateRoot
{
    /// <summary>
    /// The external uniquely identifying id across systems.
    /// </summary>
    public Guid ExternalId { get; private set; } = Guid.NewGuid();
    /// <summary>
    /// The customer for this asset lifecycle.
    /// </summary>
    public Guid CustomerId { get; init; }
    /// <summary>
    /// The name/reference of the contract signed for these asset lifecycles.
    /// </summary>
    public string ContractReferenceName { get; init; } = string.Empty;
    /// <summary>
    /// An alias describing this asset lifecycle.
    /// </summary>
    public string Alias { get; init; } = string.Empty;
    /// <summary>
    /// The start period for this asset lifecycle.
    /// </summary>
    public DateTime StartPeriod { get; init; }
    /// <summary>
    /// The end period for this asset lifecycle.
    /// </summary>
    public DateTime EndPeriod { get; init; }
    /// <summary>
    /// The purchase date of the asset lifecycle.
    /// </summary>
    public DateTime PurchaseDate { get; init; }
    /// <summary>
    /// A comment related to the asset lifecycle.
    /// </summary>
    public string Note { get; init; } = string.Empty;
    /// <summary>
    /// A description of this asset lifecycle.
    /// </summary>
    public string Description { get; init; } = string.Empty;
    /// <summary>
    /// The Currency code related to this asset lifecycle.
    /// </summary>
    public CurrencyCode CurrencyCode { get; init; }

    /// <summary>
    /// the amount that company covered/paid for the asset's overall cost.
    /// </summary>
    public decimal PaidByCompany { get; init; } = decimal.Zero;



    /// <summary>
    /// The asset currently associated with this asset lifecycle.
    /// </summary>
    public Asset? Asset { get; private set; }

    /// <summary>
    /// Return a category id based on the type of the asset.
    /// </summary>
    public int AssetCategoryId
    {
        get
        {
            return Asset switch
            {
                MobilePhone => 1,
                Tablet => 2,
                _ => 0
            };
        }
    }

    /// <summary>
    /// Return the calculated book value of the asset.
    /// </summary>
    public decimal BookValue
    {
        get
        {
            if (AssetLifecycleType != LifecycleType.Transactional)
                return 0;
            int differenceInMonth = ((DateTime.UtcNow.Year - PurchaseDate.Year) * 12) + DateTime.UtcNow.Month - PurchaseDate.Month;
            decimal bookValue = PaidByCompany - (PaidByCompany / 36 * differenceInMonth);
            return bookValue < 0 ? 0 : decimal.Round(bookValue, 2, MidpointRounding.AwayFromZero);
        }
    }

    /// <summary>
    /// Return the calculated BuyOut Price of the asset.
    /// </summary>
    public decimal BuyoutPrice
    {
        get
        {
            var vat = VATConfiguration.Amount;
            var buyOutPrice = BookValue * vat;
            return buyOutPrice < 0 ? 0 : decimal.Round(buyOutPrice, 2, MidpointRounding.AwayFromZero);
        }
    }



    /// <summary>
    /// Return the name of the category based on the type of the asset.
    /// </summary>
    public string AssetCategoryName
    {
        get
        {
            return Asset switch
            {
                MobilePhone => "Mobile phone",
                Tablet => "Tablet",
                _ => "Unknown"
            };
        }
    }

    /// <summary>
    /// The user which is now in control of the asset.
    /// </summary>
    public User? ContractHolderUser { get; private set; }

    public Guid? ManagedByDepartmentId { get; private set; }

    /// <summary>
    /// The current status of this asset lifecycle
    /// </summary>
    public AssetLifecycleStatus AssetLifecycleStatus
    {
        get => _assetLifecycleStatus;
        init => _assetLifecycleStatus = value;
    }
    private AssetLifecycleStatus _assetLifecycleStatus;

    /// <summary>
    /// The asset lifecycle type this asset lifecycle is setup with.
    /// </summary>
    public LifecycleType AssetLifecycleType
    {
        get => _assetLifecycleType;
        init => _assetLifecycleType = value;
    }
    private LifecycleType _assetLifecycleType;

    private readonly List<CustomerLabel> _labels = new();

    public AssetLifecycle(Guid externalId)
    {
        ExternalId = externalId;
    }

    public AssetLifecycle()
    {
        
    }

    /// <summary>
    /// All labels associated with this asset lifecycle.
    /// </summary>
    public IReadOnlyCollection<CustomerLabel> Labels => _labels.AsReadOnly();

    /// <summary>
    /// Assign an asset to this asset lifecycle.
    /// </summary>
    /// <param name="asset">The actual asset being linked to this asset lifecycle</param>
    /// <param name="callerId">The userid making this assignment</param>
    public void AssignAsset(Asset asset, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousAssetId = Asset?.ExternalId;
        Asset = asset;
        if (asset is MobilePhone mobilePhone && !mobilePhone.Imeis.Any())
        {
            _assetLifecycleStatus = AssetLifecycleStatus.InputRequired;
        }
        AddDomainEvent(new AssignAssetToAssetLifeCycleDomainEvent(this, callerId, previousAssetId));
    }

    /// <summary>
    /// Assign a contract holder which is in control of the asset. 
    /// </summary>
    /// <param name="contractHolderUser">A user</param>
    /// <param name="callerId">The userid making this assignment</param>
    public void AssignContractHolder(User contractHolderUser, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousContractHolderUser = ContractHolderUser;
        ContractHolderUser = contractHolderUser;
        AddDomainEvent(new AssignContractHolderToAssetLifeCycleDomainEvent(this, callerId, previousContractHolderUser));
    }

    /// <summary>
    /// Un-Assign a contract holder which is in control of the asset. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void UnAssignContractHolder(Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        AddDomainEvent(new UnAssignContractHolderToAssetLifeCycleDomainEvent(this, callerId, ContractHolderUser));
        ContractHolderUser = null;
    }

    /// <summary>
    /// Making this asset available for other user/department to grab. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void MakeAssetAvailable(Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        var previousContractHolderUser = ContractHolderUser;
        AddDomainEvent(new MakeAssetAvailableDomainEvent(this, callerId, previousLifecycleStatus, previousContractHolderUser));
        ContractHolderUser = null;
        if(_labels != null && _labels.Any())
            _labels.Clear();
        _assetLifecycleStatus = AssetLifecycleStatus.Available;
    }


    /// <summary>
    /// Assign a contract holder which is in control of the asset. 
    /// </summary>
    /// <param name="contractHolderUser">A user</param>
    /// <param name="callerId">The userid making this assignment</param>
    public void AssignDepartment(Guid departmentId, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousDepartmentId = ManagedByDepartmentId;
        ManagedByDepartmentId = departmentId;
        AddDomainEvent(new AssignDepartmentAssetLifecycleDomainEvent(this, callerId, previousDepartmentId));
    }

    /// <summary>
    /// Assign a customer label for this asset lifecycle.
    /// </summary>
    /// <param name="customerLabel">The label to assign to the asset lifecycle</param>
    /// <param name="callerId">The userid making this assignment</param>
    public void AssignCustomerLabel(CustomerLabel customerLabel, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        _labels.Add(customerLabel);
        AddDomainEvent(new AssignLabelToAssetLifecycleDomainEvent(this, callerId, customerLabel));
    }

    /// <summary>
    /// Remove a customer label to an asset lifecycle.
    /// </summary>
    /// <param name="customerLabel"></param>
    /// <param name="callerId"></param>
    public void RemoveCustomerLabel(CustomerLabel customerLabel, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        _labels.Remove(customerLabel);
        AddDomainEvent(new RemoveLabelAssignmentForAssetLifecycleDomainEvent(this, callerId, customerLabel));
    }

    public void AssignLifecycleType(LifecycleType lifecycleType, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        _assetLifecycleType = lifecycleType;
        AddDomainEvent(new AssignLifecycleTypeToAssetLifecycleDomainEvent(this, callerId, lifecycleType));
    }

    public void UpdateAssetStatus(AssetLifecycleStatus lifecycleStatus, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        _assetLifecycleStatus = lifecycleStatus;
        AddDomainEvent(new AssignLifecycleStatusToAssetLifecycleDomainEvent(this, callerId, previousLifecycleStatus));
    }
}