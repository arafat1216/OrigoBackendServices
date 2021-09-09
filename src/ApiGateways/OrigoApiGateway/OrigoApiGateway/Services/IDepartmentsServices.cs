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
    }
}
