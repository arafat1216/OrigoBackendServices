using AssetServices.Infrastructure.EntityConfiguration;
using AssetServices.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetServices.Infrastructure;

public class AssetsContext : DbContext
{
    public bool IsSQLite { get; }

    public AssetsContext(DbContextOptions<AssetsContext> options) : base(options)
    {
        foreach (var extension in options.Extensions)
        {
            if (extension.GetType().ToString().Contains("Sqlite") || extension.GetType().ToString().Contains("InMemory"))
            {
                IsSQLite = true;
            }
        }
    }

    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<MobilePhone> MobilePhones => Set<MobilePhone>();
    public DbSet<Tablet> Tablets => Set<Tablet>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<AssetLifecycle> AssetLifeCycles => Set<AssetLifecycle>();
    public DbSet<CustomerLabel> CustomerLabels => Set<CustomerLabel>();
    public DbSet<User> Users => Set<User>();
    public DbSet<LifeCycleSetting> LifeCycleSettings => Set<LifeCycleSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AssetConfiguration(IsSQLite));
        modelBuilder.ApplyConfiguration(new AssetLifecycleConfiguration(IsSQLite));
        modelBuilder.ApplyConfiguration(new SalaryDeductionTransactionConfiguration(IsSQLite));
        modelBuilder.ApplyConfiguration(new LifeCycleSettingConfiguration(IsSQLite));
    }
}