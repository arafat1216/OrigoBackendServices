using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewCustomerProductModuleDtoProfile :Profile
    {
        public NewCustomerProductModuleDtoProfile()
        {
            CreateMap<NewCustomerProductModule, NewCustomerProductModuleDTO>();
        }
    }
}
