using AutoMapper;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;

#nullable enable

namespace OrigoApiGateway.Mappings.HardwareServiceOrder
{
    /// <summary>
    ///     Configures 'AutoMapper' profiles to and from the <see cref="CustomerSettings"/>-class.
    /// </summary>
    public class CustomerSettingsProfile : Profile
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerSettingsProfile"/>-class.
        /// </summary>
        public CustomerSettingsProfile()
        {
            CreateMap<CustomerSettings, LoanDeviceSettings>();
        }
    }
}
