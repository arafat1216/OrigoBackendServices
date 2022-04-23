using Asset.API.ViewModels;
using AssetServices.ServiceModel;
using AutoMapper;

namespace Asset.API.Mappings;

public class NewAssetProfile : Profile
{
    public NewAssetProfile()
    {
        CreateMap<NewAsset, NewAssetDTO>();
    }
}