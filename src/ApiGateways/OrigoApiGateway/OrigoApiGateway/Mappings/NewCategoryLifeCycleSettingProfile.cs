using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewCategoryLifeCycleSettingProfile : Profile
    {
        public NewCategoryLifeCycleSettingProfile()
        {
            CreateMap<NewCategoryLifeCycleSetting, NewCategoryLifeCycleSettingDTO>();
        }
    }
}
