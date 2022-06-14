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
                

            //DTO to ViewModel
            CreateMap<HardwareServiceOrderDTO, ViewModels.NewHardwareServiceOrder>();
            CreateMap<DeliveryAddressDTO, ViewModels.DeliveryAddress>();
            CreateMap<HardwareServiceOrderServices.ServiceModels.HardwareServiceOrderResponseDTO, ViewModels.HardwareServiceOrderResponseDTO>()
                .ForMember(d => d.Status, opts => opts.MapFrom(s => $"{s.Status}"))
                .ForMember(d => d.Type, opts => opts.MapFrom(s => $"{s.Type}"))
                .ForMember(d => d.ServiceProvider, opts => opts.MapFrom(s => $"{s.ServiceProvider}"));

            CreateMap<ExternalServiceEventDTO, ViewModels.ServiceEvent>()
                .ForMember(d => d.Status, opts => opts.MapFrom(s => $"{(ServiceStatusEnum)s.ServiceStatusId}"));

            //ViewModels to DTO
            CreateMap<ViewModels.DeliveryAddress, DeliveryAddressDTO>();

            CreateMap<ViewModels.AssetInfo, AssetInfoDTO>();

            CreateMap<ViewModels.ContactDetails, ContactDetailsDTO>();

            CreateMap<NewHardwareServiceOrder, HardwareServiceOrderDTO>()
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress));

        }
    }
}
