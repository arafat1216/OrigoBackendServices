using AutoMapper;
using Common.Model.EventModels.DatasyncModels;
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
