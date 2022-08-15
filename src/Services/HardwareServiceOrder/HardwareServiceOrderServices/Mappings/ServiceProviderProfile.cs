using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="ServiceProvider"/>-class.
    /// </summary>
    public class ServiceProviderProfile : Profile
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="ServiceProviderProfile"/>-class.
        /// </summary>
        public ServiceProviderProfile()
        {
            CreateMap<ServiceProvider, ServiceProviderDTO>();
        }
    }
}
