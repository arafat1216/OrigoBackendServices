﻿using AutoMapper;
using Common.Enums;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public async Task<DepartmentDTO> UpdateDepartmentAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers, Guid callerId)
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
                    UpdateAccessList(customer, managersToBeRemoved, callerId);
                }

                var managersToBeAdded = new List<User>();
                foreach (var manager in departmentManagers)
                {
                    var user = await _customerRepository.GetUserAsync(customerId, manager);
                    if (user == null) continue;
                    var userPermission = await _userPermissionRepository.GetUserPermissionsAsync(user.Email);
                    if (userPermission == null) continue;
                    var role = userPermission.FirstOrDefault(a => a.Role.Name == PredefinedRole.DepartmentManager.ToString() || a.Role.Name == PredefinedRole.Manager.ToString());
                    if (role == null) continue;
                    customer.AddDepartmentManager(departmentToUpdate, user, callerId);
                    managersToBeAdded.Add(user);
                }
            }
            else
            {
                if (departmentToUpdate.Managers.Any())
                {
                    customer.RemoveDepartmentManagers(departmentToUpdate, departmentToUpdate.Managers.ToList(), callerId);
                }
            }
            await _customerRepository.SaveEntitiesAsync();
            return _mapper.Map<DepartmentDTO>(departmentToUpdate);
        }

        private void UpdateAccessList(Organization customer, IList<User> managersToBeRemoved, Guid callerId)
        {
            foreach (var user in managersToBeRemoved)
            {
                List<Guid> allDepartmentGuids = new() { customer.OrganizationId };
                foreach (var department in user.ManagesDepartments)
                {
                    var departmentIdsWithSubDepartments = department.SubDepartments(customer.Departments.ToList()).Select(d => d.ExternalDepartmentId);
                    allDepartmentGuids.AddRange(departmentIdsWithSubDepartments);
                }

                _userPermissionRepository.AssignUserPermissionsAsync(user.Email, "Manager", allDepartmentGuids, callerId);
            }
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
