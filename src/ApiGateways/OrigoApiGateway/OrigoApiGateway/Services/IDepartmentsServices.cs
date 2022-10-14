using Common.Interfaces;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IDepartmentsServices
    {
        Task<OrigoDepartment> GetDepartmentAsync(Guid customerId, Guid departmentId);
        Task<IList<OrigoDepartment>> GetDepartmentsAsync(Guid customerId);
        Task<PagedModel<OrigoDepartment>> GetPaginatedDepartmentsAsync(Guid customerId, CancellationToken cancellationToken, bool includeManagers, int page, int limit);
        Task<OrigoDepartment> AddDepartmentAsync(Guid customerId, NewDepartmentDTO department);
        Task<OrigoDepartment> UpdateDepartmentPutAsync(Guid customerId, Guid departmentId, UpdateDepartmentDTO department);
        Task<OrigoDepartment> UpdateDepartmentPatchAsync(Guid customerId, Guid departmentId, UpdateDepartmentDTO department);
        Task<OrigoDepartment> DeleteDepartmentPatchAsync(Guid customerId, Guid departmentId, Guid callerId);
        Task<HashSet<DepartmentNamesDTO>> GetAllDepartmentNamesAsync(Guid customerId, CancellationToken cancellationToken);
    }
}
