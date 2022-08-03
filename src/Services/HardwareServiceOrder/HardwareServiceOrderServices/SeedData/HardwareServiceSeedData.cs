using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrderServices.SeedData
{
    /// <summary>
    ///     Defines the seeding-data that should be added to the database. 
    ///     This should only contain seeding-data that is intended for the production-environment.
    /// </summary>
    internal static class HardwareServiceSeedData
    {
        /// <summary>
        ///     The system's user-ID. This identifies an action as being completed by the system.
        /// </summary>
        private static readonly Guid _systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");


        /// <summary>
        ///     Applies seeding-data for the <see cref="ServiceStatus"/> entities.
        /// </summary>
        /// <param name="builder"></param>
        public static void SeedServiceStatus(this ModelBuilder builder)
        {
            var statuses = Enum.GetValues(typeof(ServiceStatusEnum))
                    .Cast<int>()
                    .Where(i => i > 0) // Ignoring Null
                    .Select(m => new { Id = m, CreatedBy = _systemUserId, UpdatedBy = _systemUserId, IsDeleted = false });

            builder.Entity<ServiceStatus>().HasData(statuses);
        }


        /// <summary>
        ///     Applies seeding-data for the <see cref="ServiceType"/> entities.
        /// </summary>
        /// <param name="builder"></param>
        public static void SeedServiceType(this ModelBuilder builder)
        {
            // Seed data
            var serviceTypes = Enum.GetValues(typeof(ServiceTypeEnum))
                    .Cast<int>()
                    .Where(i => i > 0) // Ignoring Null
                    .Select(m => new { Id = m, CreatedBy = _systemUserId, UpdatedBy = _systemUserId, IsDeleted = false });

            builder.Entity<ServiceType>().HasData(serviceTypes);
        }


        /// <summary>
        ///     Applies seeding-data for the <see cref="ServiceProvider"/> entities.
        /// </summary>
        /// <param name="builder"></param>
        public static void SeedServiceProvider(this ModelBuilder builder)
        {
            builder.Entity<ServiceProvider>(entity =>
            {
                // Conmodo Norway
                entity.HasData(new { Id = (int)ServiceProviderEnum.ConmodoNo, Name = "Conmodo (NO)", OrganizationId = Guid.Empty, CreatedBy = _systemUserId, UpdatedBy = _systemUserId, IsDeleted = false });
            });
        }


        /// <summary>
        ///     Applies seeding-data for the <see cref="ServiceProviderServiceType"/> entities.
        /// </summary>
        /// <param name="builder"></param>
        public static void SeedServiceProviderServiceType(this ModelBuilder builder)
        {
            builder.Entity<ServiceProviderServiceType>(entity =>
            {
                entity.HasData(new { Id = 1, ServiceProviderId = (int)ServiceProviderEnum.ConmodoNo, ServiceTypeId = (int)ServiceTypeEnum.SUR, CreatedBy = _systemUserId, UpdatedBy = _systemUserId, IsDeleted = false });
            });
        }


        /// <summary>
        ///     Applies seeding-data for the <see cref="ServiceOrderAddon"/> entities.
        /// </summary>
        /// <param name="builder"></param>
        public static void SeedServiceOrderAddon(this ModelBuilder builder)
        {
            builder.Entity<ServiceOrderAddon>(entity =>
            {
                entity.HasData(new { Id = 1, ServiceProviderId = (int)ServiceProviderEnum.ConmodoNo, ThirdPartyId = "", IsUserSelectable = true, IsCustomerTogglable = true, CreatedBy = _systemUserId, UpdatedBy = _systemUserId, IsDeleted = false });
            });
        }
    }
}
