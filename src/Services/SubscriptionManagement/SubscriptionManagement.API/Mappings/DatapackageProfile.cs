using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings
{
    public class DatapackageProfile : Profile
    {
        public DatapackageProfile()
        {
            CreateMap<Datapackage,DatapackageDTO>();
        }
    }
}
