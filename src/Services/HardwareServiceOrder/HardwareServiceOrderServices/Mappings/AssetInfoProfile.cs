using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class AssetInfoProfile : Profile
    {
        public AssetInfoProfile()
        {
            CreateMap<AssetInfo, AssetInfoDTO>();
        }
    }
}
