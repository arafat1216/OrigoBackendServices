using AutoMapper;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoAssetLifecycleProfile : Profile
    {
        public OrigoAssetLifecycleProfile()
        {
            CreateMap<LifecycleDTO, OrigoAssetLifecycle>();
        }
    }
}
