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
    partial class SubscriptionManagmentContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SubscriptionManagementServices.Models.Datapackage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DatapackageName")
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

                    b.HasIndex("SubscriptionProductId");

                    b.ToTable("Datapackage", (string)null);
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
                        .HasColumnType("datetime2");

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
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.OperatorAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AccountName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("OperatorId")
                        .HasColumnType("int");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OperatorId");

                    b.ToTable("OperatorAccount", (string)null);
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

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int?>("SubscriptionOrderId")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SubscriptionOrderId");

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
                        .HasColumnType("datetime2");

                    b.Property<int>("DatapackageId")
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

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SubscriptionProductId")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("DatapackageId");

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
                        .HasColumnType("datetime2");

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

            modelBuilder.Entity("SubscriptionManagementServices.Models.Datapackage", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.SubscriptionProduct", null)
                        .WithMany("DataPackages")
                        .HasForeignKey("SubscriptionProductId");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.OperatorAccount", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.Operator", "Operator")
                        .WithMany("OperatorAccounts")
                        .HasForeignKey("OperatorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Operator");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionAddOnProduct", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.SubscriptionOrder", null)
                        .WithMany("SubscriptionAddOnProducts")
                        .HasForeignKey("SubscriptionOrderId");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionOrder", b =>
                {
                    b.HasOne("SubscriptionManagementServices.Models.Datapackage", "DataPackage")
                        .WithMany("SubscriptionOrders")
                        .HasForeignKey("DatapackageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SubscriptionManagementServices.Models.OperatorAccount", "OperatorAccount")
                        .WithMany("SubscriptionOrders")
                        .HasForeignKey("OperatorAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SubscriptionManagementServices.Models.SubscriptionProduct", "SubscriptionType")
                        .WithMany("SubscriptionOrders")
                        .HasForeignKey("SubscriptionProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DataPackage");

                    b.Navigation("OperatorAccount");

                    b.Navigation("SubscriptionType");
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

            modelBuilder.Entity("SubscriptionManagementServices.Models.Datapackage", b =>
                {
                    b.Navigation("SubscriptionOrders");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.Operator", b =>
                {
                    b.Navigation("OperatorAccounts");

                    b.Navigation("SubscriptionProducts");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.OperatorAccount", b =>
                {
                    b.Navigation("SubscriptionOrders");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionOrder", b =>
                {
                    b.Navigation("SubscriptionAddOnProducts");
                });

            modelBuilder.Entity("SubscriptionManagementServices.Models.SubscriptionProduct", b =>
                {
                    b.Navigation("DataPackages");

                    b.Navigation("SubscriptionOrders");
                });
#pragma warning restore 612, 618
        }
    }
}
