﻿using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;
using System.Reflection;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagmentContext : DbContext
    {
        public SubscriptionManagmentContext(DbContextOptions<SubscriptionManagmentContext> options) : base(options)
        {

        }
        public DbSet<Operator> Operator { get; set; }
        public DbSet<OperatorAccount> OperatorAccounts { get; set; }
        public DbSet<SubscriptionProduct> SubscriptionProducts { get; set; }
        public DbSet<SubscriptionOrder> SubscriptionsOrders { get; set; } 
        public DbSet<Datapackage> Datapackages { get; set; }
        public DbSet<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Guid createdById = Guid.Parse("00000000-0000-0000-0000-000000000000");



            modelBuilder.Entity<Operator>(entity =>
            {
                
                entity.HasData(new { Id = 1, OperatorName = "Telia - NO", Country = "nb", CreatedBy= createdById, CreatedDate = DateTime.Now, DeletedBy = Guid.Empty,UpdatedBy = Guid.Empty, IsDeleted = false});
                entity.HasData(new { Id = 2, OperatorName = "Telia - SE", Country = "se", CreatedBy = createdById, CreatedDate = DateTime.Now, DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });
                entity.HasData(new { Id = 3, OperatorName = "Telenor - NO", Country = "nb", CreatedBy = createdById, CreatedDate = DateTime.Now, DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });
                entity.HasData(new { Id = 4, OperatorName = "Telenor - SE", Country = "se", CreatedBy = createdById, CreatedDate = DateTime.Now, DeletedBy = Guid.Empty, UpdatedBy = Guid.Empty, IsDeleted = false });

            });
                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
