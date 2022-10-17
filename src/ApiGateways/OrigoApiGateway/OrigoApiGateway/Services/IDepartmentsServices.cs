using Common.Interfaces;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

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
        /// <summary>
        /// Will fetch a list of all department names including the departmentId
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<HashSet<DepartmentNamesDTO>> GetAllDepartmentNamesAsync(Guid customerId, CancellationToken cancellationToken);
    }
}
