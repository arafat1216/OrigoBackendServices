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
                .ForMember(m => m.OrderDate, opts => opts.MapFrom(s => s.DateCreated))
                .ForMember(m => m.ShippingLabelLink, opts => opts.MapFrom(m => m.ExternalServiceManagementLink))
                .ForMember(m => m.FirstName, opts => opts.MapFrom(m => m.Owner.FirstName))
                .ForMember(m => m.Recipient, opts => opts.MapFrom(m => m.Owner.Email))
                .ForMember(m => m.Subject, opts => opts.Ignore())
                .ForMember(m => m.OrderLink, opts => opts.Ignore())
                .ForMember(m => m.OrderId, opts => opts.MapFrom(m => m.ExternalId));

            CreateMap<HardwareServiceOrder, OrderCancellationEmail>()
                .ForMember(m => m.OrderDate, opts => opts.MapFrom(s => s.DateCreated))
                .ForMember(m => m.FirstName, opts => opts.MapFrom(m => m.Owner.FirstName))
                .ForMember(m => m.Recipient, opts => opts.MapFrom(m => m.Owner.Email))
                .ForMember(m => m.Subject, opts => opts.Ignore())
                .ForMember(m => m.OrderLink, opts => opts.Ignore())
                .ForMember(m => m.OrderId, opts => opts.MapFrom(m => m.ExternalId))
                .ForMember(m => m.AssetId, opts => opts.MapFrom(m => m.AssetLifecycleId))
                .ForMember(m => m.FaultCategory, opts => opts.MapFrom(m => m.UserDescription))
                .ForMember(m => m.RepairType, opts => opts.MapFrom(m => $"{(ServiceTypeEnum)m.ServiceType.Id}"));

            CreateMap<HardwareServiceOrder, LoanDeviceEmail>()
                .ForMember(m => m.FirstName, opts => opts.MapFrom(m => m.Owner.FirstName))
                .ForMember(m => m.Recipient, opts => opts.MapFrom(m => m.Owner.Email))
                .ForMember(m => m.Subject, opts => opts.Ignore());

            CreateMap<HardwareServiceOrder, AssetDiscardedEmail>()
               .ForMember(m => m.FirstName, opts => opts.MapFrom(m => m.Owner.FirstName))
               .ForMember(m => m.Recipient, opts => opts.MapFrom(m => m.Owner.Email))
               .ForMember(m => m.Subject, opts => opts.Ignore());
        }
    }
}
