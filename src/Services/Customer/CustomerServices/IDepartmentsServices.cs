using CustomerServices.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IDepartmentsServices
    {
        Task<DepartmentDTO> GetDepartmentAsync(Guid customerId, Guid departmentId);
        Task<IList<DepartmentDTO>> GetDepartmentsAsync(Guid customerId);
        Task<DepartmentDTO> AddDepartmentAsync(Guid customerId,Guid newDepartmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers, Guid callerId);
        Task<DepartmentDTO> UpdateDepartmentAsync(Guid customerId, Guid departmentId, Guid? parentDepartmentId, string name, string costCenterId, string description, IList<Guid> departmentManagers, Guid callerId);
        Task<DepartmentDTO> DeleteDepartmentAsync(Guid customerId, Guid departmentId,Guid callerId);
    }
}
