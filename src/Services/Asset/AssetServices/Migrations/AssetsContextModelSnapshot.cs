﻿// <auto-generated />
using System;
using AssetServices.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AssetServices.Migrations
{
    [DbContext(typeof(AssetsContext))]
    partial class AssetsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AssetServices.Models.Asset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AssetCategoryId")
                        .HasColumnType("int");

                    b.Property<Guid?>("AssetHolderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AssetTag")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("LifecycleType")
                        .HasColumnType("int");

                    b.Property<Guid?>("ManagedByDepartmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AssetCategoryId");

                    b.ToTable("Asset");
                });

            modelBuilder.Entity("AssetServices.Models.AssetCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<int?>("ParentAssetCategoryId")
                        .HasColumnType("int");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ParentAssetCategoryId");

                    b.ToTable("AssetCategory");
                });

            modelBuilder.Entity("AssetServices.Models.AssetCategoryTranslation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AssetCategoryId")
                        .HasColumnType("int");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Language")
                        .HasMaxLength(2)
                        .HasColumnType("nvarchar(2)");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AssetCategoryId");

                    b.ToTable("AssetCategoryTranslation");
                });

            modelBuilder.Entity("AssetServices.Models.AssetImei", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DeletedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("HardwareAssetId")
                        .HasColumnType("int");

                    b.Property<long>("Imei")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("HardwareAssetId");

                    b.ToTable("AssetImei");
                });

            modelBuilder.Entity("AssetServices.Models.HardwareAsset", b =>
                {
                    b.HasBaseType("AssetServices.Models.Asset");

                    b.Property<string>("MacAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("HardwareAsset");
                });

            modelBuilder.Entity("AssetServices.Models.SoftwareAsset", b =>
                {
                    b.HasBaseType("AssetServices.Models.Asset");

                    b.Property<string>("SerialKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("SoftwareAsset");
                });

            modelBuilder.Entity("AssetServices.Models.MobilePhone", b =>
                {
                    b.HasBaseType("AssetServices.Models.HardwareAsset");

                    b.ToTable("MobilePhone");
                });

            modelBuilder.Entity("AssetServices.Models.Tablet", b =>
                {
                    b.HasBaseType("AssetServices.Models.HardwareAsset");

                    b.ToTable("Tablet");
                });

            modelBuilder.Entity("AssetServices.Models.Subscription", b =>
                {
                    b.HasBaseType("AssetServices.Models.SoftwareAsset");

                    b.ToTable("Subscription");
                });

            modelBuilder.Entity("AssetServices.Models.Asset", b =>
                {
                    b.HasOne("AssetServices.Models.AssetCategory", "AssetCategory")
                        .WithMany()
                        .HasForeignKey("AssetCategoryId");

                    b.Navigation("AssetCategory");
                });

            modelBuilder.Entity("AssetServices.Models.AssetCategory", b =>
                {
                    b.HasOne("AssetServices.Models.AssetCategory", "ParentAssetCategory")
                        .WithMany()
                        .HasForeignKey("ParentAssetCategoryId");

                    b.Navigation("ParentAssetCategory");
                });

            modelBuilder.Entity("AssetServices.Models.AssetCategoryTranslation", b =>
                {
                    b.HasOne("AssetServices.Models.AssetCategory", null)
                        .WithMany("Translations")
                        .HasForeignKey("AssetCategoryId");
                });

            modelBuilder.Entity("AssetServices.Models.AssetImei", b =>
                {
                    b.HasOne("AssetServices.Models.HardwareAsset", null)
                        .WithMany("Imeis")
                        .HasForeignKey("HardwareAssetId");
                });

            modelBuilder.Entity("AssetServices.Models.HardwareAsset", b =>
                {
                    b.HasOne("AssetServices.Models.Asset", null)
                        .WithOne()
                        .HasForeignKey("AssetServices.Models.HardwareAsset", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.SoftwareAsset", b =>
                {
                    b.HasOne("AssetServices.Models.Asset", null)
                        .WithOne()
                        .HasForeignKey("AssetServices.Models.SoftwareAsset", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.MobilePhone", b =>
                {
                    b.HasOne("AssetServices.Models.HardwareAsset", null)
                        .WithOne()
                        .HasForeignKey("AssetServices.Models.MobilePhone", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.Tablet", b =>
                {
                    b.HasOne("AssetServices.Models.HardwareAsset", null)
                        .WithOne()
                        .HasForeignKey("AssetServices.Models.Tablet", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.Subscription", b =>
                {
                    b.HasOne("AssetServices.Models.SoftwareAsset", null)
                        .WithOne()
                        .HasForeignKey("AssetServices.Models.Subscription", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AssetServices.Models.AssetCategory", b =>
                {
                    b.Navigation("Translations");
                });

            modelBuilder.Entity("AssetServices.Models.HardwareAsset", b =>
                {
                    b.Navigation("Imeis");
                });
#pragma warning restore 612, 618
        }
    }
}
