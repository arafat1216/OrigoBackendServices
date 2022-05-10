using AutoMapper;
using Customer.API.ViewModels;

namespace Customer.API.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<CustomerServices.ServiceModels.DepartmentDTO, Department>();
        }
    }
}
