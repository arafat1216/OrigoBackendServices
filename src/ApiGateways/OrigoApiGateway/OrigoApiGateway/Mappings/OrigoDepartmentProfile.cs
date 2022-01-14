using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoDepartmentProfile : Profile
    {
        public OrigoDepartmentProfile()
        {
            CreateMap<DepartmentDTO, OrigoDepartment>();
            CreateMap<OrigoDepartment, UpdateDepartmentDTO>();
        }
    }
}
