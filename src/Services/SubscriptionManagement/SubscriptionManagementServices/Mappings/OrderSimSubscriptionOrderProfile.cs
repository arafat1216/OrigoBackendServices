using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class OrderSimSubscriptionOrderProfile : Profile
    {
        public OrderSimSubscriptionOrderProfile()
        {
            CreateMap<OrderSimSubscriptionOrder, OrderSimSubscriptionOrderDTO>()
                .ForPath(dest => dest.Address.Street, opts => opts.MapFrom(src => src.Street))
                .ForPath(dest => dest.Address.City, opts => opts.MapFrom(src => src.City))
                .ForPath(dest => dest.Address.Postcode, opts => opts.MapFrom(src => src.Postcode))
                .ForPath(dest => dest.Address.Country, opts => opts.MapFrom(src => src.Country))
                .ForMember(dest => dest.CallerId, opts => opts.MapFrom(src => src.CreatedBy));
        }
    }
}
