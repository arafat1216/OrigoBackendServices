using AutoMapper;
using Common.Enums;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class DepartmentsServices : IDepartmentsServices
    {
        private readonly ILogger<DepartmentsServices> _logger;
        private readonly IOrganizationRepository _customerRepository;
        private readonly IUserPermissionServices _userPermissionRepository;
        private readonly IMapper _mapper;

        public DepartmentsServices(ILogger<DepartmentsServices> logger, IOrganizationRepository customerRepository, IMapper mapper, IUserPermissionServices userPermissionRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _userPermissionRepository = userPermissionRepository;
        }

        public async Task<DepartmentDTO> GetDepartmentAsync(Guid customerId, Guid departmentId)
        {
            var department = await _customerRepository.GetDepartmentAsync(customerId, departmentId);
            return _mapper.Map<DepartmentDTO>(department);

        }

        public async Task<IList<DepartmentDTO>> GetDepartmentsAsync(Guid customerId)
        {
            var departmentList = await _customerRepository.GetDepartmentsAsync(customerId);
            return _mapper.Map<IList<DepartmentDTO>>(departmentList);
        }

        public async Task<DepartmentDTO> AddDepartmentAsync(Guid customerId, Guid newDepartmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers, Guid callerId)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
            var departments = await _customerRepository.GetDepartmentsAsync(customerId);
            var parentDepartment = departments.FirstOrDefault(dept => dept.ExternalDepartmentId == parentDepartmentId);
            var department = new Department(name, costCenterId, description, customer, newDepartmentId, callerId, parentDepartment: parentDepartment);
            customer.AddDepartment(department, callerId);

            if (departmentManagers.Any())
            {
                foreach (var managers in departmentManagers) 
                {
                    
                    var user = await _customerRepository.GetUserAsync(customerId, managers);
                    if (user != null)
                    {
                        var userPermission = await _userPermissionRepository.GetUserPermissionsAsync(user.Email);
                        if (userPermission != null)
                        {
                            var role = userPermission.FirstOrDefault(a => a.Role.Name == PredefinedRole.DepartmentManager.ToString() || a.Role.Name == PredefinedRole.Manager.ToString());
                            if (role != null) customer.AddDepartmentManager(department, user, callerId);
                        }
                    }
                    
                }
            }


            await _customerRepository.SaveEntitiesAsync();

            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task<DepartmentDTO> UpdateDepartmentPutAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers,Guid callerId)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
            var allDepartments = await _customerRepository.GetDepartmentsAsync(customerId);
            var parentDepartment = allDepartments.FirstOrDefault(dept => dept.ExternalDepartmentId == parentDepartmentId);
            var departmentToUpdate = allDepartments.FirstOrDefault(d => d.ExternalDepartmentId == departmentId);
            if (departmentToUpdate == null)
            {
                return null;
            }

            customer.ChangeDepartmentName(departmentToUpdate, name, callerId);
            customer.ChangeDepartmentCostCenterId(departmentToUpdate, costCenterId, callerId);
            customer.ChangeDepartmentDescription(departmentToUpdate, description, callerId);

            if (!departmentToUpdate.HasSubDepartment(parentDepartment)) // can't be moved to a department that is a subdepartment of itself or is itself.
            {
                customer.ChangeDepartmentsParentDepartment(departmentToUpdate, parentDepartment, callerId);
            }

            
            if (departmentManagers.Any())
            {
                List<User> users = new List<User>();
                foreach (var manager in departmentManagers)
                {
                    var user = _customerRepository.GetUserAsync(customerId, manager).Result;
                    if (user != null) 
                    {
                        var userPermission = await _userPermissionRepository.GetUserPermissionsAsync(user.Email);
                        if (userPermission != null)
                        {
                            var role = userPermission.FirstOrDefault(a => a.Role.Name == PredefinedRole.DepartmentManager.ToString() || a.Role.Name == PredefinedRole.Manager.ToString());
                            if (role != null) users.Add(user);
                        }
                    }
                }
               
                if(users.Any()) customer.UpdateDepartmentManagers(departmentToUpdate,users,callerId);
            }

            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<DepartmentDTO>(departmentToUpdate);
            
        }

        public async Task<DepartmentDTO> UpdateDepartmentPatchAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers, Guid callerId)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
            var allDepartments = await _customerRepository.GetDepartmentsAsync(customerId);
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
            if (parentDepartmentId != departmentToUpdate.ParentDepartment?.ExternalDepartmentId && // won't move this department if it already is a subdepartment of the target department
                !departmentToUpdate.HasSubDepartment(parentDepartment)) // can't be moved to a department that is a subdepartment of itself or is itself.
            {
                customer.ChangeDepartmentsParentDepartment(departmentToUpdate, parentDepartment, callerId);
            }
            if (departmentManagers.Any())
            {
                //Remove all managers from 
                var managers = departmentToUpdate.Managers.ToList();
                if (managers != null && managers.Any()) customer.RemoveDepartmentManagers(departmentToUpdate, managers, callerId);

                foreach (var manager in departmentManagers)
                {
                    var user = await _customerRepository.GetUserAsync(customerId, manager);
                    if (user != null)
                    {
                        var userPermission = await _userPermissionRepository.GetUserPermissionsAsync(user.Email);
                        if (userPermission != null)
                        {
                            var role = userPermission.FirstOrDefault(a => a.Role.Name == PredefinedRole.DepartmentManager.ToString() || a.Role.Name == PredefinedRole.Manager.ToString());
                            if (role != null) customer.AddDepartmentManager(departmentToUpdate, user, callerId);
                        }
                    }
                }
            }
            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<DepartmentDTO>(departmentToUpdate);
        }

        public async Task<DepartmentDTO> DeleteDepartmentAsync(Guid customerId, Guid departmentId, Guid callerId)
        {
            var customer = await _customerRepository.GetOrganizationAsync(customerId, includeDepartments: true);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
            var departments = await _customerRepository.GetDepartmentsAsync(customerId);
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
    }
}
