using AutoMapper;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Response;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;

namespace OrigoApiGateway.Mappings.SubscriptionManagement
{
    public class CustomerReferenceFieldProfile : Profile
    {
        public CustomerReferenceFieldProfile()
        {
            CreateMap<CustomerReferenceFieldResponseDTO, OrigoCustomerReferenceField>();
            CreateMap<NewCustomerReferenceField, CustomerReferenceFieldPostRequestDTO>();
        }
    }
}