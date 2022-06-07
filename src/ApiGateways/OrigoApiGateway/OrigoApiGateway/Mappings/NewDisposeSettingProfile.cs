using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewDisposeSettingProfile : Profile
    {
        public NewDisposeSettingProfile()
        {
            CreateMap<NewDisposeSetting, NewDisposeSettingDTO>();
        }
    }
}
