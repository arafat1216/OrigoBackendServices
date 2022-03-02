using AutoMapper;
using System.Text.Json;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class TransferToBusinessSubscriptionOrderProfile : Profile
    {
        public TransferToBusinessSubscriptionOrderProfile()
        {
            CreateMap<TransferToBusinessSubscriptionOrder, TransferToBusinessSubscriptionOrderDTO>()
                .ForMember(dto => dto.CallerId, opts => opts.MapFrom(m => m.CreatedBy))
                .ForMember(dto => dto.NewOperatorAccount, opts => opts.MapFrom(m => new NewOperatorAccountRequestedDTO { NewOperatorAccountOwner = m.OperatorAccountOwner, NewOperatorAccountPayer = m.OperatorAccountPayer }))
                .ForMember(dto => dto.CustomerReferenceFields, opts => opts.MapFrom(m => JsonSerializer.Deserialize<IList<NewCustomerReferenceField>>(m.CustomerReferenceFields, new JsonSerializerOptions { })));
            CreateMap<TransferToBusinessSubscriptionOrderDTO, TransferToBusinessSubscriptionOrder>()
                .ForMember(m => m.CreatedBy, opts => opts.MapFrom(dto => dto.CallerId));

            CreateMap<TransferToPrivateSubscriptionOrderDTO, TransferToBusinessSubscriptionOrder>()
                .ForMember(m => m.CreatedBy, opts => opts.MapFrom(dto => dto.CallerId));

        }
    }
}
