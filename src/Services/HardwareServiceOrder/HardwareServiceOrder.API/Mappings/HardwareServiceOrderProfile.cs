using AutoMapper;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrder.API.Mappings
{
    public class HardwareServiceOrderProfile : Profile
    {
        public HardwareServiceOrderProfile()
        {
            CreateMap<CustomerSettingsDTO, ViewModels.CustomerSettings>()
                .ForMember(d => d.LoanDevice, opts => opts.MapFrom(s => new LoanDevice(s.LoanDevicePhoneNumber, s.LoanDeviceEmail)));

            CreateMap<ViewModels.CustomerSettings, CustomerSettingsDTO>()
                .ForMember(d => d.LoanDevicePhoneNumber, opts => opts.MapFrom(s => s.LoanDevice.PhoneNumber));


            // DTO to ViewModel
            CreateMap<NewHardwareServiceOrderDTO, ViewModels.NewHardwareServiceOrder>();
            CreateMap<DeliveryAddressDTO, ViewModels.DeliveryAddress>();

            CreateMap<HardwareServiceOrderServices.ServiceModels.HardwareServiceOrderDTO, ViewModels.HardwareServiceOrderResponse>()
                .ForMember(destination => destination.Id, options => options.MapFrom(source => source.ExternalId))
                .ForMember(d => d.StatusId, opts => opts.MapFrom(s => $"{(ServiceStatusEnum)s.StatusId}"))
                .ForMember(d => d.ServiceTypeId, opts => opts.MapFrom(s => $"{(ServiceTypeEnum)s.ServiceTypeId}"))
                .ForMember(d => d.ServiceProviderId, opts => opts.MapFrom(s => $"{(ServiceProviderEnum)s.ServiceProviderId}"));

            CreateMap<ExternalServiceEventDTO, ViewModels.ServiceEvent>()
                .ForMember(d => d.Status, opts => opts.MapFrom(s => $"{(ServiceStatusEnum)s.ServiceStatusId}"));

            // ViewModels to DTO
            CreateMap<ViewModels.DeliveryAddress, DeliveryAddressDTO>();

            CreateMap<ViewModels.AssetInfo, AssetInfoDTO>();

            CreateMap<ViewModels.ContactDetailsExtended, ContactDetailsExtendedDTO>();

            CreateMap<NewHardwareServiceOrder, NewHardwareServiceOrderDTO>()
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress));

        }
    }
}
