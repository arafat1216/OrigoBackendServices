using Common.Enums;
using CustomerServices.Models;
using CustomerServices.SeedData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class OrganizationTestDataService : IOrganizationTestDataService
    {
        private readonly ILogger<OrganizationTestDataService> _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserPermissionServices _userPermissionServices;
        private readonly Guid _callerId = new Guid("D0326090-631F-4138-9CD2-85249AD24BBB");
        protected IList<Organization> Organizations { get; set; }
        protected List<Department> Departments { get; set; } = new List<Department>();
        protected List<User> Users { get; set; } = new List<User>();
        protected Dictionary<Guid, PredefinedRole> UserPermissions { get; set; }

        public OrganizationTestDataService(ILogger<OrganizationTestDataService> logger, IUserPermissionServices userPermissionServices, IOrganizationRepository organizationRepository)
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _userPermissionServices = userPermissionServices;
            Organizations = Seed.GetCustomersData();
            foreach (var organization in Organizations)
            {
                var departments = Seed.GetDepartmentDataForOrganization(organization);
                Departments.AddRange(departments);

                var users = Seed.GetUsersForOrganization(organization);
                Users.AddRange(users);
            }
            UserPermissions = Seed.GetUserRoles();
        }

        public async Task<string> CreateOrganizationTestData()
        {
            StringBuilder builder = new();
            try
            {
                builder.Append(await CreateCustomer());
                builder.Append(await CreateDepartments());
                builder.Append(await CreateUsers());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return builder.ToString();
        }

        protected async Task<string> CreateCustomer()
        {
            string errorMessage = string.Empty;
            try
            {
                foreach (Organization organization in Organizations)
                {
                    var org = await _organizationRepository.GetOrganizationAsync(organization.OrganizationId, includeDepartments: true);
                    if (org == null)
                    {
                        await _organizationRepository.AddAsync(organization);
                    }
                    else
                    {
                        org.UpdateOrganization(organization);
                    }
                }
                await _organizationRepository.SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                errorMessage = "Customer creation exception\r\n";
            }
            return errorMessage;
        }

        protected async Task<string> CreateDepartments()
        {
            string errorMessage = string.Empty;
            try
            {
                foreach (var item in Organizations)
                {
                    var organization = await _organizationRepository.GetOrganizationAsync(item.OrganizationId, includeDepartments: true);
                    IList<Department> departments = Seed.GetDepartmentDataForOrganization(organization);
                    foreach (Department department in departments)
                    {
                        var dept = await _organizationRepository.GetDepartmentAsync(organization.OrganizationId, department.ExternalDepartmentId);
                        if (dept == null)
                        {
                            if (department.ParentDepartment != null)
                            {
                                var parent = await _organizationRepository.GetDepartmentAsync(organization.OrganizationId, department.ParentDepartment.ExternalDepartmentId);
                                organization.ChangeDepartmentsParentDepartment(department, parent, _callerId);
                            }
                            organization.AddDepartment(department, _callerId);
                        }
                        else
                        {
                            if (department.ParentDepartment != null)
                            {
                                var parent = await _organizationRepository.GetDepartmentAsync(organization.OrganizationId, department.ParentDepartment.ExternalDepartmentId);
                                organization.ChangeDepartmentsParentDepartment(department, parent, _callerId);
                            }
                            dept.UpdateDepartment(department);
                        }
                    }
                }
                await _organizationRepository.SaveEntitiesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                errorMessage = "Department creation exception\r\n";
            }
            return errorMessage;
        }

        protected async Task<string> CreateUsers()
        {
            string errorMessage = string.Empty;
            try
            {
                foreach (var item in Organizations)
                {
                    var organization = await _organizationRepository.GetOrganizationAsync(item.OrganizationId, includeDepartments: true);
                    IList<User> users = Seed.GetUsersForOrganization(organization);
                    Guid[] accessList = Seed.GetAccessList(organization.OrganizationId);
                    foreach (User user in users)
                    {
                        var existingUser = await _organizationRepository.GetUserAsync(organization.OrganizationId, user.UserId);
                        if (existingUser == null)
                        {
                            user.Customer = organization;
                            await _organizationRepository.AddUserAsync(user);
                        }
                        else
                        {
                            existingUser.UpdateUser(user);
                        }
                        await _organizationRepository.SaveEntitiesAsync();
                        if (UserPermissions.TryGetValue(user.UserId, out PredefinedRole role))
                        {
                            var access = new List<Guid>();
                            if (role == PredefinedRole.DepartmentManager || role == PredefinedRole.Manager)
                                access.AddRange(accessList);

                            // Give both end user role and specified role
                            await _userPermissionServices.AssignUserPermissionsAsync(user.Email, $"{PredefinedRole.EndUser}", new List<Guid>(), _callerId);
                            await _userPermissionServices.AssignUserPermissionsAsync(user.Email, role.ToString(), access, _callerId);
                        }
                        else
                        {
                            await _userPermissionServices.AssignUserPermissionsAsync(user.Email, $"{PredefinedRole.EndUser}", new List<Guid>(), _callerId);
                        }
                    }
                    await _organizationRepository.SaveEntitiesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                errorMessage = "User creation exception\r\n";
            }
            return errorMessage;
        }
    }
}