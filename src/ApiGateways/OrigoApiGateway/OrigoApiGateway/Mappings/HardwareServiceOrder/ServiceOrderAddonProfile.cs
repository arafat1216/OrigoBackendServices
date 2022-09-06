using AutoMapper;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

#nullable enable

namespace OrigoApiGateway.Mappings.HardwareServiceOrder
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="ServiceOrderAddon"/>-class.
    /// </summary>
    public class ServiceOrderAddonProfile : Profile
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="ServiceOrderAddonProfile"/>-class.
        /// </summary>
        public ServiceOrderAddonProfile()
        {
            CreateMap<ServiceOrderAddon, CustomerPortalServiceOrderAddon>();
            CreateMap<ServiceOrderAddon, UserPortalServiceOrderAddon>();
        }

    }
}
