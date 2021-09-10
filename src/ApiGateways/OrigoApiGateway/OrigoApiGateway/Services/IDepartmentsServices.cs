using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IDepartmentsServices
    {
        Task<OrigoDepartment> GetDepartment(Guid customerId, Guid departmentId);
        Task<IList<OrigoDepartment>> GetDepartments(Guid customerId);
        Task<OrigoDepartment> AddDepartmentAsync(Guid customerId, NewDepartment department);
        Task<OrigoDepartment> UpdateDepartmentPutAsync(Guid customerId, Guid departmentId, OrigoDepartment department);
        Task<OrigoDepartment> UpdateDepartmentPatchAsync(Guid customerId, Guid departmentId, OrigoDepartment department);
        Task<OrigoDepartment> DeleteDepartmentPatchAsync(Guid customerId, Guid departmentId);
    }
}
