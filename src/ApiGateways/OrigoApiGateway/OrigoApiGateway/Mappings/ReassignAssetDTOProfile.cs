using AutoMapper;
using OrigoApiGateway.Models.Asset;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class ReassignAssetDTOProfile : Profile
    {
        public ReassignAssetDTOProfile()
        {
            CreateMap<ReAssignmentPersonal, ReassignedToUserDTO>();
            CreateMap<ReAssignmentNonPersonal, ReassignedToDepartmentDTO>();
        }
    }
}
