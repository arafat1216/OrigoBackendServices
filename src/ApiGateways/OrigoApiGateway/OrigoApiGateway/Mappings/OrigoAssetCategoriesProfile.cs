using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoAssetCategoriesProfile : Profile
    {
        public OrigoAssetCategoriesProfile()
        {
            CreateMap<AssetCategoryDTO, OrigoAssetCategory>();
        }
    }
}
