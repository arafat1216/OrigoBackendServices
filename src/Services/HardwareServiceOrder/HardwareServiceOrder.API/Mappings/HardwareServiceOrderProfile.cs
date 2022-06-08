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

            CreateMap<HardwareServiceOrderDTO, ViewModels.HardwareServiceOrder>();

            CreateMap<ViewModels.HardwareServiceOrder, HardwareServiceOrderDTO>();
        }
    }
}
