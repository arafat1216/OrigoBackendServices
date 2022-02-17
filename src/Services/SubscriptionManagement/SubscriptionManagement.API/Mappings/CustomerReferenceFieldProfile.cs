using AutoMapper;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings;

public class CustomerReferenceFieldProfile : Profile
{
    public CustomerReferenceFieldProfile()
    {
        CreateMap<CustomerReferenceFieldDTO, CustomerReferenceField>()
            .ConstructUsing(dest => new CustomerReferenceField(dest.Name, dest.ReferenceType.ToString(), dest.Id));
    }
}