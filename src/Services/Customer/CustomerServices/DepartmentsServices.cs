using CustomerServices.Exceptions;
using CustomerServices.Models;
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
        public DepartmentsServices(ILogger<DepartmentsServices> logger, IOrganizationRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<Department> GetDepartmentAsync(Guid customerId, Guid departmentId)
        {
            return await _customerRepository.GetDepartmentAsync(customerId, departmentId);
        }

        public async Task<IList<Department>> GetDepartmentsAsync(Guid customerId)
        {
            return await _customerRepository.GetDepartmentsAsync(customerId);
        }

        public async Task<Department> AddDepartmentAsync(Guid customerId, Guid newDepartmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, Guid callerId)
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

            await _customerRepository.SaveEntitiesAsync();
            return department;
        }

        public async Task<Department> UpdateDepartmentPutAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, Guid callerId)
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

            if (!departmentToUpdate.HasSubdepartment(parentDepartment)) // can't be moved to a department that is a subdepartment of itself or is itself.
            {
                customer.ChangeDepartmentsParentDepartment(departmentToUpdate, parentDepartment, callerId);
            }

            await _customerRepository.SaveEntitiesAsync();
            return departmentToUpdate;
        }

        public async Task<Department> UpdateDepartmentPatchAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, Guid callerId)
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
                !departmentToUpdate.HasSubdepartment(parentDepartment)) // can't be moved to a department that is a subdepartment of itself or is itself.
            {
                customer.ChangeDepartmentsParentDepartment(departmentToUpdate, parentDepartment, callerId);
            }
            await _customerRepository.SaveEntitiesAsync();
            return departmentToUpdate;
        }

        public async Task<Department> DeleteDepartmentAsync(Guid customerId, Guid departmentId, Guid callerId)
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
            var departmentsToDelete = department.Subdepartments(departments);
            foreach (var deleteDepartment in departmentsToDelete)
            {
                customer.RemoveDepartment(deleteDepartment, callerId);
            }
            await _customerRepository.DeleteDepartmentsAsync(departmentsToDelete);
            return department;
        }
    }
}
