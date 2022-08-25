using AutoMapper;

namespace HardwareServiceOrder.API.Mappings
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="ViewModels.CustomerServiceProvider"/>-class.
    /// </summary>
    public class CustomerServiceProviderProfile : Profile
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerServiceProviderProfile"/>.
        /// </summary>
        public CustomerServiceProviderProfile()
        {
            CreateMap<CustomerServiceProviderDto, ViewModels.CustomerServiceProvider>();
        }
    }
}
