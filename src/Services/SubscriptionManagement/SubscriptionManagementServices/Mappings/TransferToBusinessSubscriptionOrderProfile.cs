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
                .ForMember(dto => dto.CustomerReferenceFields, opts => opts.MapFrom(m => JsonSerializer.Deserialize<IList<NewCustomerReferenceValue>>(m.CustomerReferenceFields, new JsonSerializerOptions { })));
            CreateMap<TransferToBusinessSubscriptionOrderDTO, TransferToBusinessSubscriptionOrder>()
                .ForMember(m => m.CreatedBy, opts => opts.MapFrom(dto => dto.CallerId));

            CreateMap<TransferToPrivateSubscriptionOrderDTO, TransferToBusinessSubscriptionOrder>()
                .ForMember(m => m.CreatedBy, opts => opts.MapFrom(dto => dto.CallerId));

            CreateMap<TransferToBusinessSubscriptionOrder, TransferToBusinessSubscriptionOrderDTOResponse>()
               .ForMember(dto => dto.CallerId, opts => opts.MapFrom(m => m.CreatedBy))
               .ForMember(dto => dto.NewOperatorAccount, opts => {
                   opts.Condition(m => m.OperatorAccountOwner != null);
                   opts.MapFrom(m => new NewOperatorAccountResponseDTO { NewOperatorName = m.OperatorName, NewOperatorAccountOwner = m.OperatorAccountOwner, NewOperatorAccountPayer = m.OperatorAccountPayer }); 
               
               })
               .ForMember(dto => dto.CustomerReferenceFields, opts => opts.MapFrom(m => JsonSerializer.Deserialize<IList<NewCustomerReferenceValue>>(m.CustomerReferenceFields, new JsonSerializerOptions { })))
               .ForMember(d => d.DataPackage,opt => opt.MapFrom(m => m.DataPackageName))
               .ForMember(d => d.AddOnProducts, opt => opt.MapFrom(m => m.SubscriptionAddOnProducts.Select(a => a.AddOnProductName)));


        }
    }
}
