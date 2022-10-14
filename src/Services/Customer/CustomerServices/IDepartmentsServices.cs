using Common.Interfaces;
using CustomerServices.ServiceModels;

namespace CustomerServices
{
    public interface IDepartmentsServices
    {
        Task<DepartmentDTO> GetDepartmentAsync(Guid customerId, Guid departmentId);
        Task<IList<DepartmentDTO>> GetDepartmentsAsync(Guid customerId);
        Task<List<DepartmentNamesDTO>> GetAllDepartmentNamesAsync(Guid customerId, CancellationToken cancellationToken);
        Task<PagedModel<DepartmentDTO>> GetPaginatedDepartmentsAsync(Guid organizationId, bool includeManagers, CancellationToken cancellationToken, int page = 1, int limit = 25);
        Task<DepartmentDTO> AddDepartmentAsync(Guid customerId, Guid newDepartmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers, Guid callerId);
        Task<DepartmentDTO> UpdateDepartmentAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers, Guid callerId);
        Task<DepartmentDTO> DeleteDepartmentAsync(Guid customerId, Guid departmentId, Guid callerId);
    }
}
