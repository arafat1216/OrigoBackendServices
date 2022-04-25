using HardwareServiceOrderServices.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceBaseTests
    {
        protected readonly Guid CUSTOMER_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD62");
        protected readonly Guid CALLER_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD63");
        protected HardwareServiceOrderServiceBaseTests(DbContextOptions<HardwareServiceOrderContext> dbContext)
        {
            ContextOptions = dbContext;
            Seed();
        }
        protected DbContextOptions<HardwareServiceOrderContext> ContextOptions { get; }

        private void Seed()
        {
            using var context = new HardwareServiceOrderContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
