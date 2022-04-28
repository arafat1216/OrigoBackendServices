using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoPagedAssetProfile : Profile
    {
        public OrigoPagedAssetProfile()
        {
            CreateMap<PagedAssetsDTO, OrigoPagedAssets>()
                     .ForMember(dest => dest.Assets, opts => opts.MapFrom(src => src.Items));
        }
    }
}
