using AutoMapper;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class UpdateAssetStatusDtoProfile : Profile
    {
        public UpdateAssetStatusDtoProfile()
        {
            CreateMap<UpdateAssetsStatus, UpdateAssetsStatusDTO>();
        }
    }
}
