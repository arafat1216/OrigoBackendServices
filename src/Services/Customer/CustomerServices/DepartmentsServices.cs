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
        private readonly ICustomerRepository _customerRepository;
        public DepartmentsServices(ILogger<DepartmentsServices> logger, ICustomerRepository customerRepository)
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

        public async Task<Department> AddDepartmentAsync(Guid customerId, Guid newDepartmentId, Guid? parentDepartmentId, string name, string costCenterId, string description)
        {
            var customer = await _customerRepository.GetCustomerAsync(customerId);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }
            var departments = await _customerRepository.GetDepartmentsAsync(customerId);
            var parentDepartment = departments.FirstOrDefault(dept => dept.ExternalDepartmentId == parentDepartmentId);
            var department = new Department(name, costCenterId, description, customer, newDepartmentId, parentDepartment: parentDepartment);
            customer.AddDepartment(department);

            await _customerRepository.SaveEntitiesAsync();
            return department;
        }

        public async Task<Department> UpdateDepartmentPutAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description)
        {
            var customer = await _customerRepository.GetCustomerAsync(customerId);
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

            customer.ChangeDepartmentName(departmentToUpdate, name);
            customer.ChangeDepartmentCostCenterId(departmentToUpdate, costCenterId);
            customer.ChangeDepartmentDescription(departmentToUpdate, description);

            if (!departmentToUpdate.HasSubdepartment(parentDepartment)) // can't be moved to a department that is a subdepartment of itself or is itself.
            {
                customer.ChangeDepartmentsParentDepartment(departmentToUpdate, parentDepartment);
            }

            await _customerRepository.SaveEntitiesAsync();
            return departmentToUpdate;
        }

        public async Task<Department> UpdateDepartmentPatchAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description)
        {
            var customer = await _customerRepository.GetCustomerAsync(customerId);
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
                customer.ChangeDepartmentName(departmentToUpdate, name);
            }
            if (costCenterId != null && costCenterId != departmentToUpdate.CostCenterId)
            {
                customer.ChangeDepartmentCostCenterId(departmentToUpdate, costCenterId);
            }
            if (description != null && description != departmentToUpdate.Description)
            {
                customer.ChangeDepartmentDescription(departmentToUpdate, description);
            }
            if (parentDepartmentId != departmentToUpdate.ParentDepartment?.ExternalDepartmentId && // won't move this department if it already is a subdepartment of the target department
                !departmentToUpdate.HasSubdepartment(parentDepartment)) // can't be moved to a department that is a subdepartment of itself or is itself.
            {
                customer.ChangeDepartmentsParentDepartment(departmentToUpdate, parentDepartment);
            }
            await _customerRepository.SaveEntitiesAsync();
            return departmentToUpdate;
        }

        public async Task<Department> DeleteDepartmentAsync(Guid customerId, Guid departmentId)
        {
            var customer = await _customerRepository.GetCustomerAsync(customerId);
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
                customer.RemoveDepartment(deleteDepartment);
            }
            await _customerRepository.DeleteDepartmentsAsync(departmentsToDelete);
            return department;
        }
    }
}
