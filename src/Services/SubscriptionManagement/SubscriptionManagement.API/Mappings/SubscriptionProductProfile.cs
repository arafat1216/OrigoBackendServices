using AutoMapper;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings
{
    public class SubscriptionProductProfile : Profile
    {
        public SubscriptionProductProfile()
        {
            CreateMap<ViewModels.SubscriptionProduct, CustomerSubscriptionProductDTO>();
            CreateMap<CustomerSubscriptionProductDTO, ViewModels.SubscriptionProduct>();
            CreateMap<ViewModels.UpdatedSubscriptionProduct, CustomerSubscriptionProductDTO>()
                .ForMember(d=>d.Id, opts=>opts.Ignore())
                .ForMember(d => d.OperatorId, opts => opts.Ignore())
                .ForMember(d => d.IsGlobal, opts => opts.Ignore());
        }
    }
}
