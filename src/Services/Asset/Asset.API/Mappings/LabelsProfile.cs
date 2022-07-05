using Asset.API.ViewModels;
using AssetServices.Models;
using AutoMapper;

namespace Asset.API.Mappings
{
    public class LabelsProfile : Profile
    {
        public LabelsProfile()
        {
            CreateMap<AssetServices.Models.Label, ViewModels.Label>();

        }
    }
}
