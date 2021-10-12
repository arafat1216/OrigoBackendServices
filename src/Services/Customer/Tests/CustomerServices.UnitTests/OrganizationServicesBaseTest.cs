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
                new ContactPerson("JOHN DOE", "john.doe@example.com", "99999999"),
                new OrganizationPreferences(CUSTOMER_ONE_ID, USER_ONE_ID, "webPage 1", "logoUrl 1", "organizationNotes 1", true, "NO", 0),
                new Location(Guid.NewGuid(), USER_ONE_ID, "COMPANY ONE", "Location of COMPANY ONE", "My Way 1A", "My Way 1B", "0585", "Oslo", "Norway"));

            var customerTwo = new Organization(CUSTOMER_TWO_ID, USER_ONE_ID, null, "COMPANY TWO", "999777666",
                new Address("My Way 2", "1111", "My City", "NO"),
                new ContactPerson("Ola Nordmann", "ola.nordmann@example.com", "99999998"),
                new OrganizationPreferences(CUSTOMER_TWO_ID, USER_ONE_ID, "webPage 2", "logoUrl 2", "organizationNotes 2", true, "NO", 0),
                new Location(Guid.NewGuid(), USER_ONE_ID, "name", "description", "My Way 2A", "My Way 2B", "0585", "Oslo", "Norway"));

            var customerThree = new Organization(CUSTOMER_THREE_ID, USER_ONE_ID, CUSTOMER_ONE_ID, "COMPANY THREE", "999666555",
                new Address("My Way 3", "1111", "My Other City", "NO"),
                new ContactPerson("Kari Nordmann", "kari.nordmann@example.com", "99999997"),
                new OrganizationPreferences(CUSTOMER_THREE_ID, USER_ONE_ID, "webPage 3", "logoUrl 3", "organizationNotes 3", true, "NO", 0),
                new Location(Guid.NewGuid(), USER_ONE_ID, "name", "description", "My Way 3A", "My Way 3B", "0585", "Oslo", "Norway"));

            var customerFour = new Organization(CUSTOMER_FOUR_ID, USER_ONE_ID, null, "COMPANY FOUR", "999555444", 
                new Address("My Way 4", "1111", "My City", "NO"),
                new ContactPerson("Petter Smart", "petter.smart@example.com", "99999996"),
                new OrganizationPreferences(CUSTOMER_FOUR_ID, USER_ONE_ID, "webPage 4", "logoUrl 4", "organizationNotes 4", true, "NO", 0),
                new Location(Guid.NewGuid(), USER_ONE_ID, "name", "description", "My Way 4A", "My Way 4B", "0585", "Oslo", "Norway"));

            context.AddRange(customerOne, customerTwo, customerThree);

            context.AddRange(customerOne, customerTwo, customerThree, customerFour);
            context.OrganizationPreferences.AddRange(customerOne.OrganizationPreferences, customerTwo.OrganizationPreferences, customerThree.OrganizationPreferences, customerFour.OrganizationPreferences);
            context.Locations.AddRange(customerOne.OrganizationLocation, customerTwo.OrganizationLocation, customerThree.OrganizationLocation, customerFour.OrganizationLocation);
            var departmentOneForCustomerOne = new Department("Cust1Dept1", "1123", "Department one for customer one", customerOne, DEPARTMENT_ONE_ID);
            context.Add(departmentOneForCustomerOne);

            var userOne = new User(customerOne, USER_ONE_ID, "Jane", "Doe", "jane@doe.com", "+4799999999", "007");
            var userTwo = new User(customerTwo, USER_TWO_ID, "John", "Doe", "john@doe.com", "+4791111111", "X");

            context.AddRange(userOne, userTwo);

            context.SaveChanges();
        }
    }
}