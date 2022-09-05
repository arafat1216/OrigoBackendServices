using CustomerServices.Infrastructure.Context;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Infrastructure;
using Moq;

// ReSharper disable InconsistentNaming

namespace CustomerServices.UnitTests
{
    public class OrganizationServicesBaseTest
    {
        protected readonly Guid CUSTOMER_ONE_ID = new("661b01e0-481b-4e7d-8076-a0e7b6496ae3");
        protected readonly Guid DEPARTMENT_ONE_ID = new("f0680388-145a-11ec-a469-00155d98690f");
        protected readonly Guid DEPARTMENT_TWO_ID = new("424f4485-53cc-4e59-8fae-59b27f12ff61");
        protected readonly Guid CUSTOMER_TWO_ID = new("f1530515-fe2e-4e2f-84c2-c60da5875e22");
        private readonly Guid CUSTOMER_THREE_ID = new("6fb371c9-da3e-4ce4-b4e4-bc7f020eebf9");
        protected readonly Guid CUSTOMER_FOUR_ID = new("2C005777-ED56-43D9-9B1E-2B8112E67D10");
        protected readonly Guid CUSTOMER_FIVE_ID = new("94e47c5b-9486-4017-8bab-252422b1262a");

        protected readonly Guid USER_ONE_ID = new Guid("42803f8e-5608-4beb-a3e6-029b8e229d91");
        private readonly Guid USER_TWO_ID = new Guid("39349c24-6e47-4a5e-9bab-7b65f438fac5");
        protected readonly Guid USER_THREE_ID = new Guid("08DB1C4F-FAA3-436A-9598-90822649B793");
        protected readonly Guid USER_FOUR_ID = new Guid("a0c3ee8d-b543-4dc9-88b5-958c54b9d270");
        protected readonly Guid USER_FIVE_ID = new Guid("07c8ce13-a6a1-4a80-b029-619098a76bb5"); 
        protected readonly Guid USER_SIX_ID = new Guid("cee1e91f-e6d0-44ef-866a-23b032f3a214"); 
        protected readonly Guid USER_SEVEN_ID = new Guid("c43a8c1e-8ded-42ab-a0d1-0e4c2ce92cdc"); 

        protected readonly Guid LOCATION_ONE_ID = new("8080A5F0-57C6-4D72-B164-82D54A94C776");
        private readonly Guid LOCATION_TWO_ID = new("DDF4FDB7-B1B9-4F03-B343-2B2F38AC6138");
        private readonly Guid LOCATION_THREE_ID = new("E52D8C49-727F-49B1-8DAC-2CE1B199AE15");
        private readonly Guid LOCATION_FOUR_ID = new("8C5D801A-E7E8-45BB-B9AD-F32111D7AA8D");
        protected readonly Guid EMPTY_CALLER_ID = Guid.Empty;
        protected readonly Guid CALLER_ID = new("5E9729DA-8D91-41AD-9D88-BB0E0BC25C99");

