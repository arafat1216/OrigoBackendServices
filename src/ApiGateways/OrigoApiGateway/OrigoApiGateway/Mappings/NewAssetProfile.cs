using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewAssetProfile : Profile
    {
        public NewAssetProfile()
        {
            CreateMap<NewAsset, NewAssetDTO>();
        }
    }
}
