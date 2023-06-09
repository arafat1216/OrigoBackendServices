﻿using Common.Converters.EntityFramework;
using Common.EntityFramework;
using HardwareServiceOrderServices.Infrastructure.EntityConfiguration;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.SeedData;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrderServices.Infrastructure
{
    /// <summary>
    ///     The default <see cref="DbContext"/> that is used for handling service-request entities.
    /// </summary>
    public class HardwareServiceOrderContext : DbContext, IDataProtectionKeyContext
    {
        private readonly bool _isSQLite;
        private readonly IApiRequesterService? apiRequesterService;

        /// <summary>
        ///     Automatically set during creation, and indicates if this DbContext is currently using SQLite (unit-testing).
        /// </summary>
        public bool IsSQLite => _isSQLite;

        /// <summary>
        ///     If available, the ID of the authenticated user that made the API call.
        /// </summary>
        public Guid? AuthenticatedUserId { get { return apiRequesterService?.AuthenticatedUserId; } }

        public DbSet<CustomerSettings> CustomerSettings { get; set; } = null!;
        public DbSet<HardwareServiceOrder> HardwareServiceOrders { get; set; } = null!;
        public DbSet<ServiceProvider> ServiceProviders { get; set; } = null!;
        public DbSet<ServiceStatus> ServiceStatuses { get; set; } = null!;
        public DbSet<ServiceType> ServiceTypes { get; set; } = null!;
        public DbSet<CustomerServiceProvider> CustomerServiceProviders { get; set; } = null!;
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
        public DbSet<ServiceOrderAddon> ServiceOrderAddons { get; set; } = null!;
        public DbSet<ApiCredential> ApiCredentials { get; set; } = null!;
        public DbSet<CustomerServiceProviderServiceOrderAddon> CustomerServiceProviderServiceOrderAddons { get; set; } = null!;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HardwareServiceOrderContext"/>-class.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="apiRequesterService"> A dependency-injected interface that provides information about the current HTTP request,
        ///     such as the user that initiated the request. This information is accessed when entities in this DbContext is saved. </param>
        public HardwareServiceOrderContext(DbContextOptions<HardwareServiceOrderContext> options, IApiRequesterService? apiRequesterService = null) : base(options)
        {
            this.apiRequesterService = apiRequesterService;

            foreach (var extension in options.Extensions)
            {
                if (!extension.GetType().ToString().Contains("Sqlite"))
                    continue;

                _isSQLite = true;
                break;
            }
        }


        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerSettingsConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new HardwareServiceOrderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceProviderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceStatusConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceTypeConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new CustomerServiceProviderConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceProviderServiceTypeConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceOrderAddonConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ApiCredentialConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new CustomerServiceProviderServiceOrderAddonConfiguration(_isSQLite));
            modelBuilder.ApplyConfiguration(new ServiceEventConfiguration(_isSQLite));


            // Add mandatory (production ready) seeding-data
            modelBuilder.SeedServiceStatus();
            modelBuilder.SeedServiceType();
            modelBuilder.SeedServiceProvider();
            modelBuilder.SeedServiceProviderServiceType();
            modelBuilder.SeedServiceOrderAddon();
        }


        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_isSQLite is false)
                optionsBuilder.AddInterceptors(new SaveContextChangesInterceptor(apiRequesterService?.AuthenticatedUserId));
            else if (_isSQLite && apiRequesterService is not null)
                optionsBuilder.AddInterceptors(new SaveContextChangesInterceptor(apiRequesterService?.AuthenticatedUserId));
        }


        // Add support for DataOnly conversions, as this is currently not fully implemented in all of EF's database extensions.
        /// <inheritdoc/>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            if (IsSQLite)
            {
                configurationBuilder.Properties<DateOnly>()
                                    .HaveConversion<DateOnlyConverter>();

                configurationBuilder.Properties<DateOnly?>()
                                    .HaveConversion<NullableDateOnlyConverter>();
            }
            else
            {
                configurationBuilder.Properties<DateOnly>()
                                    .HaveConversion<DateOnlyConverter>()
                                    .HaveColumnType("date");

                configurationBuilder.Properties<DateOnly?>()
                                    .HaveConversion<NullableDateOnlyConverter>()
                                    .HaveColumnType("date");
            }

            base.ConfigureConventions(configurationBuilder);
        }
    }
}
