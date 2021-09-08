using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IDepartmentsServices
    {
        Task<Department> GetDepartmentAsync(Guid customerId, Guid departmentId);
        Task<IList<Department>> GetDepartmentsAsync(Guid customerId);
        Task<Department> AddDepartmentAsync(Guid customerId,Guid newDepartmentId, Guid? parentDepartmentId, string name, string costCenterId, string description);
    }
}
