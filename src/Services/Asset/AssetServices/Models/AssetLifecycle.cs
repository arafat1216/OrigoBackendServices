using System;
using System.Collections.Generic;
using System.Linq;
using AssetServices.DomainEvents.AssetLifecycleEvents;
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

    public static bool IsActiveState(AssetLifecycleStatus assetLifecycleStatus)
    {
        return assetLifecycleStatus is AssetLifecycleStatus.InputRequired or AssetLifecycleStatus.InUse
            or AssetLifecycleStatus.Repair or AssetLifecycleStatus.PendingReturn
            or AssetLifecycleStatus.Available or AssetLifecycleStatus.Active;
    }

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

        //Assign to user
        if (contractHolderUser != null && (departmentId == null || departmentId == Guid.Empty))
        {
            //unassigne previous owner and add domain events for it - cant have to owners
            if (ContractHolderUser != null) AddDomainEvent(new UnAssignContractHolderToAssetLifeCycleDomainEvent(this, callerId, ContractHolderUser));
            if (ManagedByDepartmentId != null) AddDomainEvent(new UnAssignDepartmentAssetLifecycleDomainEvent(this, callerId));
            ManagedByDepartmentId = null;

            
            IsPersonal = true;
            ContractHolderUser = contractHolderUser;
            var previousContractHolderUser = ContractHolderUser;
            IsPersonal = true;

            if (_assetLifecycleStatus != AssetLifecycleStatus.InUse && _assetLifecycleType != LifecycleType.NoLifecycle) UpdateAssetStatus(AssetLifecycleStatus.InUse, callerId);

            AddDomainEvent(new AssignContractHolderToAssetLifeCycleDomainEvent(this, callerId, previousContractHolderUser));
            UpdatedBy = callerId;
            LastUpdatedDate = DateTime.UtcNow;
        }
        else if ((departmentId != null || departmentId != Guid.Empty) && contractHolderUser == null)
        {
            //unassigne previous owner and add domain events for it
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

    public void UpdatePurchaseDate(DateTime? purchaseDate, Guid callerId)
    {
        throw new NotImplementedException();
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
        if(_labels.Any())
            _labels.Clear();
        _assetLifecycleStatus = AssetLifecycleStatus.Available;
    }

    /// <summary>
    /// Re-assigning this asset fto other user/department. 
    /// </summary>
    /// <param name="callerId">The userid making this assignment</param>
    public void ReAssignAssetLifeCycleToHolder(User? contractHolderUser, Guid departmentId, Guid callerId)
    {
        var previousDepartmentId = ManagedByDepartmentId;
        var previousContractHolderUser = ContractHolderUser;
        if (contractHolderUser != null)
        {
            // Re-assign to another user
            if (ContractHolderUser != null)
                AddDomainEvent(new UnAssignContractHolderToAssetLifeCycleDomainEvent(this, callerId, ContractHolderUser));
            ContractHolderUser = contractHolderUser;
        }
        // Re-assign to another department
        if (ManagedByDepartmentId != null)
            AddDomainEvent(new UnAssignDepartmentAssetLifecycleDomainEvent(this, callerId));
        ManagedByDepartmentId = departmentId;
        if (_assetLifecycleStatus != AssetLifecycleStatus.InUse) 
            UpdateAssetStatus(AssetLifecycleStatus.InUse, callerId);
        IsPersonal = contractHolderUser != null ? true : false;
        AddDomainEvent(new ReAssignAssetLifeCycleDomainEvent(this, callerId, previousContractHolderUser, previousDepartmentId));
        UpdatedBy = callerId;
        LastUpdatedDate = DateTime.UtcNow;
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
        var previousLifecycleStatus = _assetLifecycleStatus;
        _assetLifecycleStatus = AssetLifecycleStatus.Stolen;
        AddDomainEvent(new AssetHasBeenStolenDomainEvent(this, previousLifecycleStatus, callerId));
    }

    public static AssetLifecycle CreateAssetLifecycle(Guid customerId, string alias, LifecycleType assetLifecycleType, DateTime purchaseDate, string note, string description, decimal paidByCompany, string orderNumber, string productId, string invoiceNumber, string transactionId, AssetLifeCycleSource source)
    {
        var assetLifecycleStatus = assetLifecycleType == LifecycleType.Transactional ? AssetLifecycleStatus.InputRequired : AssetLifecycleStatus.Active;
        return new AssetLifecycle
        {
            CustomerId = customerId,
            Alias = alias,
            AssetLifecycleType = assetLifecycleType,
            AssetLifecycleStatus = assetLifecycleStatus,
            PurchaseDate = purchaseDate,
            Note = note,
            Description = description,
            PaidByCompany = paidByCompany,
            OrderNumber = orderNumber,
            ProductId = productId,
            InvoiceNumber = invoiceNumber,
            TransactionId = transactionId,
            Source = source
        };
    }
}