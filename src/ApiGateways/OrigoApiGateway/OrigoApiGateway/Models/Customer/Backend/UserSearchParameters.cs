#nullable enable

using Common.Enums;

namespace OrigoApiGateway.Models.Customer.Backend
{
    /// <summary>
    ///     Contains all the user-provided search-parameters used when performing an advanced search.
    /// </summary>
    public class UserSearchParameters
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
        /// <item></item>
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
        /// Filter the search-results to only contain results from these organizations. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<Guid>? OrganizationIds { get; set; }

        /// <summary>
        /// Filter the search-results to only include users that is a member of these departments. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<Guid>? DepartmentIds { get; set; }

        /// <summary>
        /// Filter the search-results so only users with these status-IDs is retrieved. The filter will be ignored if the value is omitted or <see langword="null"/>.
        /// </summary>
        public HashSet<int>? StatusIds { get; set; }

        /*
         * Filters (SQL "LIKE")
         */

        /// <summary>
        /// The users fist-name.
        /// </summary>
        /// <example>John</example>
        public string? FirstName { get; set; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="FirstName"/></c>.
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
        public StringSearchType FirstNameSearchType { get; set; } = StringSearchType.StartsWith;


        /// <summary>
        /// The users last-name.
        /// </summary>
        /// <example>Doe</example>
        public string? LastName { get; set; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="LastName"/></c>.
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
        public StringSearchType LastNameSearchType { get; set; } = StringSearchType.StartsWith;

        /// <summary>
        /// The users full name.
        /// </summary>
        /// <example>John Doe</example>
        public string? FullName { get; set; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="FullName"/></c>.
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
        public StringSearchType FullNameSearchType { get; set; } = StringSearchType.Contains;

        /// <summary>
        /// The users email-address.
        /// </summary>
        /// <example>user@example.com</example>
        [MaxLength(320)]
        public string? Email { get; set; }

        /// <summary>
        /// Determines the search-condition that should be used for <c><see cref="Email"/></c>.
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
        public StringSearchType EmailSearchType { get; set; } = StringSearchType.Contains;
    }
}
