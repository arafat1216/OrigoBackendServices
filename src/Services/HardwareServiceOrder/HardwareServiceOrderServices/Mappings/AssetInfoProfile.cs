using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class AssetInfoProfile : Profile
    {
        public AssetInfoProfile()
        {
            // TODO: Remove the AfterMap as part of the refactoring task for handling multiple IMEI throughout the solution
            // AfterMap was used instead of ForMember due to ForMember not getting the value from SingleImei.
            CreateMap<AssetInfo, AssetInfoDTO>().AfterMap((s, d) => { d.Imei = s.SingleImei; });
        }
    }
}
