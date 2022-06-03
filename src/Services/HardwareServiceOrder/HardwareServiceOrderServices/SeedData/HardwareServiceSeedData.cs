using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Extensions;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.SeedData
{
    public static class HardwareServiceSeedData
    {
        static Guid systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static void SeedServiceStatus(this ModelBuilder modelBuilder)
        {
            var statuses = Enum.GetValues(typeof(ServiceStatusEnum))
                    .Cast<int>()
                    .Where(i => i > 0) //Ignoring Null
                    .Select(m => new { Id = m, CreatedBy = systemUserId, UpdatedBy = systemUserId, IsDeleted = false });

            modelBuilder.Entity<ServiceStatus>().HasData(statuses);
        }

        public static void SeedServiceType(this ModelBuilder modelBuilder)
        {
            //Seed data
            var serviceTypes = Enum.GetValues(typeof(ServiceTypeEnum))
                    .Cast<int>()
                    .Where(i => i > 0) //Ignoring Null
                    .Select(m => new { Id = m, CreatedBy = systemUserId, UpdatedBy = systemUserId, IsDeleted = false });

            modelBuilder.Entity<ServiceType>().HasData(serviceTypes);
        }

        public static void SeedServiceProvider(this ModelBuilder modelBuilder)
        {
            //Conmodo Norway
            modelBuilder.Entity<ServiceProvider>().HasData(new { Id = 1, CreatedBy = systemUserId, UpdatedBy = systemUserId, IsDeleted = false, OrganizationId = Guid.Empty });
            //Conmodo Sweden
            modelBuilder.Entity<ServiceProvider>().HasData(new { Id = 2, CreatedBy = systemUserId, UpdatedBy = systemUserId, IsDeleted = false, OrganizationId = Guid.Empty });
        }
    }
}
