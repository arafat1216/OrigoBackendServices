using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="CustomerSettings"/>-class.
    /// </summary>
    public class CustomerSettingsProfile : Profile
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerSettingsProfile"/>.
        /// </summary>
        public CustomerSettingsProfile()
        {
            CreateMap<CustomerSettings, CustomerSettingsDTO>();
        }
    }
}
