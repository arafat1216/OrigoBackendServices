using AutoMapper;
using OrigoApiGateway.Models.Asset;

namespace OrigoApiGateway.Mappings
{
    public class ImportedAssetProfile : Profile
    {
        public ImportedAssetProfile()
        {
            CreateMap<ImportedAsset, InvalidImportedAsset>();
        }
    }
}
