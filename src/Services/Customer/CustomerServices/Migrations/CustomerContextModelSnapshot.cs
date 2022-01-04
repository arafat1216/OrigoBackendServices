﻿// <auto-generated />
using System;
using CustomerServices.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CustomerServices.Migrations
{
    [DbContext(typeof(CustomerContext))]
    partial class CustomerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CustomerServices.Models.AssetCategoryLifecycleType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("AssetCategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("AssetCategoryTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("LifecycleType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AssetCategoryTypeId");

                    b.ToTable("AssetCategoryLifecycleType");
                });

            modelBuilder.Entity("CustomerServices.Models.AssetCategoryType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("AssetCategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ExternalCustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("OrganizationId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("AssetCategory");
                });

            modelBuilder.Entity("CustomerServices.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CostCenterId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ExternalDepartmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentDepartmentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ParentDepartmentId");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("CustomerServices.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("LocationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("CustomerServices.Models.Organization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OrganizationNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PrimaryLocation")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Customer");
                });

            modelBuilder.Entity("CustomerServices.Models.OrganizationPreferences", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<short>("DefaultDepartmentClassification")
                        .HasColumnType("smallint");

                    b.Property<bool>("EnforceTwoFactorAuth")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OrganizationNotes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrimaryLanguage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("WebPage")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OrganizationPreferences");
                });

            modelBuilder.Entity("CustomerServices.Models.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Permissions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 295, DateTimeKind.Utc).AddTicks(6905),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "CanCreateCustomer"
                        },
                        new
                        {
                            Id = 2,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 295, DateTimeKind.Utc).AddTicks(7927),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "CanReadCustomer"
                        },
                        new
                        {
                            Id = 3,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 295, DateTimeKind.Utc).AddTicks(7931),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "CanUpdateCustomer"
                        },
                        new
                        {
                            Id = 4,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 295, DateTimeKind.Utc).AddTicks(7933),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "CanDeleteCustomer"
                        });
                });

            modelBuilder.Entity("CustomerServices.Models.PermissionSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PermissionSets");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 296, DateTimeKind.Utc).AddTicks(9989),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "FullCustomerAccess"
                        });
                });

            modelBuilder.Entity("CustomerServices.Models.ProductModule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProductModuleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ProductModule");
                });

            modelBuilder.Entity("CustomerServices.Models.ProductModuleGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProductModuleExternalId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProductModuleGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ProductModuleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductModuleId");

                    b.ToTable("ProductModuleGroups");
                });

            modelBuilder.Entity("CustomerServices.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1405),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "EndUser"
                        },
                        new
                        {
                            Id = 2,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1901),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "DepartmentManager"
                        },
                        new
                        {
                            Id = 3,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1905),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "CustomerAdmin"
                        },
                        new
                        {
                            Id = 4,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1906),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "GroupAdmin"
                        },
                        new
                        {
                            Id = 5,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1907),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "PartnerAdmin"
                        },
                        new
                        {
                            Id = 6,
                            CreatedDate = new DateTime(2021, 11, 4, 20, 16, 32, 297, DateTimeKind.Utc).AddTicks(1908),
                            LastUpdatedDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "SystemAdmin"
                        });
                });

            modelBuilder.Entity("CustomerServices.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmployeeId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastUpdatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("MobileNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("CustomerServices.Models.UserPermissions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccessList")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPermissions");
                });

            modelBuilder.Entity("DepartmentUser", b =>
                {
                    b.Property<int>("ManagersId")
                        .HasColumnType("int");

                    b.Property<int>("ManagesDepartmentsId")
                        .HasColumnType("int");

                    b.HasKey("ManagersId", "ManagesDepartmentsId");

                    b.HasIndex("ManagesDepartmentsId");

                    b.ToTable("DepartmentManager");
                });

            modelBuilder.Entity("DepartmentUser1", b =>
                {
                    b.Property<int>("DepartmentsId")
                        .HasColumnType("int");

                    b.Property<int>("UsersId")
                        .HasColumnType("int");

                    b.HasKey("DepartmentsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("DepartmentUser");
                });

            modelBuilder.Entity("OrganizationProductModule", b =>
                {
                    b.Property<int>("CustomersId")
                        .HasColumnType("int");

                    b.Property<int>("SelectedProductModulesId")
                        .HasColumnType("int");

                    b.HasKey("CustomersId", "SelectedProductModulesId");

                    b.HasIndex("SelectedProductModulesId");

                    b.ToTable("CustomerProductModule");
                });

            modelBuilder.Entity("OrganizationProductModuleGroup", b =>
                {
                    b.Property<int>("CustomersId")
                        .HasColumnType("int");

                    b.Property<int>("SelectedProductModuleGroupsId")
                        .HasColumnType("int");

                    b.HasKey("CustomersId", "SelectedProductModuleGroupsId");

                    b.HasIndex("SelectedProductModuleGroupsId");

                    b.ToTable("CustomerProductModuleGroup");
                });

            modelBuilder.Entity("PermissionPermissionSet", b =>
                {
                    b.Property<int>("PermissionSetsId")
                        .HasColumnType("int");

                    b.Property<int>("PermissionsId")
                        .HasColumnType("int");

                    b.HasKey("PermissionSetsId", "PermissionsId");

                    b.HasIndex("PermissionsId");

                    b.ToTable("PermissionPermissionSet");
                });

            modelBuilder.Entity("PermissionSetRole", b =>
                {
                    b.Property<int>("GrantedPermissionsId")
                        .HasColumnType("int");

                    b.Property<int>("RolesId")
                        .HasColumnType("int");

                    b.HasKey("GrantedPermissionsId", "RolesId");

                    b.HasIndex("RolesId");

                    b.ToTable("PermissionSetRole");
                });

            modelBuilder.Entity("CustomerServices.Models.AssetCategoryLifecycleType", b =>
                {
                    b.HasOne("CustomerServices.Models.AssetCategoryType", null)
                        .WithMany("LifecycleTypes")
                        .HasForeignKey("AssetCategoryTypeId");
                });

            modelBuilder.Entity("CustomerServices.Models.AssetCategoryType", b =>
                {
                    b.HasOne("CustomerServices.Models.Organization", null)
                        .WithMany("SelectedAssetCategories")
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("CustomerServices.Models.Department", b =>
                {
                    b.HasOne("CustomerServices.Models.Organization", "Customer")
                        .WithMany("Departments")
                        .HasForeignKey("CustomerId");

                    b.HasOne("CustomerServices.Models.Department", "ParentDepartment")
                        .WithMany()
                        .HasForeignKey("ParentDepartmentId");

                    b.Navigation("Customer");

                    b.Navigation("ParentDepartment");
                });

            modelBuilder.Entity("CustomerServices.Models.Organization", b =>
                {
                    b.OwnsOne("CustomerServices.Models.Address", "Address", b1 =>
                        {
                            b1.Property<int>("OrganizationId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("City")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Country")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PostCode")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Street")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("OrganizationId");

                            b1.ToTable("Customer");

                            b1.WithOwner()
                                .HasForeignKey("OrganizationId");
                        });

                    b.OwnsOne("CustomerServices.Models.ContactPerson", "ContactPerson", b1 =>
                        {
                            b1.Property<int>("OrganizationId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("Email")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("FirstName")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("LastName")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PhoneNumber")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("OrganizationId");

                            b1.ToTable("Customer");

                            b1.WithOwner()
                                .HasForeignKey("OrganizationId");
                        });

                    b.Navigation("Address");

                    b.Navigation("ContactPerson");
                });

            modelBuilder.Entity("CustomerServices.Models.ProductModuleGroup", b =>
                {
                    b.HasOne("CustomerServices.Models.ProductModule", null)
                        .WithMany("ProductModuleGroup")
                        .HasForeignKey("ProductModuleId");
                });

            modelBuilder.Entity("CustomerServices.Models.User", b =>
                {
                    b.HasOne("CustomerServices.Models.Organization", "Customer")
                        .WithMany("Users")
                        .HasForeignKey("CustomerId");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("CustomerServices.Models.UserPermissions", b =>
                {
                    b.HasOne("CustomerServices.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("CustomerServices.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DepartmentUser", b =>
                {
                    b.HasOne("CustomerServices.Models.User", null)
                        .WithMany()
                        .HasForeignKey("ManagersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CustomerServices.Models.Department", null)
                        .WithMany()
                        .HasForeignKey("ManagesDepartmentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DepartmentUser1", b =>
                {
                    b.HasOne("CustomerServices.Models.Department", null)
                        .WithMany()
                        .HasForeignKey("DepartmentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CustomerServices.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OrganizationProductModule", b =>
                {
                    b.HasOne("CustomerServices.Models.Organization", null)
                        .WithMany()
                        .HasForeignKey("CustomersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CustomerServices.Models.ProductModule", null)
                        .WithMany()
                        .HasForeignKey("SelectedProductModulesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OrganizationProductModuleGroup", b =>
                {
                    b.HasOne("CustomerServices.Models.Organization", null)
                        .WithMany()
                        .HasForeignKey("CustomersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CustomerServices.Models.ProductModuleGroup", null)
                        .WithMany()
                        .HasForeignKey("SelectedProductModuleGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PermissionPermissionSet", b =>
                {
                    b.HasOne("CustomerServices.Models.PermissionSet", null)
                        .WithMany()
                        .HasForeignKey("PermissionSetsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CustomerServices.Models.Permission", null)
                        .WithMany()
                        .HasForeignKey("PermissionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PermissionSetRole", b =>
                {
                    b.HasOne("CustomerServices.Models.PermissionSet", null)
                        .WithMany()
                        .HasForeignKey("GrantedPermissionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CustomerServices.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CustomerServices.Models.AssetCategoryType", b =>
                {
                    b.Navigation("LifecycleTypes");
                });

            modelBuilder.Entity("CustomerServices.Models.Organization", b =>
                {
                    b.Navigation("Departments");

                    b.Navigation("SelectedAssetCategories");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("CustomerServices.Models.ProductModule", b =>
                {
                    b.Navigation("ProductModuleGroup");
                });
#pragma warning restore 612, 618
        }
    }
}
