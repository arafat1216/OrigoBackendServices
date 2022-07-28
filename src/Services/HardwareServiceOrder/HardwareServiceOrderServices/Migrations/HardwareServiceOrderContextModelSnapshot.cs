﻿// <auto-generated />
using System;
using HardwareServiceOrderServices.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HardwareServiceOrderServices.Migrations
{
    [DbContext(typeof(HardwareServiceOrderContext))]
    partial class HardwareServiceOrderContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("HardwareServiceOrderServices.Models.CustomerServiceProvider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1000L, 1);

                    b.Property<string>("ApiPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ApiUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<DateTimeOffset?>("DateDeleted")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("DateUpdated")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("LastUpdateFetched")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("ServiceProviderId")
                        .HasColumnType("int");

                    b.Property<Guid?>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasAlternateKey("CustomerId", "ServiceProviderId");

                    b.HasIndex("ServiceProviderId");

                    b.ToTable("CustomerServiceProvider", (string)null);
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.CustomerSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1000L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<DateTimeOffset?>("DateDeleted")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("DateUpdated")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LoanDeviceEmail")
                        .HasMaxLength(320)
                        .HasColumnType("nvarchar(320)");

                    b.Property<string>("LoanDevicePhoneNumber")
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasColumnType("varchar(15)")
                        .HasComment("A phone-number using E.164 format.");

                    b.Property<bool>("ProvidesLoanDevice")
                        .HasColumnType("bit");

                    b.Property<Guid?>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("CustomerSettings", (string)null);
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.HardwareServiceOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1000L, 1);

                    b.Property<int>("AssetLifecycleCategoryId")
                        .HasColumnType("int");

                    b.Property<Guid>("AssetLifecycleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<DateTimeOffset?>("DateDeleted")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("DateUpdated")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ExternalServiceManagementLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOrderCancellationEmailSent")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOrderDiscardedEmailSent")
                        .HasColumnType("bit");

                    b.Property<bool>("IsReturnLoanDeviceEmailSent")
                        .HasColumnType("bit");

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

                    b.Property<Guid?>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasAlternateKey("ExternalId");

                    b.HasIndex("AssetLifecycleId");

                    b.HasIndex("CustomerId");

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

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1000L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<DateTimeOffset?>("DateDeleted")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("DateUpdated")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ServiceProviders");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            OrganizationId = new Guid("00000000-0000-0000-0000-000000000000"),
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 2,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            OrganizationId = new Guid("00000000-0000-0000-0000-000000000000"),
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        });
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.ServiceStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1000L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<DateTimeOffset?>("DateDeleted")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("DateUpdated")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid?>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ServiceStatuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 2,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 3,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 4,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 5,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 6,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 7,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 8,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 9,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 10,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 11,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 12,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 13,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 14,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 15,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 16,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        });
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.ServiceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1000L, 1);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("SYSUTCDATETIME()");

                    b.Property<DateTimeOffset?>("DateDeleted")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("DateUpdated")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid?>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ServiceTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 2,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 3,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = 4,
                            CreatedBy = new Guid("00000000-0000-0000-0000-000000000001"),
                            IsDeleted = false,
                            UpdatedBy = new Guid("00000000-0000-0000-0000-000000000001")
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("FriendlyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Xml")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.CustomerServiceProvider", b =>
                {
                    b.HasOne("HardwareServiceOrderServices.Models.ServiceProvider", "ServiceProvider")
                        .WithMany("CustomerServiceProviders")
                        .HasForeignKey("ServiceProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceProvider");
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

                    b.OwnsOne("HardwareServiceOrderServices.Models.ContactDetails", "Owner", b1 =>
                        {
                            b1.Property<int>("HardwareServiceOrderId")
                                .HasColumnType("int");

                            b1.Property<string>("Email")
                                .IsRequired()
                                .HasMaxLength(320)
                                .HasColumnType("nvarchar(320)");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PhoneNumber")
                                .HasMaxLength(15)
                                .HasColumnType("nvarchar(15)");

                            b1.Property<Guid>("UserId")
                                .HasColumnType("uniqueidentifier");

                            b1.HasKey("HardwareServiceOrderId");

                            b1.HasIndex("UserId");

                            b1.ToTable("HardwareServiceOrders");

                            b1.WithOwner()
                                .HasForeignKey("HardwareServiceOrderId");
                        });

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
                                .HasComment("The 2-character country-code using the uppercase 'ISO 3166 alpha-2' standard.");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .HasMaxLength(12)
                                .HasColumnType("nvarchar(12)");

                            b1.Property<string>("Recipient")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("RecipientType")
                                .HasColumnType("int");

                            b1.HasKey("HardwareServiceOrderId");

                            b1.ToTable("HardwareServiceOrders");

                            b1.WithOwner()
                                .HasForeignKey("HardwareServiceOrderId");
                        });

                    b.OwnsOne("HardwareServiceOrderServices.Models.AssetInfo", "AssetInfo", b1 =>
                        {
                            b1.Property<int>("HardwareServiceOrderId")
                                .HasColumnType("int");

                            b1.Property<string>("Accessories")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Brand")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Imei")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Model")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<DateTime?>("PurchaseDate")
                                .HasColumnType("date");

                            b1.Property<string>("SerialNumber")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("HardwareServiceOrderId");

                            b1.ToTable("HardwareServiceOrders");

                            b1.WithOwner()
                                .HasForeignKey("HardwareServiceOrderId");
                        });

                    b.OwnsOne("HardwareServiceOrderServices.Models.AssetInfo", "ReturnedAssetInfo", b1 =>
                        {
                            b1.Property<int>("HardwareServiceOrderId")
                                .HasColumnType("int");

                            b1.Property<string>("Accessories")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Brand")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Imei")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Model")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<DateTime?>("PurchaseDate")
                                .HasColumnType("date");

                            b1.Property<string>("SerialNumber")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("HardwareServiceOrderId");

                            b1.ToTable("HardwareServiceOrders");

                            b1.WithOwner()
                                .HasForeignKey("HardwareServiceOrderId");
                        });

                    b.OwnsMany("HardwareServiceOrderServices.Models.ServiceEvent", "ServiceEvents", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1000L, 1);

                            b1.Property<int>("HardwareServiceOrderId")
                                .HasColumnType("int");

                            b1.Property<int>("ServiceStatusId")
                                .HasColumnType("int");

                            b1.Property<DateTimeOffset>("Timestamp")
                                .HasColumnType("datetimeoffset")
                                .HasComment("When this event was recorded in the external service-provider's system");

                            b1.HasKey("Id");

                            b1.HasIndex("HardwareServiceOrderId");

                            b1.ToTable("ServiceEvents", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("HardwareServiceOrderId");
                        });

                    b.Navigation("AssetInfo")
                        .IsRequired();

                    b.Navigation("DeliveryAddress");

                    b.Navigation("Owner")
                        .IsRequired();

                    b.Navigation("ReturnedAssetInfo");

                    b.Navigation("ServiceEvents");

                    b.Navigation("ServiceProvider");

                    b.Navigation("ServiceType");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("HardwareServiceOrderServices.Models.ServiceProvider", b =>
                {
                    b.Navigation("CustomerServiceProviders");
                });
#pragma warning restore 612, 618
        }
    }
}