        protected OrganizationServicesBaseTest(DbContextOptions<CustomerContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        protected DbContextOptions<CustomerContext> ContextOptions { get; }

        private void Seed()
        {
            var apiRequesterServiceMock = new Mock<IApiRequesterService>();
            apiRequesterServiceMock.Setup(r => r.AuthenticatedUserId).Returns(CALLER_ID);
            using var context = new CustomerContext(ContextOptions, apiRequesterServiceMock.Object);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var customerOne = new Organization(CUSTOMER_ONE_ID, null, "COMPANY ONE", "999888777",
                new Address("My Way 1", "1111", "My City", "NO"),
                new ContactPerson("JOHN", "DOE", "john.doe@example.com", "99999999"),
                new OrganizationPreferences(CUSTOMER_ONE_ID, USER_ONE_ID, "webPage 1", "logoUrl 1", "organizationNotes 1", true, "nb", 0),
                new Location("COMPANY ONE", "Location of COMPANY ONE", "My Way 1A", "My Way 1B", "0585", "Oslo", "NO"),
                null, true, 1, "");

            var customerTwo = new Organization(CUSTOMER_TWO_ID, null, "COMPANY TWO", "999777666",
                new Address("My Way 2", "1111", "My City", "NO"),
                new ContactPerson("Ola", "Nordmann", "ola.nordmann@example.com", "99999998"),
                new OrganizationPreferences(CUSTOMER_TWO_ID, USER_ONE_ID, "webPage 2", "logoUrl 2", "organizationNotes 2", true, "nb", 0),
                new Location("name", "description", "My Way 2A", "My Way 2B", "0585", "Oslo", "NO"),
                null, true, 1, "",true);

            var customerThree = new Organization(CUSTOMER_THREE_ID, CUSTOMER_ONE_ID, "COMPANY THREE", "999666555",
                new Address("My Way 3", "1111", "My Other City", "NO"),
                new ContactPerson("Kari", "Nordmann", "kari.nordmann@example.com", "99999997"),
                new OrganizationPreferences(CUSTOMER_THREE_ID, USER_ONE_ID, "webPage 3", "logoUrl 3", "organizationNotes 3", true, "nb", 0),
                new Location("name", "description", "My Way 3A", "My Way 3B", "0585", "Oslo", "NO"),
                null, true, 1, "");

            var customerFour = new Organization(CUSTOMER_FOUR_ID, null, "COMPANY FOUR", "999555444",
                new Address("My Way 4", "1111", "My City", "NO"),
                new ContactPerson("Petter", "Smart", "petter.smart@example.com", "99999996"),
                new OrganizationPreferences(CUSTOMER_FOUR_ID, USER_ONE_ID, "webPage 4", "logoUrl 4", "organizationNotes 4", true, "nb", 0),
                new Location("name", "description", "My Way 4A", "My Way 4B", "0585", "Oslo", "NO"),
                null, true, 1, "",true);
            customerFour.InitiateOnboarding();

            var customerFive = new Organization(CUSTOMER_FIVE_ID, null, "COMPANY FIVE", "999555555",
               new Address("My Way 5", "1111", "My City", "NO"),
               new ContactPerson("Donald", "Duck", "donald.duck@example.com", "99999995"),
               new OrganizationPreferences(CUSTOMER_FIVE_ID, USER_ONE_ID, "webPage 5", "logoUrl 5", "organizationNotes 5", true, "nb", 0),
               new Location("name", "description", "My Way 5A", "My Way 5B", "0585", "Oslo", "NO"),
               null, true, 1, "");
            customerFive.AddTechstepCustomerId(123456789);


            context.AddRange(customerOne, customerTwo, customerThree, customerFour, customerFive);
            context.OrganizationPreferences.AddRange(customerOne.Preferences, customerTwo.Preferences, customerThree.Preferences, customerFour.Preferences);
            context.Locations.AddRange(customerOne.PrimaryLocation!, customerTwo.PrimaryLocation!, customerThree.PrimaryLocation!, customerFour.PrimaryLocation!);
            var departmentOneForCustomerOne = new Department("Cust1Dept1", "1123", "Department one for customer one", customerOne, DEPARTMENT_ONE_ID, Guid.Empty);
            var departmentTwoForCustomerOne = new Department("Department1", "456", "Desc", customerOne, DEPARTMENT_TWO_ID, Guid.Empty, null);

            context.Add(departmentTwoForCustomerOne);
            context.Add(departmentOneForCustomerOne);

            var userPreferences1 = new UserPreference("NO", EMPTY_CALLER_ID);
            var userPreferences2 = new UserPreference("EN", EMPTY_CALLER_ID);
            var userPreferences3 = new UserPreference("EN", EMPTY_CALLER_ID);
            var userOne = new User(customerOne, USER_ONE_ID, "Jane", "Doe", "jane@doe.com", "+4799999999", "007", userPreferences1, EMPTY_CALLER_ID);
            userOne.ChangeUserStatus("123",Common.Enums.UserStatus.Deactivated);

            var userTwo = new User(customerOne, USER_THREE_ID, "Gordon", "Freeman", "gordon@freeman.com", "+4755555555", "DH-101", userPreferences2, EMPTY_CALLER_ID);
            userTwo.AssignDepartment(departmentOneForCustomerOne, EMPTY_CALLER_ID);
            userTwo.ChangeUserStatus("123", Common.Enums.UserStatus.Activated);

            var userPreferences4 = new UserPreference("EN", EMPTY_CALLER_ID);
            var userPreferences5 = new UserPreference("EN", EMPTY_CALLER_ID);
            var userFour = new User(customerOne, USER_FOUR_ID, "Al", "Pacino", "al@Pacino.com", "+4755555555", "DH-104", userPreferences4, EMPTY_CALLER_ID);
            var userFive = new User(customerOne, USER_FIVE_ID, "Robert", "De Niro", "robert@deniro.com", "+4755555555", "DH-105", userPreferences5, EMPTY_CALLER_ID);
            userFive.AssignDepartment(departmentTwoForCustomerOne, EMPTY_CALLER_ID);

            //CustomerTwo
            var userThree = new User(customerTwo, USER_TWO_ID, "John", "Doe", "john@doe.com", "+4791111111", "X", userPreferences3, EMPTY_CALLER_ID);
            userThree.ChangeUserStatus("123", Common.Enums.UserStatus.Invited);
            var userSix = new User(customerTwo, USER_SIX_ID, "Elvis", "Presley", "the@king.com", "+4790000000", "X", userPreferences3, EMPTY_CALLER_ID);

            var userSeven = new User(customerTwo, USER_SEVEN_ID, "Lisa Marie", "Presley", "the@princess.com", "+4790000001", "X", userPreferences3, EMPTY_CALLER_ID);
            userSeven.ChangeUserStatus("123",Common.Enums.UserStatus.OnboardInitiated);


            var role1 = new Role("admin");
            var managerRole = new Role("Manager");
            var userPersmission = new UserPermissions(userOne, role1, new List<Guid>(), EMPTY_CALLER_ID);
            var managerPersmission = new UserPermissions(userFour, managerRole, new List<Guid>(), EMPTY_CALLER_ID);
            var managerPersmissionTwo = new UserPermissions(userFive, managerRole, new List<Guid>(), EMPTY_CALLER_ID);

            userFour.AssignManagerToDepartment(departmentTwoForCustomerOne, EMPTY_CALLER_ID);
            userFive.AssignManagerToDepartment(departmentTwoForCustomerOne,EMPTY_CALLER_ID);


            context.AddRange(role1,managerRole);
            context.AddRange(userPersmission, managerPersmission, managerPersmissionTwo);
            context.AddRange(userOne, userTwo, userThree, userFour, userFive, userSix, userSeven);

            context.SaveChanges();
        }
    }
}
