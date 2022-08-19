using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using AssetServices.DomainEvents;
using AssetServices.DomainEvents.AssetLifecycleEvents;
using AssetServices.DomainEvents.EndOfLifeCycleEvents;
using AssetServices.Exceptions;
using AssetServices.ServiceModel;
using Common.Enums;
using Common.Seedwork;

namespace AssetServices.Models;

public class AssetLifecycle : Entity, IAggregateRoot
{
    public AssetLifecycle(Guid externalId)
    {
        ExternalId = externalId;
    }

    public AssetLifecycle()
    {
    }

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
    public string Alias
    {
        get => _alias;
        init => _alias = value;
    }
    private string _alias;
    /// <summary>
    /// The start period for this asset lifecycle.
    /// </summary>
    public DateTime? StartPeriod { get; init; }
    /// <summary>
    /// The end period for this asset lifecycle.
    /// </summary>
    public DateTime? EndPeriod { get; init; }
    /// <summary>
    /// The purchase date of the asset lifecycle.
    /// </summary>
    public DateTime PurchaseDate
    {
        get => _purchaseDate;
        init => _purchaseDate = value;
    }
    private DateTime _purchaseDate;
    /// <summary>
    /// A comment related to the asset lifecycle.
    /// </summary>
    public string Note
    {
        get => _note;
        init => _note = value;
    }
    private string _note = string.Empty;
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
    /// the buyout amount that will be deduce after last working day
    /// </summary>
    public decimal OffboardBuyoutPrice { get; private set; } = decimal.Zero;

    /// <summary>
    /// Is a personal or non-personal asset.
    /// </summary>
    public bool IsPersonal { get; private set; }
    
    /// <summary>
    /// The asset currently associated with this asset lifecycle.
    /// </summary>
    public Asset? Asset { get; private set; }

    /// <summary>
    /// Which source was this fetched from.
    /// </summary>
    public AssetLifeCycleSource Source { get; set; }

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
            var differenceInMonth = ((DateTime.UtcNow.Year - PurchaseDate.Year) * 12) + DateTime.UtcNow.Month - PurchaseDate.Month;
            var bookValue = PaidByCompany - (PaidByCompany / 36 * differenceInMonth);
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
    /// Checks if the asset lifecycle status being passed is active state.
    /// </summary>
    /// <param name="assetLifecycleStatus">The asset lifecycle status to check if it is a active state.</param>
    /// <returns>True if the asset lifecycle has an active state. False if the asset lifecycle has an inactive state.</returns>
    public static bool HasActiveState(AssetLifecycleStatus assetLifecycleStatus)
    {
        return assetLifecycleStatus is AssetLifecycleStatus.InputRequired or AssetLifecycleStatus.InUse
            or AssetLifecycleStatus.Repair or AssetLifecycleStatus.PendingReturn or AssetLifecycleStatus.PendingBuyout
            or AssetLifecycleStatus.Available or AssetLifecycleStatus.Active or AssetLifecycleStatus.ExpiresSoon 
            or AssetLifecycleStatus.PendingRecycle or AssetLifecycleStatus.Expired or AssetLifecycleStatus.ExpiresSoon;
    }

    /// <summary>
    /// Returns the state of the AssetLifecycle.
    /// </summary>
    public bool IsActiveState => HasActiveState(AssetLifecycleStatus);

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

    /// <summary>
    /// All labels associated with this asset lifecycle.
    /// </summary>
    public IReadOnlyCollection<CustomerLabel> Labels => _labels.AsReadOnly();

    /// <summary>
    ///     Order# this asset was part of.
    /// </summary>
    public string OrderNumber { get; init; } = string.Empty;

    /// <summary>
    ///     The product id for the asset.
    /// </summary>
    public string ProductId { get; init; } = string.Empty;

    /// <summary>
    ///     The invoice# this asset will be invoiced on.
    /// </summary>
    public string InvoiceNumber { get; init; } = string.Empty;

