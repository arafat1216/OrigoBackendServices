using AutoMapper;
using OrigoApiGateway.Models.Asset;

namespace OrigoApiGateway.Mappings
{
    public class AssignAssetToUserProfile : Profile
    {
        public AssignAssetToUserProfile()
        {
            CreateMap<AssignAssetToUser, AssignAssetToUserDTO>();

        }
    }
}
