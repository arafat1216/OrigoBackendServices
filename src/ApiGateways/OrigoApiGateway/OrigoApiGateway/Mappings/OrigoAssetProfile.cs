using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoAssetProfile : Profile
    {
        public OrigoAssetProfile()
        {
            CreateMap<AssetDTO, OrigoMobilePhone>();
            CreateMap<AssetDTO, OrigoTablet>();
        }
    }
}
