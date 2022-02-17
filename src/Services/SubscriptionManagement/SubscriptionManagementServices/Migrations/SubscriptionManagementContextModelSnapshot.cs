﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SubscriptionManagementServices.Infrastructure;

#nullable disable

namespace SubscriptionManagementServices.Migrations
{
    [DbContext(typeof(SubscriptionManagementContext))]
    partial class SubscriptionManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CustomerOperatorAccountCustomerOperatorSettings", b =>
                {
                    b.Property<int>("CustomerOperatorAccountsId")
                        .HasColumnType("int");

                    b.Property<int>("CustomerOperatorSettingsId")
                        .HasColumnType("int");

                    b.HasKey("CustomerOperatorAccountsId", "CustomerOperatorSettingsId");

                    b.HasIndex("CustomerOperatorSettingsId");

                    b.ToTable("CustomersOperatorAccounts", (string)null);
                });

            modelBuilder.Entity("SubscriptionAddOnProductSubscriptionOrder", b =>
                {
                    b.Property<int>("SubscriptionAddOnProductsId")
                        .HasColumnType("int");

                    b.Property<int>("SubscriptionOrdersId")
                        .HasColumnType("int");

                    b.HasKey("SubscriptionAddOnProductsId", "SubscriptionOrdersId");

                    b.HasIndex("SubscriptionOrdersId");

                    b.ToTable("SubscriptionAddOnProductSubscriptionOrder");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerOperatorAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int>("OperatorId")
                        .HasColumnType("int");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OperatorId");

                    b.HasIndex("OrganizationId", "AccountNumber")
                        .IsUnique();

                    b.ToTable("CustomerOperatorAccount", (string)null);
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerOperatorSettings", b =>
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

                    b.Property<int?>("CustomerSettingsId")
                        .HasColumnType("int");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int>("OperatorId")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CustomerSettingsId");

                    b.HasIndex("OperatorId");

                    b.ToTable("CustomerOperatorSettings", (string)null);
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerSettings", b =>
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

                    b.Property<Guid>("DeletedBy")
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

                    b.ToTable("CustomerSettings", (string)null);
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerSubscriptionProduct", b =>
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

                    b.Property<int?>("CustomerOperatorSettingsId")
                        .HasColumnType("int");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("GlobalSubscriptionProductId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int>("OperatorId")
                        .HasColumnType("int");

                    b.Property<string>("SubscriptionName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CustomerOperatorSettingsId");

                    b.HasIndex("GlobalSubscriptionProductId");

                    b.HasIndex("OperatorId");

                    b.ToTable("CustomerSubscriptionProduct", (string)null);
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.DataPackage", b =>
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

                    b.Property<int?>("CustomerSubscriptionProductId")
                        .HasColumnType("int");

                    b.Property<string>("DataPackageName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int?>("SubscriptionProductId")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CustomerSubscriptionProductId");

                    b.HasIndex("SubscriptionProductId");

                    b.ToTable("DataPackage", (string)null);
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.Operator", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("char(2)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<string>("OperatorName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Operator", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Country = "nb",
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000000"),
                            CreatedDate = new DateTime(2022, 2, 9, 13, 10, 2, 47, DateTimeKind.Unspecified).AddTicks(4381),
                            DeletedBy = new Guid("00000000-0000-0000-0000-000000000000"),
                            IsDeleted = false,
                            OperatorName = "Telia - NO",
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000000")
                        },
                        new
                        {
                            Id = 2,
                            Country = "se",
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000000"),
                            CreatedDate = new DateTime(2022, 2, 9, 13, 10, 2, 47, DateTimeKind.Unspecified).AddTicks(4381),
                            DeletedBy = new Guid("00000000-0000-0000-0000-000000000000"),
                            IsDeleted = false,
                            OperatorName = "Telia - SE",
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000000")
                        },
                        new
                        {
                            Id = 3,
                            Country = "nb",
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000000"),
                            CreatedDate = new DateTime(2022, 2, 9, 13, 10, 2, 47, DateTimeKind.Unspecified).AddTicks(4381),
                            DeletedBy = new Guid("00000000-0000-0000-0000-000000000000"),
                            IsDeleted = false,
                            OperatorName = "Telenor - NO",
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000000")
                        },
                        new
                        {
                            Id = 4,
                            Country = "se",
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000000"),
                            CreatedDate = new DateTime(2022, 2, 9, 13, 10, 2, 47, DateTimeKind.Unspecified).AddTicks(4381),
                            DeletedBy = new Guid("00000000-0000-0000-0000-000000000000"),
                            IsDeleted = false,
                            OperatorName = "Telenor - SE",
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000000")
                        });
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionAddOnProduct", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AddOnProductName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("BelongsToSubscriptionProductId")
                        .HasColumnType("int");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("DeletedBy")
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

                    b.HasIndex("BelongsToSubscriptionProductId");

                    b.ToTable("SubscriptionAddOnProduct", (string)null);
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionOrder", b =>
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

                    b.Property<int>("DataPackageId")
                        .HasColumnType("int");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int>("OperatorAccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("OrderExecutionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SimCardNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("SubscriptionProductId")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("DataPackageId");

                    b.HasIndex("OperatorAccountId");

                    b.HasIndex("SubscriptionProductId");

                    b.ToTable("SubscriptionOrder", (string)null);
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionProduct", b =>
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

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int>("OperatorId")
                        .HasColumnType("int");

                    b.Property<string>("SubscriptionName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OperatorId");

                    b.ToTable("SubscriptionProduct", (string)null);
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.TransferSubscriptionOrder", b =>
                {
                    b.HasBaseType("SubscriptionManagementServices.Models.SubscriptionOrder");

                    b.Property<int>("NewOperatorAccountId")
                        .HasColumnType("int");

                    b.HasIndex("NewOperatorAccountId");

                    b.ToTable("TransferSubscriptionOrder", (string)null);
                });

            modelBuilder.Entity("CustomerOperatorAccountCustomerOperatorSettings", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.CustomerOperatorAccount", null)
                        .WithMany()
                        .HasForeignKey("CustomerOperatorAccountsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SubscriptionManagementServices.Models.CustomerOperatorSettings", null)
                        .WithMany()
                        .HasForeignKey("CustomerOperatorSettingsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SubscriptionAddOnProductSubscriptionOrder", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.SubscriptionAddOnProduct", null)
                        .WithMany()
                        .HasForeignKey("SubscriptionAddOnProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SubscriptionManagementServices.Models.SubscriptionOrder", null)
                        .WithMany()
                        .HasForeignKey("SubscriptionOrdersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerOperatorAccount", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.Operator", "Operator")
                        .WithMany("CustomerOperatorAccounts")
                        .HasForeignKey("OperatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Operator");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerOperatorSettings", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.CustomerSettings", null)
                        .WithMany("CustomerOperatorSettings")
                        .HasForeignKey("CustomerSettingsId");

                    b.HasOne("SubscriptionManagementServices.Models.Operator", "Operator")
                        .WithMany("CustomerOperatorSettings")
                        .HasForeignKey("OperatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Operator");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerSettings", b =>
                {
                    b.OwnsMany("SubscriptionManagementServices.Models.CustomerReferenceField", "CustomerReferenceFields", b1 =>
                        {
                            b1.Property<int>("CustomerSettingsId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<Guid>("CreatedBy")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTime>("CreatedDate")
                                .HasColumnType("datetime2");

                            b1.Property<Guid>("DeletedBy")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<bool>("IsDeleted")
                                .HasColumnType("bit");

                            b1.Property<DateTime>("LastUpdatedDate")
                                .HasColumnType("datetime2");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("ReferenceType")
                                .HasColumnType("int");

                            b1.Property<Guid>("UpdatedBy")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("CustomerSettingsId", "Id");

                            b1.ToTable("CustomerReferenceField");

                            b1.WithOwner()
                                .HasForeignKey("CustomerSettingsId");
                        });

                    b.Navigation("CustomerReferenceFields");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerSubscriptionProduct", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.CustomerOperatorSettings", null)
                        .WithMany("AvailableSubscriptionProducts")
                        .HasForeignKey("CustomerOperatorSettingsId");

                    b.HasOne("SubscriptionManagementServices.Models.SubscriptionProduct", "GlobalSubscriptionProduct")
                        .WithMany()
                        .HasForeignKey("GlobalSubscriptionProductId");

                    b.HasOne("SubscriptionManagementServices.Models.Operator", "Operator")
                        .WithMany()
                        .HasForeignKey("OperatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GlobalSubscriptionProduct");

                    b.Navigation("Operator");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.DataPackage", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.CustomerSubscriptionProduct", null)
                        .WithMany("DataPackages")
                        .HasForeignKey("CustomerSubscriptionProductId");

                    b.HasOne("SubscriptionManagementServices.Models.SubscriptionProduct", null)
                        .WithMany("DataPackages")
                        .HasForeignKey("SubscriptionProductId");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionAddOnProduct", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.SubscriptionProduct", "BelongsToSubscriptionProduct")
                        .WithMany()
                        .HasForeignKey("BelongsToSubscriptionProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BelongsToSubscriptionProduct");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionOrder", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.DataPackage", "DataPackage")
                        .WithMany("SubscriptionOrders")
                        .HasForeignKey("DataPackageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SubscriptionManagementServices.Models.CustomerOperatorAccount", "OperatorAccount")
                        .WithMany("SubscriptionOrders")
                        .HasForeignKey("OperatorAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SubscriptionManagementServices.Models.CustomerSubscriptionProduct", "CustomerSubscriptionProduct")
                        .WithMany("SubscriptionOrders")
                        .HasForeignKey("SubscriptionProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("CustomerSubscriptionProduct");

                    b.Navigation("DataPackage");

                    b.Navigation("OperatorAccount");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionProduct", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.Operator", "Operator")
                        .WithMany("SubscriptionProducts")
                        .HasForeignKey("OperatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Operator");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.TransferSubscriptionOrder", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.SubscriptionOrder", null)
                        .WithOne()
                        .HasForeignKey("SubscriptionManagementServices.Models.TransferSubscriptionOrder", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("SubscriptionManagementServices.Models.CustomerOperatorAccount", "NewOperatorAccount")
                        .WithMany("TransferSubscriptionOrders")
                        .HasForeignKey("NewOperatorAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("NewOperatorAccount");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerOperatorAccount", b =>
                {
                    b.Navigation("SubscriptionOrders");

                    b.Navigation("TransferSubscriptionOrders");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerOperatorSettings", b =>
                {
                    b.Navigation("AvailableSubscriptionProducts");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerSettings", b =>
                {
                    b.Navigation("CustomerOperatorSettings");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.CustomerSubscriptionProduct", b =>
                {
                    b.Navigation("DataPackages");

                    b.Navigation("SubscriptionOrders");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.DataPackage", b =>
                {
                    b.Navigation("SubscriptionOrders");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.Operator", b =>
                {
                    b.Navigation("CustomerOperatorAccounts");

                    b.Navigation("CustomerOperatorSettings");

                    b.Navigation("SubscriptionProducts");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionProduct", b =>
                {
                    b.Navigation("DataPackages");
                });
#pragma warning restore 612, 618
        }
    }
}
