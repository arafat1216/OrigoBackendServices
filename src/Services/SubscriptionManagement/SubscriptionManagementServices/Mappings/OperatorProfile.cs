using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings;

public class OperatorProfile : Profile
{
    public OperatorProfile()
    {
        CreateMap<Operator, OperatorDTO>();
    }
}