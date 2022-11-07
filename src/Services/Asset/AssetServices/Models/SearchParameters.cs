using Common.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace AssetServices.Models
{
    public record SearchParameters : IValidatableObject
    {
        /*
         * Generic search
         */

        /// <summary>
        /// A quick and simple search. The search only looks through a few pre-determine high-priority properties, 
        /// so it can find results from the most important and most used properties, while also trying to keeping the results to a minimum.
        /// 
        /// <para>
        /// <b>Supported properties:</b>
        /// <list type="bullet">
        /// <item>IMEI</item>
        /// <item>Serial number</item>
        /// <item>Name of the contract-holder (the user)</item>
        /// </list>
        /// </para>
        /// </summary>
        public string? QuickSearch { get; set; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="QuickSearch"/></c>.
        /// 
        /// <para>
        /// <list type="bullet">
        ///     <item><term>1</term>The result must be an exact match.</item>
        ///     <item><term>2</term>The result must start with the value.</item>
        ///     <item><term>3</term>The result must contain the value.</item>
        /// </list>
        /// </para>
        /// </summary>
        [EnumDataType(typeof(StringSearchType))]
        public StringSearchType QuickSearchSearchType { get; init; } = StringSearchType.Contains;


        /*
         * List filters (.NET contains / SQL "WHERE IN")
         */

        /// <summary>
        /// Filter the search-results to only contain results from these customers. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<Guid>? CustomerIds { get; init; }

        /// <summary>
        /// Filter the search-results to only contain results from these users. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<Guid>? UserIds { get; init; }

        /// <summary>
        /// Filter the search-results to only contain results from these departments. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<Guid>? DepartmentIds { get; init; }

        /// <summary>
        /// Filter the search-results so only assets with the specified categories is retrieved. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<int>? AssetCategoryIds { get; init; }

        /// <summary>
        /// Filter the search-results so only assets with the specified lifecycle status-IDs is retrieved. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<int>? AssetLifecycleStatusIds { get; init; }

        /// <summary>
        /// Filter the search-results so only assets with the specified lifecycle types is retrieved. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<int>? AssetLifecycleTypeIds { get; init; }


        /*
         * Filters (SQL "LIKE")
         */

        /// <summary>
        /// The assets IMEI number.
        /// </summary>
        /// <example>514127746123926</example>
        [MaxLength(15)]
        [RegularExpression("^[0-9]{0,15}$")]
        public string? Imei { get; init; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="Imei"/></c>.
        /// 
        /// <para>
        /// <list type="bullet">
        ///     <item><term>1</term>The result must be an exact match.</item>
        ///     <item><term>2</term>The result must start with the value.</item>
        ///     <item><term>3</term>The result must contain the value.</item>
        /// </list>
        /// </para>
        /// </summary>
        [EnumDataType(typeof(StringSearchType))]
        public StringSearchType ImeiSearchType { get; init; } = StringSearchType.StartsWith;

        /// <summary>
        /// The assets serial-number.
        /// </summary>
        public string? SerialNumber { get; init; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="SerialNumber"/></c>.
        /// 
        /// <para>
        /// <list type="bullet">
        ///     <item><term>1</term>The result must be an exact match.</item>
        ///     <item><term>2</term>The result must start with the value.</item>
        ///     <item><term>3</term>The result must contain the value.</item>
        /// </list>
        /// </para>
        /// </summary>
        [EnumDataType(typeof(StringSearchType))]
        public StringSearchType SerialNumberSearchType { get; init; } = StringSearchType.StartsWith;

        public string? Alias { get; init; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="Alias"/></c>.
        /// 
        /// <para>
        /// <list type="bullet">
        ///     <item><term>1</term>The result must be an exact match.</item>
        ///     <item><term>2</term>The result must start with the value.</item>
        ///     <item><term>3</term>The result must contain the value.</item>
        /// </list>
        /// </para>
        /// </summary>
        [EnumDataType(typeof(StringSearchType))]
        public StringSearchType AliasSearchType { get; init; } = StringSearchType.Contains;

        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        /// <example>Samsung</example>
        [MaxLength(50, ErrorMessage = "The brand name can't be longer then 50 characters.")]
        public string? Brand { get; init; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="Brand"/></c>.
        /// 
        /// <para>
        /// <list type="bullet">
        ///     <item><term>1</term>The result must be an exact match.</item>
        ///     <item><term>2</term>The result must start with the value.</item>
        ///     <item><term>3</term>The result must contain the value.</item>
        /// </list>
        /// </para>
        /// </summary>
        [EnumDataType(typeof(StringSearchType))]
        public StringSearchType BrandSearchType { get; init; } = StringSearchType.StartsWith;

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        /// <example>Galaxy S22 Ultra</example>
        [MaxLength(50, ErrorMessage = "The model/product-name can't be longer then 50 characters.")]
        public string? ProductName { get; init; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="ProductName"/></c>.
        /// 
        /// <para>
        /// <list type="bullet">
        ///     <item><term>1</term>The result must be an exact match.</item>
        ///     <item><term>2</term>The result must start with the value.</item>
        ///     <item><term>3</term>The result must contain the value.</item>
        /// </list>
        /// </para>
        /// </summary>
        [EnumDataType(typeof(StringSearchType))]
        public StringSearchType ProductNameSearchType { get; init; } = StringSearchType.StartsWith;

        /// <summary>
        /// The start period for this asset lifecycle.
        /// </summary>
        /// <example>2022-06-01</example>
        [SwaggerSchema(Format = "date")]
        public DateTime? StartPeriod { get; init; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="StartPeriod"/></c>.
        /// 
        /// <para>
        /// <list type="bullet">
        ///     <item><term>1</term>The results is on the exact date.</item>
        ///     <item><term>2</term>The result is on, or before the selected date.</item>
        ///     <item><term>3</term>The result is on, or after the selected date.</item>
        /// </list>
        /// </para>
        /// </summary>
        [EnumDataType(typeof(DateSearchType))]
        public DateSearchType StartPeriodSearchType { get; init; } = DateSearchType.ExcactDate;

        /// <summary>
        /// The end period for this asset lifecycle.
        /// </summary>
        /// <example>2024-06-01</example>
        [SwaggerSchema(Format = "date")]
        public DateTime? EndPeriod { get; init; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="EndPeriod"/></c>.
        /// 
        /// <para>
        /// <list type="bullet">
        ///     <item><term>1</term>The results is on the exact date.</item>
        ///     <item><term>2</term>The result is on, or before the selected date.</item>
        ///     <item><term>3</term>The result is on, or after the selected date.</item>
        /// </list>
        /// </para>
        /// </summary>
        [EnumDataType(typeof(DateSearchType))]
        public DateSearchType EndPeriodSearchType { get; init; } = DateSearchType.ExcactDate;

        /// <summary>
        /// The purchase date of the asset lifecycle.
        /// </summary>
        /// <example>2022-05-22</example>
        [SwaggerSchema(Format = "date")]
        public DateTime? PurchaseDate { get; init; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="PurchaseDate"/></c>.
        /// 
        /// <para>
        /// <list type="bullet">
        ///     <item><term>1</term>The results is on the exact date.</item>
        ///     <item><term>2</term>The result is on, or before the selected date.</item>
        ///     <item><term>3</term>The result is on, or after the selected date.</item>
        /// </list>
        /// </para>
        /// </summary>
        [EnumDataType(typeof(DateSearchType))]
        public DateSearchType PurchaseDateSearchType { get; init; } = DateSearchType.ExcactDate;


        /*
         * Sorting
         */

        /// <summary>
        /// When set, the search-results is ordered using the specified property. 
        /// If the value is omitted or <see langword="null"/>, the default sorting is used.
        /// 
        /// <para>Only database-persisted values from the <c>Entity Framework</c> <see cref="AssetLifecycle"/>-entity is supported.
        /// All properties must match the entity-name. </para>
        /// 
        /// <para>
        /// <b>Supported values:</b>
        /// <list type="bullet">
        ///     <item>startPeriod</item>
        ///     <item>endPeriod</item>
        ///     <item>purchaseDate</item>
        ///     <item>alias</item>
        ///     <item>source</item>
        ///     <item>assetLifecycleStatus</item>
        /// </list>
        /// </para>
        /// </summary>
        [JsonIgnore] // Temporarily ignored so it can't be used, as it required additional testing.
        public string? OrderBy { get; init; }

        /// <summary>
        /// When <c><see langword="true"/></c>, it is sorted in <c>ascending</c> order.
        /// <br/>
        /// When <c><see langword="false"/></c>, it is sorted in <c>descending</c> order.
        /// 
        /// <para>
        /// If the value is omitted or <see langword="null"/>, it will be sorted in ascending order.
        /// </para>
        /// </summary>
        [JsonIgnore] // TODO: This requires more testing before it's put into use! Therefore we temporarily ignore the property in the APIs so it can't be used.
        public bool OrderIsAscending { get; init; } = true;


        /*
         * Other
         */

        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(OrderBy))
            {
                List<string> allowedValues = new()
                {
                    nameof(AssetLifecycle.StartPeriod),
                    nameof(AssetLifecycle.EndPeriod),
                    nameof(AssetLifecycle.PurchaseDate),
                    nameof(AssetLifecycle.Alias),
                    nameof(AssetLifecycle.Source),
                    nameof(AssetLifecycle.AssetLifecycleStatus),
                };

                if (allowedValues.Any(e => e.Equals(OrderBy, StringComparison.InvariantCultureIgnoreCase)))
                {
                    yield return new ValidationResult("The provided value is invalid or not supported, and can't be used for sorting.", new[] { nameof(OrderBy) });
                }
            }
        }
    }
}

