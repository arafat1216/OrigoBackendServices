using AutoMapper;

namespace HardwareServiceOrder.API.Mappings
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="ViewModels.ObscuredApiCredential"/>-class.
    /// </summary>
    public class ObscuredApiCredentialProfile : Profile
    {

        /// <summary>
        ///     Creates a new instance of the <see cref="ObscuredApiCredentialProfile"/>.
        /// </summary>
        public ObscuredApiCredentialProfile()
        {
            CreateMap<ApiCredentialDTO, ViewModels.ObscuredApiCredential>()
                .AfterMap((source, destination) => { destination.ApiUsernameFilled = !string.IsNullOrEmpty(source.ApiUsername); })
                .AfterMap((source, destination) => { destination.ApiPasswordFilled = !string.IsNullOrEmpty(source.ApiPassword); });
        }
    }
}