    /// <summary>
    ///     A unique id for the transaction where this asset was requested for.
    /// </summary>
    public string TransactionId { get; init; } = string.Empty;

    private List<SalaryDeductionTransaction> _salaryDeductionTransactionList = new();

    public IReadOnlyCollection<SalaryDeductionTransaction> SalaryDeductionTransactionList =>
        _salaryDeductionTransactionList.AsReadOnly();

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
        AddDomainEvent(new AssignAssetToAssetLifeCycleDomainEvent(this,callerId, previousAssetId));
    }

    /// <summary>
    /// Assign a contract holder which is in control of the asset. 
    /// </summary>
    /// <param name="contractHolderUser">Contract holder is a user</param>
    /// <param name="departmentId">Contract holder is a department</param>
    /// <param name="callerId">The userid making this assignment</param>
    public void AssignAssetLifecycleHolder(User? contractHolderUser, Guid? departmentId, Guid callerId)
    {
        // Assign to user if user is set.
        if (contractHolderUser != null && contractHolderUser != ContractHolderUser)
        {
            // Unassign previous owner and add domain events for it - cant have two owners
            if (ContractHolderUser != null) AddDomainEvent(new UnAssignContractHolderToAssetLifeCycleDomainEvent(this, callerId, ContractHolderUser));
            if (ManagedByDepartmentId != null) AddDomainEvent(new UnAssignDepartmentAssetLifecycleDomainEvent(this, callerId));
            ManagedByDepartmentId = departmentId;
            IsPersonal = true;
            ContractHolderUser = contractHolderUser;
            var previousContractHolderUser = ContractHolderUser;
            IsPersonal = true;

            if (_assetLifecycleStatus != AssetLifecycleStatus.InUse && _assetLifecycleType != LifecycleType.NoLifecycle) UpdateAssetStatus(AssetLifecycleStatus.InUse,callerId);

            AddDomainEvent(new AssignContractHolderToAssetLifeCycleDomainEvent(this, callerId, previousContractHolderUser));
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }
        else if (contractHolderUser != null)
        {
            ManagedByDepartmentId = departmentId;
        }
        else if (departmentId != null && departmentId != Guid.Empty)
        {
            //unassign previous owner and add domain events for it
            if (ContractHolderUser != null) AddDomainEvent(new UnAssignContractHolderToAssetLifeCycleDomainEvent(this,callerId, ContractHolderUser));
            if (ManagedByDepartmentId != null) AddDomainEvent(new UnAssignDepartmentAssetLifecycleDomainEvent(this,callerId));
            ContractHolderUser = null;
            
            
            var previousDepartmentId = ManagedByDepartmentId;
            ManagedByDepartmentId = departmentId;
            IsPersonal = false;

            //Change state to in use if not no lifecycle
            if(_assetLifecycleStatus != AssetLifecycleStatus.InUse && _assetLifecycleType != LifecycleType.NoLifecycle) UpdateAssetStatus(AssetLifecycleStatus.InUse,callerId);

            AddDomainEvent(new AssignDepartmentAssetLifecycleDomainEvent(this, callerId, previousDepartmentId));
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }
    }

    public void UpdateAlias(string alias, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousAlias = _alias;
        _alias = alias;
        AddDomainEvent(new ChangedAliasDomainEvent(this, callerId, previousAlias));
    }

    public void UpdatePurchaseDate(DateTime purchaseDate, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousPurchaseDate = PurchaseDate;
        _purchaseDate = purchaseDate;
        AddDomainEvent(new ChangedPurchaseDateDomainEvent(this, callerId, previousPurchaseDate));
    }
    
    /// <summary>
    /// Update or Creating a Note/Comment for an asset lifecycle
    /// </summary>
    /// <param name="note">The Updated note/comment made for the asset lifecycle<see cref="Note"/></param>
    /// <param name="callerId">The userid making this assignment</param>
    public void UpdateNote(string note, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousNote = Note;
        _note = note;
        AddDomainEvent(new ChangedNoteDomainEvent(this, callerId, previousNote));
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
        if(Labels.Any())
            _labels.Clear();
        _assetLifecycleStatus = AssetLifecycleStatus.Available;
    }

    /// <summary>
    /// Making this asset expired for user/department. 
    /// </summary>
    /// <param name="callerId">The userid making this request</param>
    public void MakeAssetExpired(Guid callerId)
    {
        if (_assetLifecycleType == LifecycleType.NoLifecycle)
            throw new AssetExpireRequestException("Asset has No Life Cycle to Expire", Guid.Parse("57421580-2f0f-4e94-b8cd-ed26e4b7ee64"));
        if (!IsActiveState)
            throw new InvalidOperationForInactiveState($"{_assetLifecycleStatus}", ExternalId, Guid.Parse("4c4af52f-2b2a-44a5-bad0-11f8a9df5772"));
        if (_assetLifecycleStatus != AssetLifecycleStatus.ExpiresSoon)
            throw new AssetExpireRequestException("Asset does not have 'ExpiresSoon' status", Guid.Parse("0a3cb7d5-8ceb-46ba-841b-056e9aaf40d1"));
        if (EndPeriod.HasValue && (int) (EndPeriod.Value - DateTime.UtcNow).TotalDays >= 0)
            throw new AssetExpireRequestException("Asset is not expiring.", Guid.Parse("7d0860fa-9144-4f26-b9dc-cbaaf80af5a2"));
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        AddDomainEvent(new MakeAssetExpiredDomainEvent(this, callerId));
        _assetLifecycleStatus = AssetLifecycleStatus.Expired;
    }

    /// <summary>
    /// Making this asset expired for user/department. 
    /// </summary>
    /// <param name="callerId">The userid making this request</param>
    public void MakeAssetExpiresSoon(Guid callerId)
    {
        if (_assetLifecycleType == LifecycleType.NoLifecycle)
            throw new AssetExpiresSoonRequestException("Asset has No Life Cycle that can expires soon", Guid.Parse("51307f19-ee44-4897-a90c-bb8eca6cb44e"));
        if (!IsActiveState)
            throw new InvalidOperationForInactiveState($"{_assetLifecycleStatus}", ExternalId, Guid.Parse("46b0157b-9366-4aa8-9bbb-1b9f77962378"));

        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        AddDomainEvent(new MakeAssetExpiresSoonDomainEvent(this, callerId));
        _assetLifecycleStatus = AssetLifecycleStatus.ExpiresSoon;
    }

    /// <summary>
    /// Making this asset's return request for managers to Confirm. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void MakeReturnRequest(Guid callerId)
    {
        if (_assetLifecycleType != LifecycleType.Transactional)
            throw new ReturnDeviceRequestException($"Only Assets that have Transactionl Life cycle type can make return request!!! asset Id: {ExternalId}", Guid.Parse("af416bd6-0879-4f26-9dba-176f7d01e971"));

        if (!IsActiveState)
            throw new InvalidOperationForInactiveState($"{_assetLifecycleStatus}", ExternalId, Guid.Parse("fdabaf1c-7586-4c01-89cf-80efc6220cce"));

        if (_assetLifecycleStatus != AssetLifecycleStatus.ExpiresSoon)
            throw new ReturnDeviceRequestException($"Asset is not Expiring Soon to make return request!!! asset Id: {ExternalId}", Guid.Parse("e1e4a4a3-af75-4a14-a9d0-24d354f550f6"));

        if (_assetLifecycleStatus == AssetLifecycleStatus.PendingReturn)
            throw new ReturnDeviceRequestException($"Asset already have pending return request!!! asset Id: {ExternalId}", Guid.Parse("a6def7ce-75bd-4985-96a1-f0d9d9fe7299"));

        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        AddDomainEvent(new MakeReturnRequestDomainEvent(this, callerId, previousLifecycleStatus));
        _assetLifecycleStatus = AssetLifecycleStatus.PendingReturn;
    }

    /// <summary>
    /// Confirm thi asset as Retrned Device. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void ConfirmReturnDevice(Guid callerId, string locationName, string description)
    {
        if (_assetLifecycleType != LifecycleType.Transactional)
            throw new ReturnDeviceRequestException($"Only Assets that have Transactionl Life cycle type can make return request!!! asset Id: {ExternalId}", Guid.Parse("9ca1610a-16ba-432a-9e8a-f601933ba7a1"));

        if (_assetLifecycleStatus != AssetLifecycleStatus.ExpiresSoon && _assetLifecycleStatus != AssetLifecycleStatus.PendingReturn)
            throw new ReturnDeviceRequestException($"Asset is not Expiring Soon and does not have pending return request!!! asset Id: {ExternalId}", Guid.Parse("46d0a2b2-73a9-40b7-9ccf-3d44e6c0a35f"));

        if (!IsActiveState)
            throw new InvalidOperationForInactiveState($"{_assetLifecycleStatus}", ExternalId, Guid.Parse("d7f038ac-0af5-4018-a3bc-6d851772b15c"));

        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        AddDomainEvent(new ConfirmReturnDeviceDomainEvent(this, callerId, previousLifecycleStatus, locationName, description));
        _assetLifecycleStatus = AssetLifecycleStatus.Returned;
    }
    
    /// <summary>
    /// User has buought out this asset.. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void BuyoutDevice(Guid callerId)
    {
        if (!IsActiveState)
            throw new InvalidOperationForInactiveState($"{_assetLifecycleStatus}", ExternalId, Guid.Parse("950d05f4-ca1b-43ed-86ea-937c1b1da98d"));

        if (!IsPersonal || ContractHolderUser is null)
            throw new BuyoutDeviceRequestException($"Only Personal Assets can be bought out!!! asset Id: {ExternalId}", Guid.Parse("1c7358ca-e3bf-44eb-89c3-977004bdb749"));

        if (_assetLifecycleType != LifecycleType.Transactional)
            throw new BuyoutDeviceRequestException($"Only Assets that have Transactionl Life cycle type can be bought out!!! asset Id: {ExternalId}", Guid.Parse("1462a412-af22-4d2f-94c4-082f658400d3"));

        if (_assetLifecycleStatus != AssetLifecycleStatus.ExpiresSoon)
            throw new BuyoutDeviceRequestException($"Asset is not Expiring Soon to do buyout!!! asset Id: {ExternalId}", Guid.Parse("2428fe7a-ec4d-440c-8e7b-79092ca3800e"));

        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        AddDomainEvent(new BuyoutDeviceDomainEvent(this, callerId, BuyoutPrice, previousLifecycleStatus));
        _assetLifecycleStatus = AssetLifecycleStatus.BoughtByUser;
    }

    /// <summary>
    /// User has buought out this asset.. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void ConfirmBuyoutDevice(Guid callerId)
    {
        if (_assetLifecycleStatus != AssetLifecycleStatus.PendingBuyout)
            throw new BuyoutDeviceRequestException($"Asset is not pending to do buyout!!! asset Id: {ExternalId}", Guid.Parse("2428fe7a-ec4d-440c-8e7b-79092ca3800e"));

        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        AddDomainEvent(new BuyoutDeviceDomainEvent(this, callerId, OffboardBuyoutPrice, previousLifecycleStatus));
        _assetLifecycleStatus = AssetLifecycleStatus.BoughtByUser;
    }
    /// <summary>
    /// User has buought out this asset.. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void RequestPendingBuyout(DateTime lastWorkingDay, Guid callerId)
    {
        if (!IsActiveState)
            throw new InvalidOperationForInactiveState($"{_assetLifecycleStatus}", ExternalId, Guid.Parse("950d05f4-ca1b-43ed-86ea-937c1b1da98d"));

        if (!IsPersonal || ContractHolderUser is null)
            throw new BuyoutDeviceRequestException($"Only Personal Assets can be requested for bought out!!! asset Id: {ExternalId}", Guid.Parse("1c7358ca-e3bf-44eb-89c3-977004bdb749"));

        if (_assetLifecycleType != LifecycleType.Transactional)
            throw new BuyoutDeviceRequestException($"Only Assets that have Transactionl Life cycle type can be bought out!!! asset Id: {ExternalId}", Guid.Parse("1462a412-af22-4d2f-94c4-082f658400d3"));

        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        OffboardBuyoutPrice = BuyoutPriceByDate(lastWorkingDay);
        AddDomainEvent(new PendingBuyoutDeviceDomainEvent(this, callerId, previousLifecycleStatus));
        _assetLifecycleStatus = AssetLifecycleStatus.PendingBuyout;
    }

    /// <summary>
    /// User Offboarding cancelled.. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void OffboardingCancelled(Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        OffboardBuyoutPrice = decimal.Zero;
        AddDomainEvent(new OffboardingCancelledDomainEvent(this, callerId));
        ReactivatePendingAsset(callerId);
    }

    /// <summary>
    /// User has Reported this asset.. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void ReportDevice(ReportCategory reportCategory, Guid callerId)
    {
        if (!IsActiveState)
            throw new InvalidOperationForInactiveState($"{_assetLifecycleStatus}", ExternalId, Guid.Parse("1efc46cc-5045-4811-a49c-ec234ba3b9ef"));

        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        AddDomainEvent(new ReportDeviceDomainEvent(this, reportCategory, callerId, previousLifecycleStatus));
        if(reportCategory == ReportCategory.Stolen)
            _assetLifecycleStatus = AssetLifecycleStatus.Stolen;
        if (reportCategory == ReportCategory.Lost)
            _assetLifecycleStatus = AssetLifecycleStatus.Lost;
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
    /// Reactivating pending assets.
    /// </summary>
    public void ReactivatePendingAsset(Guid callerId)
    {
        if (AssetLifecycleStatus != AssetLifecycleStatus.PendingReturn || AssetLifecycleStatus != AssetLifecycleStatus.PendingBuyout)
            return;
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        AddDomainEvent(new ReactivatePendingAssetDomainEvent(this, callerId));
        UpdateAssetStatus(AssetLifecycleStatus.Active, callerId);
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

    [Obsolete("LifecycleType should be immutable for Asset Lifecycles")]
    public void AssignLifecycleType(LifecycleType lifecycleType, Guid callerId)
    {
        if (lifecycleType == AssetLifecycleType)
        {
            return;
        }
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        switch (lifecycleType)
        {
            case LifecycleType.Transactional when (IsPersonal && ContractHolderUser != null) || (!IsPersonal && (ManagedByDepartmentId != null && ManagedByDepartmentId != Guid.Empty)):
                UpdateAssetStatus(AssetLifecycleStatus.InUse, callerId);
                break;
            case LifecycleType.Transactional:
                UpdateAssetStatus(AssetLifecycleStatus.InputRequired, callerId);
                break;
            case LifecycleType.NoLifecycle:
                UpdateAssetStatus(AssetLifecycleStatus.Active, callerId);
                break;
            default:
                UpdateAssetStatus(AssetLifecycleStatus.Active, callerId);
                break;
        }

        _assetLifecycleType = lifecycleType;
        AddDomainEvent(new AssignLifecycleTypeToAssetLifecycleDomainEvent(this, callerId, lifecycleType));
    }

    private void UpdateAssetStatus(AssetLifecycleStatus lifecycleStatus, Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        _assetLifecycleStatus = lifecycleStatus;
        AddDomainEvent(new UpdateAssetLifecycleStatusDomainEvent(this, previousLifecycleStatus,callerId));
    }

    public void HasBeenStolen(Guid callerId)
    {
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        UpdateAssetStatus(AssetLifecycleStatus.Stolen, callerId);
        AddDomainEvent(new AssetHasBeenStolenDomainEvent(this, _assetLifecycleStatus, callerId));
    }

    public static AssetLifecycle CreateAssetLifecycle(CreateAssetLifecycleDTO assetLifecycleDTO)
    {
        var assetLifecycleStatus = assetLifecycleDTO.LifecycleType == LifecycleType.Transactional ? AssetLifecycleStatus.InputRequired : AssetLifecycleStatus.Active;
        var salaryDeductionTransactions = CreateSalaryDeductionTransactions(assetLifecycleDTO.PurchaseDate,
            assetLifecycleDTO.MonthlySalaryDeductionRuntime, assetLifecycleDTO.MonthlySalaryDeduction, assetLifecycleDTO.LifecycleType);
        return new AssetLifecycle
        {
            CustomerId = assetLifecycleDTO.CustomerId,
            Alias = assetLifecycleDTO.Alias,
            AssetLifecycleType = assetLifecycleDTO.LifecycleType,
            AssetLifecycleStatus = assetLifecycleStatus,
            PurchaseDate = assetLifecycleDTO.PurchaseDate,
            StartPeriod = GetStartPeriodFromPurchaseDate(assetLifecycleDTO.PurchaseDate, assetLifecycleDTO.LifecycleType),
            EndPeriod = GetEndPeriodFromPurchaseDate(assetLifecycleDTO.PurchaseDate, (assetLifecycleDTO.LifecycleType == LifecycleType.Transactional? assetLifecycleDTO.Runtime : assetLifecycleDTO.MonthlySalaryDeductionRuntime) , assetLifecycleDTO.LifecycleType),
            _salaryDeductionTransactionList = salaryDeductionTransactions,
            Note = assetLifecycleDTO.Note,
            Description = assetLifecycleDTO.Description,
            PaidByCompany = assetLifecycleDTO.PaidByCompany,
            OrderNumber = assetLifecycleDTO.OrderNumber ?? string.Empty,
            ProductId = assetLifecycleDTO.ProductId ?? string.Empty,
            InvoiceNumber = assetLifecycleDTO.InvoiceNumber ?? string.Empty,
            TransactionId = assetLifecycleDTO.TransactionId ?? string.Empty,
            Source = assetLifecycleDTO.Source,
            IsPersonal = assetLifecycleDTO.IsPersonal ?? assetLifecycleDTO.LifecycleType == LifecycleType.Transactional // Default is true only for transactional assets
        };
    }

    private static List<SalaryDeductionTransaction> CreateSalaryDeductionTransactions(DateTime purchaseDate,
        int? monthlySalaryDeductionRuntime, decimal? monthlySalaryDeduction, LifecycleType lifecycleType)
    {
        var startDate = GetStartPeriodFromPurchaseDate(purchaseDate, lifecycleType);
        if (lifecycleType == LifecycleType.NoLifecycle || !monthlySalaryDeductionRuntime.HasValue || !monthlySalaryDeduction.HasValue || startDate == null)
        {
            return new List<SalaryDeductionTransaction>();
        }
        var salaryDeductionList = new List<SalaryDeductionTransaction>();
        for (var monthIndex = 0; monthIndex < monthlySalaryDeductionRuntime; monthIndex++)
        {
            salaryDeductionList.Add(new SalaryDeductionTransaction
            {
                Amount = monthlySalaryDeduction.Value,
                CurrencyCode = CurrencyCode.NOK,
                Year = startDate.Value.AddMonths(monthIndex).Year,
                Month = startDate.Value.AddMonths(monthIndex).Month
            });
        }

        return salaryDeductionList;
    }

    private static DateTime? GetEndPeriodFromPurchaseDate(DateTime purchaseDate, int? runtime, LifecycleType lifecycleType)
    {
        var startDate = GetStartPeriodFromPurchaseDate(purchaseDate, lifecycleType);
        if (!runtime.HasValue || lifecycleType == LifecycleType.NoLifecycle || startDate == null)
        {
            return null;
        }

        return startDate.Value.AddMonths(runtime.Value).AddDays(-1);
    }

    private static DateTime? GetStartPeriodFromPurchaseDate(DateTime purchaseDate, LifecycleType lifecycleType)
    {
        if (lifecycleType == LifecycleType.NoLifecycle)
        {
            return null;
        }
        return purchaseDate.Date.AddMonths(1).AddDays(-purchaseDate.Day + 1);
    }

    private decimal BuyoutPriceByDate(DateTime buyoutDate)
    {
        if (AssetLifecycleType != LifecycleType.Transactional)
            return 0;
        var differenceInMonth = ((buyoutDate.Year - PurchaseDate.Year) * 12) + buyoutDate.Month - PurchaseDate.Month;
        var bookValue = PaidByCompany - (PaidByCompany / 36 * differenceInMonth);
        bookValue = bookValue < 0 ? 0 : decimal.Round(bookValue, 2, MidpointRounding.AwayFromZero);
        var vat = VATConfiguration.Amount;
        var buyOutPrice = bookValue * vat;
        return buyOutPrice < 0 ? 0 : decimal.Round(buyOutPrice, 2, MidpointRounding.AwayFromZero);
    }

    public void IsSentToRepair(Guid callerId)
    {
        if (_assetLifecycleStatus == AssetLifecycleStatus.Lost ||
      _assetLifecycleStatus == AssetLifecycleStatus.Stolen ||
      _assetLifecycleStatus == AssetLifecycleStatus.Recycled ||
      _assetLifecycleStatus == AssetLifecycleStatus.BoughtByUser ||
      _assetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
      _assetLifecycleStatus == AssetLifecycleStatus.Returned ||
      _assetLifecycleStatus == AssetLifecycleStatus.Discarded) throw new InvalidAssetDataException($"Invalid asset lifecycle status: {_assetLifecycleStatus} for sending asset lifecycle on repair.",Guid.Parse("c3c2cde5-b627-4de9-9dfd-e69f71804535"));

        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        UpdateAssetStatus(AssetLifecycleStatus.Repair, callerId);
        AddDomainEvent(new AssetSentToRepairDomainEvent(this, _assetLifecycleStatus, callerId));
    }
    public void RepairCompleted(Guid callerId, bool discarded)
    {
        if (_assetLifecycleStatus == AssetLifecycleStatus.Lost ||
            _assetLifecycleStatus == AssetLifecycleStatus.Stolen ||
            _assetLifecycleStatus == AssetLifecycleStatus.Recycled ||
            _assetLifecycleStatus == AssetLifecycleStatus.BoughtByUser ||
            _assetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
            _assetLifecycleStatus == AssetLifecycleStatus.Returned) throw new InvalidAssetDataException($"Invalid asset lifecycle status: {_assetLifecycleStatus} for completing return.", Guid.Parse("c8e7d181-8eb6-4fa3-b553-58c8fac0544e"));
;
        if (_assetLifecycleStatus == AssetLifecycleStatus.Repair)
        {
            if (discarded) UpdateAssetStatus(AssetLifecycleStatus.Discarded, callerId);
            else UpdateAssetStatus(AssetLifecycleStatus.InUse, callerId);

            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
            AddDomainEvent(new AssetRepairCompletedDomainEvent(this, callerId, _assetLifecycleStatus));
        }
    }
    public void SetActiveStatus(Guid callerId)
    {
        if (_assetLifecycleType == LifecycleType.NoLifecycle || _assetLifecycleType == LifecycleType.BYOD)
        {
            UpdateAssetStatus(AssetLifecycleStatus.Active, callerId);
        }
    }
    public void SetInactiveStatus(Guid callerId)
    {
        if (_assetLifecycleType == LifecycleType.NoLifecycle || _assetLifecycleType == LifecycleType.BYOD)
        {
            UpdateAssetStatus(AssetLifecycleStatus.Inactive, callerId);
        }
    }

    /// <summary>
    /// Sets the asset lifecycle to asset status recycled. 
    /// </summary>
    /// <param name="callerId">The id of the user making the request.</param>
    public void SetRecycledStatus(Guid callerId)
    {
        //Change status and add a domain event only if the status is changed
        if (_assetLifecycleStatus != AssetLifecycleStatus.Recycled)
        {
            if(_assetLifecycleType != LifecycleType.Transactional) throw new InvalidAssetDataException($"Invalid asset lifecycle status: {_assetLifecycleStatus} to recycle the device.", Guid.Parse("d59598f5-e2bc-4e61-bc47-19b3eb1124ca"));

            if (_assetLifecycleStatus == AssetLifecycleStatus.Lost ||
            _assetLifecycleStatus == AssetLifecycleStatus.Stolen ||
            _assetLifecycleStatus == AssetLifecycleStatus.BoughtByUser ||
            _assetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
            _assetLifecycleStatus == AssetLifecycleStatus.Repair ||
            _assetLifecycleStatus == AssetLifecycleStatus.Discarded) throw new InvalidAssetDataException($"Invalid asset lifecycle status: {_assetLifecycleStatus} to recycle the device.", Guid.Parse("e996c4e8-fbb0-4088-861d-380a591af474"));
            
            UpdateAssetStatus(AssetLifecycleStatus.Recycled, callerId);
        }
    }
    /// <summary>
    /// Sets the asset lifecycle to asset status pending recycle.
    /// </summary>
    /// <param name="callerId">The id of the user making the request.</param>
    public void SetPendingRecycledStatus(Guid callerId)
    {
        if (_assetLifecycleStatus != AssetLifecycleStatus.PendingRecycle)
        {
            if (_assetLifecycleType != LifecycleType.Transactional) throw new InvalidAssetDataException($"Invalid asset lifecycle status: {_assetLifecycleStatus} to recycle the device.", Guid.Parse("dbc2a756-3a66-455d-9f33-c0464c610602"));

            if (_assetLifecycleStatus == AssetLifecycleStatus.Lost ||
                _assetLifecycleStatus == AssetLifecycleStatus.Stolen ||
                _assetLifecycleStatus == AssetLifecycleStatus.BoughtByUser ||
                _assetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
                _assetLifecycleStatus == AssetLifecycleStatus.Repair ||
                _assetLifecycleStatus == AssetLifecycleStatus.Discarded) throw new InvalidAssetDataException($"Invalid asset lifecycle status: {_assetLifecycleStatus} to set the device to pending recycle.", Guid.Parse("e996c4e8-fbb0-4088-861d-380a591af474"));

            UpdateAssetStatus(AssetLifecycleStatus.PendingRecycle,callerId);
        }
    }
    /// <summary>
    /// Sets the asset lifecycle values to the right active state when a asset lifecycle order is cancelled.
    /// Should only be usable if the asset lifecycle indicates a return/recycle is ongoing or has been completed.
    /// It also checks if the asset lifecycle has gotten ExpiredSoon (30 days warning) or Expired during the time the order was processed.
    /// </summary>
    /// <param name="callerId">The id of the user making the request.</param>
    /// <param name="today">Todays date and time.</param>
    public void CancelReturn(Guid callerId, DateTime today)
    {

        if (_assetLifecycleStatus != AssetLifecycleStatus.PendingReturn &&
            _assetLifecycleStatus != AssetLifecycleStatus.Returned &&
            _assetLifecycleStatus != AssetLifecycleStatus.Recycled &&
            _assetLifecycleStatus != AssetLifecycleStatus.PendingRecycle
            )
            throw new InvalidAssetDataException($"Invalid asset lifecycle status: {_assetLifecycleStatus} for completing return.", Guid.Parse("05f64b6e-5491-4088-9e7f-df824758aadf"));

        AssetLifecycleStatus status = AssetLifecycleStatus.InUse;

        if(_assetLifecycleType == LifecycleType.NoLifecycle || _assetLifecycleType == LifecycleType.BYOD) status = AssetLifecycleStatus.Active;
         
        if (EndPeriod != null) 
        {
            if ((EndPeriod - today).Value.TotalDays <= 30) status = AssetLifecycleStatus.ExpiresSoon;
            if (EndPeriod <= today) status = AssetLifecycleStatus.Expired;
        }

        UpdateAssetStatus(status, callerId);
    }
}