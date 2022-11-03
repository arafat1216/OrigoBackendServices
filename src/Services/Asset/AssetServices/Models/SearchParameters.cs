#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AssetServices.Models
{
    public record SearchParameters
    {
        // Filters: .NET contains / SQL "WHERE IN"
        public HashSet<Guid>? CustomerIds { get; init; }
        public HashSet<Guid>? UserIds { get; init; }
        public HashSet<Guid>? DepartmentIds { get; init; }
        public HashSet<int>? AssetCategoryIds { get; init; }
        public HashSet<int>? AssetLifecycleStatusIds { get; init; }
        public HashSet<int>? AssetLifecycleTypeIds { get; init; }

        [RegularExpression("^[0-9]{0,15}$")]
        public string? Imei { get; init; }
        public StringSearchType ImeiSearchType { get; init; } = StringSearchType.StartsWith;

        // public string? SerialNumber { get; init; }
        // public StringSearchType SerialNumberSearchType { get; init; } = StringSearchType.StartsWith;

        // public string? Alias { get; init; }
        // public StringSearchType AliasSearchType { get; init; } = StringSearchType.Contains;

        // [StringLength(50, ErrorMessage = "The brand name can't be longer then 50 characters.")]
        // public string? Brand { get; init; }
        // public StringSearchType BrandSearchType { get; init; } = StringSearchType.StartsWith;

        // [StringLength(50, ErrorMessage = "The model/product-name can't be longer then 50 characters.")]
        // public string? ProductName { get; init; }
        // public StringSearchType ProductNameSearchType { get; init; } = StringSearchType.StartsWith;

        public DateTime? StartPeriod { get; init; }
        public DateSearchType StartPeriodSearchType { get; init; } = DateSearchType.ExcactDate;

        public DateTime? EndPeriod { get; init; }
        public DateSearchType EndPeriodSearchType { get; init; } = DateSearchType.ExcactDate;

        public DateTime? PurchaseDate { get; init; }
        public DateSearchType PurchaseDateSearchType { get; init; } = DateSearchType.ExcactDate;

        // public string? SortBy { get; init; }
        // public bool SortAscending { get; init; } = true;
    }
}

