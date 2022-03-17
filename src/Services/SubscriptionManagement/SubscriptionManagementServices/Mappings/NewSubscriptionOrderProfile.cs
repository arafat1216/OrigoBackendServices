using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using System.Text.Json;

namespace SubscriptionManagementServices.Mappings
{
    public class NewSubscriptionOrderProfile : Profile
    {
        public NewSubscriptionOrderProfile()
        {
            CreateMap<NewSubscriptionOrder, NewSubscriptionOrderDTO>()
                    .ForMember(dto => dto.SimCardAddress, opts => opts.MapFrom(sa => new SimCardAddressRequestDTO
                    {
                        Address = sa.SimCardAddress.Address, Country = sa.SimCardAddress.Country, PostalCode = sa.SimCardAddress.PostalCode,FirstName = sa.SimCardAddress.FirstName, LastName = sa.SimCardAddress.LastName
                    }))
                    .ForMember(dto => dto.DataPackage, opts => opts.MapFrom(dp => dp.DataPackageName))
                    .ForMember(dto => dto.CustomerReferenceFields, opts => opts.MapFrom(m => JsonSerializer.Deserialize<IList<NewCustomerReferenceValue>>(m.CustomerReferenceFields, new JsonSerializerOptions { })))
                    .ForMember(dto => dto.NewOperatorAccount, opts => opts.MapFrom(m => new NewOperatorAccountRequestedDTO { OperatorId = m.OperatorId, NewOperatorAccountOwner = m.OperatorAccountOwner, NewOperatorAccountPayer = m.OperatorAccountPayer }))
                    .ForMember(dto => dto.AddOnProducts, opt => opt.MapFrom(m => m.SubscriptionAddOnProducts.Select(a => a.AddOnProductName)));

        }
    }
}
