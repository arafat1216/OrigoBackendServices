#nullable enable

using Common.Enums;

namespace OrigoApiGateway.Models.Asset.Backend
{
    /// <summary>
    ///     Contains all potential search-parameters when performing a advanced asset-search
    /// </summary>
    public class AssetSearchParameters
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
        public StringSearchType QuickSearchSearchType { get; set; } = StringSearchType.Contains;


        /*
         * List filters (.NET contains / SQL "WHERE IN")
         */

        /// <summary>
        /// Filter the search-results to only contain results from these customers. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<Guid>? CustomerIds { get; set; }

        /// <summary>
        /// Filter the search-results to only contain results from these users. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<Guid>? UserIds { get; set; }

        /// <summary>
        /// Filter the search-results to only contain results from these departments. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<Guid>? DepartmentIds { get; set; }

        /// <summary>
        /// Filter the search-results so only assets with the specified categories is retrieved. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<int>? AssetCategoryIds { get; set; }

        /// <summary>
        /// Filter the search-results so only assets with the specified lifecycle status-IDs is retrieved. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<int>? AssetLifecycleStatusIds { get; set; }

        /// <summary>
        /// Filter the search-results so only assets with the specified lifecycle types is retrieved. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<int>? AssetLifecycleTypeIds { get; set; }


        /*
         * Filters (SQL "LIKE")
         */

        /// <summary>
        /// The assets IMEI number.
        /// </summary>
        /// <example>514127746123926</example>
        [MaxLength(15)]
        [RegularExpression("^[0-9]{0,15}$")]
        public string? Imei { get; set; }

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
        public StringSearchType ImeiSearchType { get; set; } = StringSearchType.StartsWith;

        /// <summary>
        /// The assets serial-number.
        /// </summary>
        public string? SerialNumber { get; set; }

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
        public StringSearchType SerialNumberSearchType { get; set; } = StringSearchType.StartsWith;

        public string? Alias { get; set; }

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
        public StringSearchType AliasSearchType { get; set; } = StringSearchType.Contains;

        /// <summary>
        /// The asset brand (e.g. Samsung)
        /// </summary>
        /// <example>Samsung</example>
        [MaxLength(50, ErrorMessage = "The brand name can't be longer then 50 characters.")]
        public string? Brand { get; set; }

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
        public StringSearchType BrandSearchType { get; set; } = StringSearchType.StartsWith;

        /// <summary>
        /// The model or product name of this asset (e.g. Samsung Galaxy)
        /// </summary>
        /// <example>Galaxy S22 Ultra</example>
        [MaxLength(50, ErrorMessage = "The model/product-name can't be longer then 50 characters.")]
        public string? ProductName { get; set; }

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
        public StringSearchType ProductNameSearchType { get; set; } = StringSearchType.StartsWith;

        /// <summary>
        /// The start period for this asset lifecycle.
        /// </summary>
        /// <example>2022-06-01</example>
        [SwaggerSchema(Format = "date")]
        public DateTime? StartPeriod { get; set; }

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
        public DateSearchType StartPeriodSearchType { get; set; } = DateSearchType.ExcactDate;

        /// <summary>
        /// The end period for this asset lifecycle.
        /// </summary>
        /// <example>2024-06-01</example>
        [SwaggerSchema(Format = "date")]
        public DateTime? EndPeriod { get; set; }

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
        public DateSearchType EndPeriodSearchType { get; set; } = DateSearchType.ExcactDate;

        /// <summary>
        /// The purchase date of the asset lifecycle.
        /// </summary>
        /// <example>2022-05-22</example>
        [SwaggerSchema(Format = "date")]
        public DateTime? PurchaseDate { get; set; }

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
        public DateSearchType PurchaseDateSearchType { get; set; } = DateSearchType.ExcactDate;


        // TODO: The sorting/ordering properties (and validation) has been omitted from the API gateway model, as it's still experimental.
    }
}

