using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="CustomerServiceProvider"/>-class.
    /// </summary>
    public class CustomerServiceProviderProfile : Profile
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerServiceProviderProfile"/>.
        /// </summary>
        public CustomerServiceProviderProfile()
        {
            CreateMap<CustomerServiceProvider, CustomerServiceProviderDto>()
                .ForMember(destination => destination.OrganizationId, options => options.MapFrom(source => source.CustomerId));
        }
    }
}
