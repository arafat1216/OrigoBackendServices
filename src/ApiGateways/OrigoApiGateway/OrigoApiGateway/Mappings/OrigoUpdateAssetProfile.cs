using AutoMapper;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoUpdateAssetProfile : Profile
    {
        public OrigoUpdateAssetProfile()
        {
            CreateMap<OrigoUpdateAsset, OrigoUpdateAssetDTO>();
        }
    }
}
