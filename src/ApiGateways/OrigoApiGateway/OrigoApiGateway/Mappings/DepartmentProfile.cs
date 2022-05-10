using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<UpdateDepartment, UpdateDepartmentDTO>();
            CreateMap<NewDepartment, NewDepartmentDTO>();

        }
    }
}
