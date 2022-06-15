using AutoMapper;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class ChangeAssetStatusProfile : Profile
    {
        public ChangeAssetStatusProfile()
        {
            CreateMap<ChangeAssetStatus, ChangeAssetStatusDTO>();
        }
    }
}
