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
                    .ForMember(dto => dto.SimCardAddress, opts => opts.MapFrom(m => new SimCardAddressRequestDTO {
                        FirstName = m.SimCardReceiverFirstName,
                        LastName = m.SimCardReceiverLastName,
                        Address = m.SimCardReceiverAddress,
                        Country = m.SimCardReceiverCountry,
                        PostalCode = m.SimCardReceiverPostalCode,
                        PostalPlace = m.SimCardReceiverPostalPlace }))
                    .ForMember(dto => dto.DataPackage, opts => opts.MapFrom(dp => dp.DataPackageName))
                    .ForMember(dto => dto.CustomerReferenceFields, opts => opts.MapFrom(m => JsonSerializer.Deserialize<IList<NewCustomerReferenceValue>>(m.CustomerReferenceFields, new JsonSerializerOptions { })))
                    .ForMember(dto => dto.NewOperatorAccount, opts => {
                        opts.Condition(m => m.OperatorAccountName == null);
                        opts.MapFrom(m => new NewOperatorAccountRequestedDTO { 
                            OperatorId = m.OperatorId, 
                            NewOperatorAccountOwner = m.OperatorAccountOwner, 
                            NewOperatorAccountPayer = m.OperatorAccountPayer , 
                            OrganizationNumberOwner = m.OrganizationNumberOwner,
                            OrganizationNumberPayer = m.OrganizationNumberPayer});
                    })
                    .ForMember(dto => dto.AddOnProducts, opt => opt.MapFrom(m => m.SubscriptionAddOnProducts.Select(a => a.AddOnProductName)));

        }
    }
}
