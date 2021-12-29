using CustomerServices.Infrastructure;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using System;

// ReSharper disable InconsistentNaming

namespace CustomerServices.UnitTests
{
    public class OrganizationServicesBaseTest
    {
        protected readonly Guid CUSTOMER_ONE_ID = new("661b01e0-481b-4e7d-8076-a0e7b6496ae3");
        protected readonly Guid DEPARTMENT_ONE_ID = new("f0680388-145a-11ec-a469-00155d98690f");
        private readonly Guid CUSTOMER_TWO_ID = new("f1530515-fe2e-4e2f-84c2-c60da5875e22");
        private readonly Guid CUSTOMER_THREE_ID = new("6fb371c9-da3e-4ce4-b4e4-bc7f020eebf9");
        private readonly Guid CUSTOMER_FOUR_ID = new("2C005777-ED56-43D9-9B1E-2B8112E67D10");

        protected readonly Guid USER_ONE_ID = new Guid("42803f8e-5608-4beb-a3e6-029b8e229d91");
        private readonly Guid USER_TWO_ID = new Guid("39349c24-6e47-4a5e-9bab-7b65f438fac5");

        protected readonly Guid LOCATION_ONE_ID = new("8080A5F0-57C6-4D72-B164-82D54A94C776");
        private readonly Guid LOCATION_TWO_ID = new("DDF4FDB7-B1B9-4F03-B343-2B2F38AC6138");
        private readonly Guid LOCATION_THREE_ID = new("E52D8C49-727F-49B1-8DAC-2CE1B199AE15");
        private readonly Guid LOCATION_FOUR_ID = new("8C5D801A-E7E8-45BB-B9AD-F32111D7AA8D");
        protected readonly Guid EMPTY_CALLER_ID = Guid.Empty;

        protected OrganizationServicesBaseTest(DbContextOptions<CustomerContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        protected DbContextOptions<CustomerContext> ContextOptions { get; }

        private void Seed()
        {
            using var context = new CustomerContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var customerOne = new Organization(CUSTOMER_ONE_ID, USER_ONE_ID, null, "COMPANY ONE", "999888777",
                new Address("My Way 1", "1111", "My City", "NO"),
                new ContactPerson("JOHN", "DOE", "john.doe@example.com", "99999999"),
                new OrganizationPreferences(CUSTOMER_ONE_ID, USER_ONE_ID, "webPage 1", "logoUrl 1", "organizationNotes 1", true, "NO", 0),
                new Location(LOCATION_ONE_ID, USER_ONE_ID, "COMPANY ONE", "Location of COMPANY ONE", "My Way 1A", "My Way 1B", "0585", "Oslo", "Norway"));

            var customerTwo = new Organization(CUSTOMER_TWO_ID, USER_ONE_ID, null, "COMPANY TWO", "999777666",
                new Address("My Way 2", "1111", "My City", "NO"),
                new ContactPerson("Ola", "Nordmann", "ola.nordmann@example.com", "99999998"),
                new OrganizationPreferences(CUSTOMER_TWO_ID, USER_ONE_ID, "webPage 2", "logoUrl 2", "organizationNotes 2", true, "NO", 0),
                new Location(LOCATION_TWO_ID, USER_ONE_ID, "name", "description", "My Way 2A", "My Way 2B", "0585", "Oslo", "Norway"));

            var customerThree = new Organization(CUSTOMER_THREE_ID, USER_ONE_ID, CUSTOMER_ONE_ID, "COMPANY THREE", "999666555",
                new Address("My Way 3", "1111", "My Other City", "NO"),
                new ContactPerson("Kari", "Nordmann", "kari.nordmann@example.com", "99999997"),
                new OrganizationPreferences(CUSTOMER_THREE_ID, USER_ONE_ID, "webPage 3", "logoUrl 3", "organizationNotes 3", true, "NO", 0),
                new Location(LOCATION_THREE_ID, USER_ONE_ID, "name", "description", "My Way 3A", "My Way 3B", "0585", "Oslo", "Norway"));

            var customerFour = new Organization(CUSTOMER_FOUR_ID, USER_ONE_ID, null, "COMPANY FOUR", "999555444", 
                new Address("My Way 4", "1111", "My City", "NO"),
                new ContactPerson("Petter", "Smart", "petter.smart@example.com", "99999996"),
                new OrganizationPreferences(CUSTOMER_FOUR_ID, USER_ONE_ID, "webPage 4", "logoUrl 4", "organizationNotes 4", true, "NO", 0),
                new Location(LOCATION_FOUR_ID, USER_ONE_ID, "name", "description", "My Way 4A", "My Way 4B", "0585", "Oslo", "Norway"));

            context.AddRange(customerOne, customerTwo, customerThree);

            context.AddRange(customerOne, customerTwo, customerThree, customerFour);
            context.OrganizationPreferences.AddRange(customerOne.Preferences, customerTwo.Preferences, customerThree.Preferences, customerFour.Preferences);
            context.Locations.AddRange(customerOne.Location, customerTwo.Location, customerThree.Location, customerFour.Location);
            //var department = new Department("Department1","456","Desc", customerFour, DEPARTMENT_ONE_ID, Guid.Empty, null);
            var departmentOneForCustomerOne = new Department("Cust1Dept1", "1123", "Department one for customer one", customerOne, DEPARTMENT_ONE_ID,Guid.Empty);
            //context.Add(department);
            context.Add(departmentOneForCustomerOne);

            var userPreferences1 = new UserPreference("NO", EMPTY_CALLER_ID);
            var userPreferences2 = new UserPreference("EN", EMPTY_CALLER_ID);
            var userPreferences3 = new UserPreference("EN", EMPTY_CALLER_ID);
            var userOne = new User(customerOne, USER_ONE_ID, "Jane", "Doe", "jane@doe.com", "+4799999999", "007", userPreferences1, EMPTY_CALLER_ID);
            var userTwo = new User(customerOne, Guid.NewGuid(), "Gordon", "Freeman", "gordon@freeman.com", "+4755555555", "DH-101", userPreferences2, EMPTY_CALLER_ID);
            var userThree = new User(customerTwo, USER_TWO_ID, "John", "Doe", "john@doe.com", "+4791111111", "X", userPreferences3, EMPTY_CALLER_ID);

            context.AddRange(userOne, userTwo, userThree);

            context.SaveChanges();
        }
    }
}
