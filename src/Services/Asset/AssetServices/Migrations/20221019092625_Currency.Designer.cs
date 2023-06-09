﻿// <auto-generated />
using System;
using AssetServices.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AssetServices.Migrations
{
    [DbContext(typeof(AssetsContext))]
    [Migration("20221019092625_Currency")]
    partial class Currency
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AssetLifecycleCustomerLabel", b =>
                {
                    b.Property<int>("AssetLifecyclesId")
                        .HasColumnType("int");

                    b.Property<int>("LabelsId")
                        .HasColumnType("int");

                    b.HasKey("AssetLifecyclesId", "LabelsId");

                    b.HasIndex("LabelsId");

                    b.ToTable("AssetLifecycleCustomerLabel");
                });

            modelBuilder.Entity("AssetServices.Models.Asset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasAlternateKey("ExternalId");

                    b.ToTable("Assets");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Asset");
                });

            modelBuilder.Entity("AssetServices.Models.AssetImei", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<long>("Imei")
                        .HasColumnType("bigint");

                    b.Property<int?>("MobilePhoneId")
                        .HasColumnType("int");

                    b.Property<int?>("TabletId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MobilePhoneId");

                    b.HasIndex("TabletId");

                    b.ToTable("AssetImei");
                });

            modelBuilder.Entity("AssetServices.Models.AssetLifecycle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("AssetId")
                        .HasColumnType("int");

                    b.Property<int>("AssetLifecycleStatus")
                        .HasColumnType("int");

                    b.Property<int>("AssetLifecycleType")
                        .HasColumnType("int");

                    b.Property<int?>("ContractHolderUserId")
                        .HasColumnType("int");

                    b.Property<string>("ContractReferenceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndPeriod")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("InvoiceNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPersonal")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid?>("ManagedByDepartmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PurchasedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Source")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartPeriod")
                        .HasColumnType("datetime2");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasAlternateKey("ExternalId");

                    b.HasIndex("AssetId");

                    b.HasIndex("ContractHolderUserId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ManagedByDepartmentId");

                    b.ToTable("AssetLifeCycles");
                });

            modelBuilder.Entity("AssetServices.Models.CustomerLabel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("CustomerLabels");
                });

            modelBuilder.Entity("AssetServices.Models.CustomerSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("CustomerSettings");
                });

            modelBuilder.Entity("AssetServices.Models.LifeCycleSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AssetCategoryId")
                        .HasColumnType("int");

                    b.Property<bool>("BuyoutAllowed")
                        .HasColumnType("bit");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int?>("CustomerSettingsId")
                        .HasColumnType("int");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int>("Runtime")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasAlternateKey("ExternalId");

                    b.HasIndex("CustomerSettingsId");

                    b.ToTable("LifeCycleSetting");
                });

            modelBuilder.Entity("AssetServices.Models.SalaryDeductionTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AssetLifecycleId")
                        .HasColumnType("int");

                    b.Property<bool>("Cancelled")
                        .HasColumnType("bit");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int>("Month")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssetLifecycleId");

                    b.ToTable("SalaryDeductionTransaction");
                });

            modelBuilder.Entity("AssetServices.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasAlternateKey("ExternalId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AssetServices.Models.MobilePhone", b =>
                {
                    b.HasBaseType("AssetServices.Models.Asset");

                    b.Property<string>("MacAddress")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerialNumber")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("MobilePhone");
                });

            modelBuilder.Entity("AssetServices.Models.Subscription", b =>
                {
                    b.HasBaseType("AssetServices.Models.Asset");

                    b.Property<string>("SerialKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Subscription");
                });

            modelBuilder.Entity("AssetServices.Models.Tablet", b =>
                {
                    b.HasBaseType("AssetServices.Models.Asset");

                    b.Property<string>("MacAddress")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerialNumber")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("Tablet");
                });

            modelBuilder.Entity("AssetLifecycleCustomerLabel", b =>
                {
                    b.HasOne("AssetServices.Models.AssetLifecycle", null)
                        .WithMany()
                        .HasForeignKey("AssetLifecyclesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AssetServices.Models.CustomerLabel", null)
                        .WithMany()
                        .HasForeignKey("LabelsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.AssetImei", b =>
                {
                    b.HasOne("AssetServices.Models.MobilePhone", null)
                        .WithMany("Imeis")
                        .HasForeignKey("MobilePhoneId");

                    b.HasOne("AssetServices.Models.Tablet", null)
                        .WithMany("Imeis")
                        .HasForeignKey("TabletId");
                });

            modelBuilder.Entity("AssetServices.Models.AssetLifecycle", b =>
                {
                    b.HasOne("AssetServices.Models.Asset", "Asset")
                        .WithMany()
                        .HasForeignKey("AssetId");

                    b.HasOne("AssetServices.Models.User", "ContractHolderUser")
                        .WithMany()
                        .HasForeignKey("ContractHolderUserId");

                    b.OwnsOne("Common.Model.Money", "OffboardBuyoutPrice", b1 =>
                        {
                            b1.Property<int>("AssetLifecycleId")
                                .HasColumnType("int");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<string>("CurrencyCode")
                                .HasColumnType("char(3)");

                            b1.HasKey("AssetLifecycleId");

                            b1.ToTable("AssetLifeCycles");

                            b1.WithOwner()
                                .HasForeignKey("AssetLifecycleId");
                        });

                    b.OwnsOne("Common.Model.Money", "PaidByCompany", b1 =>
                        {
                            b1.Property<int>("AssetLifecycleId")
                                .HasColumnType("int");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<string>("CurrencyCode")
                                .HasColumnType("char(3)");

                            b1.HasKey("AssetLifecycleId");

                            b1.ToTable("AssetLifeCycles");

                            b1.WithOwner()
                                .HasForeignKey("AssetLifecycleId");
                        });

                    b.Navigation("Asset");

                    b.Navigation("ContractHolderUser");

                    b.Navigation("OffboardBuyoutPrice")
                        .IsRequired();

                    b.Navigation("PaidByCompany")
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.CustomerLabel", b =>
                {
                    b.OwnsOne("AssetServices.Models.Label", "Label", b1 =>
                        {
                            b1.Property<int>("CustomerLabelId")
                                .HasColumnType("int");

                            b1.Property<int>("Color")
                                .HasColumnType("int");

                            b1.Property<string>("Text")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("CustomerLabelId");

                            b1.ToTable("CustomerLabels");

                            b1.WithOwner()
                                .HasForeignKey("CustomerLabelId");
                        });

                    b.Navigation("Label")
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.CustomerSettings", b =>
                {
                    b.OwnsOne("AssetServices.Models.DisposeSetting", "DisposeSetting", b1 =>
                        {
                            b1.Property<int>("CustomerSettingsId")
                                .HasColumnType("int");

                            b1.Property<Guid>("CreatedBy")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTime>("CreatedDate")
                                .HasColumnType("datetime2");

                            b1.Property<Guid?>("DeletedBy")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .HasColumnType("int");

                            b1.Property<bool>("IsDeleted")
                                .HasColumnType("bit");

                            b1.Property<DateTime>("LastUpdatedDate")
                                .HasColumnType("datetime2");

                            b1.Property<Guid>("UpdatedBy")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("CustomerSettingsId");

                            b1.ToTable("CustomerSettings");

                            b1.WithOwner()
                                .HasForeignKey("CustomerSettingsId");

                            b1.OwnsMany("AssetServices.Models.ReturnLocation", "ReturnLocations", b2 =>
                                {
                                    b2.Property<int>("DisposeSettingCustomerSettingsId")
                                        .HasColumnType("int");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("int");

                                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b2.Property<int>("Id"), 1L, 1);

                                    b2.Property<Guid>("ExternalId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<Guid>("LocationId")
                                        .HasColumnType("uniqueidentifier");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("ReturnDescription")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("DisposeSettingCustomerSettingsId", "Id");

                                    b2.ToTable("ReturnLocation", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("DisposeSettingCustomerSettingsId");
                                });

                            b1.Navigation("ReturnLocations");
                        });

                    b.Navigation("DisposeSetting");
                });

            modelBuilder.Entity("AssetServices.Models.LifeCycleSetting", b =>
                {
                    b.HasOne("AssetServices.Models.CustomerSettings", null)
                        .WithMany("LifeCycleSettings")
                        .HasForeignKey("CustomerSettingsId");

                    b.OwnsOne("Common.Model.Money", "MinBuyoutPrice", b1 =>
                        {
                            b1.Property<int>("LifeCycleSettingId")
                                .HasColumnType("int");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<string>("CurrencyCode")
                                .HasColumnType("char(3)");

                            b1.HasKey("LifeCycleSettingId");

                            b1.ToTable("LifeCycleSetting");

                            b1.WithOwner()
                                .HasForeignKey("LifeCycleSettingId");
                        });

                    b.Navigation("MinBuyoutPrice")
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.SalaryDeductionTransaction", b =>
                {
                    b.HasOne("AssetServices.Models.AssetLifecycle", null)
                        .WithMany("SalaryDeductionTransactionList")
                        .HasForeignKey("AssetLifecycleId");

                    b.OwnsOne("Common.Model.Money", "Deduction", b1 =>
                        {
                            b1.Property<int>("SalaryDeductionTransactionId")
                                .HasColumnType("int");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<string>("CurrencyCode")
                                .HasColumnType("char(3)");

                            b1.HasKey("SalaryDeductionTransactionId");

                            b1.ToTable("SalaryDeductionTransaction");

                            b1.WithOwner()
                                .HasForeignKey("SalaryDeductionTransactionId");
                        });

                    b.Navigation("Deduction")
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.AssetLifecycle", b =>
                {
                    b.Navigation("SalaryDeductionTransactionList");
                });

            modelBuilder.Entity("AssetServices.Models.CustomerSettings", b =>
                {
                    b.Navigation("LifeCycleSettings");
                });

            modelBuilder.Entity("AssetServices.Models.MobilePhone", b =>
                {
                    b.Navigation("Imeis");
                });

            modelBuilder.Entity("AssetServices.Models.Tablet", b =>
                {
                    b.Navigation("Imeis");
                });
#pragma warning restore 612, 618
        }
    }
}
