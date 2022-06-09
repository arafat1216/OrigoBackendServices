using AutoMapper;
using HardwareServiceOrder.API.ViewModels;

namespace HardwareServiceOrder.API.Mappings
{
    public class HardwareServiceOrderProfile : Profile
    {
        public HardwareServiceOrderProfile()
        {
            CreateMap<CustomerSettingsDTO, CustomerSettings>()
                .ForMember(d => d.LoanDevice, opts => opts.MapFrom(s => new LoanDevice(s.LoanDevicePhoneNumber, s.LoanDeviceEmail)))
                .ForMember(d => d.ServiceId, opts => opts.MapFrom(s => s.ApiUserName));

            CreateMap<CustomerSettings, CustomerSettingsDTO>()
                .ForMember(d => d.LoanDevicePhoneNumber, opts => opts.MapFrom(s => s.LoanDevice.PhoneNumber))
                .ForMember(d => d.ApiUserName, opts => opts.MapFrom(s => s.ServiceId))
                .ForMember(d => d.ApiPassword, opts => opts.Ignore());

            //DTO to ViewModel
            CreateMap<HardwareServiceOrderDTO, ViewModels.NewHardwareServiceOrder>();
            CreateMap<DeliveryAddressDTO, DeliveryAddress>();

            //ViewModels to DTO
            CreateMap<ViewModels.DeliveryAddress, DeliveryAddressDTO>();

            CreateMap<ViewModels.AssetInfo, AssetInfoDTO>()
                .ForMember(d => d.PurchaseDate, opts => opts.MapFrom(s => DateOnly.FromDateTime(s.PurchaseDate.GetValueOrDefault())));

            CreateMap<ViewModels.ContactDetails, ContactDetailsDTO>();

            CreateMap<ViewModels.NewHardwareServiceOrder, HardwareServiceOrderDTO>()
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress));

        }
    }
}
