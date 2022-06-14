using System;
using System.Collections.Generic;
using Common.Enums;

namespace AssetServices.ServiceModel;

public record AssetLifecycleDTO
{
    public Guid ExternalId { get; init; }
    public Guid CustomerId { get; init; }
    public string ContractReferenceName { get; init; } = string.Empty;
    public string Alias { get; init; } = string.Empty;
    public DateTime StartPeriod { get; init; }
    public DateTime EndPeriod { get; init; }
    public DateTime PurchaseDate { get; init; }
    public string Note { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal PaidByCompany { get; init; } = 0;
    public CurrencyCode CurrencyCode { get; init; }
    public AssetDTO? Asset { get; init; }
    public int AssetCategoryId { get; init; }
    public decimal BookValue { get; init; }
    public decimal BuyoutPrice { get; init; }
    public string AssetCategoryName { get; init; } = string.Empty;
    public Guid? ContractHolderUserId { get; init; }
    public Guid? ManagedByDepartmentId { get; init; }
    public AssetLifecycleStatus AssetLifecycleStatus { get; init; }
    public LifecycleType AssetLifecycleType { get; init; }
    public IReadOnlyCollection<LabelDTO> Labels { get; init; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? OrderNumber { get; set; }
    public string? ProductId { get; set; }
    public string? TransactionId { get; set; }
    public string? InvoiceNumber { get; set; }
    public bool? IsPersonal { get; set; }
    public string? Source { get; set; }
}