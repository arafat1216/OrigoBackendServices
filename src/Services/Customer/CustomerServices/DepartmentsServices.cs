using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using Microsoft.Extensions.Logging;

namespace CustomerServices
{
    public class DepartmentsServices : IDepartmentsServices
    {
        private readonly ILogger<DepartmentsServices> _logger;
        private readonly IOrganizationRepository _customerRepository;
        private readonly IUserPermissionServices _userPermissionServices;
        private readonly IMapper _mapper;

        public DepartmentsServices(ILogger<DepartmentsServices> logger, IOrganizationRepository customerRepository, IMapper mapper, IUserPermissionServices userPermissionServices)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _userPermissionServices = userPermissionServices;
        }

        public async Task<DepartmentDTO> GetDepartmentAsync(Guid customerId, Guid departmentId)
        {
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task<IList<DepartmentDTO>> GetDepartmentsAsync(Guid customerId)
        {
            var departmentList = await _customerRepository.GetDepartmentsAsync(customerId, true);
            return _mapper.Map<IList<DepartmentDTO>>(departmentList);
        }

        public async Task<PagedModel<DepartmentDTO>> GetPaginatedDepartmentsAsync(Guid organizationId, bool includeManagers, CancellationToken cancellationToken, int page = 1, int limit = 25)
        {
            var pagedDepartmentList = await _customerRepository.GetPaginatedDepartmentsAsync(organizationId, includeManagers, true, cancellationToken, page, limit);
            PagedModel<DepartmentDTO> remappedModel = new()
            {
                Items = _mapper.Map<IList<DepartmentDTO>>(pagedDepartmentList.Items),
                CurrentPage = pagedDepartmentList.CurrentPage,
                PageSize = pagedDepartmentList.PageSize,
                TotalItems = pagedDepartmentList.TotalItems,
                TotalPages = pagedDepartmentList.TotalPages
            };

            return remappedModel;
        }

        public async Task<DepartmentDTO> AddDepartmentAsync(Guid customerId, Guid newDepartmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers, Guid callerId)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
            var departments = await _customerRepository.GetDepartmentsAsync(customerId, false);
            var parentDepartment = departments.FirstOrDefault(dept => dept.ExternalDepartmentId == parentDepartmentId);
            var department = new Department(name, costCenterId, description, customer, newDepartmentId, callerId, parentDepartment: parentDepartment);
            customer.AddDepartment(department, callerId);

            //If department is a sub department and has no managers then the managers of the parent department should have access
            if (!departmentManagers.Any() && parentDepartment != null)
            {
                if (parentDepartment.Managers.Any()) departmentManagers = parentDepartment.Managers.Select(a => a.UserId).ToList();
            }

            if (departmentManagers.Any())
            {
                var managersToBeAdded = new List<User>();
                foreach (var manager in departmentManagers)
                {
                    var user = await _customerRepository.GetUserAsync(customerId, manager);
                    if (user == null) continue;
                    var userPermission = await _userPermissionServices.GetUserPermissionsAsync(user.Email);
                    if (userPermission == null) continue;
                    var role = userPermission.FirstOrDefault(a => a.Role == PredefinedRole.DepartmentManager.ToString() || a.Role == PredefinedRole.Manager.ToString());
                    if (role == null) continue;
                    customer.AddDepartmentManager(department, user, callerId);
                    managersToBeAdded.Add(user);
                }

                if (managersToBeAdded.Any())
                {
                    await UpdateAccessListAsync(customer, managersToBeAdded, callerId);
                }
            }


            await _customerRepository.SaveEntitiesAsync();

            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task<DepartmentDTO> UpdateDepartmentAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers, Guid callerId)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
            var allDepartments = await _customerRepository.GetDepartmentsAsync(customerId, false);
            var parentDepartment = allDepartments.FirstOrDefault(dept => dept.ExternalDepartmentId == parentDepartmentId);
            var departmentToUpdate = allDepartments.FirstOrDefault(d => d.ExternalDepartmentId == departmentId);
            if (departmentToUpdate == null)
            {
                return null;
            }
            if (name != null && name != departmentToUpdate.Name)
            {
                customer.ChangeDepartmentName(departmentToUpdate, name, callerId);
            }
            if (costCenterId != null && costCenterId != departmentToUpdate.CostCenterId)
            {
                customer.ChangeDepartmentCostCenterId(departmentToUpdate, costCenterId, callerId);
            }
            if (description != null && description != departmentToUpdate.Description)
            {
                customer.ChangeDepartmentDescription(departmentToUpdate, description, callerId);
            }
            if (parentDepartment != null && parentDepartmentId != departmentToUpdate.ParentDepartment?.ExternalDepartmentId && // won't move this department if it already is a sub department of the target department
                !departmentToUpdate.HasSubDepartment(parentDepartment)) // can't be moved to a department that is a sub department of itself or is itself.
            {
                customer.ChangeDepartmentsParentDepartment(departmentToUpdate, parentDepartment, callerId);
            }
            if (departmentManagers.Any())
            {
                var managersToBeRemoved = departmentToUpdate.Managers.Where(m => departmentManagers.All(n => n != m.UserId)).ToList();

                if (managersToBeRemoved.Any())
                {
                    customer.RemoveDepartmentManagers(departmentToUpdate, managersToBeRemoved, callerId);
                    await UpdateAccessListAsync(customer, managersToBeRemoved, callerId);
                }

                var managersToBeAdded = new List<User>();
                foreach (var manager in departmentManagers)
                {
                    var user = await _customerRepository.GetUserAsync(customerId, manager);
                    if (user == null) continue;
                    var userPermission = await _userPermissionServices.GetUserPermissionsAsync(user.Email);
                    if (userPermission == null) continue;
                    var role = userPermission.FirstOrDefault(a => a.Role == PredefinedRole.DepartmentManager.ToString() || a.Role == PredefinedRole.Manager.ToString());
                    if (role == null) continue;
                    customer.AddDepartmentManager(departmentToUpdate, user, callerId);
                    managersToBeAdded.Add(user);
                }

                if (managersToBeAdded.Any())
                {
                    await UpdateAccessListAsync(customer, managersToBeAdded, callerId);
                }
            }
            else
            {
                if (departmentToUpdate.Managers.Any())
                {
                    var managersToBeUpdated = departmentToUpdate.Managers.ToList();
                    customer.RemoveDepartmentManagers(departmentToUpdate, managersToBeUpdated, callerId);
                    await UpdateAccessListAsync(customer, managersToBeUpdated, callerId);
                }
            }
            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<DepartmentDTO>(departmentToUpdate);
        }

        private async Task UpdateAccessListAsync(Organization customer, IList<User> managersToBeUpdated, Guid callerId)
        {
            await _customerRepository.SaveEntitiesAsync(); // Must save to set manager and department lists correctly.

            foreach (var user in managersToBeUpdated)
            {
                List<Guid> allDepartmentGuids = new() { customer.OrganizationId };
                var updatedUser = await _customerRepository.GetUserAsync(customer.OrganizationId, user.UserId);
                if (updatedUser == null)
                {
                    continue;
                }
                foreach (var department in updatedUser.ManagesDepartments)
                {
                    var departmentIdsWithSubDepartments = department.SubDepartments(customer.Departments.ToList()).Select(d => d.ExternalDepartmentId);
                    allDepartmentGuids.AddRange(departmentIdsWithSubDepartments);
                }

                await _userPermissionServices.UpdateAccessListAsync(updatedUser, allDepartmentGuids, callerId);
            }
        }

        public async Task<DepartmentDTO> DeleteDepartmentAsync(Guid customerId, Guid departmentId, Guid callerId)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
            var departments = await _customerRepository.GetDepartmentsAsync(customerId, false);
            var department = departments.FirstOrDefault(d => d.ExternalDepartmentId == departmentId);
            if (department == null)
                return null;
            var departmentsToDelete = department.SubDepartments(departments);
            foreach (var deleteDepartment in departmentsToDelete)
            {
                customer.RemoveDepartment(deleteDepartment, callerId);
            }
            await _customerRepository.DeleteDepartmentsAsync(departmentsToDelete);

            return _mapper.Map<DepartmentDTO>(department);

        }

        public async Task<List<DepartmentNamesDTO>> GetAllDepartmentNamesAsync(Guid customerId, CancellationToken cancellationToken)
        {
            return await _customerRepository.GetAllDepartmentNamesAsync(customerId, cancellationToken) ?? new List<DepartmentNamesDTO>();
        }
    }
}
