using AutoMapper;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class AssetLabelsProfile : Profile
    {
        public AssetLabelsProfile() 
        { 
            CreateMap<AssetLabels,AssetLabelsDTO>();
        }
    }
}
