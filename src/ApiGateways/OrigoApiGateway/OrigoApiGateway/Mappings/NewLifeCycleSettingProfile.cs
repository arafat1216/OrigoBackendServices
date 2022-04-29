using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewLifeCycleSettingProfile : Profile
    {
        public NewLifeCycleSettingProfile()
        {
            CreateMap<NewLifeCycleSetting, NewLifeCycleSettingDTO>();
        }
    }
}
