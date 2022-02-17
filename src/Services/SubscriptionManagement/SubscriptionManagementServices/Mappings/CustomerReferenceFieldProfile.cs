using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings;

public class CustomerReferenceFieldProfile : Profile
{
    public CustomerReferenceFieldProfile()
    {
        CreateMap<CustomerReferenceField, CustomerReferenceFieldDTO>();
    }
}