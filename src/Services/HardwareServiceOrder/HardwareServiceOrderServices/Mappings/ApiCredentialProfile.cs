using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="ApiCredential"/>-class.
    /// </summary>
    public class ApiCredentialProfile : Profile
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="ApiCredentialProfile"/>.
        /// </summary>
        public ApiCredentialProfile()
        {
            CreateMap<ApiCredential, ApiCredentialDTO>();
        }
    }
}
