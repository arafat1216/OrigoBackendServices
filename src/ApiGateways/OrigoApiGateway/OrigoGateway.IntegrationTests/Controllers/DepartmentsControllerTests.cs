using Common.Interfaces;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;
using OrigoGateway.IntegrationTests.Helpers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace OrigoGateway.IntegrationTests.Controllers
{
    public class DepartmentsControllerTests : IClassFixture<OrigoGatewayWebApplicationFactory<DepartmentsController>>
    {
        private readonly OrigoGatewayWebApplicationFactory<DepartmentsController> _factory;
        private readonly ITestOutputHelper _output;
        private readonly Guid ORGANIZATION_ID;
        private readonly Guid DEPARTMENT_ID;

        public DepartmentsControllerTests(OrigoGatewayWebApplicationFactory<DepartmentsController> factory, ITestOutputHelper output)
        {
            _factory = factory;
            factory.ClientOptions.AllowAutoRedirect = false;
            _output = output;
            ORGANIZATION_ID = factory.ORGANIZATION_ID;
            DEPARTMENT_ID = Guid.Parse("6c514552-ea67-48c8-91ec-83c2b16248ee");
        }

        // public static IEnumerable<object[]> EmailAccess =>
        // new List<object[]>
        //{
        //     new object[] { "unknown@test.io", HttpStatusCode.Forbidden, new List<string> { "CanReadCustomer", "CanUpdateCustomer" } },
        //     new object[] { "admin@test.io", HttpStatusCode.Forbidden, new List<string> { "CanReadCustomer", "CanUpdateCustomer" } },
        //     new object[] { "systemadmin@test.io", HttpStatusCode.OK, new List<string> { "CanReadCustomer", "CanUpdateCustomer" } }
        //};



        [Fact]
        public async Task DeleteDepartment_EnsureSystemAdminAccsess()
        {
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "systemadmin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, "systemadmin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "SystemAdmin"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "systemadmin@test.io", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options =>
                        {
                            options.Email = "systemadmin@test.io";
                        });
                    var departmentService = new Mock<IDepartmentsServices>();


                    var organization = new OrigoDepartment
                    {
                        DepartmentId = DEPARTMENT_ID
                    };

                    departmentService.Setup(x => x.DeleteDepartmentPatchAsync(ORGANIZATION_ID, DEPARTMENT_ID, It.IsAny<Guid>())).Returns(Task.FromResult(organization));

                    services.AddSingleton(departmentService.Object);
                });
            }).CreateClient();

            var response = await client.DeleteAsync($"/origoapi/v1.0/customers/{ORGANIZATION_ID}/departments/{DEPARTMENT_ID}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task DeleteDepartment_AdminAccsess()
        {
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", ORGANIZATION_ID.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "admin@test.io", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options =>
                        {
                            options.Email = "admin@test.io";
                        });
                    var departmentService = new Mock<IDepartmentsServices>();


                    var organization = new OrigoDepartment
                    {
                        DepartmentId = Guid.Parse("6c514552-ea67-48c8-91ec-83c2b16248ee")
                    };

                    departmentService.Setup(x => x.DeleteDepartmentPatchAsync(ORGANIZATION_ID, DEPARTMENT_ID, It.IsAny<Guid>())).Returns(Task.FromResult(organization));

                    services.AddSingleton(departmentService.Object);
                });
            }).CreateClient();

            var response = await client.DeleteAsync($"/origoapi/v1.0/customers/{ORGANIZATION_ID}/departments/{DEPARTMENT_ID}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PatchDepartment_AdminAccsess()
        {
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", ORGANIZATION_ID.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "admin@test.io", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options =>
                        {
                            options.Email = "admin@test.io";
                        });
                    var departmentService = new Mock<IDepartmentsServices>();


                    var organization = new OrigoDepartment
                    {
                        DepartmentId = Guid.Parse("6c514552-ea67-48c8-91ec-83c2b16248ee")
                    };

                    departmentService.Setup(x => x.UpdateDepartmentPatchAsync(ORGANIZATION_ID, DEPARTMENT_ID, It.IsAny<UpdateDepartmentDTO>())).Returns(Task.FromResult(organization));

                    services.AddSingleton(departmentService.Object);
                });
            }).CreateClient();

            var department = new UpdateDepartment { DepartmentId = DEPARTMENT_ID, Name = "Department" };
            var response = await client.PatchAsync($"/origoapi/v1.0/customers/{ORGANIZATION_ID}/departments/{DEPARTMENT_ID}", JsonContent.Create(department));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PutDepartment_AdminAccsess()
        {
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", ORGANIZATION_ID.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "admin@test.io", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options =>
                        {
                            options.Email = "admin@test.io";
                        });

                    var departmentService = new Mock<IDepartmentsServices>();


                    var organization = new OrigoDepartment
                    {
                        DepartmentId = Guid.Parse("6c514552-ea67-48c8-91ec-83c2b16248ee")
                    };

                    departmentService.Setup(x => x.UpdateDepartmentPutAsync(ORGANIZATION_ID, DEPARTMENT_ID, It.IsAny<UpdateDepartmentDTO>())).Returns(Task.FromResult(organization));

                    services.AddSingleton(departmentService.Object);
                });
            }).CreateClient();

            var department = new UpdateDepartment { DepartmentId = DEPARTMENT_ID, Name = "Department" };
            var response = await client.PutAsync($"/origoapi/v1.0/customers/{ORGANIZATION_ID}/departments/{DEPARTMENT_ID}", JsonContent.Create(department));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostDepartment_AdminAccsess()
        {
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", ORGANIZATION_ID.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "admin@test.io", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options =>
                        {
                            options.Email = "admin@test.io";
                        });
                    var departmentService = new Mock<IDepartmentsServices>();


                    var organization = new OrigoDepartment
                    {
                        ParentDepartmentId = Guid.Parse("6c514552-ea67-48c8-91ec-83c2b16248ee")
                    };

                    departmentService.Setup(x => x.AddDepartmentAsync(ORGANIZATION_ID, It.IsAny<NewDepartmentDTO>())).Returns(Task.FromResult(organization));

                    services.AddSingleton(departmentService.Object);
                });
            }).CreateClient();

            var department = new NewDepartment
            {
                ParentDepartmentId = DEPARTMENT_ID,
                Name = "Department"
            };

            var response = await client.PostAsync($"/origoapi/v1.0/customers/{ORGANIZATION_ID}/departments", JsonContent.Create(department));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PostDepartment_EndUserDeniedAccsess()
        {
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "unknown@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, "unknown@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "EndUser"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", "14df57bf-e398-41b7-badd-c784cebaa5b5"));


            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "unknown@test.io", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options =>
                        {
                            options.Email = "unknown@test.io";
                        });
                    var departmentService = new Mock<IDepartmentsServices>();


                    var organization = new OrigoDepartment
                    {
                        ParentDepartmentId = Guid.Parse("6c514552-ea67-48c8-91ec-83c2b16248ee")
                    };

                    departmentService.Setup(x => x.AddDepartmentAsync(ORGANIZATION_ID, It.IsAny<NewDepartmentDTO>())).Returns(Task.FromResult(organization));

                    services.AddSingleton(departmentService.Object);
                });
            }).CreateClient();

            var department = new NewDepartment
            {
                ParentDepartmentId = DEPARTMENT_ID,
                Name = "Department"
            };

            var response = await client.PostAsync($"/origoapi/v1.0/customers/{ORGANIZATION_ID}/departments", JsonContent.Create(department));

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }



        [Fact]
        public async Task GetPaginatedDepartmentsAsync()
        {
            // Arrange
            var permissionsIdentity = new ClaimsIdentity();
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Email, "admin@test.io"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            permissionsIdentity.AddClaim(new Claim(ClaimTypes.Actor, Guid.NewGuid().ToString()));
            permissionsIdentity.AddClaim(new Claim("AccessList", ORGANIZATION_ID.ToString()));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanReadCustomer"));
            permissionsIdentity.AddClaim(new Claim("Permissions", "CanUpdateCustomer"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var userPermissionServiceMock = new Mock<IUserPermissionService>();
                    userPermissionServiceMock.Setup(_ => _.GetUserPermissionsIdentityAsync(It.IsAny<string>(), "admin@test.io", CancellationToken.None)).Returns(Task.FromResult(permissionsIdentity));
                    services.AddSingleton(userPermissionServiceMock.Object);

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthenticationHandler.DefaultScheme;
                        options.DefaultScheme = TestAuthenticationHandler.DefaultScheme;
                    }).AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        TestAuthenticationHandler.DefaultScheme, options =>
                        {
                            options.Email = "admin@test.io";
                        });
                    var departmentService = new Mock<IDepartmentsServices>();


                    var pagedResponse = new PagedModel<OrigoDepartment>()
                    {
                        CurrentPage = 1,
                        Items = new List<OrigoDepartment>() 
                        { 
                            new() { DepartmentId = Guid.NewGuid(), Name = "Department 1" },
                        },
                        PageSize = 25,
                        TotalItems = 1,
                        TotalPages = 1
                    };

                    departmentService.Setup(x => x.GetPaginatedDepartmentsAsync(ORGANIZATION_ID, It.IsAny<CancellationToken>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                                     .Returns(Task.FromResult(pagedResponse));

                    services.AddSingleton(departmentService.Object);
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync($"/origoapi/v1.0/customers/{ORGANIZATION_ID}/departments/paginated");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
