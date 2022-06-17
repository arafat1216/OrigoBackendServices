using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class HardwareServiceOrderProfile : Profile
    {
        public HardwareServiceOrderProfile()
        {
            CreateMap<HardwareServiceOrder, HardwareServiceOrderResponseDTO>()
                .ForMember(m => m.Created, opts => opts.MapFrom(s => s.DateCreated))
                .ForMember(m => m.Updated, opts => opts.MapFrom(s => s.DateCreated))
                .ForMember(m => m.Id, opts => opts.MapFrom(s => s.ExternalId))
                .ForMember(m => m.Events, opts => opts.MapFrom(s => s.ServiceEvents))
                .ForMember(m => m.Owner, opts => opts.MapFrom(s => s.Owner.UserId))
                .ForMember(m => m.Events, opts => opts.MapFrom(s => s.ServiceEvents))
                .ForMember(m => m.ErrorDescription, opts => opts.MapFrom(s => s.UserDescription))
                .ForMember(m => m.DeliveryAddress, opts => opts.MapFrom(s => s.DeliveryAddress))
                .ForMember(m => m.ServiceProvider, opts => opts.MapFrom(s => (ServiceProviderEnum)s.ServiceProviderId))
                .ForMember(m => m.Status, opts => opts.MapFrom(s => (ServiceStatusEnum)s.StatusId))
                .ForMember(m => m.Type, opts => opts.MapFrom(s => (ServiceTypeEnum)s.ServiceTypeId));
        }
    }
}
