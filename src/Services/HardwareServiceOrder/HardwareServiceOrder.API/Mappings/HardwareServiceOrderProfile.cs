using AutoMapper;
using HardwareServiceOrder.API.ViewModels;

namespace HardwareServiceOrder.API.Mappings
{
    public class HardwareServiceOrderProfile : Profile
    {
        public HardwareServiceOrderProfile()
        {
            CreateMap<CustomerSettingsDTO, CustomerSettings>()
                .ForMember(d => d.LoanDevice, opts => opts.MapFrom(s => new LoanDevice(s.LoanDevicePhoneNumber, s.LoanDeviceEmail)));

            CreateMap<HardwareServiceOrderDTO, ViewModels.HardwareServiceOrder>();
            CreateMap<ViewModels.HardwareServiceOrder, HardwareServiceOrderDTO>();
        }
    }
}
