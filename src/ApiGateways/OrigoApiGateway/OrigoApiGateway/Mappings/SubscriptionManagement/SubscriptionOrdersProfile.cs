using AutoMapper;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;

namespace OrigoApiGateway.Mappings.SubscriptionManagement;

public class SubscriptionOrdersProfile : Profile
{
    public SubscriptionOrdersProfile()
    {
        CreateMap<TransferToBusinessSubscriptionOrder, TransferToBusinessSubscriptionOrderDTO>();
        CreateMap<NewCustomerReferenceValue, CustomerReferenceValuePostRequestDTO>();
        CreateMap<TransferToBusinessSubscriptionOrderDTO, TransferToBusinessSubscriptionOrder>();
        CreateMap<TransferToPrivateSubscriptionOrder, TransferToPrivateSubscriptionOrderDTO>();
        CreateMap<TransferToPrivateSubscriptionOrderDTO, TransferToPrivateSubscriptionOrder>();
    }
}