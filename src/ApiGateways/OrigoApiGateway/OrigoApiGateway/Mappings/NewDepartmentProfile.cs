using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewDepartmentProfile : Profile
    {
        public NewDepartmentProfile()
        {
            CreateMap<NewDepartment,NewDepartmentDTO>();
        }
    }
}
