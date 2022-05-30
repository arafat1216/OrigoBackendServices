using System;
using AssetServices.Models;
using Common.Enums;

namespace AssetServices.ServiceModel;

public class CreateAssetLifecycleDTO
{
    /// <summary>
    /// The customer linked to this asset lifecycle.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The category this asset belongs to.
    /// </summary>
    public int AssetCategoryId { get; init; }

    /// <summary>
    /// Alias for the asset.
    /// </summary>
    public string Alias { get; set; }

    /// <summary>
    /// A note containing additional information or comments for the asset.
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// The type of lifecycle for this asset.
    /// </summary>
    public LifecycleType LifecycleType { get; set; }

    /// <summary>
    /// The date the asset was purchased.
    /// </summary>
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// The department or cost center this asset is assigned to.
    /// </summary>
    public Guid? ManagedByDepartmentId { get; set; }

    /// <summary>
    /// The employee holding the asset.
    /// </summary>
    public Guid? AssetHolderId { get; set; }

    /// <summary>
    /// Id of user making the endpoint call.
    /// </summary>
    public Guid CallerId { get; set; }

    /// <summary>
    /// A description of the asset.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The payment amount by company for the asset. For mobile phones and other devices
    /// This is the amount that company covered/paid for the asset's overall cost
    /// </summary>
    public decimal PaidByCompany { get; set; }

    /// <summary>
    /// Tags associated with this asset.
    /// </summary>
    public string AssetTag { get; set; }

    /// <summary>
    ///     The product id for the asset.
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    ///     Order# this asset was part of.
    /// </summary>
    public string? OrderNumber { get; set; }

    /// <summary>
    ///     The salary deduction the holder of the asset will be given per month.
    /// </summary>
    public decimal? MonthlySalaryDeduction { get; set; }

    /// <summary>
    ///     The number of months a salary deduction will be done for the asset holder.
    /// </summary>
    public int? MonthlySalaryDeductionRuntime { get; set; }

    /// <summary>
    ///     The invoice# this asset will be invoiced on.
    /// </summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    ///     A unique id for the transaction where this asset was requested for.
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// Is a personal or non-personal asset.
    /// </summary>
    public bool IsPersonal { get; set; }

    public AssetLifeCycleSource Source { get; set; }
}