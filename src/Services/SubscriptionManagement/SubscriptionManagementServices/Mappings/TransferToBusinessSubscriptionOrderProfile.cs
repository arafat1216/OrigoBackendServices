using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using System.Text.Json;
namespace SubscriptionManagementServices.Mappings
{
    public class TransferToBusinessSubscriptionOrderProfile : Profile
    {
        public TransferToBusinessSubscriptionOrderProfile()
        {
            CreateMap<TransferToBusinessSubscriptionOrder, TransferToBusinessSubscriptionOrderDTO>()
                .ForMember(d => d.CallerId, opts => opts.Ignore())
                .ForMember(d => d.NewOperatorAccount, opts => opts.MapFrom(s => new NewOperatorAccountRequestedDTO { NewOperatorAccountOwner = s.OperatorAccountOwner, NewOperatorAccountPayer = s.OperatorAccountPayer }))
                .ForMember(d => d.CustomerReferenceFields, opts => opts.MapFrom(s => JsonSerializer.Deserialize<IList<NewCustomerReferenceField>>(s.CustomerReferenceFields, new JsonSerializerOptions { })));
        }
    }
}
