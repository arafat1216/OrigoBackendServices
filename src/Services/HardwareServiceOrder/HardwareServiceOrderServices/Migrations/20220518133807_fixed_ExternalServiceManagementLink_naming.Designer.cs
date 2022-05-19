﻿// <auto-generated />
using System;
using HardwareServiceOrderServices.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    [DbContext(typeof(HardwareServiceOrderContext))]
    [Migration("20220518133807_fixed_ExternalServiceManagementLink_naming")]
    partial class fixed_ExternalServiceManagementLink_naming
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("HardwareServiceOrderServices.Models.CustomerSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("LastUpdatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<string>("LoanDeviceEmail")
                        .HasMaxLength(320)
                        .HasColumnType("nvarchar(320)");

                    b.Property<string>("LoanDevicePhoneNumber")
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasColumnType("varchar(15)")
                        .HasComment("A phone-number using E.164 format.");

                    b.Property<string>("ServiceId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("CustomerSettings", (string)null);
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.HardwareServiceOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("AssetLifecycleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ExternalServiceManagementLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("LastUpdatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<int>("ServiceProviderId")
                        .HasColumnType("int");

                    b.Property<string>("ServiceProviderOrderId1")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceProviderOrderId2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServiceTypeId")
                        .HasColumnType("int");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasAlternateKey("ExternalId");

                    b.HasIndex("AssetLifecycleId");

                    b.HasIndex("ServiceProviderId");

                    b.HasIndex("ServiceTypeId");

                    b.HasIndex("StatusId");

                    b.ToTable("HardwareServiceOrders");
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.ServiceProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("LastUpdatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ServiceProviders");
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.ServiceStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("LastUpdatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ServiceStatuses");
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.ServiceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("LastUpdatedDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ServiceTypes");
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.HardwareServiceOrder", b =>
                {
                    b.HasOne("HardwareServiceOrderServices.Models.ServiceProvider", "ServiceProvider")
                        .WithMany()
                        .HasForeignKey("ServiceProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HardwareServiceOrderServices.Models.ServiceType", "ServiceType")
                        .WithMany()
                        .HasForeignKey("ServiceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HardwareServiceOrderServices.Models.ServiceStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("HardwareServiceOrderServices.Models.DeliveryAddress", "DeliveryAddress", b1 =>
                        {
                            b1.Property<int>("HardwareServiceOrderId")
                                .HasColumnType("int");

                            b1.Property<string>("Address1")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Address2")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(85)
                                .HasColumnType("nvarchar(85)");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(2)
                                .IsUnicode(false)
                                .HasColumnType("char(2)")
                                .IsFixedLength()
                                .HasComment("The 2-character country-code using the uppercase <c>ISO 3166 alpha-2</c> standard.");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .HasMaxLength(12)
                                .HasColumnType("nvarchar(12)");

                            b1.Property<string>("Recipient")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("HardwareServiceOrderId");

                            b1.ToTable("HardwareServiceOrders");

                            b1.WithOwner()
                                .HasForeignKey("HardwareServiceOrderId");
                        });

                    b.OwnsOne("HardwareServiceOrderServices.Models.User", "OrderedBy", b1 =>
                        {
                            b1.Property<int>("HardwareServiceOrderId")
                                .HasColumnType("int");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<Guid>("ExternalId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("HardwareServiceOrderId");

                            b1.ToTable("HardwareServiceOrders");

                            b1.WithOwner()
                                .HasForeignKey("HardwareServiceOrderId");
                        });

                    b.Navigation("DeliveryAddress");

                    b.Navigation("OrderedBy")
                        .IsRequired();

                    b.Navigation("ServiceProvider");

                    b.Navigation("ServiceType");

                    b.Navigation("Status");
                });
#pragma warning restore 612, 618
        }
    }
}
