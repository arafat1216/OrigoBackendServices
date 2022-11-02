using AutoMapper;
using Customer.API.CustomerDatasyncModels;
using Customer.API.ViewModels;
using Customer.API.WriteModels;

namespace Customer.API.Mappings;

public class EmployeeUserProfile : Profile
{
    public EmployeeUserProfile()
    {
        CreateMap<CreateEmployeeEvent, NewUser>();
        CreateMap<User, CreateEmployeeEvent>();

    }

}
