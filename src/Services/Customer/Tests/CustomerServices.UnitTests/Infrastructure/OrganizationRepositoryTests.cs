using Common.Enums;
using Common.Logging;
using CustomerServices.Infrastructure.Context;
using CustomerServices.Models;
using CustomerServices.UnitTests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CustomerServices.Infrastructure.Tests
{
    public class OrganizationRepositoryTests
    {
        private DbContextOptions<CustomerContext> ContextOptions { get; }


        public OrganizationRepositoryTests()
        {
            ContextOptions = new DbContextOptionsBuilder<CustomerContext>().UseSqlite($"Data Source={Guid.NewGuid()}.db").Options;
            new UnitTestDatabaseSeeder().SeedUnitTestDatabase(ContextOptions);
        }


        [Fact]
        public async Task GetUserByMobileNumber_WithNumberFromOtherOrganization_ShouldNotBeFound()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            const string USED_MOBILE_NUMBER_IN_CUSTOMER_ONE = "+4799999999";
            var notFoundUser = await organizationRepository.GetUserByMobileNumber(USED_MOBILE_NUMBER_IN_CUSTOMER_ONE, UnitTestDatabaseSeeder.CUSTOMER_THREE_ID);
            Assert.Null(notFoundUser);
        }


        [Fact]
        public async Task GetUserByMobileNumber_WithNumberFromSameOrganization_ShouldBeFound()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            const string USED_MOBILE_NUMBER_IN_CUSTOMER_ONE = "+4799999999";
            var notFoundUser = await organizationRepository.GetUserByMobileNumber(USED_MOBILE_NUMBER_IN_CUSTOMER_ONE, UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
            Assert.Equal(notFoundUser!.MobileNumber, USED_MOBILE_NUMBER_IN_CUSTOMER_ONE);
        }


        [Fact(DisplayName = "User advanced search: Quicksearch (name)")]
        public async Task UserAdvancedSearchAsync_Quicksearch_Name_Test()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            UserSearchParameters nameContainsSearchParameters1 = new() { QuickSearch = "Gordon F", QuickSearchSearchType = Common.Enums.StringSearchType.Contains };
            var nameContainsResults1 = await organizationRepository.UserAdvancedSearchAsync(nameContainsSearchParameters1, 1, 25, new());

            UserSearchParameters nameContainsSearchParameters2 = new() { QuickSearch = "Gordon", QuickSearchSearchType = Common.Enums.StringSearchType.Contains };
            var nameContainsResults2 = await organizationRepository.UserAdvancedSearchAsync(nameContainsSearchParameters2, 1, 25, new());

            UserSearchParameters nameContainsSearchParameters3 = new() { QuickSearch = "Freeman", QuickSearchSearchType = Common.Enums.StringSearchType.Contains };
            var nameContainsResults3 = await organizationRepository.UserAdvancedSearchAsync(nameContainsSearchParameters3, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters1 = new() { QuickSearch = "Gordon Freeman", QuickSearchSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults1 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters1, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters2 = new() { QuickSearch = "Gordon", QuickSearchSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults2 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters2, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters3 = new() { QuickSearch = "Freeman", QuickSearchSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults3 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters3, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters4 = new() { QuickSearch = "Gordon F", QuickSearchSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults4 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters4, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters1 = new() { QuickSearch = "Gordon", QuickSearchSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults1 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters1, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters2 = new() { QuickSearch = "Gordon F", QuickSearchSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults2 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters2, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters3 = new() { QuickSearch = "Freeman", QuickSearchSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults3 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters3, 1, 25, new());


            // Contains
            Assert.Equal(1, nameContainsResults1.Items.Count);
            Assert.Equal(1, nameContainsResults2.Items.Count);
            Assert.Equal(1, nameContainsResults3.Items.Count);

            // Equals
            Assert.Equal(1, nameEqualsResults1.Items.Count);
            Assert.Equal(0, nameEqualsResults2.Items.Count);
            Assert.Equal(0, nameEqualsResults3.Items.Count);
            Assert.Equal(0, nameEqualsResults4.Items.Count);

            // Starts with
            Assert.Equal(1, nameStartsWithResults1.Items.Count);
            Assert.Equal(1, nameStartsWithResults2.Items.Count);
            Assert.Equal(0, nameStartsWithResults3.Items.Count);
        }


        [Fact(DisplayName = "User advanced search: Quicksearch (email)")]
        public async Task UserAdvancedSearchAsync_Quicksearch_Test()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            UserSearchParameters emailContainsSearchParameters1 = new() { QuickSearch = "don@free", QuickSearchSearchType = Common.Enums.StringSearchType.Contains };
            var emailContainsResults1 = await organizationRepository.UserAdvancedSearchAsync(emailContainsSearchParameters1, 1, 25, new());

            UserSearchParameters emailStartsWithSearchParameters1 = new() { QuickSearch = "gordon@freeman", QuickSearchSearchType = Common.Enums.StringSearchType.StartsWith };
            var emailStartsWithResults1 = await organizationRepository.UserAdvancedSearchAsync(emailStartsWithSearchParameters1, 1, 25, new());

            UserSearchParameters emailStartsWithSearchParameters2 = new() { QuickSearch = "don@free", QuickSearchSearchType = Common.Enums.StringSearchType.StartsWith };
            var emailStartsWithResults2 = await organizationRepository.UserAdvancedSearchAsync(emailStartsWithSearchParameters2, 1, 25, new());

            UserSearchParameters emailEqualsSearchParameters1 = new() { QuickSearch = "gordon@freeman.com", QuickSearchSearchType = Common.Enums.StringSearchType.Equals };
            var emailEqualsResults1 = await organizationRepository.UserAdvancedSearchAsync(emailEqualsSearchParameters1, 1, 25, new());

            UserSearchParameters emailEqualsSearchParameters2 = new() { QuickSearch = "gordon@freeman", QuickSearchSearchType = Common.Enums.StringSearchType.Equals };
            var emailEqualsResults2 = await organizationRepository.UserAdvancedSearchAsync(emailEqualsSearchParameters2, 1, 25, new());


            // Assert
            Assert.Equal(1, emailContainsResults1.Items.Count);

            Assert.Equal(1, emailStartsWithResults1.Items.Count);
            Assert.Equal(0, emailStartsWithResults2.Items.Count);

            Assert.Equal(1, emailEqualsResults1.Items.Count);
            Assert.Equal(0, emailEqualsResults2.Items.Count);
        }


        [Fact(DisplayName = "User advanced search: Email")]
        public async Task UserAdvancedSearchAsync_Email_Test()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            UserSearchParameters emailContainsSearchParameters1 = new() { Email = "don@free", EmailSearchType = Common.Enums.StringSearchType.Contains };
            var emailContainsResults1 = await organizationRepository.UserAdvancedSearchAsync(emailContainsSearchParameters1, 1, 25, new());

            UserSearchParameters emailStartsWithSearchParameters1 = new() { Email = "gordon@freeman", EmailSearchType = Common.Enums.StringSearchType.StartsWith };
            var emailStartsWithResults1 = await organizationRepository.UserAdvancedSearchAsync(emailStartsWithSearchParameters1, 1, 25, new());

            UserSearchParameters emailStartsWithSearchParameters2 = new() { Email = "don@free", EmailSearchType = Common.Enums.StringSearchType.StartsWith };
            var emailStartsWithResults2 = await organizationRepository.UserAdvancedSearchAsync(emailStartsWithSearchParameters2, 1, 25, new());

            UserSearchParameters emailEqualsSearchParameters1 = new() { Email = "gordon@freeman.com", EmailSearchType = Common.Enums.StringSearchType.Equals };
            var emailEqualsResults1 = await organizationRepository.UserAdvancedSearchAsync(emailEqualsSearchParameters1, 1, 25, new());

            UserSearchParameters emailEqualsSearchParameters2 = new() { Email = "gordon@freeman", EmailSearchType = Common.Enums.StringSearchType.Equals };
            var emailEqualsResults2 = await organizationRepository.UserAdvancedSearchAsync(emailEqualsSearchParameters2, 1, 25, new());


            // Assert
            Assert.Equal(1, emailContainsResults1.Items.Count);

            Assert.Equal(1, emailStartsWithResults1.Items.Count);
            Assert.Equal(0, emailStartsWithResults2.Items.Count);

            Assert.Equal(1, emailEqualsResults1.Items.Count);
            Assert.Equal(0, emailEqualsResults2.Items.Count);
        }


        [Fact(DisplayName = "User advanced search: Full Name")]
        public async Task UserAdvancedSearchAsync_FullName_Test()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            UserSearchParameters nameContainsSearchParameters1 = new() { FullName = "Gordon F", FullNameSearchType = Common.Enums.StringSearchType.Contains };
            var nameContainsResults1 = await organizationRepository.UserAdvancedSearchAsync(nameContainsSearchParameters1, 1, 25, new());

            UserSearchParameters nameContainsSearchParameters2 = new() { FullName = "Gordon", FullNameSearchType = Common.Enums.StringSearchType.Contains };
            var nameContainsResults2 = await organizationRepository.UserAdvancedSearchAsync(nameContainsSearchParameters2, 1, 25, new());

            UserSearchParameters nameContainsSearchParameters3 = new() { FullName = "Freeman", FullNameSearchType = Common.Enums.StringSearchType.Contains };
            var nameContainsResults3 = await organizationRepository.UserAdvancedSearchAsync(nameContainsSearchParameters3, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters1 = new() { FullName = "Gordon Freeman", FullNameSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults1 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters1, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters2 = new() { FullName = "Gordon", FullNameSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults2 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters2, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters3 = new() { FullName = "Freeman", FullNameSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults3 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters3, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters4 = new() { FullName = "Gordon F", FullNameSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults4 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters4, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters1 = new() { FullName = "Gordon", FullNameSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults1 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters1, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters2 = new() { FullName = "Gordon F", FullNameSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults2 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters2, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters3 = new() { FullName = "Freeman", FullNameSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults3 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters3, 1, 25, new());


            // Contains
            Assert.Equal(1, nameContainsResults1.Items.Count);
            Assert.Equal(1, nameContainsResults2.Items.Count);
            Assert.Equal(1, nameContainsResults3.Items.Count);

            // Equals
            Assert.Equal(1, nameEqualsResults1.Items.Count);
            Assert.Equal(0, nameEqualsResults2.Items.Count);
            Assert.Equal(0, nameEqualsResults3.Items.Count);
            Assert.Equal(0, nameEqualsResults4.Items.Count);

            // Starts with
            Assert.Equal(1, nameStartsWithResults1.Items.Count);
            Assert.Equal(1, nameStartsWithResults2.Items.Count);
            Assert.Equal(0, nameStartsWithResults3.Items.Count);
        }


        [Fact(DisplayName = "User advanced search: First Name")]
        public async Task UserAdvancedSearchAsync_FirstName_Test()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            UserSearchParameters nameContainsSearchParameters1 = new() { FirstName = "rdon", FirstNameSearchType = Common.Enums.StringSearchType.Contains };
            var nameContainsResults1 = await organizationRepository.UserAdvancedSearchAsync(nameContainsSearchParameters1, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters1 = new() { FirstName = "Gord", FirstNameSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults1 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters1, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters2 = new() { FirstName = "ordon", FirstNameSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults2 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters2, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters1 = new() { FirstName = "Gordon", FirstNameSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults1 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters1, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters2 = new() { FirstName = "ordon", FirstNameSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults2 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters2, 1, 25, new());


            // Contains
            Assert.Equal(1, nameContainsResults1.Items.Count);

            // Starts with
            Assert.Equal(1, nameStartsWithResults1.Items.Count);
            Assert.Equal(0, nameStartsWithResults2.Items.Count);

            // Equals
            Assert.Equal(1, nameEqualsResults1.Items.Count);
            Assert.Equal(0, nameEqualsResults2.Items.Count);
        }


        [Fact(DisplayName = "User advanced search: Last Name")]
        public async Task UserAdvancedSearchAsync_LastName_Test()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            UserSearchParameters nameContainsSearchParameters1 = new() { LastName = "reeman", LastNameSearchType = Common.Enums.StringSearchType.Contains };
            var nameContainsResults1 = await organizationRepository.UserAdvancedSearchAsync(nameContainsSearchParameters1, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters1 = new() { LastName = "Free", LastNameSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults1 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters1, 1, 25, new());

            UserSearchParameters nameStartsWithSearchParameters2 = new() { LastName = "reeman", LastNameSearchType = Common.Enums.StringSearchType.StartsWith };
            var nameStartsWithResults2 = await organizationRepository.UserAdvancedSearchAsync(nameStartsWithSearchParameters2, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters1 = new() { LastName = "Freeman", LastNameSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults1 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters1, 1, 25, new());

            UserSearchParameters nameEqualsSearchParameters2 = new() { LastName = "reeman", LastNameSearchType = Common.Enums.StringSearchType.Equals };
            var nameEqualsResults2 = await organizationRepository.UserAdvancedSearchAsync(nameEqualsSearchParameters2, 1, 25, new());


            // Contains
            Assert.Equal(1, nameContainsResults1.Items.Count);

            // Starts with
            Assert.Equal(1, nameStartsWithResults1.Items.Count);
            Assert.Equal(0, nameStartsWithResults2.Items.Count);

            // Equals
            Assert.Equal(1, nameEqualsResults1.Items.Count);
            Assert.Equal(0, nameEqualsResults2.Items.Count);
        }


        [Fact(DisplayName = "User advanced search: Organization IDs")]
        public async Task UserAdvancedSearchAsync_OrganizationIds_Test()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            UserSearchParameters existingOrganizationSearchParameters1 = new() { OrganizationIds = new() { UnitTestDatabaseSeeder.CUSTOMER_ONE_ID } };
            var existingOrganizationResults1 = await organizationRepository.UserAdvancedSearchAsync(existingOrganizationSearchParameters1, 1, 25, new());

            UserSearchParameters existingOrganizationSearchParameters2 = new() { OrganizationIds = new() { UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, UnitTestDatabaseSeeder.CUSTOMER_TWO_ID } };
            var existingOrganizationResults2 = await organizationRepository.UserAdvancedSearchAsync(existingOrganizationSearchParameters2, 1, 25, new());

            UserSearchParameters nonExistingOrganizationSearchParameters = new() { OrganizationIds = new() { Guid.NewGuid() } };
            var nonExistingOrganizationResults = await organizationRepository.UserAdvancedSearchAsync(nonExistingOrganizationSearchParameters, 1, 25, new());

            UserSearchParameters existingAndNonExistingOrganizationSearchParameters = new() { OrganizationIds = new() { Guid.NewGuid(), UnitTestDatabaseSeeder.CUSTOMER_ONE_ID } };
            var existingAndNonExistingOrganizationResults = await organizationRepository.UserAdvancedSearchAsync(existingAndNonExistingOrganizationSearchParameters, 1, 25, new());

            // Assert
            Assert.Equal(4, existingOrganizationResults1.Items.Count);
            Assert.Equal(8, existingOrganizationResults2.Items.Count);
            Assert.Equal(0, nonExistingOrganizationResults.Items.Count);
            Assert.Equal(4, existingAndNonExistingOrganizationResults.Items.Count);
        }


        [Fact(DisplayName = "User advanced search: Department IDs")]
        public async Task UserAdvancedSearchAsync_DepartmentIds_Test()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            UserSearchParameters existingDepartmentSearchParameters1 = new() { DepartmentIds = new() { UnitTestDatabaseSeeder.DEPARTMENT_ONE_ID } };
            var existingDepartmentResults1 = await organizationRepository.UserAdvancedSearchAsync(existingDepartmentSearchParameters1, 1, 25, new());

            UserSearchParameters existingDepartmentSearchParameters2 = new() { DepartmentIds = new() { UnitTestDatabaseSeeder.DEPARTMENT_ONE_ID, UnitTestDatabaseSeeder.DEPARTMENT_TWO_ID } };
            var existingDepartmentResults2 = await organizationRepository.UserAdvancedSearchAsync(existingDepartmentSearchParameters2, 1, 25, new());

            UserSearchParameters nonExistingDepartmentSearchParameters = new() { DepartmentIds = new() { Guid.NewGuid() } };
            var nonExistingDepartmentResults = await organizationRepository.UserAdvancedSearchAsync(nonExistingDepartmentSearchParameters, 1, 25, new());

            UserSearchParameters existingAndNonExistingDepartmentSearchParameters = new() { DepartmentIds = new() { Guid.NewGuid(), UnitTestDatabaseSeeder.DEPARTMENT_ONE_ID } };
            var existingAndNonExistingDepartmentResults = await organizationRepository.UserAdvancedSearchAsync(existingAndNonExistingDepartmentSearchParameters, 1, 25, new());

            // Assert
            Assert.Equal(1, existingDepartmentResults1.Items.Count);
            Assert.Equal(2, existingDepartmentResults2.Items.Count);
            Assert.Equal(0, nonExistingDepartmentResults.Items.Count);
            Assert.Equal(1, existingAndNonExistingDepartmentResults.Items.Count);
        }


        [Fact(DisplayName = "User advanced search: Status IDs")]
        public async Task UserAdvancedSearchAsync_StatusIds_Test()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

            // Act
            UserSearchParameters existingStatusSearchParameters1 = new() { StatusIds = new() { (int)UserStatus.Invited } };
            var existingStatusResults1 = await organizationRepository.UserAdvancedSearchAsync(existingStatusSearchParameters1, 1, 25, new());

            UserSearchParameters existingStatusSearchParameters2 = new() { StatusIds = new() { (int)UserStatus.Invited, (int)UserStatus.Deactivated } };
            var existingStatusResults2 = await organizationRepository.UserAdvancedSearchAsync(existingStatusSearchParameters2, 1, 25, new());

            UserSearchParameters nonExistingStatusSearchParameters = new() { StatusIds = new() { 99 } };
            var nonExistingStatusResults = await organizationRepository.UserAdvancedSearchAsync(nonExistingStatusSearchParameters, 1, 25, new());

            UserSearchParameters existingAndNonExistingStatusSearchParameters = new() { StatusIds = new() { 99, (int)UserStatus.Invited } };
            var existingAndNonExistingStatusResults = await organizationRepository.UserAdvancedSearchAsync(existingAndNonExistingStatusSearchParameters, 1, 25, new());

            // Assert
            Assert.Equal(1, existingStatusResults1.Items.Count);
            Assert.Equal(2, existingStatusResults2.Items.Count);
            Assert.Equal(0, nonExistingStatusResults.Items.Count);
            Assert.Equal(1, existingAndNonExistingStatusResults.Items.Count);
        }

    }
}