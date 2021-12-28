using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewCustomerAssetCategoryTypeDtoProfile:Profile
    {
        public NewCustomerAssetCategoryTypeDtoProfile()
        {
            CreateMap<NewCustomerAssetCategoryType, NewCustomerAssetCategoryTypeDTO>();
        }
    }
}
