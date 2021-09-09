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
            if (parentDepartment == null && departments.FirstOrDefault(dept => dept.ParentDepartment == null) != null)
            {
                throw new RootDepartmentAlreadyExistException();
            }
            var department = new Department(name, costCenterId, description, customer, newDepartmentId, parentDepartment: parentDepartment);
            customer.AddDepartment(department);

            await _customerRepository.SaveEntitiesAsync();
            return department;
        }
    }
}
