using CustomerServices.Models;
using AutoMapper;
using CustomerServices.ServiceModels;
using System.Linq;

namespace CustomerServices.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDTO>()
            .ForMember(u => u.ParentDepartmentId, opt => opt.MapFrom(src => src.ParentDepartment.ExternalDepartmentId))
            .ForMember(u => u.DepartmentId, opt => opt.MapFrom(src => src.ExternalDepartmentId))
            .ForMember(u => u.ManagedBy, opt => opt.MapFrom(src => src.Managers.Select(a => new ManagedByDTO
            {
                UserId = a.UserId,
                UserName   = a.Email,
                Name = $"{a.FirstName} {a.LastName}"
            })));
        }
    }
}
