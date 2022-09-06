using AutoMapper;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

#nullable enable

namespace OrigoApiGateway.Mappings.HardwareServiceOrder
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="CustomerServiceProvider"/>-class.
    /// </summary>
    public class CustomerServiceProviderProfile : Profile
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerServiceProviderProfile"/>-class.
        /// </summary>
        public CustomerServiceProviderProfile()
        {
            CreateMap<CustomerServiceProvider, CustomerPortalCustomerServiceProvider>()
                .ForMember(destination => destination.ActiveServiceOrderAddons, options =>
                {
                    options.MapFrom(source => source.ActiveServiceOrderAddons!.Where(e => e.IsCustomerTogglable));
                });

            CreateMap<CustomerServiceProvider, UserPortalCustomerServiceProvider>()
                .ForMember(destination => destination.ActiveServiceOrderAddons, options =>
                {
                    options.MapFrom(source => source.ActiveServiceOrderAddons!.Where(e => e.IsUserSelectable));
                });

        }
    }
}
