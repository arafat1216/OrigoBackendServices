using System;
using CustomerServices.Infrastructure;
using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
// ReSharper disable InconsistentNaming

namespace CustomerServices.UnitTests
{
    public class CustomerBaseTest
    {
        protected readonly Guid CUSTOMER_ONE_ID = new("661b01e0-481b-4e7d-8076-a0e7b6496ae3");
        private readonly Guid CUSTOMER_TWO_ID = new("f1530515-fe2e-4e2f-84c2-c60da5875e22");
        private readonly Guid CUSTOMER_THREE_ID = new("6fb371c9-da3e-4ce4-b4e4-bc7f020eebf9");

        protected CustomerBaseTest(DbContextOptions<CustomerContext> contextOptions)
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

            var customerOne = new Customer(CUSTOMER_ONE_ID, "COMPANY ONE", "999888777",
                new Address("My Way 1", "My City", "NO", "1111"),
                new ContactPerson("JOHN DOE", "john.doe@example.com", "99999999"));

            var customerTwo = new Customer(CUSTOMER_TWO_ID, "COMPANY TWO", "999777666",
                new Address("My Way 2", "My City", "NO", "1111"),
                new ContactPerson("Ola Nordmann", "ola.nordmann@example.com", "99999998"));

            var customerThree = new Customer(CUSTOMER_THREE_ID, "COMPANY THREE", "999666555",
                new Address("My Way 3", "My Other City", "NO", "1111"),
                new ContactPerson("Kari Nordmann", "kari.nordmann@example.com", "99999997"));


            context.AddRange(customerOne, customerTwo, customerThree);

            context.SaveChanges();
        }
    }
}
