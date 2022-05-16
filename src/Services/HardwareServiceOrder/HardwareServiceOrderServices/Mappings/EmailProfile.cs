using AutoMapper;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.Mappings
{
    public class EmailProfile : Profile
    {
        public EmailProfile()
        {
            CreateMap<HardwareServiceOrder, AssetRepairEmail>()
                .ForMember(m => m.OrderDate, opts => opts.MapFrom(s => s.CreatedDate))
                .ForMember(m => m.PackageSlipLink, opts => opts.MapFrom(m => m.ExternalProviderLink))
                .ForMember(m => m.FirstName, opts => opts.MapFrom(m => m.OrderedBy.Name))
                .ForMember(m => m.Recipient, opts => opts.MapFrom(m => m.OrderedBy.Email))
                .ForMember(m => m.Subject, opts => opts.Ignore())
                .ForMember(m => m.OrderLink, opts => opts.Ignore())
                .ForMember(m => m.OrderId, opts => opts.MapFrom(m => m.ExternalId));

            CreateMap<HardwareServiceOrder, LoanDeviceEmail>()
                .ForMember(m => m.FirstName, opts => opts.MapFrom(m => m.OrderedBy.Name))
                .ForMember(m => m.Recipient, opts => opts.MapFrom(m => m.OrderedBy.Email))
                .ForMember(m => m.Subject, opts => opts.Ignore());

            CreateMap<HardwareServiceOrder, AssetDiscardedEmail>()
               .ForMember(m => m.FirstName, opts => opts.MapFrom(m => m.OrderedBy.Name))
               .ForMember(m => m.Recipient, opts => opts.MapFrom(m => m.OrderedBy.Email))
               .ForMember(m => m.Subject, opts => opts.Ignore());
        }
    }
}
