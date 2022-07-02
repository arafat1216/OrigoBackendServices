﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
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

    public static bool HasActiveState(AssetLifecycleStatus assetLifecycleStatus)
    {
        return assetLifecycleStatus is AssetLifecycleStatus.InputRequired or AssetLifecycleStatus.InUse
            or AssetLifecycleStatus.Repair or AssetLifecycleStatus.PendingReturn
            or AssetLifecycleStatus.Available or AssetLifecycleStatus.Active or AssetLifecycleStatus.ExpiresSoon;
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
        if (contractHolderUser != null)
        {
            // Unassign previous owner and add domain events for it - cant have two owners
            if (ContractHolderUser != null) AddDomainEvent(new UnAssignContractHolderToAssetLifeCycleDomainEvent(this, callerId, ContractHolderUser));
            if (ManagedByDepartmentId != null) AddDomainEvent(new UnAssignDepartmentAssetLifecycleDomainEvent(this, callerId));
            ManagedByDepartmentId = departmentId;
            IsPersonal = true;
            ContractHolderUser = contractHolderUser;
            var previousContractHolderUser = ContractHolderUser;
            IsPersonal = true;

            if (_assetLifecycleStatus != AssetLifecycleStatus.InUse && _assetLifecycleType != LifecycleType.NoLifecycle) UpdateAssetStatus(AssetLifecycleStatus.InUse, callerId);

            AddDomainEvent(new AssignContractHolderToAssetLifeCycleDomainEvent(this, callerId, previousContractHolderUser));
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
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
            if(_assetLifecycleStatus != AssetLifecycleStatus.InUse && _assetLifecycleType != LifecycleType.NoLifecycle) UpdateAssetStatus(AssetLifecycleStatus.InUse, callerId);

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
            throw new AssetExpireRequestException("Asset has No Life Cycle to Expire");
        if (!IsActiveState)
            throw new AssetExpireRequestException("Asset is not in Active state");
        if (_assetLifecycleStatus != AssetLifecycleStatus.ExpiresSoon)
            throw new AssetExpireRequestException("Asset does not have 'ExpiresSoon' status");
        if (EndPeriod.HasValue && (int) (EndPeriod.Value - DateTime.UtcNow).TotalDays >= 0)
            throw new AssetExpireRequestException("Asset is not expiring.");
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
            throw new AssetExpiresSoonRequestException("Asset has No Life Cycle that can expires soon");
        if (!IsActiveState)
            throw new AssetExpiresSoonRequestException("Asset is not in Active state");

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
            throw new ReturnDeviceRequestException($"Only Assets that have Transactionl Life cycle type can make return request!!! asset Id: {ExternalId}");

        if (!IsActiveState)
            throw new ReturnDeviceRequestException($"Only Active devices can make return request!!! asset Id: {ExternalId}");

        if (_assetLifecycleStatus != AssetLifecycleStatus.ExpiresSoon)
            throw new ReturnDeviceRequestException($"Asset is not Expiring Soon to make return request!!! asset Id: {ExternalId}");

        if (_assetLifecycleStatus == AssetLifecycleStatus.PendingReturn)
            throw new ReturnDeviceRequestException($"Asset already have pending return request!!! asset Id: {ExternalId}");

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
            throw new ReturnDeviceRequestException($"Only Assets that have Transactionl Life cycle type can make return request!!! asset Id: {ExternalId}");

        if (_assetLifecycleStatus != AssetLifecycleStatus.ExpiresSoon && _assetLifecycleStatus != AssetLifecycleStatus.PendingReturn)
            throw new ReturnDeviceRequestException($"Asset is not Expiring Soon and does not have pending return request!!! asset Id: {ExternalId}");

        if (!IsActiveState)
            throw new ReturnDeviceRequestException($"Only Active devices can make return request!!! asset Id: {ExternalId}");

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
            throw new BuyoutDeviceRequestException($"Only Active devices can do buyout!!! asset Id: {ExternalId}");

        if (!IsPersonal || ContractHolderUser is null)
            throw new BuyoutDeviceRequestException($"Only Personal Assets can be bought out!!! asset Id: {ExternalId}");

        if (_assetLifecycleType != LifecycleType.Transactional)
            throw new BuyoutDeviceRequestException($"Only Assets that have Transactionl Life cycle type can be bought out!!! asset Id: {ExternalId}");

        if (_assetLifecycleStatus != AssetLifecycleStatus.ExpiresSoon)
            throw new ReturnDeviceRequestException($"Asset is not Expiring Soon to do buyout!!! asset Id: {ExternalId}");

        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
        var previousLifecycleStatus = _assetLifecycleStatus;
        AddDomainEvent(new BuyoutDeviceDomainEvent(this, callerId, previousLifecycleStatus));
        _assetLifecycleStatus = AssetLifecycleStatus.BoughtByUser;
    }

    /// <summary>
    /// User has Reported this asset.. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void ReportDevice(ReportCategory reportCategory, Guid callerId)
    {
        if (!IsActiveState)
            throw new InactiveDeviceRequestException($"Only Active devices can be reported!!! asset Id: {ExternalId}");

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
        AddDomainEvent(new UpdateAssetLifecycleStatusDomainEvent(this, callerId, previousLifecycleStatus));
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

    public void IsSentToRepair(Guid callerId)
    {
        if (_assetLifecycleStatus == AssetLifecycleStatus.Lost ||
      _assetLifecycleStatus == AssetLifecycleStatus.Stolen ||
      _assetLifecycleStatus == AssetLifecycleStatus.Recycled ||
      _assetLifecycleStatus == AssetLifecycleStatus.BoughtByUser ||
      _assetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
      _assetLifecycleStatus == AssetLifecycleStatus.Returned ||
      _assetLifecycleStatus == AssetLifecycleStatus.Discarded) throw new InvalidAssetDataException($"Invalid asset lifecycle status: {_assetLifecycleStatus} for sending asset lifecycle on repair.");

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
            _assetLifecycleStatus == AssetLifecycleStatus.Returned) throw new InvalidAssetDataException($"Invalid asset lifecycle status: {_assetLifecycleStatus} for completing return.");
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
}