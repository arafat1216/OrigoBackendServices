using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class SimCardAddressProfile : Profile
    {
        public SimCardAddressProfile()
        {
            CreateMap<SimCardAddressRequestDTO,SimCardAddress>();
        }
    }
}
