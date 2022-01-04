using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoPagedAssetProfile:Profile
    {
        public OrigoPagedAssetProfile()
        {
            CreateMap<PagedAssetsDTO, OrigoPagedAssets>();
        }
    }
}
