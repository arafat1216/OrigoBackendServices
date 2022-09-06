using AutoMapper;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

#nullable enable

namespace OrigoApiGateway.Mappings.HardwareServiceOrder
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="Models.HardwareServiceOrder.Backend.ServiceProvider"/>-class.
    /// </summary>
    public class ServiceProviderProfile : Profile
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="ServiceProviderProfile"/>-class.
        /// </summary>
        public ServiceProviderProfile()
        {
            CreateMap<Models.HardwareServiceOrder.Backend.ServiceProvider, CustomerPortalServiceProvider>()
                .ForMember(destination => destination.OfferedServiceOrderAddons, options =>
                {
                    options.MapFrom(source => source.OfferedServiceOrderAddons!.Where(e => e.IsCustomerTogglable));
                });

        }
    }
}
