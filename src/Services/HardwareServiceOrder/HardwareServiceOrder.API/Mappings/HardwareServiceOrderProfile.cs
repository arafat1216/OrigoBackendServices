using AutoMapper;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrder.API.Mappings
{
    public class HardwareServiceOrderProfile : Profile
    {
        public HardwareServiceOrderProfile()
        {
            CreateMap<CustomerSettingsDto, CustomerSettings>()
                .ForMember(d => d.LoanDevice, opts => opts.MapFrom(s => new LoanDevice(s.LoanDevicePhoneNumber, s.LoanDeviceEmail)));

            CreateMap<HardwareServiceOrderDTO, ViewModels.HardwareServiceOrder>();
            CreateMap<ViewModels.HardwareServiceOrder, HardwareServiceOrderDTO>();
        }
    }
}
